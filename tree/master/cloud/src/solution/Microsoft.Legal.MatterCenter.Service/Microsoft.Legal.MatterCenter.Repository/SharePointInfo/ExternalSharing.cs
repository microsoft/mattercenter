using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Globalization;
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client.Utilities;
namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ExternalSharing:IExternalSharing
    {
        private ISPOAuthorization spoAuthorization;
        private ListNames listNames;
        private GeneralSettings generalSettings;
        private MatterSettings matterSettings;
        private LogTables logTables;
        public ExternalSharing(ISPOAuthorization spoAuthorization, IOptionsMonitor<ListNames> listNames, 
            IOptionsMonitor<GeneralSettings> generalSettings, IOptionsMonitor<MatterSettings> matterSettings, IOptionsMonitor<LogTables> logTables)
        {
            this.spoAuthorization = spoAuthorization;
            this.listNames = listNames.CurrentValue;
            this.generalSettings = generalSettings.CurrentValue;
            this.matterSettings = matterSettings.CurrentValue;
            this.logTables = logTables.CurrentValue;
        }
        /// <summary>
        /// This method will store the external sharing request in a list called "MatterCenterExternalRequests"
        /// and send notification to the external user regarding the information that is getting shared
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        public GenericResponseVM ShareMatter(MatterInformationVM matterInformation)
        {
            var tempMatterInformation = matterInformation;
            int index = 0;
            foreach (var assignUserEmails in matterInformation.Matter.AssignUserEmails)
            {
                
                foreach (string email in assignUserEmails)
                {
                    //First check whether the user exists in SharePoint or not
                    if (CheckUserPresentInMatterCenter(generalSettings.SiteURL, email) == false)
                    {                        
                        //If not, store external request in a list
                        SaveExternalSharingRequest(matterInformation);                       
                        //Send notification to the user with appropriate information
                        SendExternalNotification(matterInformation, matterInformation.Matter.Permissions[index], matterInformation.Matter.AssignUserEmails[index][0]);
                    }
                    
                }
                index = index + 1;
            }     
            return null;
        }

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private bool CheckUserPresentInMatterCenter(string clientUrl,string email)
        {
            try
            {
                var clientContext = spoAuthorization.GetClientContext(clientUrl);                
                string userAlias = email;
                ClientPeoplePickerQueryParameters queryParams = new ClientPeoplePickerQueryParameters();
                queryParams.AllowMultipleEntities = false;
                queryParams.MaximumEntitySuggestions = 500;
                queryParams.PrincipalSource = PrincipalSource.All;
                queryParams.PrincipalType = PrincipalType.User | PrincipalType.SecurityGroup;
                queryParams.QueryString = userAlias; 
                ClientResult<string> clientResult = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(clientContext, queryParams);
                clientContext.ExecuteQuery();
                string results = clientResult.Value;
                int peoplePickerMaxRecords = 30;
                IList<PeoplePickerUser> foundUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeoplePickerUser>>(results).Where(result => (string.Equals(result.EntityType, ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER,
                        StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.Description)) || (!string.Equals(result.EntityType,
                        ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.EntityData.Email))).Take(peoplePickerMaxRecords).ToList();
                return foundUsers.Count>0;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will store external requests information in Azure Table Storage
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private void SaveExternalSharingRequest(MatterInformationVM matterInformation)
        {            
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(logTables.CloudStorageConnectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions = new TableRequestOptions
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(logTables.ExternalAccessRequests);                
                // Create the table if it doesn't exist.
                table.CreateIfNotExists();
                //Insert the entity into Table Storage              
                matterInformation.PartitionKey = matterInformation.Matter.Name;
                matterInformation.RowKey = $"{Guid.NewGuid().ToString()}${matterInformation.Matter.Id}";
                matterInformation.Status = "Pending";
                string matterInformationObject = Newtonsoft.Json.JsonConvert.SerializeObject(matterInformation);
                matterInformation.SerializeMatter = matterInformationObject;                
                TableOperation insertOperation = TableOperation.Insert(matterInformation);
                table.Execute(insertOperation);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will send notifications to external users
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private GenericResponseVM SendExternalNotification(MatterInformationVM matterInformation, string permission, string externalEmail)
        {
            var clientUrl = $"{matterInformation.Client.Url}";
            try
            {                
                var users = new List<UserRoleAssignment>();
                UserRoleAssignment userRole = new UserRoleAssignment();
                userRole.UserId = externalEmail;
                switch (permission.ToLower())
                {
                    case "full control":
                        userRole.Role = SharePoint.Client.Sharing.Role.Owner;
                        break;
                    case "contribute":
                        userRole.Role = SharePoint.Client.Sharing.Role.Edit;
                        break;
                    case "read":
                        userRole.Role = SharePoint.Client.Sharing.Role.View;
                        break;
                }
                users.Add(userRole);                
                #region Doc Sharing API
                string matterLandingPageUrl = $"{clientUrl}/sitepages/{matterInformation.Matter.Id + ServiceConstants.ASPX_EXTENSION}";
                string catalogSiteAssetsLibraryUrl = $"{generalSettings.CentralRepositoryUrl}/SitePages/home.aspx";
                using (var clientContext = spoAuthorization.GetClientContext(clientUrl))
                {
                    //Send notification to the matter landing page url with permission the user has selected
                    IList<UserSharingResult> matterLandingPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                    matterLandingPageUrl,
                    users, true, true, true, "The following matter page has been shared with you", true, true);
                    clientContext.ExecuteQuery();
                }
                //Need to send notification to one catalog page so that user can be added later to sharepoint groups which will be used when 
                //rendering the matter landing page to external user
                //users = new List<UserRoleAssignment>();
                //userRole = new UserRoleAssignment();
                //userRole.UserId = externalEmail;
                //userRole.Role = SharePoint.Client.Sharing.Role.Owner;
                //users.Add(userRole);
                //using (var clientContext = spoAuthorization.GetClientContext(generalSettings.CentralRepositoryUrl))
                //{
                //    IList<UserSharingResult> catalogSiteAssetsLibrary = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                //    catalogSiteAssetsLibraryUrl,
                //    users, true, true, true, "The following catalog page has been shared with you", true, true);
                //    clientContext.ExecuteQuery();
                //}
                return null;
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }    
}
