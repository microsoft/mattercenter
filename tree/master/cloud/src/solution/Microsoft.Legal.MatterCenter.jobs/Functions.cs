using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using System;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Net.Http;

namespace Microsoft.Legal.MatterCenter.jobs
{
    public class Functions
    {
        /// <summary>
        /// This method will get executed for every 5 seconds for testing purposes. This can be configurable. For testing purposes
        /// I have configured this for 5 seconds
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="externalSharingRequests"></param>
        /// <param name="log"></param>
        public static void ReadExternalAccessRequests([TimerTrigger("00:00:05", RunOnStartup = true)]TimerInfo timerInfo,
            [Table("ExternalAccessRequests")] IQueryable<ExternalSharingRequest> externalSharingRequests, 
            TextWriter log)
        {

            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            var configuration = builder.Build();
            UpdateMatterData(configuration);
            var query = from p in externalSharingRequests select p;
            foreach(ExternalSharingRequest externalSharingRequest in query)
            {
                log.WriteLine("PK:{0}, RK:{1}, Role:{2}",
                externalSharingRequest.PartitionKey, externalSharingRequest.RowKey, externalSharingRequest.Permission);
                if(externalSharingRequest.Status.ToLower() == "pending")
                {
                    GetExternalAccessRequestsFromSPO(externalSharingRequest, log, configuration);
                }                
            }
            
        }

        private static async Task UpdateMatterData(IConfigurationRoot configuration)
        {
            var authResult = GetTokenFromAAD(configuration).Result;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44323");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        authResult.AccessTokenType, authResult.AccessToken);


            // Call the Web API to get the values          
            HttpResponseMessage httpResponse = await httpClient.GetAsync("api/v1/taxonomy/getcurrentsitetitlev1");
            if(httpResponse.IsSuccessStatusCode)
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            }
            else
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            }
            
        }

        private static void GetExternalAccessRequestsFromSPO(ExternalSharingRequest externalSharingRequest, TextWriter log, IConfigurationRoot configuration)
        {
            string requestedForPerson = externalSharingRequest.Person;
            string matterId = externalSharingRequest.MatterId;
            SecureString password = GetEncryptedPassword(configuration["Settings:AdminPassword"]);

            
            using (var ctx = new ClientContext(externalSharingRequest.MatterUrl))
            {
                ctx.Credentials = new SharePointOnlineCredentials(configuration["Settings:AdminUserName"], password);
                var listTitle = configuration["Settings:ExternalAccessRequests"];      
                var list = ctx.Web.Lists.GetByTitle(listTitle);
                CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery();
                camlQuery.ViewXml = "";
                ListItemCollection listItemCollection = list.GetItems(camlQuery);
                ctx.Load(listItemCollection);
                ctx.ExecuteQuery();
                foreach (ListItem listItem in listItemCollection)
                {        
                    //The matter id for whom the request has been sent            
                    string requestedObjectTitle = listItem["RequestedObjectTitle"].ToString();
                    //The person to whom the request has been sent
                    string requestedFor = listItem["RequestedFor"].ToString();
                    //The matter url for which the request has been sent
                    string url = ((FieldUrlValue)listItem["RequestedObjectUrl"]).Url;
                    //The status of the request whether it has been in pending =0, accepeted=2 or withdrawn=5
                    string status = listItem["Status"].ToString(); 
                    //If the status is accepted and the person and matter in table storage equals to item in Access Requests list
                    if(requestedFor == requestedForPerson && matterId == requestedObjectTitle && status == "2")
                    {
                        #region Update For Matter Client
                        //Update item in table storage to "Accepted"
                        //UpdateTableStorageEntity(externalSharingRequest, log, configuration);
                        //Add the user to corresponding matter document library, one note library, tasks and calendar
                        string matterDocumentUrl = $"{externalSharingRequest.MatterUrl}/{externalSharingRequest.MatterId}";
                        string matterOneNoteUrl = $"{externalSharingRequest.MatterUrl}/{externalSharingRequest.MatterId}_onenote";
                        string matterCalendarUrl = $"{externalSharingRequest.MatterUrl}/lists{externalSharingRequest.MatterId}_calendar";
                        string matterTasksUrl = $"{externalSharingRequest.MatterUrl}/lists{externalSharingRequest.MatterId}_task";


                        var users = new List<UserRoleAssignment>();
                        UserRoleAssignment userRole = new UserRoleAssignment();
                        userRole.UserId = externalSharingRequest.Person;
                        userRole.Role = SharePoint.Client.Sharing.Role.Owner;
                        users.Add(userRole);

                        //Give access to document library
                        IList<UserSharingResult> documentPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(ctx,
                        matterDocumentUrl,
                        users, true, true, false, "The following document library has been shared with you", true, true);
                        ctx.ExecuteQuery();

                        //Give access to one note library
                        IList<UserSharingResult> oneNoteSharingResult = DocumentSharingManager.UpdateDocumentSharingInfo(ctx,
                        matterOneNoteUrl,
                        users, true, true, false, "The following one note document has been shared with you", true, true);
                        ctx.ExecuteQuery();

                        //Give access to javascript library
                        users = new List<UserRoleAssignment>();
                        userRole = new UserRoleAssignment();
                        userRole.UserId = externalSharingRequest.Person;
                        userRole.Role = SharePoint.Client.Sharing.Role.View;
                        users.Add(userRole);
                        IList<UserSharingResult> javaScriptLibrary = DocumentSharingManager.UpdateDocumentSharingInfo(ctx,
                        $"{configuration["Settings:CatalogUrl"]}/SiteAssets/Matter%20Center%20Assets",
                        users, true, true, false, "The following one note document has been shared with you", true, true);
                        ctx.ExecuteQuery();

                        string roleValue = ""; // int depends on the group IDs at site
                        int groupId = 0;
                        bool propageAcl = true; // Not relevant for external accounts
                        bool sendEmail = false;
                        bool includedAnonymousLinkInEmail = false;
                        string emailSubject = null;
                        string emailBody = "List shared";
                        var email = externalSharingRequest.Person.Replace('@', '_');
                        string peoplePickerInput = @"[{
                                            'Key' : 'i:0#.f|membership|^#ext#@msmatter.onmicrosoft.com', 
                                            'Description' : '^#ext#@msmatter.onmicrosoft.com', 
                                            'DisplayText' : '', 
                                            'EntityType' : 'User', 
                                            'ProviderDisplayName' : 'Tenant', 
                                            'ProviderName' : 'Tenant', 
                                            'IsResolved' : true, 
                                            'EntityData' : {
                                                                'MobilePhone' : '', 
                                                                'Email' : '$', 
                                                                'Department' : '', 
                                                                'Title' : '$', 
                                                                'PrincipalType' : 'GUEST_USER'}, 
                                            'MultipleMatches' : []}]";
                        peoplePickerInput = peoplePickerInput.Replace("^", email);
                        peoplePickerInput = peoplePickerInput.Replace("$", externalSharingRequest.Person);

                        //Give access to calendar  list
                        SharingResult calendarResult = SharePoint.Client.Web.ShareObject(ctx, 
                            matterCalendarUrl, peoplePickerInput, roleValue,
                        groupId, propageAcl, sendEmail, includedAnonymousLinkInEmail, emailSubject, emailBody, true);
                        ctx.Load(calendarResult);
                        ctx.ExecuteQuery();

                        //give access to task list
                        SharingResult taskResult = SharePoint.Client.Web.ShareObject(ctx, matterTasksUrl, 
                            peoplePickerInput, roleValue,
                        groupId, propageAcl, sendEmail, includedAnonymousLinkInEmail, emailSubject, emailBody, true);
                        ctx.Load(taskResult);
                        ctx.ExecuteQuery();
                        #endregion

                        #region update for repository
                        using (var catalogContext = new ClientContext(configuration["Settings:CatalogUrl"]))
                        {
                            catalogContext.Credentials = new SharePointOnlineCredentials("matteradmin@msmatter.onmicrosoft.com", password);
                            users = new List<UserRoleAssignment>();
                            userRole = new UserRoleAssignment();
                            userRole.UserId = externalSharingRequest.Person;
                            userRole.Role = SharePoint.Client.Sharing.Role.View;
                            users.Add(userRole);

                            //Give access to document library
                            IList<UserSharingResult> documentCatalogPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(catalogContext,
                            $"{configuration["Settings:CatalogUrl"]}/SiteAssets",
                            users, true, true, false, "The following document library has been shared with you", true, true);
                            ctx.ExecuteQuery();
                        }
                        #endregion
                    }
                }
            } 
        }

        private static SecureString GetEncryptedPassword(string plainTextPassword)
        {      
            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            foreach(char c in plainTextPassword)
            {                
                securePassword.AppendChar(c);               
            }
            //while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }

        private static async Task<AuthenticationResult> GetTokenFromAAD(IConfigurationRoot configuration)
        {
            string clientId = "78146e7f-04a6-4b67-b079-c4190113454e";    
            string aadInstance = "https://login.windows.net/{0}";
            string tenant = "msmatter.onmicrosoft.com";
            string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, aadInstance, tenant);

            var context = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenant));
            var userCredential = new UserCredential(configuration["Settings:AdminUserName"], configuration["Settings:AdminPassword"]);
            AuthenticationResult result = await context.AcquireTokenAsync("https://matterwebapp.azurewebsites.net/", clientId, userCredential);
            //var token = result.CreateAuthorizationHeader().Substring("Bearer ".Length);
            //return token;
            return result;
        }

        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        private static void UpdateTableStorageEntity(ExternalSharingRequest externalSharingRequest, TextWriter log, IConfigurationRoot configuration)
        {
            CloudStorageAccount cloudStorageAccount = 
                CloudStorageAccount.Parse(configuration["Data:DefaultConnection:AzureStorageConnectionString"]);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(configuration["Settings:TableStorageForExternalRequests"]);
            // Create a retrieve operation that takes a entity.
            TableOperation retrieveOperation = 
                TableOperation.Retrieve<ExternalSharingRequest>(externalSharingRequest.PartitionKey, externalSharingRequest.RowKey);
            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            // Assign the result to a ExternalSharingRequest object.
            ExternalSharingRequest updateEntity = (ExternalSharingRequest)retrievedResult.Result;
            if(updateEntity!=null)
            {
                updateEntity.Status = "Accepted";
                TableOperation updateOperation = TableOperation.Replace(updateEntity);
                table.Execute(updateOperation);
            }
        }
    }
}
