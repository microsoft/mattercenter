using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using System;
using System.Security;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client.Utilities;
using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Jobs
{
    /// <summary>
    /// This WebJob function will update the matter related lists by giving permissions to external users
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// This method will read external access requests azure table storage for all
        /// pending requests and update the matter related lists and libraries permissions for external users
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="matterInformationVM"></param>
        /// <param name="log"></param>
        public static void ReadExternalAccessRequests([TimerTrigger("00:00:05", RunOnStartup = true)]TimerInfo timerInfo,
            [Table("ExternalAccessRequests")] IQueryable<MatterInformationVM> matterInformationVM, 
            TextWriter log)
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
                var configuration = builder.Build();
                //Read all rows from table storage which are in pending state
                var query = from p in matterInformationVM select p;
                foreach (MatterInformationVM matterInformation in query)
                {
                    if (matterInformation != null)
                    {
                        string serializedMatter = matterInformation.SerializeMatter;
                        //De Serialize the matter information
                        MatterInformationVM originalMatter = Newtonsoft.Json.JsonConvert.DeserializeObject<MatterInformationVM>(serializedMatter);
                        if (originalMatter.Status.ToLower() == "pending")
                        {
                            //Read all external access requests records from azure table storge
                            GetExternalAccessRequestsFromSPO(originalMatter, log, configuration);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine($"Exception occured in the method ReadExternalAccessRequests. {ex}");
            }
        }

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private static bool CheckUserPresentInMatterCenter(ClientContext ctx, string clientUrl, 
            string email, IConfigurationRoot configuration, TextWriter log)
        {
            try
            {                
                string userAlias = email;
                ClientPeoplePickerQueryParameters queryParams = new ClientPeoplePickerQueryParameters();
                queryParams.AllowMultipleEntities = false;
                queryParams.MaximumEntitySuggestions = 500;
                queryParams.PrincipalSource = PrincipalSource.All;
                queryParams.PrincipalType = PrincipalType.User | PrincipalType.SecurityGroup;
                queryParams.QueryString = userAlias;
                ClientResult<string> clientResult = 
                    ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(ctx, queryParams);
                ctx.ExecuteQuery();
                string results = clientResult.Value;
                int peoplePickerMaxRecords = 30;
                IList<PeoplePickerUser> foundUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeoplePickerUser>>(results).Where(result => (string.Equals(result.EntityType, ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER,
                        StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.Description)) || (!string.Equals(result.EntityType,
                        ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.EntityData.Email))).Take(peoplePickerMaxRecords).ToList();
                return foundUsers.Count > 0; 
            }
            catch (Exception ex)
            {
                log.WriteLine($"Exception occured in the method CheckUserPresentInMatterCenter. {ex}");
                throw;
            }
        }

        /// <summary>
        /// This method will get all list items from external access requests and process all
        /// requests which are in accpeted state
        /// </summary>
        /// <param name="originalMatter"></param>
        /// <param name="log"></param>
        /// <param name="configuration"></param>
        private static void GetExternalAccessRequestsFromSPO(MatterInformationVM originalMatter, 
            TextWriter log, 
            IConfigurationRoot configuration)
        {
            try
            {
                foreach (var assignUserEmails in originalMatter.Matter.AssignUserEmails)
                {
                    foreach (string email in assignUserEmails)
                    {
                        using (var ctx = new ClientContext(originalMatter.Client.Url))
                        {
                            SecureString password = GetEncryptedPassword(configuration["Settings:AdminPassword"]);
                            ctx.Credentials = new SharePointOnlineCredentials(configuration["Settings:AdminUserName"], password);
                            //First check whether the user exists in SharePoint or not
                            if (CheckUserPresentInMatterCenter(ctx, originalMatter.Client.Url, email, configuration, log) == true)
                            {
                                string requestedForPerson = email;
                                string matterId = originalMatter.Matter.Id;
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
                                    //The status of the request whether it has been in pending=0, accepeted=2 or withdrawn=5
                                    string status = listItem["Status"].ToString();
                                    //If the status is accepted and the person and matter in table storage equals to item in Access Requests list
                                    if (requestedFor == requestedForPerson && matterId == requestedObjectTitle && status == "2")
                                    {
                                        UpdateMatter umd = new UpdateMatter();
                                        //Update all matter related lists and libraries permissions for external users
                                        umd.UpdateUserPermissionsForMatter(originalMatter, configuration, password);

                                        //Update permissions for external users in Catalog Site Collection
                                        using (var catalogContext = new ClientContext(configuration["Catalog:CatalogUrl"]))
                                        {
                                            catalogContext.Credentials =
                                                new SharePointOnlineCredentials(configuration["Settings:AdminUserName"], password);
                                            umd.AssignPermissionToCatalogLists(configuration["Catalog:SiteAssets"], catalogContext,
                                                email.Trim(), configuration["Catalog:SiteAssetsPermissions"], configuration);
                                        }
                                        UpdateTableStorageEntity(originalMatter, log, configuration);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.WriteLine($"Exception occured in the method GetExternalAccessRequestsFromSPO. {ex}");
            }
        }        

        /// <summary>
        /// This method will return the secure password for authentication to SharePoint Online
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
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

        #region Need to explore this for future purposes
        private static async Task UpdateMatterData(IConfigurationRoot configuration)
        {
            //var authResult = GetTokenFromAAD(configuration).Result;
            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://localhost:44323");
            //httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            //            authResult.AccessTokenType, authResult.AccessToken);


            //// Call the Web API to get the values          
            //HttpResponseMessage httpResponse = await httpClient.GetAsync("api/v1/taxonomy/getcurrentsitetitlev1");
            //if (httpResponse.IsSuccessStatusCode)
            //{
            //    var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            //}
            //else
            //{
            //    var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            //}
            

        }

        private static async Task<AuthenticationResult> GetTokenFromAAD(IConfigurationRoot configuration)
        {
            try
            {
                string clientId = "b9de791e-0b7b-402a-a3fa-d2a26f463783";
                string aadInstance = "https://login.windows.net/{0}";
                string tenant = "msmatter.onmicrosoft.com";
                string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, aadInstance, tenant);

                var context = new AuthenticationContext(string.Format("https://login.windows.net/{0}", tenant));
                var userCredential = new UserCredential(configuration["Settings:AdminUserName"], configuration["Settings:AdminPassword"]);
                AuthenticationResult result = await context.AcquireTokenAsync("matterwebapp", clientId, userCredential);
                //var token = result.CreateAuthorizationHeader().Substring("Bearer ".Length);
                //return token;
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        #endregion


        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        private static void UpdateTableStorageEntity(MatterInformationVM matterInformation, TextWriter log, IConfigurationRoot configuration)
        {
            try
            { 
                CloudStorageAccount cloudStorageAccount = 
                    CloudStorageAccount.Parse(configuration["Data:DefaultConnection:AzureStorageConnectionString"]);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference(configuration["Settings:TableStorageForExternalRequests"]);
                // Create a retrieve operation that takes a entity.
                TableOperation retrieveOperation = 
                    TableOperation.Retrieve<MatterInformationVM>(matterInformation.PartitionKey, matterInformation.RowKey);
                // Execute the operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);
                // Assign the result to a ExternalSharingRequest object.
                MatterInformationVM updateEntity = (MatterInformationVM)retrievedResult.Result;
                if(updateEntity!=null)
                {
                    updateEntity.Status = "Accepted";                
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                }
            }
            catch (Exception ex)
            {
                log.WriteLine($"Exception occured in the method UpdateTableStorageEntity. {ex}");
            }
        }
    }
}
