using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;

using System;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Reflection;
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
        private ICustomLogger customLogger;
        public ExternalSharing(ISPOAuthorization spoAuthorization, IOptions<ListNames> listNames, 
            IOptions<GeneralSettings> generalSettings, 
            IOptions<MatterSettings> matterSettings, 
            IOptions<LogTables> logTables, IUsersDetails userDetails, ICustomLogger customLogger)
        {
            this.spoAuthorization = spoAuthorization;
            this.listNames = listNames.Value;
            this.generalSettings = generalSettings.Value;
            this.matterSettings = matterSettings.Value;
            this.logTables = logTables.Value;
            this.userDetails = userDetails;
            this.customLogger = customLogger;

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
                matterInformation.MatterUpdateStatus = "Pending";
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
                string roleId = "";                
                switch (permission.ToLower())
                {
                    case "full control":                        
                        roleId = ((int)SPORoleIdMapping.FullControl).ToString();
                        break;
                    case "contribute":                        
                        roleId = ((int)SPORoleIdMapping.Contribute).ToString();
                        break;
                    case "read":                        
                        roleId = ((int)SPORoleIdMapping.Read).ToString();
                        break;
                }               

                PeoplePickerUser peoplePickerUser = new PeoplePickerUser();
                peoplePickerUser.Key = externalEmail;
                peoplePickerUser.Description = externalEmail;
                peoplePickerUser.DisplayText = externalEmail;
                peoplePickerUser.EntityType = "";
                peoplePickerUser.ProviderDisplayName = "";
                peoplePickerUser.ProviderName = "";
                peoplePickerUser.IsResolved = true;

                EntityData entityData = new EntityData();
                entityData.SPUserID = externalEmail;
                entityData.Email = externalEmail;
                entityData.IsBlocked = "False";
                entityData.PrincipalType = "UNVALIDATED_EMAIL_ADDRESS";
                entityData.AccountName = externalEmail;
                entityData.SIPAddress = externalEmail;
                peoplePickerUser.EntityData = entityData;

                string peoplePicker = Newtonsoft.Json.JsonConvert.SerializeObject(peoplePickerUser);
                peoplePicker = $"[{peoplePicker}]";
                string roleValue = $"role:{roleId}";

                bool sendEmail = true;
                
                bool includeAnonymousLinkInEmail = false;
                bool propagateAcl = false;
                bool useSimplifiedRoles = true;
                string matterLandingPageUrl = $"{clientUrl}/sitepages/{matterInformation.Matter.MatterGuid + ServiceConstants.ASPX_EXTENSION}";
                string url = matterLandingPageUrl;
                string emailBody = $"The following information has been shared with you {matterInformation.Matter.Name}";
                string emailSubject = $"The following information has been shared with you {matterInformation.Matter.Name}";
                #region Doc Sharing API                
                SharingResult sharingResult = null;                
                using (var clientContext = spoAuthorization.GetClientContext(clientUrl))
                {
                    sharingResult = Web.ShareObject(clientContext, url, peoplePicker, roleValue, 0, propagateAcl, sendEmail, includeAnonymousLinkInEmail, emailSubject, emailBody, useSimplifiedRoles);                    
                    clientContext.Load(sharingResult);
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
