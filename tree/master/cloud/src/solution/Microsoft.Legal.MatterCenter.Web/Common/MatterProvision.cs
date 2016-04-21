using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Service
{
    public class MatterProvision:IMatterProvision
    {
        private MatterSettings matterSettings;
        private IMatterRepository matterRepositoy;
        private ISPOAuthorization spoAuthorization;
        private IEditFunctions editFunctions;
        private ErrorSettings errorSettings;
        public MatterProvision(IMatterRepository matterRepositoy, IOptions<MatterSettings> matterSettings, IOptions<ErrorSettings> errorSettings,
            ISPOAuthorization spoAuthorization, IEditFunctions editFunctions)
        {
            this.matterRepositoy = matterRepositoy;
            this.matterSettings = matterSettings.Value;
            this.spoAuthorization = spoAuthorization;
            this.editFunctions = editFunctions;
            this.errorSettings = errorSettings.Value;
        }


        public GenericResponseVM UpdateMatter(MatterInformationVM matterInformation)
        {
            var matter = matterInformation.Matter;
            var matterDetails = matterInformation.MatterDetails;
            var client = matterInformation.Client;
            int listItemId = -1;
            string loggedInUserName = "";
            bool isEditMode = matterInformation.EditMode;
            ClientContext clientContext = null;
            IEnumerable<RoleAssignment> userPermissionOnLibrary = null;
            GenericResponseVM genericResponse = null;
            try
            {                
                clientContext = spoAuthorization.GetClientContext(matterInformation.Client.Url);
                PropertyValues matterStampedProperties = matterRepositoy.GetStampedProperties(clientContext, matter.Name);
                loggedInUserName = matterRepositoy.GetLoggedInUserDetails(clientContext).Name;
                bool isFullControlPresent = editFunctions.ValidateFullControlPermission(matter);
                
                if (!isFullControlPresent)
                {
                    return ServiceUtility.GenericResponse(errorSettings.IncorrectInputSelfPermissionRemoval, errorSettings.ErrorEditMatterMandatoryPermission);
                }               
                
                // Get matter library current permissions
                userPermissionOnLibrary = matterRepositoy.FetchUserPermissionForLibrary(clientContext, matter.Name);
                string originalMatterName = matterRepositoy.GetMatterName(clientContext, matter.Name);
                listItemId = matterRepositoy.RetrieveItemId(clientContext, matterSettings.MatterLandingPageRepositoryName, originalMatterName);
                List<string> usersToRemove = RetrieveMatterUsers(userPermissionOnLibrary);
                bool hasFullPermission = CheckFullPermissionInAssignList(matter.AssignUserNames, matter.Permissions, loggedInUserName);
                List<string> listExists = matterRepositoy.MatterAssociatedLists(clientContext, matter.Name);
                matterRepositoy.AssignRemoveFullControl(clientContext, matter, loggedInUserName, listItemId, listExists, true, hasFullPermission);
                bool result = false;
                if (listExists.Contains(matter.Name))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.OneNoteLibrarySuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.OneNoteLibrarySuffix, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.CalendarNameSuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.CalendarNameSuffix, -1, isEditMode);
                }
                if (listExists.Contains(matter.Name + matterSettings.TaskNameSuffix))
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + matterSettings.TaskNameSuffix, -1, isEditMode);
                }
                if (0 <= listItemId)
                {
                    result = matterRepositoy.UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, true, matterSettings.MatterLandingPageRepositoryName, listItemId, isEditMode);
                }
                // Update matter metadata
                result = matterRepositoy.UpdateMatterStampedProperties(clientContext, matterDetails, matter, matterStampedProperties, isEditMode);
                if(result)
                {
                    return genericResponse;
                }
            }
            catch(Exception ex)
            {
                MatterRevertList matterRevertListObject = new MatterRevertList()
                {
                    MatterLibrary = matter.Name,
                    MatterOneNoteLibrary = matter.Name + matterSettings.OneNoteLibrarySuffix,
                    MatterCalendar = matter.Name + matterSettings.CalendarNameSuffix,
                    MatterTask = matter.Name + matterSettings.TaskNameSuffix,
                    MatterSitePages = matterSettings.MatterLandingPageRepositoryName
                };
                matterRepositoy.RevertMatterUpdates(client, matter, clientContext, matterRevertListObject, loggedInUserName, 
                    userPermissionOnLibrary, listItemId, isEditMode);                
            }
            return ServiceUtility.GenericResponse("9999999", "Error in updating matter information");
        }


        public GenericResponseVM CreateMatter()
        {
            return null;
        }

        #region private functions
        /// <summary>
        /// Gets the display name of users having permission on library.
        /// </summary>
        /// <param name="userPermissionOnLibrary">Users having permission on library</param>
        /// <returns></returns>
        internal List<string> RetrieveMatterUsers(IEnumerable<RoleAssignment> userPermissionOnLibrary)
        {
            List<string> users = new List<string>();
            try
            {
                if (null != userPermissionOnLibrary && 0 < userPermissionOnLibrary.Count())
                {
                    foreach (RoleAssignment roles in userPermissionOnLibrary)
                    {
                        users.Add(roles.Member.Title);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return users;
        }

        /// <summary>
        /// Check Full Permission for logged in User.
        /// </summary>
        /// <param name="AssignUserNames">List of Assigned UserNames</param>
        /// <param name="Permissions">List of Permission</param>
        /// <param name="loggedInUserName">Name of logged in User</param>
        /// <returns>Status of Full Permission</returns>
        internal bool CheckFullPermissionInAssignList(IList<IList<string>> AssignUserNames, IList<string> Permissions, string loggedInUserName)
        {
            bool result = false;
            if (null != Permissions && null != AssignUserNames && Permissions.Count == AssignUserNames.Count)
            {
                int position = 0;
                foreach (string roleName in Permissions)
                {
                    IList<string> assignUserNames = AssignUserNames[position];
                    if (!string.IsNullOrWhiteSpace(roleName) && null != assignUserNames)
                    {
                        foreach (string user in assignUserNames)
                        {
                            if (!string.IsNullOrWhiteSpace(user) && user.Trim().Equals(loggedInUserName.Trim()))
                            {
                                if (roleName == matterSettings.EditMatterAllowedPermissionLevel)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    position++;
                }
                return result;
            }
            return result;
        }
        #endregion
    }
}
