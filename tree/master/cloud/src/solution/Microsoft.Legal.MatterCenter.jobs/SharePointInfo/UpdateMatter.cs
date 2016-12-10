using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Net;
using Microsoft.Extensions.Configuration;
namespace Microsoft.Legal.MatterCenter.Jobs
{
    public class UpdateMatter
    {

        public GenericResponseVM CheckSecurityGroupInTeamMembers(ClientContext clientContext, Matter matter, IList<string> userId)
        {
            try
            {
                GenericResponseVM genericResponse = null;
                int securityGroupRowNumber = -1; // Blocked user field has security group
                List<Tuple<int, Principal>> teamMemberPrincipalCollection = new List<Tuple<int, Principal>>();
                if (null != matter && null != matter.AssignUserNames && null != matter.BlockUserNames)
                {
                    teamMemberPrincipalCollection = SPList.CheckUserSecurity(clientContext, matter, userId);
                    foreach (Tuple<int, Principal> teamMemberPrincipal in teamMemberPrincipalCollection)
                    {
                        Principal currentTeamMemberPrincipal = teamMemberPrincipal.Item2;
                        if (currentTeamMemberPrincipal.PrincipalType == SharePoint.Client.Utilities.PrincipalType.SecurityGroup)
                        {
                            securityGroupRowNumber = teamMemberPrincipal.Item1;
                            //return ServiceUtility.GenericResponse(errorSettings.ErrorCodeSecurityGroupExists,
                            //    errorSettings.ErrorMessageSecurityGroupExists + ServiceConstants.DOLLAR + userId[securityGroupRowNumber]);
                            return new GenericResponseVM() {
                                Code = "",
                                Value=""
                            };
                        }
                    }
                }
                else
                {
                    //return ServiceUtility.GenericResponse(errorSettings.IncorrectTeamMembersCode,
                    //                errorSettings.IncorrectTeamMembersMessage);
                    return new GenericResponseVM()
                    {
                        Code = "",
                        Value = ""
                    };
                }
                return genericResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateUserPermissionsForMatter(MatterInformationVM matterInformation,
            IConfigurationRoot configuration, System.Security.SecureString securePassword)
        {
            var matter = matterInformation.Matter;
            var matterDetails = matterInformation.MatterDetails;
            var client = matterInformation.Client;
            int listItemId = -1;
            string loggedInUserName = "";
            bool isEditMode = matterInformation.EditMode;
            ClientContext clientContext = null;
            IEnumerable<RoleAssignment> userPermissionOnLibrary = null;
            //GenericResponseVM genericResponse = null;
            try
            {
                clientContext = new ClientContext(matterInformation.Client.Url);
                clientContext.Credentials = new SharePointOnlineCredentials(configuration["General:AdminUserName"], securePassword);

                //if (null != matter.Conflict && !string.IsNullOrWhiteSpace(matter.Conflict.Identified))
                //{
                //    if (Convert.ToBoolean(matter.Conflict.Identified, System.Globalization.CultureInfo.InvariantCulture))
                //    {
                //        genericResponse = CheckSecurityGroupInTeamMembers(clientContext, matter, matterInformation.UserIds);
                //    }
                //}
                //else
                //{
                //    //genericResponse = string.Format(System.Globalization.CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, TextConstants.IncorrectInputConflictIdentifiedCode, TextConstants.IncorrectInputConflictIdentifiedMessage);
                //    return;
                //}
                //if (genericResponse == null)
                //{
                    PropertyValues matterStampedProperties = SPList.GetListProperties(clientContext, matter.Name);
                    loggedInUserName = SPList.GetLoggedInUserDetails(clientContext).Name;
                    // Get matter library current permissions
                    userPermissionOnLibrary = SPList.FetchUserPermissionForLibrary(clientContext, matter.Name);
                    string originalMatterName = SPList.GetMatterName(clientContext, matter.Name);
                    listItemId = SPList.RetrieveItemId(clientContext, "Site Pages", originalMatterName);
                    List<string> usersToRemove = RetrieveMatterUsers(userPermissionOnLibrary);
                    bool hasFullPermission = CheckFullPermissionInAssignList(matter.AssignUserNames, matter.Permissions, loggedInUserName);
                    List<string> listExists = MatterAssociatedLists(clientContext, matter.Name);
                    AssignRemoveFullControl(clientContext, matter, loggedInUserName, listItemId, listExists, true, hasFullPermission);
                    bool result = false;
                    if (listExists.Contains(matter.Name))
                    {
                        result = UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name, -1, isEditMode);
                    }
                    if (listExists.Contains(matter.Name + configuration["Matter:OneNoteLibrarySuffix"]))
                    {
                        result = UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + configuration["Matter:OneNoteLibrarySuffix"], -1, isEditMode);
                    }
                    if (listExists.Contains(matter.Name + configuration["Matter:CalendarNameSuffix"]))
                    {
                        result = UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + configuration["Matter:CalendarNameSuffix"], -1, isEditMode);
                    }
                    if (listExists.Contains(matter.Name + configuration["Matter:TaskNameSuffix"]))
                    {
                        result = UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, false, matter.Name + configuration["Matter:TaskNameSuffix"], -1, isEditMode);
                    }
                    if (0 <= listItemId)
                    {
                        result = UpdatePermission(clientContext, matter, usersToRemove, loggedInUserName, true, "Site Pages", listItemId, isEditMode);
                    }
                    // Update matter metadata
                    result = UpdateMatterStampedProperties(clientContext, matterDetails, matter, matterStampedProperties, isEditMode, configuration);
                //}

            }
            catch (Exception ex)
            {
                MatterRevertList matterRevertListObject = new MatterRevertList()
                {
                    MatterLibrary = matter.Name,
                    MatterOneNoteLibrary = matter.Name + configuration["Matter:OneNoteLibrarySuffix"],
                    MatterCalendar = matter.Name + configuration["Matter:CalendarNameSuffix"],
                    MatterTask = matter.Name + configuration["Matter:TaskNameSuffix"],
                    MatterSitePages = "Site Pages"
                };
                RevertMatterUpdates(client, matter, clientContext, matterRevertListObject, loggedInUserName,
                    userPermissionOnLibrary, listItemId, isEditMode);
            }
            //return ServiceUtility.GenericResponse("9999999", "Error in updating matter information");
        }

        public void AssignPermissionToCatalogLists(string listName, ClientContext catalogContext, string userEmail, 
            string role, IConfigurationRoot configuration)
        {
            string tempUserEmail = userEmail;            
            GroupCollection groupCollection = catalogContext.Web.SiteGroups;
            Group group = groupCollection.GetByName(configuration["General:MatterUsersGroup"].ToString());
            Principal teamMemberPrincipal = catalogContext.Web.EnsureUser(userEmail.Trim());
            catalogContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title,
                                                teamMemberPrincipalProperties => teamMemberPrincipalProperties.LoginName);
            catalogContext.ExecuteQuery();           

            UserCreationInformation userInfo = new UserCreationInformation();
            userInfo.LoginName = teamMemberPrincipal.LoginName;
            userInfo.Email = userEmail;
            group.Users.Add(userInfo);
            catalogContext.ExecuteQuery();
        }

        public bool UpdateMatterStampedProperties(ClientContext clientContext, MatterDetails matterDetails, Matter matter, 
            PropertyValues matterStampedProperties, bool isEditMode, IConfigurationRoot configuration)
        {

            try
            {
                if (null != clientContext && null != matter && null != matterDetails && (0 < matterStampedProperties.FieldValues.Count))
                {
                    Dictionary<string, string> propertyList = new Dictionary<string, string>();

                    // Get existing stamped properties
                    string stampedUsers = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyMatterCenterUsers"]);
                    string stampedUserEmails = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyMatterCenterUserEmails"]);
                    string stampedPermissions = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyMatterCenterPermissions"]);
                    string stampedRoles = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyMatterCenterRoles"]);
                    string stampedResponsibleAttorneys = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyResponsibleAttorney"]);
                    string stampedResponsibleAttorneysEmail = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyResponsibleAttorneyEmail"]);
                    string stampedTeamMembers = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyTeamMembers"]);
                    string stampedBlockedUploadUsers = GetStampPropertyValue(matterStampedProperties.FieldValues, configuration["Matter:StampedPropertyBlockedUploadUsers"]);

                    string currentPermissions = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Permissions.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentRoles = string.Join(ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, matter.Roles.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentBlockedUploadUsers = string.Join(ServiceConstants.SEMICOLON, matterDetails.UploadBlockedUsers.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentUsers = GetMatterAssignedUsers(matter);
                    string currentUserEmails = SPList.GetMatterAssignedUsersEmail(clientContext, matter);

                    string finalMatterPermissions = string.Concat(stampedPermissions, ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, currentPermissions);
                    string finalMatterRoles = string.Concat(stampedRoles, ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, currentRoles);
                    
                    string finalTeamMembers = string.Concat(stampedTeamMembers, ServiceConstants.SEMICOLON, ServiceConstants.SEMICOLON, matterDetails.TeamMembers);
                    string finalMatterCenterUsers = string.Concat(stampedUsers, ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, currentUsers);
                    string finalBlockedUploadUsers = string.Concat(stampedBlockedUploadUsers, ServiceConstants.SEMICOLON, currentBlockedUploadUsers);

                    //if(stampedUserEmails.LastIndexOf("$|$")>0)
                    //{
                    //    stampedUserEmails = stampedUserEmails.Remove(stampedUserEmails.Length - 3);
                    //}

                    string finalMatterCenterUserEmails = string.Concat(stampedUserEmails, ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR, currentUserEmails);


                    string finalResponsibleAttorneysEmail = "";
                    string finalResponsibleAttorneys = "";
                    if (matterDetails.ResponsibleAttorneyEmail!=null)
                    {
                        finalResponsibleAttorneysEmail = string.IsNullOrWhiteSpace(stampedResponsibleAttorneysEmail) || isEditMode ? matterDetails.ResponsibleAttorneyEmail : string.Concat(stampedResponsibleAttorneysEmail, ServiceConstants.SEMICOLON, matterDetails.ResponsibleAttorneyEmail);
                        finalResponsibleAttorneys = string.IsNullOrWhiteSpace(stampedResponsibleAttorneys) || isEditMode ? matterDetails.ResponsibleAttorney : string.Concat(stampedResponsibleAttorneys, ServiceConstants.SEMICOLON, matterDetails.ResponsibleAttorney);
                    }
                    else
                    {
                        finalResponsibleAttorneysEmail = stampedResponsibleAttorneysEmail;
                        finalResponsibleAttorneys = stampedResponsibleAttorneys;
                    }   

                    propertyList.Add(configuration["Matter:StampedPropertyResponsibleAttorney"], WebUtility.HtmlEncode(finalResponsibleAttorneys));
                    propertyList.Add(configuration["Matter:StampedPropertyResponsibleAttorneyEmail"], WebUtility.HtmlEncode(finalResponsibleAttorneysEmail));
                    propertyList.Add(configuration["Matter:StampedPropertyTeamMembers"], WebUtility.HtmlEncode(finalTeamMembers));
                    propertyList.Add(configuration["Matter:StampedPropertyBlockedUploadUsers"], WebUtility.HtmlEncode(finalBlockedUploadUsers));
                    propertyList.Add(configuration["Matter:StampedPropertyMatterCenterRoles"], WebUtility.HtmlEncode(finalMatterRoles));
                    propertyList.Add(configuration["Matter:StampedPropertyMatterCenterPermissions"], WebUtility.HtmlEncode(finalMatterPermissions));
                    propertyList.Add(configuration["Matter:StampedPropertyMatterCenterUsers"], WebUtility.HtmlEncode(finalMatterCenterUsers));
                    propertyList.Add(configuration["Matter:StampedPropertyMatterCenterUserEmails"], WebUtility.HtmlEncode(finalMatterCenterUserEmails));

                    SPList.SetPropertBagValuesForList(clientContext, matterStampedProperties, matter.Name, propertyList);
                    return true;
                }
            }
            catch (Exception)
            {
                throw; //// This will transfer control to catch block of parent function.
            }
            return false;
        }

        /// <summary>
        /// Converts the matter users in a form that can be stamped to library.
        /// </summary>
        /// <param name="matter">Matter object</param>
        /// <returns>Users that can be stamped</returns>
        private string GetMatterAssignedUsers(Matter matter)
        {
            string currentUsers = string.Empty;
            string separator = string.Empty;
            if (null != matter && 0 < matter.AssignUserNames.Count)
            {
                foreach (IList<string> userNames in matter.AssignUserNames)
                {
                    currentUsers += separator + string.Join(ServiceConstants.SEMICOLON, userNames.Where(user => !string.IsNullOrWhiteSpace(user)));
                    separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
                }
            }
            return currentUsers;
        }

        /// <summary>
        /// Checks if the property exists in property bag. Returns the value for the property from property bag.
        /// </summary>
        /// <param name="stampedPropertyValues">Dictionary object containing matter property bag key/value pairs</param>
        /// <param name="key">Key to check in dictionary</param>
        /// <returns>Property bag value for </returns>
        internal string GetStampPropertyValue(Dictionary<string, object> stampedPropertyValues, string key)
        {
            string result = string.Empty;
            if (stampedPropertyValues.ContainsKey(key))
            {
                result = WebUtility.HtmlDecode(Convert.ToString(stampedPropertyValues[key], System.Globalization.CultureInfo.InvariantCulture));
            }

            // This is just to check for null value in key, if exists
            return (!string.IsNullOrWhiteSpace(result)) ? result : string.Empty;
        }

        /// <summary>
        /// Reverts the permission of users from matter, OneNote, Calendar libraries and matter landing page
        /// </summary>
        /// <param name="requestObject">Request object</param>
        /// <param name="client">Client object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matterRevertListObject">MatterRevertObjectList object</param>
        /// <param name="loggedInUserTitle">Logged-in user title</param>
        /// <param name="oldUserPermissions">Old library users</param>
        /// <param name="matterLandingPageId">List item id</param>
        /// <param name="isEditMode">Add/ Edit mode</param>
        /// <returns>Status of operation</returns>
        public bool RevertMatterUpdates(Client client, Matter matter, ClientContext clientContext,
            MatterRevertList matterRevertListObject, string loggedInUserTitle, IEnumerable<RoleAssignment> oldUserPermissions,
            int matterLandingPageId, bool isEditMode)
        {
            bool result = false;
            try
            {
                if (null != client && null != matter && null != clientContext && null != matterRevertListObject)
                {
                    List<string> users = new List<string>();
                    users = matter.AssignUserNames.SelectMany(user => user).Distinct().ToList();

                    // Remove recently added users
                    if (null != matterRevertListObject.MatterLibrary)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterLibrary, -1);
                    }
                    if (null != matterRevertListObject.MatterCalendar)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterCalendar, -1);
                    }
                    if (null != matterRevertListObject.MatterOneNoteLibrary)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterOneNoteLibrary, -1);
                    }
                    if (null != matterRevertListObject.MatterTask)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterTask, -1);
                    }
                    if (null != matterRevertListObject.MatterSitePages)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, true, matterRevertListObject.MatterSitePages, matterLandingPageId);
                    }

                    if (isEditMode)
                    {
                        Matter matterRevertUserPermission = PrepareUserPermission(oldUserPermissions);
                        if (null != matterRevertListObject.MatterLibrary)
                        {
                            result = SPList.SetPermission(clientContext, matterRevertUserPermission.AssignUserNames, matterRevertUserPermission.Permissions, matterRevertListObject.MatterLibrary);
                        }
                        if (null != matterRevertListObject.MatterOneNoteLibrary)
                        {
                            result = SPList.SetPermission(clientContext, matterRevertUserPermission.AssignUserNames, matterRevertUserPermission.Permissions, matterRevertListObject.MatterOneNoteLibrary);
                        }
                        if (null != matterRevertListObject.MatterCalendar)
                        {
                            result = SPList.SetPermission(clientContext, matterRevertUserPermission.AssignUserNames, matterRevertUserPermission.Permissions, matterRevertListObject.MatterCalendar);
                        }
                        if (null != matterRevertListObject.MatterTask)
                        {
                            result = SPList.SetPermission(clientContext, matterRevertUserPermission.AssignUserNames, matterRevertUserPermission.Permissions, matterRevertListObject.MatterTask);
                        }
                        if (null != matterRevertListObject.MatterSitePages && 0 <= matterLandingPageId)
                        {
                            result = SPList.SetItemPermission(clientContext, matterRevertUserPermission.AssignUserNames, "Site Pages", matterLandingPageId, matterRevertUserPermission.Permissions);
                        }
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstants.LogTableName);
            }
            // To avoid the invalid symbol error while parsing the JSON, return the response in lower case
            return result;
        }

        /// <summary>
        /// Fetches the users to remove permission.
        /// </summary>
        /// <param name="userPermissions">Users having permission on library</param>
        /// <returns>Matter object containing user name and permissions</returns>
        internal Matter PrepareUserPermission(IEnumerable<RoleAssignment> userPermissions)
        {
            Matter matterUserPermission = new Matter();
            matterUserPermission.AssignUserNames = new List<IList<string>>();
            matterUserPermission.Permissions = new List<string>();

            if (null != userPermissions && 0 < userPermissions.Count())
            {
                foreach (RoleAssignment userPermission in userPermissions)
                {
                    foreach (RoleDefinition roleDefinition in userPermission.RoleDefinitionBindings)
                    {
                        matterUserPermission.AssignUserNames.Add(new List<string> { userPermission.Member.Title });
                        matterUserPermission.Permissions.Add(roleDefinition.Name);
                    }
                }
            }
            return matterUserPermission;
        }


        /// <summary>
        /// Assign or Remove Full Control base on parameter given.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="loggedInUser">Name of logged in user</param>
        /// <param name="listExists">List of existed list</param>
        /// <param name="listItemId">ID of the list</param>
        /// <param name="assignFullControl">Flag to determine Assign or Remove Permission</param>
        public void AssignRemoveFullControl(ClientContext clientContext, Matter matter, string loggedInUser,
            int listItemId, List<string> listExists, bool assignFullControl, bool hasFullPermission)
        {
            IList<IList<string>> currentUser = new List<IList<string>>();
            IList<string> currentLoggedInUser = new List<string>() { loggedInUser };
            currentUser.Add(currentLoggedInUser);

            IList<string> permission = new List<string>() { "Full Control" };

            if (assignFullControl)
            {
                //Assign full control to Matter
                if (listExists.Contains(matter.Name))
                {
                    SPList.SetPermission(clientContext, currentUser, permission, matter.Name);
                }
                //Assign full control to OneNote
                if (listExists.Contains(matter.Name + "_OneNote"))
                {
                    SPList.SetPermission(clientContext, currentUser, permission, matter.Name + "_OneNote");
                }
                // Assign full control to Task list 
                if (listExists.Contains(matter.Name + "_Task"))
                {
                    SPList.SetPermission(clientContext, currentUser, permission, matter.Name + "_Task");
                }
                //Assign full control to calendar 
                if (listExists.Contains(matter.Name + "_Calendar"))
                {
                    SPList.SetPermission(clientContext, currentUser, permission, matter.Name + "_Calendar");
                }
                // Assign full control to Matter Landing page
                if (0 <= listItemId)
                {
                    SPList.SetItemPermission(clientContext, currentUser, "Site Pages", listItemId, permission);
                }
            }
            else
            {
                if (!hasFullPermission)
                {
                    //Remove full control to Matter
                    if (listExists.Contains(matter.Name))
                    {
                        RemoveFullControl(clientContext, matter.Name, loggedInUser, false, -1);
                    }
                    //Remove full control to OneNote
                    if (listExists.Contains(matter.Name + "_OneNote"))
                    {
                        RemoveFullControl(clientContext, matter.Name + "_OneNote", loggedInUser, false, -1);
                    }
                    // Remove full control to Task list 
                    if (listExists.Contains(matter.Name + "_Task"))
                    {
                        RemoveFullControl(clientContext, matter.Name + "_Task", loggedInUser, false, -1);
                    }
                    //Remove full control to calendar 
                    if (listExists.Contains(matter.Name + "_Calendar"))
                    {
                        RemoveFullControl(clientContext, matter.Name + "_Calendar", loggedInUser, false, -1);
                    }
                    if (0 <= listItemId)
                    {
                        RemoveFullControl(clientContext, "Site Pages", loggedInUser, true, listItemId);
                    }
                }
            }
        }

        /// <summary>
        /// Remove Full Permission.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="currentLoggedInUser">Name of logged in User</param>
        internal void RemoveFullControl(ClientContext clientContext, string listName, string currentLoggedInUser, bool isListItem, int matterLandingPageId)
        {
            ListItem listItem = null;
            RoleAssignmentCollection roleCollection = null;
            List list = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(list);
            clientContext.ExecuteQuery();
            if (isListItem)
            {
                // Fetch the list item
                if (0 <= matterLandingPageId)
                {
                    listItem = list.GetItemById(matterLandingPageId);
                    clientContext.Load(listItem, listProperties => listProperties.RoleAssignments.Include(roleAssignmentProperties =>
                        roleAssignmentProperties.Member,
                        roleAssignmentProperties => roleAssignmentProperties.Member.Title,
                        roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)));
                    clientContext.ExecuteQuery();
                    roleCollection = listItem.RoleAssignments;
                }
            }
            else
            {
                clientContext.Load(list, listProperties => listProperties.RoleAssignments.Include(roleAssignmentProperties =>
                    roleAssignmentProperties.Member,
                    roleAssignmentProperties => roleAssignmentProperties.Member.Title,
                    roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)));
                clientContext.ExecuteQuery();
                roleCollection = list.RoleAssignments;
            }


            if (null != roleCollection && 0 < roleCollection.Count)
            {
                foreach (RoleAssignment role in roleCollection)
                {
                    if (role.Member.Title == currentLoggedInUser)
                    {
                        IList<RoleDefinition> roleDefinationList = new List<RoleDefinition>();
                        foreach (RoleDefinition roleDef in role.RoleDefinitionBindings)
                        {
                            if (roleDef.Name == "Full Control")
                            {
                                roleDefinationList.Add(roleDef);
                            }
                        }
                        foreach (RoleDefinition roleDef in roleDefinationList)
                        {
                            role.RoleDefinitionBindings.Remove(roleDef);
                        }
                    }
                    role.Update();
                }
            }
            clientContext.ExecuteQuery();

        }

        public List<string> MatterAssociatedLists(ClientContext clientContext, string matterName, MatterConfigurations matterConfigurations = null)
        {
            List<string> lists = new List<string>();
            lists.Add(matterName);
            lists.Add(matterName + "_OneNote");
            if (null == matterConfigurations || matterConfigurations.IsCalendarSelected)
            {
                lists.Add(matterName + "_Calendar");
            }
            if (null == matterConfigurations || matterConfigurations.IsTaskSelected)
            {
                lists.Add(matterName + "_Task");
            }
            List<string> listExists = SPList.MatterAssociatedLists(clientContext, new System.Collections.ObjectModel.ReadOnlyCollection<string>(lists));
            return listExists;
        }

        /// <summary>
        /// Remove old users and assign permissions to new users.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="requestObject">RequestObject</param>
        /// <param name="client">Client object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="users">List of users to remove</param>
        /// <param name="isListItem">ListItem or list</param>
        /// <param name="list">List object</param>
        /// <param name="matterLandingPageId">List item id</param>
        /// <param name="isEditMode">Add/ Edit mode</param>
        /// <returns></returns>
        public static bool UpdatePermission(ClientContext clientContext, Matter matter, List<string> users,
            string loggedInUserTitle, bool isListItem, string listName, int matterLandingPageId, bool isEditMode)
        {
            bool result = false;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {
                    if (isEditMode)
                    {
                        SPList.RemoveSpecificUsers(clientContext, users, loggedInUserTitle, isListItem, listName, matterLandingPageId);
                    }
                    // Add permission
                    if (!isListItem)
                    {
                        result = SPList.SetPermission(clientContext, matter.AssignUserNames, matter.Permissions, listName);
                    }
                    else
                    {
                        result = SPList.SetItemPermission(clientContext, matter.AssignUserNames, "Site Pages",
                            matterLandingPageId, matter.Permissions);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            // To avoid the invalid symbol error while parsing the JSON, return the response in lower case 
            return result;
        }

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
                                if (roleName == "Full Control")
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
    }
}
