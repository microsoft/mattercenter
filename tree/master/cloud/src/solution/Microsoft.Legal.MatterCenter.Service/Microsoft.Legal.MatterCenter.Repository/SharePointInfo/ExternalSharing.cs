using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ExternalSharing:IExternalSharing
    {
        private ISPOAuthorization spoAuthorization;
        private ListNames listNames;
        private GeneralSettings generalSettings;
        private MatterSettings matterSettings;
        public ExternalSharing(ISPOAuthorization spoAuthorization, IOptions<ListNames> listNames, 
            IOptions<GeneralSettings> generalSettings, IOptions<MatterSettings> matterSettings)
        {
            this.spoAuthorization = spoAuthorization;
            this.listNames = listNames.Value;
            this.generalSettings = generalSettings.Value;
            this.matterSettings = matterSettings.Value;
        }
        /// <summary>
        /// This method will store the external sharing request in a list called "MatterCenterExternalRequests"
        /// and send notification to the external user regarding the information that is getting shared
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        public GenericResponseVM ShareMatter(ExternalSharingRequest externalSharingRequest)
        {
            IList<ExternalUserInfo> externalUsers = externalSharingRequest.ExternalUserInfoList;
            foreach (ExternalUserInfo userInfo in externalUsers)
            {
                //First check whether the user exists in SharePoint or not
                if (CheckUserPresentInMatterCenter(externalSharingRequest, userInfo) == false)
                {
                    //If not, store external request in a list
                    SaveExternalSharingRequest(externalSharingRequest, userInfo);
                    //Prepare message body for the email notification
                    //Send notification to the user with appropriate information
                    SendExternalNotification(externalSharingRequest, userInfo);
                }
            }
            return null;

        }

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private bool CheckUserPresentInMatterCenter(ExternalSharingRequest externalSharingRequest, ExternalUserInfo userInfo)
        {
            try
            {
                
                var context = spoAuthorization.GetClientContext(externalSharingRequest.Client.Url);
                var siteUsers = context.Web.SiteUsers;
                string userAlias = userInfo.Person;
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
        /// 
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        private void SaveExternalSharingRequest(ExternalSharingRequest externalSharingRequest, ExternalUserInfo userInfo)
        {
            
            try
            {
                var clientContext = spoAuthorization.GetClientContext(generalSettings.CentralRepositoryUrl);
                var list = clientContext.Web.Lists.GetByTitle(listNames.MatterCenterExternalRequests);
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = list.AddItem(itemCreateInfo);
                newItem["Person"] = userInfo.Person;
                newItem["Permission"] = userInfo.Permission;
                newItem["Role"] = userInfo.Role;
                newItem["Status"] = userInfo.Status;
                newItem["MatterId"] = externalSharingRequest.MatterId;
                newItem.Update();
                clientContext.ExecuteQuery();              
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
        private GenericResponseVM SendExternalNotification(ExternalSharingRequest externalSharingRequest, ExternalUserInfo userInfo)
        {
            var clientUrl = $"{generalSettings.SiteURL}/sites/{externalSharingRequest.ClientName}";
            try
            {
                var clientContext = spoAuthorization.GetClientContext(generalSettings.SiteURL + "/sites/" + externalSharingRequest.ClientName);   
                var users = new List<UserRoleAssignment>();
                UserRoleAssignment userRole = new UserRoleAssignment();
                userRole.UserId = userInfo.Person;
                
                switch(userInfo.Permission.ToLower())
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

                DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterDocumentUrl,
                users, true, true, true, "The following document library has been shared with you", true, true);
                clientContext.ExecuteQuery();


                IList<UserSharingResult> oneNoteSharingResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterOneNoteUrl,
                users, true, true, true, "The following document library has been shared with you", true, true);
                clientContext.ExecuteQuery();


                IList<UserSharingResult> matterLandingPageResult = DocumentSharingManager.UpdateDocumentSharingInfo(clientContext,
                matterLandingPageUrl,
                users, true, true, true, "The following document library has been shared with you", true, true);
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
                var email = userInfo.Person.Replace('@', '_');
                PeoplePickerUser peoplePickerUser = new PeoplePickerUser()
                {
                    Key = "i:0#.f|membership|" + email + "#ext#@" + generalSettings.Tenant,
                    Description = email + "#ext#@" + generalSettings.Tenant,
                    DisplayText = userInfo.Person,
                    EntityType = "User",
                    ProviderDisplayName= "Tenant",
                    ProviderName = "Tenant",
                    EntityData = new EntityData()
                    {
                        Department = "",
                        Email = userInfo.Person,
                        Title = userInfo.Person,
                        PrincipalType = "GUEST_USER"
                    }
                };
                string jsonResult = JsonConvert.SerializeObject(peoplePickerUser, Formatting.Indented);
                SharingResult calendarResult = Web.ShareObject(clientContext, matterCalendarUrl, jsonResult, roleValue,
                groupId, propageAcl, sendEmail, includedAnonymousLinkInEmail, emailSubject, emailBody, true);
                clientContext.Load(calendarResult);
                clientContext.ExecuteQuery();

                SharingResult taskResult = Web.ShareObject(clientContext, matterTasksUrl, jsonResult, roleValue,
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

        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string MessageBodyForExternalRequest(ExternalSharingRequest externalSharingRequest)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
