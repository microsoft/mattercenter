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

using System.Threading.Tasks;
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client.Utilities;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System.Globalization;

namespace Microsoft.Legal.MatterCenter.Jobs
{
    /// <summary>
    /// This WebJob function will update the matter related lists by giving permissions to external users
    /// </summary>
    public class Functions
    {

        /// <summary>
        /// This web job function will process matter that is there in azure table storage called MatterRequests.
        /// If there are any new matter is created, this web job function will get invoked for the time duration that has
        /// been specified and will preocess all new matters and will send notification for those respective users
        /// Once the matter has been processed, it will change the status of that matter to "Send" so that it will not
        /// be processed again
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="matterInformationVM"></param>
        public static void ProcessMatter([TimerTrigger("00:00:05", RunOnStartup = true)]TimerInfo timerInfo, 
            [Table("MatterRequests")] IQueryable<MatterInformationVM> matterInformationVM, TextWriter log)
        {       
            var query = from p in matterInformationVM select p;
            if(query.ToList().Count() > 0)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json")
                    .AddEnvironmentVariables();//appsettings.json will be overridden with azure web appsettings
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);
                var configuration = builder.Build();
                //// can use on premise exchange server credentials with service.UseDefaultCredentials = true, or 
                //explicitly specify the admin account (set default to false)
                string adminUserName = configuration.GetSection("Settings").GetSection("AdminUserName").Value;
                string adminPassword = configuration.GetSection("Settings").GetSection("AdminPassword").Value;
                service.Credentials = new WebCredentials(adminUserName, adminPassword);
                service.Url = new Uri(configuration.GetSection("Settings").GetSection("ExchangeServiceURL").Value);
                string mailSubject = configuration.GetSection("Mail").GetSection("MatterMailSubject").Value;
                string defaultHtmlChunk = configuration.GetSection("Mail").GetSection("MatterMailDefaultContentTypeHtmlChunk").Value;
                string oneNoteLibrarySuffix = configuration.GetSection("Matter").GetSection("OneNoteLibrarySuffix").Value;
                string matterMailBodyMatterInformation = configuration.GetSection("Mail").GetSection("MatterMailBodyMatterInformation").Value;
                string matterMailBodyConflictCheck = configuration.GetSection("Mail").GetSection("MatterMailBodyConflictCheck").Value;
                string matterCenterDateFormat = configuration.GetSection("Mail").GetSection("MatterCenterDateFormat").Value;
                string matterMailBodyTeamMembers = configuration.GetSection("Mail").GetSection("MatterMailBodyTeamMembers").Value;

                foreach (MatterInformationVM matterInformation1 in query)
                {
                    if (matterInformation1 != null)
                    {
                        if (matterInformation1.Status.ToLower() == "pending")
                        {                        
                            //De Serialize the matter information
                            MatterInformationVM originalMatter = JsonConvert.DeserializeObject<MatterInformationVM>(matterInformation1.SerializeMatter);
                            Matter matter = originalMatter.Matter;
                            MatterDetails matterDetails = originalMatter.MatterDetails;
                            Client client = originalMatter.Client;

                            string matterMailBody, blockUserNames;
                            // Generate Mail Subject
                            string matterMailSubject = string.Format(CultureInfo.InvariantCulture, mailSubject,
                                matter.Id, matter.Name, originalMatter.MatterCreator);

                            // Step 1: Create Matter Information
                            string defaultContentType = string.Format(CultureInfo.InvariantCulture,
                                defaultHtmlChunk, matter.DefaultContentType);
                            string matterType = string.Join(";", matter.ContentTypes.ToArray()).TrimEnd(';').Replace(matter.DefaultContentType, defaultContentType);

                            // Step 2: Create Team Information
                            string secureMatter = ServiceConstants.FALSE.ToUpperInvariant() == matter.Conflict.SecureMatter.ToUpperInvariant() ?
                                ServiceConstants.NO : ServiceConstants.YES;
                            string mailBodyTeamInformation = string.Empty;
                            mailBodyTeamInformation = TeamMembersPermissionInformation(matterDetails, mailBodyTeamInformation);

                            string oneNotePath = string.Concat(client.Url, ServiceConstants.FORWARD_SLASH,
                                    matter.MatterGuid, oneNoteLibrarySuffix,
                                            ServiceConstants.FORWARD_SLASH, matter.MatterGuid, ServiceConstants.FORWARD_SLASH, matter.MatterGuid);

                            if (originalMatter.IsConflictCheck)
                            {
                                string conflictIdentified = ServiceConstants.FALSE.ToUpperInvariant() == matter.Conflict.Identified.ToUpperInvariant() ?
                                                ServiceConstants.NO : ServiceConstants.YES;
                                blockUserNames = string.Join(";", matter.BlockUserNames.ToArray()).Trim().TrimEnd(';');

                                blockUserNames = !String.IsNullOrEmpty(blockUserNames) ? string.Format(CultureInfo.InvariantCulture,
                            "<div>{0}: {1}</div>", "Conflicted User", blockUserNames) : string.Empty;
                                matterMailBody = string.Format(CultureInfo.InvariantCulture,
                                    matterMailBodyMatterInformation, client.Name, client.Id,
                                    matter.Name, matter.Id, matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture,
                                    matterMailBodyConflictCheck, ServiceConstants.YES, matter.Conflict.CheckBy,
                                    Convert.ToDateTime(matter.Conflict.CheckOn, CultureInfo.InvariantCulture).ToString(matterCenterDateFormat, CultureInfo.InvariantCulture),
                                    conflictIdentified) + string.Format(CultureInfo.InvariantCulture,
                                    matterMailBodyTeamMembers, secureMatter, mailBodyTeamInformation,
                                    blockUserNames, client.Url, oneNotePath, matter.Name, originalMatter.MatterLocation, matter.Name);

                            }
                            else
                            {
                                blockUserNames = string.Empty;
                                matterMailBody = string.Format(CultureInfo.InvariantCulture, matterMailBodyMatterInformation,
                                    client.Name, client.Id, matter.Name, matter.Id,
                                    matter.Description, matterType) + string.Format(CultureInfo.InvariantCulture, matterMailBodyTeamMembers, secureMatter,
                                    mailBodyTeamInformation, blockUserNames, client.Url, oneNotePath, matter.Name, originalMatter.MatterLocation, matter.Name);
                            } 

                            EmailMessage email = new EmailMessage(service);
                            foreach(IList<string> userNames  in matter.AssignUserEmails)
                            {
                                foreach(string userName in userNames)
                                {
                                    if(!string.IsNullOrWhiteSpace(userName))
                                    {
                                        email.ToRecipients.Add(userName);
                                    }
                                }
                            }                            
                            email.From = new EmailAddress("matteradmin@msmatter.onmicrosoft.com");
                            email.Subject = matterMailSubject;
                            email.Body = matterMailBody;
                            email.Send();
                            Utility.UpdateTableStorageEntity(originalMatter, log, configuration["Data:DefaultConnection:AzureStorageConnectionString"],
                                            configuration["Settings:MatterRequests"], "Accepted");
                        }
                    }
                }
            }
        }

        



        /// <summary>
        /// Provides the team members and their respective permission details.
        /// </summary>
        /// <param name="matterDetails">Matter Details object</param>
        /// <param name="mailBodyTeamInformation">Team members permission information</param>
        /// <returns>Team members permission information</returns>
        private static string TeamMembersPermissionInformation(MatterDetails matterDetails, string mailBodyTeamInformation)
        {
            if (null != matterDetails && !string.IsNullOrWhiteSpace(matterDetails.RoleInformation))
            {
                Dictionary<string, string> roleInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(matterDetails.RoleInformation);

                foreach (KeyValuePair<string, string> entry in roleInformation)
                {
                    mailBodyTeamInformation = string.Format(CultureInfo.InvariantCulture, ServiceConstants.RoleInfoHtmlChunk, entry.Key, entry.Value) +
                        mailBodyTeamInformation;
                }
            }
            return mailBodyTeamInformation;
        }

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
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json")
                    .AddEnvironmentVariables();//appsettings.json will be overridden with azure web appsettings
                var configuration = builder.Build();
                //Read all rows from table storage which are in pending state
                var query = from p in matterInformationVM select p;
                foreach (MatterInformationVM matterInformation in query)
                {
                    if (matterInformation != null)
                    {
                        if (matterInformation.Status.ToLower() == "pending")
                        {
                            string serializedMatter = matterInformation.SerializeMatter;
                            //De Serialize the matter information
                            MatterInformationVM originalMatter = Newtonsoft.Json.JsonConvert.DeserializeObject<MatterInformationVM>(serializedMatter);
                            log.WriteLine($"Checking the matter name {originalMatter.Matter.Name} has been acceped by the user or not");
                            //Read all external access requests records from azure table storge
                            GetExternalAccessRequestsFromSPO(originalMatter, log, configuration);
                        }
                    }
                }
            }
            catch (Exception ex)
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
                            SecureString password = Utility.GetEncryptedPassword(configuration["Settings:AdminPassword"]);
                            ctx.Credentials = new SharePointOnlineCredentials(configuration["Settings:AdminUserName"], password);
                            //First check whether the user exists in SharePoint or not
                            log.WriteLine($"Checking whether the user {email} has been present in the system or not");
                            if (CheckUserPresentInMatterCenter(ctx, originalMatter.Client.Url, email, configuration, log) == true)
                            {
                                
                                string requestedForPerson = email;
                                string matterId = originalMatter.Matter.MatterGuid;
                                var listTitle = configuration["Settings:ExternalAccessRequests"];
                                var list = ctx.Web.Lists.GetByTitle(listTitle);
                                CamlQuery camlQuery = CamlQuery.CreateAllItemsQuery();
                                camlQuery.ViewXml = "";
                                ListItemCollection listItemCollection = list.GetItems(camlQuery);
                                ctx.Load(listItemCollection);
                                ctx.ExecuteQuery();
                                log.WriteLine($"Looping all the records from {configuration["Settings:ExternalAccessRequests"]} lists");
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
                                        log.WriteLine($"The user {email} has been present in the system and he has accepted the invitation and providing permssions to  matter {originalMatter.Matter.Name} from the user {email}");
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
                                        log.WriteLine($"The matter permissions has been updated for the user {email}");
                                        log.WriteLine($"Updating the matter status to Accepted in Azure Table Storage");
                                        Utility.UpdateTableStorageEntity(originalMatter, log, configuration["Data:DefaultConnection:AzureStorageConnectionString"], 
                                            configuration["Settings:TableStorageForExternalRequests"], "Accepted");
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
    }
}
