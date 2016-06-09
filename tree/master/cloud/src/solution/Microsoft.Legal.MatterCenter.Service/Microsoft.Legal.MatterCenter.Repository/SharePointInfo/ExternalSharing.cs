using Microsoft.Extensions.OptionsModel;
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

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ExternalSharing:IExternalSharing
    {
        private ISPOAuthorization spoAuthorization;
        private ListNames listNames;
        private GeneralSettings generalSettings;
        private MatterSettings matterSettings;
        private LogTables logTables;
        public ExternalSharing(ISPOAuthorization spoAuthorization, IOptions<ListNames> listNames, 
            IOptions<GeneralSettings> generalSettings, IOptions<MatterSettings> matterSettings, IOptions<LogTables> logTables)
        {
            this.spoAuthorization = spoAuthorization;
            this.listNames = listNames.Value;
            this.generalSettings = generalSettings.Value;
            this.matterSettings = matterSettings.Value;
            this.logTables = logTables.Value;
        }
        /// <summary>
        /// This method will store the external sharing request in a list called "MatterCenterExternalRequests"
        /// and send notification to the external user regarding the information that is getting shared
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        public GenericResponseVM ShareMatter(ExternalSharingRequest externalSharingRequest)
        {           
            
            //First check whether the user exists in SharePoint or not
            if (CheckUserPresentInMatterCenter(externalSharingRequest) == false)
            {
                //If not, store external request in a list
                SaveExternalSharingRequest(externalSharingRequest);
                //Prepare message body for the email notification
                //Send notification to the user with appropriate information
                SendExternalNotification(externalSharingRequest);
            }
           
            return null;
        }

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private bool CheckUserPresentInMatterCenter(ExternalSharingRequest externalSharingRequest)
        {
            try
            {                
                var context = spoAuthorization.GetClientContext(externalSharingRequest.Client.Url);
                var siteUsers = context.Web.SiteUsers;
                string userAlias = externalSharingRequest.Person;
                var usersResult = context.LoadQuery(siteUsers.Include(u => u.Email).Where(u => u.Email == userAlias));
                context.ExecuteQuery();
                return usersResult.Any();
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
        private void SaveExternalSharingRequest(ExternalSharingRequest externalSharingRequest)
        {            
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(logTables.CloudStorageConnectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(logTables.ExternalAccessRequests);                
                // Create the table if it doesn't exist.
                table.CreateIfNotExists();
                //Insert the entity into Table Storage
                string date = DateTime.Now.ToUniversalTime().ToString(logTables.AzureRowKeyDateFormat, CultureInfo.InvariantCulture);
                externalSharingRequest.RowKey = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", date, Guid.NewGuid().ToString());
                externalSharingRequest.PartitionKey = externalSharingRequest.MatterId;
                TableOperation insertOperation = TableOperation.Insert(externalSharingRequest);
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
        private GenericResponseVM SendExternalNotification(ExternalSharingRequest externalSharingRequest)
        {
            var clientUrl = $"{generalSettings.SiteURL}/sites/{externalSharingRequest.ClientName}";
            try
            {
                var clientContext = spoAuthorization.GetClientContext(generalSettings.SiteURL + "/sites/" + externalSharingRequest.ClientName);   
                var users = new List<UserRoleAssignment>();
                UserRoleAssignment userRole = new UserRoleAssignment();
                userRole.UserId = externalSharingRequest.Person;                
                switch(externalSharingRequest.Permission.ToLower())
                {
                    case "full control":
                        userRole.Role = SharePoint.Client.Sharing.Role.Owner;
                        break;
                    case "contribute":
                        userRole.Role = SharePoint.Client.Sharing.Role.Edit;
                        break;
                    case "edit":
                        userRole.Role = SharePoint.Client.Sharing.Role.View;
                        break;
                }                
                users.Add(userRole);
                //Share all the matter related documents and lists to external user
                #region Doc Sharing API
                string matterDocumentUrl = $"{clientUrl}/{externalSharingRequest.MatterId}";
                string matterOneNoteUrl = $"{clientUrl}/{externalSharingRequest.MatterId+ matterSettings.OneNoteLibrarySuffix}";
                string matterCalendarUrl = $"{clientUrl}/{externalSharingRequest.MatterId + matterSettings.CalendarNameSuffix}";
                string matterTasksUrl = $"{clientUrl}/lists{externalSharingRequest.MatterId + matterSettings.TaskNameSuffix}";
                string matterLandingPageUrl = $"{clientUrl}/sitepages/{externalSharingRequest.MatterId + ServiceConstants.ASPX_EXTENSION}";

                IList<UserSharingResult> matterLandingPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterLandingPageUrl,
                users, true, true, true, "The following matter page has been shared with you", true, true);
                clientContext.ExecuteQuery();
                IList<UserSharingResult> documentPageResult =  DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterDocumentUrl,
                users, true, true, false, "The following document library has been shared with you", true, true);
                clientContext.ExecuteQuery();
                IList<UserSharingResult> oneNoteSharingResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterOneNoteUrl,
                users, true, true, false, "The following one note document has been shared with you", true, true);
                clientContext.ExecuteQuery();
                //#endregion
                //#region List sharing api
                string roleValue = ""; // int depends on the group IDs at site
                int groupId = 0;
                bool propageAcl = true; // Not relevant for external accounts
                bool sendEmail = true;
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
                                                                'Email' : '@', 
                                                                'Department' : '', 
                                                                'Title' : '@', 
                                                                'PrincipalType' : 'GUEST_USER'}, 
                                            'MultipleMatches' : []}]";
                peoplePickerInput = peoplePickerInput.Replace("^", email);
                peoplePickerInput = peoplePickerInput.Replace("@", externalSharingRequest.Person);
                SharingResult calendarResult = Web.ShareObject(clientContext, matterCalendarUrl, peoplePickerInput, roleValue,
                groupId, propageAcl, sendEmail, includedAnonymousLinkInEmail, emailSubject, emailBody, true);
                clientContext.Load(calendarResult);
                clientContext.ExecuteQuery();
                SharingResult taskResult = Web.ShareObject(clientContext, matterTasksUrl, peoplePickerInput, roleValue,
                groupId, propageAcl, sendEmail, includedAnonymousLinkInEmail, emailSubject, emailBody, true);
                clientContext.Load(calendarResult);
                clientContext.ExecuteQuery();
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
