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
        private IUsersDetails userDetails;
        public ExternalSharing(ISPOAuthorization spoAuthorization, IOptions<ListNames> listNames, 
            IOptions<GeneralSettings> generalSettings, 
            IOptions<MatterSettings> matterSettings, 
            IOptions<LogTables> logTables, IUsersDetails userDetails)
        {
            this.spoAuthorization = spoAuthorization;
            this.listNames = listNames.Value;
            this.generalSettings = generalSettings.Value;
            this.matterSettings = matterSettings.Value;
            this.logTables = logTables.Value;
            this.userDetails = userDetails;
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
                    if (!string.IsNullOrWhiteSpace(email) && userDetails.CheckUserPresentInMatterCenter(generalSettings.SiteURL, email) == false)
                    {                        
                        //If not, store external request in a list
                        SaveExternalSharingRequest(matterInformation);                       
                        //Send notification to the user with appropriate information
                        SendExternalNotification(matterInformation, 
                            matterInformation.Matter.Permissions[index], 
                            matterInformation.Matter.AssignUserEmails[index][0]);
                    }
                    
                }
                index = index + 1;
            }     
            return null;
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
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
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
                //Need to use MatterGuid instead of Id
                string matterLandingPageUrl = $"{clientUrl}/sitepages/{matterInformation.Matter.MatterGuid + ServiceConstants.ASPX_EXTENSION}";
                string catalogSiteAssetsLibraryUrl = $"{generalSettings.CentralRepositoryUrl}/SitePages/home.aspx";
                using (var clientContext = spoAuthorization.GetClientContext(clientUrl))
                {
                    //Send notification to the matter landing page url with permission the user has selected
                    IList<UserSharingResult> matterLandingPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                    matterLandingPageUrl,
                    users, true, true, true, "The following matter page has been shared with you", true, true);
                    clientContext.ExecuteQuery();
                }                
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
