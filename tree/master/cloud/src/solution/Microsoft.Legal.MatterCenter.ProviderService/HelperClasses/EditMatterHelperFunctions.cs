// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-akvira
// Created          : 29-01-2015
//
// ***********************************************************************
// <copyright file="EditMatterHelperFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides helper methods to update matter properties.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.HelperClasses
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.Security.Application;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides helper methods to update matter properties.
    /// </summary>
    internal static class EditMatterHelperFunctions
    {
        /// <summary>
        /// Checks if the property exists in property bag. Returns the value for the property from property bag.
        /// </summary>
        /// <param name="stampedPropertyValues">Dictionary object containing matter property bag key/value pairs</param>
        /// <param name="key">Key to check in dictionary</param>
        /// <returns>Property bag value for </returns>
        internal static string GetStampPropertyValue(Dictionary<string, object> stampedPropertyValues, string key)
        {
            string result = string.Empty;
            if (stampedPropertyValues.ContainsKey(key))
            {
                result = System.Web.HttpUtility.HtmlDecode(Convert.ToString(stampedPropertyValues[key], CultureInfo.InvariantCulture));
            }

            // This is just to check for null value in key, if exists
            return (!string.IsNullOrWhiteSpace(result)) ? result : string.Empty;
        }

        /// <summary>
        /// Retrieves the users assigned to matter.
        /// </summary>
        /// <param name="matterCenterUsers">Users tagged with matter in property bag</param>
        /// <returns>Users assigned to matter</returns>
        internal static List<List<string>> GetMatterAssignedUsers(string matterCenterUsers)
        {
            List<List<string>> matterCenterUserCollection = new List<List<string>>();

            if (!string.IsNullOrWhiteSpace(matterCenterUsers))
            {
                List<string> userCollection = matterCenterUsers.Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string userRow in userCollection)
                {
                    List<string> users = userRow.Split(new string[] { ConstantStrings.Semicolon }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    matterCenterUserCollection.Add(users);
                }
            }
            return matterCenterUserCollection;
        }

        /// <summary>
        /// Extracts matter details from matter library property bag.
        /// </summary>
        /// <param name="stampedPropertyValues">Dictionary object containing matter property bag key/value pairs</param>
        /// <returns>Matter details from matter library property bag</returns>
        internal static MatterDetails ExtractMatterDetails(Dictionary<string, object> stampedPropertyValues)
        {
            MatterDetails matterDetails = new MatterDetails()
            {
                PracticeGroup = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyPracticeGroup),
                AreaOfLaw = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyAreaOfLaw),
                SubareaOfLaw = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertySubAreaOfLaw),
                ResponsibleAttorney = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyResponsibleAttorney),
                TeamMembers = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyTeamMembers),
                UploadBlockedUsers = GetStampPropertyValue(stampedPropertyValues, ServiceConstantStrings.StampedPropertyBlockedUploadUsers).Split(new string[] { ConstantStrings.Semicolon }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };
            return matterDetails;
        }

        /// <summary>
        /// Fetches matter library stamped properties.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="libraryName">Library name</param>
        /// <returns>Stamped properties</returns>
        internal static PropertyValues FetchMatterStampedProperties(ClientContext clientContext, string libraryName)
        {
            PropertyValues stampedProperties = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
                {
                    stampedProperties = clientContext.Web.Lists.GetByTitle(libraryName).RootFolder.Properties;
                    clientContext.Load(stampedProperties);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception)
            {
                throw; //// This will transfer control to catch block of parent function.
            }

            return stampedProperties;
        }

        /// <summary>
        /// Updates the matter stamped properties with new details for user permissions.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matterDetails">MatterDetails object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="matterStampedProperties">Matter stamped properties</param>
        /// <param name="isEditMode">Page opened in edit mode</param>
        /// <returns>Status of operation</returns>
        internal static string UpdateMatterStampedProperties(ClientContext clientContext, MatterDetails matterDetails, Matter matter, PropertyValues matterStampedProperties, bool isEditMode)
        {
            string result = ConstantStrings.FALSE;
            try
            {
                if (null != clientContext && null != matter && null != matterDetails && (0 < matterStampedProperties.FieldValues.Count))
                {
                    Dictionary<string, string> propertyList = new Dictionary<string, string>();

                    // Get existing stamped properties
                    string stampedUsers = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyMatterCenterUsers);
                    string stampedUserEmails = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyMatterCenterUserEmails);
                    string stampedPermissions = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyMatterCenterPermissions);
                    string stampedRoles = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyMatterCenterRoles);
                    string stampedResponsibleAttorneys = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyResponsibleAttorney);
                    string stampedResponsibleAttorneysEmail = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyResponsibleAttorneyEmail);
                    string stampedTeamMembers = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyTeamMembers);
                    string stampedBlockedUploadUsers = GetStampPropertyValue(matterStampedProperties.FieldValues, ServiceConstantStrings.StampedPropertyBlockedUploadUsers);

                    string currentPermissions = string.Join(ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, matter.Permissions.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentRoles = string.Join(ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, matter.Roles.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentBlockedUploadUsers = string.Join(ConstantStrings.Semicolon, matterDetails.UploadBlockedUsers.Where(user => !string.IsNullOrWhiteSpace(user)));
                    string currentUsers = GetMatterAssignedUsers(matter);
                    string currentUserEmails = GetMatterAssignedUsersEmail(clientContext, matter);

                    string finalMatterPermissions = string.IsNullOrWhiteSpace(stampedPermissions) || isEditMode ? currentPermissions : string.Concat(stampedPermissions, ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, currentPermissions);
                    string finalMatterRoles = string.IsNullOrWhiteSpace(stampedRoles) || isEditMode ? currentRoles : string.Concat(stampedRoles, ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, currentRoles);
                    string finalResponsibleAttorneys = string.IsNullOrWhiteSpace(stampedResponsibleAttorneys) || isEditMode ? matterDetails.ResponsibleAttorney : string.Concat(stampedResponsibleAttorneys, ConstantStrings.Semicolon, matterDetails.ResponsibleAttorney);
                    string finalResponsibleAttorneysEmail = string.IsNullOrWhiteSpace(stampedResponsibleAttorneysEmail) || isEditMode ? matterDetails.ResponsibleAttorneyEmail : string.Concat(stampedResponsibleAttorneysEmail, ConstantStrings.Semicolon, matterDetails.ResponsibleAttorneyEmail);
                    string finalTeamMembers = string.IsNullOrWhiteSpace(stampedTeamMembers) || isEditMode ? matterDetails.TeamMembers : string.Concat(stampedTeamMembers, ConstantStrings.Semicolon, matterDetails.TeamMembers);
                    string finalMatterCenterUsers = string.IsNullOrWhiteSpace(stampedUsers) || isEditMode ? currentUsers : string.Concat(stampedUsers, ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, currentUsers);
                    string finalMatterCenterUserEmails = string.IsNullOrWhiteSpace(stampedUserEmails) || isEditMode ? currentUserEmails : string.Concat(stampedUserEmails, ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR, currentUserEmails);
                    string finalBlockedUploadUsers = string.IsNullOrWhiteSpace(stampedBlockedUploadUsers) || isEditMode ? currentBlockedUploadUsers : string.Concat(stampedBlockedUploadUsers, ConstantStrings.Semicolon, currentBlockedUploadUsers);

                    propertyList.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorney, Encoder.HtmlEncode(finalResponsibleAttorneys));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyResponsibleAttorneyEmail, Encoder.HtmlEncode(finalResponsibleAttorneysEmail));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyTeamMembers, Encoder.HtmlEncode(finalTeamMembers));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyBlockedUploadUsers, Encoder.HtmlEncode(finalBlockedUploadUsers));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterRoles, Encoder.HtmlEncode(finalMatterRoles));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterPermissions, Encoder.HtmlEncode(finalMatterPermissions));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterUsers, Encoder.HtmlEncode(finalMatterCenterUsers));
                    propertyList.Add(ServiceConstantStrings.StampedPropertyMatterCenterUserEmails, Encoder.HtmlEncode(finalMatterCenterUserEmails));

                    Lists.SetPropertBagValuesForList(clientContext, matterStampedProperties, matter.Name, propertyList);
                    result = ConstantStrings.TRUE;
                }
            }
            catch (Exception)
            {
                throw; //// This will transfer control to catch block of parent function.
            }
            return result;
        }

        /// <summary>
        /// Converts the matter users in a form that can be stamped to library.
        /// </summary>
        /// <param name="matter">Matter object</param>
        /// <returns>Users that can be stamped</returns>
        private static string GetMatterAssignedUsers(Matter matter)
        {
            string currentUsers = string.Empty;
            string separator = string.Empty;
            if (null != matter && 0 < matter.AssignUserNames.Count)
            {
                foreach (IList<string> userNames in matter.AssignUserNames)
                {
                    currentUsers += separator + string.Join(ConstantStrings.Semicolon, userNames.Where(user => !string.IsNullOrWhiteSpace(user)));
                    separator = ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR;
                }
            }
            return currentUsers;
        }

        /// <summary>
        /// Converts the project users emails in a form that can be stamped to library.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matter">Matter object</param>
        /// <returns>Users that can be stamped</returns>
        private static string GetMatterAssignedUsersEmail(ClientContext clientContext, Matter matter)
        {
            string currentUsers = string.Empty;
            string separator = string.Empty;
            if (null != matter && 0 < matter.AssignUserEmails.Count)
            {
                foreach (IList<string> userNames in matter.AssignUserEmails)
                {
                    List<string> userEmails = new List<string>();
                    if (null != clientContext && null != userNames)
                    {
                        foreach (string userName in userNames)
                        {
                            if (!string.IsNullOrWhiteSpace(userName))
                            {
                                if (ValidationHelperFunctions.ValidateExternalUserInput(userName))
                                {
                                    userEmails.Add(userName);
                                }
                                else
                                {
                                    User user = clientContext.Web.EnsureUser(userName.Trim());
                                    ///// Only Fetch the User ID which is required
                                    clientContext.Load(user, u => u.Email);
                                    clientContext.ExecuteQuery();
                                    ///// Add the user to the first element of the FieldUserValue array.
                                    userEmails.Add(user.Email);
                                }
                            }
                        }
                        currentUsers += separator + string.Join(ConstantStrings.Semicolon, userEmails);
                        separator = ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR;
                    }
                }
            }

            return currentUsers;
        }


        /// <summary>
        /// Fetches the effective permissions of users present in matter library.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matterLibrary">Matter library name</param>
        /// <returns>List permissions</returns>
        internal static IEnumerable<RoleAssignment> FetchUserPermission(ClientContext clientContext, string matterLibrary)
        {
            IEnumerable<RoleAssignment> userPermissionCollection = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(matterLibrary))
                {
                    List list = clientContext.Web.Lists.GetByTitle(matterLibrary);
                    userPermissionCollection = clientContext.LoadQuery(list.RoleAssignments.Include(listRoleAssignment => listRoleAssignment.PrincipalId, listRoleAssignment => listRoleAssignment.Member, listRoleAssignment => listRoleAssignment.Member.Title, listRoleAssignment => listRoleAssignment.Member.PrincipalType, listRoleAssignment => listRoleAssignment.RoleDefinitionBindings.Include(userRoles => userRoles.BasePermissions, userRoles => userRoles.Name, userRoles => userRoles.Id)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType)));
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception)
            {
                throw; //// This will transfer control to catch block of parent function.
            }
            return userPermissionCollection;
        }

        /// <summary>
        /// Remove old users and assign permissions to new users.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="users">List of users to remove</param>
        /// <param name="loggedInUserTitle">Title of logged-in user</param>
        /// <param name="isListItem">ListItem or list</param>
        /// <param name="listName">List Name</param>
        /// <param name="matterLandingPageId">List item id</param>
        /// <param name="isEditMode">Add/ Edit mode</param>
        /// <returns></returns>
        internal static string UpdatePermission(ClientContext clientContext, Matter matter, List<string> users, string loggedInUserTitle, bool isListItem, string listName, int matterLandingPageId, bool isEditMode)
        {
            bool result = false;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {
                    if (isEditMode)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, isListItem, listName, matterLandingPageId);
                    }
                    // Add permission
                    if (!isListItem)
                    {
                        result = Lists.SetPermission(clientContext, matter.AssignUserEmails, matter.Permissions, listName);
                    }
                    else
                    {
                        if (0 <= matterLandingPageId)
                        {
                            result = Lists.SetItemPermission(clientContext, matter.AssignUserEmails, ServiceConstantStrings.MatterLandingPageRepositoryName, matterLandingPageId, matter.Permissions);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            // To avoid the invalid symbol error while parsing the JSON, return the response in lower case 
            return Convert.ToString(result, CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Removes the users' permission from list or list item.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="usersToRemove">List of users</param>
        /// <param name="loggedInUserTitle">Title of logged-in user</param>
        /// <param name="isListItem">ListItem or list</param>
        /// <param name="listName">Name of the List</param>
        /// <param name="matterLandingPageId">List item id</param>
        private static void RemoveSpecificUsers(ClientContext clientContext, List<string> usersToRemove, string loggedInUserTitle, bool isListItem, string listName, int matterLandingPageId)
        {
            try
            {
                ListItem listItem = null;
                RoleAssignmentCollection roleCollection = null;
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                List list = web.Lists.GetByTitle(listName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                if (0 < usersToRemove.Count)
                {
                    if (isListItem)
                    {
                        // Fetch the list item
                        if (0 <= matterLandingPageId)
                        {
                            listItem = list.GetItemById(matterLandingPageId);
                            clientContext.Load(listItem, listItemProperties => listItemProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member, roleAssignmentProperties => roleAssignmentProperties.Member.Title, roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType || (PrincipalType.SharePointGroup == listUsers.Member.PrincipalType))));
                            clientContext.ExecuteQuery();
                            roleCollection = listItem.RoleAssignments;
                        }
                    }
                    else
                    {
                        clientContext.Load(list, listProperties => listProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member, roleAssignmentProperties => roleAssignmentProperties.Member.Title, roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType) || (PrincipalType.SharePointGroup == listUsers.Member.PrincipalType)));
                        clientContext.ExecuteQuery();
                        roleCollection = list.RoleAssignments;
                    }

                    if (null != roleCollection && 0 < roleCollection.Count && 0 < usersToRemove.Count)
                    {
                        foreach (string user in usersToRemove)
                        {
                            foreach (RoleAssignment role in roleCollection)
                            {
                                List<RoleDefinition> roleDefinationList = new List<RoleDefinition>();
                                foreach (RoleDefinition roleDef in role.RoleDefinitionBindings)
                                {
                                    // Removing permission for all the user except current user with full control
                                    // Add those users in list, then traverse the list and removing all users from that list
                                    if (PrincipalType.SharePointGroup == role.Member.PrincipalType)
                                    {
                                        roleDefinationList.Add(roleDef);
                                    }
                                    else
                                    {
                                        string email = ((User)role.Member).Email;
                                        if (email == user && !((email == loggedInUserTitle) && (roleDef.Name == ConstantStrings.EditMatterAllowedPermissionLevel)))
                                        {
                                            roleDefinationList.Add(roleDef);
                                        }
                                    }
                                }
                                foreach (RoleDefinition roleDef in roleDefinationList)
                                {
                                    role.RoleDefinitionBindings.Remove(roleDef);
                                }
                                role.Update();
                            }
                        }
                    }
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Gets the display name of users having permission on library.
        /// </summary>
        /// <param name="userPermissionOnLibrary">Users having permission on library</param>
        /// <returns></returns>
        internal static List<string> RetrieveMatterUsers(IEnumerable<RoleAssignment> userPermissionOnLibrary)
        {
            List<string> users = new List<string>();
            try
            {
                if (null != userPermissionOnLibrary && 0 < userPermissionOnLibrary.Count())
                {
                    foreach (RoleAssignment roles in userPermissionOnLibrary)
                    {
                        users.Add(((User)roles.Member).Email);
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
        internal static string RevertMatterUpdates(RequestObject requestObject, Client client, Matter matter, ClientContext clientContext, MatterRevertList matterRevertListObject, string loggedInUserTitle, IEnumerable<RoleAssignment> oldUserPermissions, int matterLandingPageId, bool isEditMode)
        {
            bool result = false;
            try
            {
                if (null != requestObject && null != client && null != matter && null != clientContext && null != matterRevertListObject)
                {
                    List<string> users = new List<string>();
                    users = matter.AssignUserEmails.SelectMany(user => user).Distinct().ToList();

                    // Remove recently added users
                    if (null != matterRevertListObject.MatterLibrary)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterLibrary, -1);
                    }
                    if (null != matterRevertListObject.MatterCalendar)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterCalendar, -1);
                    }
                    if (null != matterRevertListObject.MatterOneNoteLibrary)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterOneNoteLibrary, -1);
                    }
                    if (null != matterRevertListObject.MatterTask)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, false, matterRevertListObject.MatterTask, -1);
                    }
                    if (null != matterRevertListObject.MatterSitePages)
                    {
                        RemoveSpecificUsers(clientContext, users, loggedInUserTitle, true, matterRevertListObject.MatterSitePages, matterLandingPageId);
                    }

                    if (isEditMode)
                    {
                        Matter matterRevertUserPermission = PrepareUserPermission(oldUserPermissions);
                        if (null != matterRevertListObject.MatterLibrary)
                        {
                            result = Lists.SetPermission(clientContext, matterRevertUserPermission.AssignUserEmails, matterRevertUserPermission.Permissions, matterRevertListObject.MatterLibrary);
                        }
                        if (null != matterRevertListObject.MatterOneNoteLibrary)
                        {
                            result = Lists.SetPermission(clientContext, matterRevertUserPermission.AssignUserEmails, matterRevertUserPermission.Permissions, matterRevertListObject.MatterOneNoteLibrary);
                        }
                        if (null != matterRevertListObject.MatterCalendar)
                        {
                            result = Lists.SetPermission(clientContext, matterRevertUserPermission.AssignUserEmails, matterRevertUserPermission.Permissions, matterRevertListObject.MatterCalendar);
                        }
                        if (null != matterRevertListObject.MatterTask)
                        {
                            result = Lists.SetPermission(clientContext, matterRevertUserPermission.AssignUserEmails, matterRevertUserPermission.Permissions, matterRevertListObject.MatterTask);
                        }
                        if (null != matterRevertListObject.MatterSitePages && 0 <= matterLandingPageId)
                        {
                            result = Lists.SetItemPermission(clientContext, matterRevertUserPermission.AssignUserEmails, ServiceConstantStrings.MatterLandingPageRepositoryName, matterLandingPageId, matterRevertUserPermission.Permissions);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            // To avoid the invalid symbol error while parsing the JSON, return the response in lower case
            return Convert.ToString(result, CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Fetches the users to remove permission.
        /// </summary>
        /// <param name="userPermissions">Users having permission on library</param>
        /// <returns>Matter object containing user name and permissions</returns>
        internal static Matter PrepareUserPermission(IEnumerable<RoleAssignment> userPermissions)
        {
            Matter matterUserPermission = new Matter();
            matterUserPermission.AssignUserEmails = new List<IList<string>>();
            matterUserPermission.Permissions = new List<string>();

            if (null != userPermissions && 0 < userPermissions.Count())
            {
                foreach (RoleAssignment userPermission in userPermissions)
                {
                    foreach (RoleDefinition roleDefinition in userPermission.RoleDefinitionBindings)
                    {
                        matterUserPermission.AssignUserEmails.Add(new List<string> { ((User)userPermission.Member).Email });
                        matterUserPermission.Permissions.Add(roleDefinition.Name);
                    }
                }
            }
            return matterUserPermission;
        }

        /// <summary>
        /// Checks if security group exists in team members list.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matter">Matter object</param>
        /// <returns>Security group existence</returns>
        internal static string CheckSecurityGroupInTeamMembers(ClientContext clientContext, Matter matter, IList<string> userId)
        {
            string result = ConstantStrings.TRUE;
            int teamMembersRowCount, securityGroupRowNumber = -1; // Blocked user field has security group
            List<Tuple<int, Principal>> teamMemberPrincipalCollection = new List<Tuple<int, Principal>>();
            if (null != clientContext && null != matter && null != matter.AssignUserEmails && null != matter.BlockUserNames)
            {
                try
                {
                    teamMembersRowCount = matter.AssignUserEmails.Count;
                    List<string> blockedUsers = matter.BlockUserNames.Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                    if (0 < teamMembersRowCount)
                    {
                        securityGroupRowNumber = -2; // Invalid user
                        for (int iterator = 0; iterator < teamMembersRowCount; iterator++)
                        {
                            List<string> currentRowTeamMembers = matter.AssignUserEmails[iterator].Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                            foreach (string teamMember in currentRowTeamMembers)
                            {
                                Principal teamMemberPrincipal = clientContext.Web.EnsureUser(teamMember);
                                clientContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties => teamMemberPrincipalProperties.PrincipalType, teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title);
                                teamMemberPrincipalCollection.Add(new Tuple<int, Principal>(iterator, teamMemberPrincipal));
                            }
                        }
                    }
                    if (0 < blockedUsers.Count)
                    {
                        foreach (string blockedUser in blockedUsers)
                        {
                            Principal teamMemberPrincipal = clientContext.Web.EnsureUser(blockedUser);
                            clientContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties => teamMemberPrincipalProperties.PrincipalType, teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title);
                            teamMemberPrincipalCollection.Add(new Tuple<int, Principal>(-1, teamMemberPrincipal));
                        }
                    }
                    clientContext.ExecuteQuery();
                    foreach (Tuple<int, Principal> teamMemberPrincipal in teamMemberPrincipalCollection)
                    {
                        Principal currentTeamMemberPrincipal = teamMemberPrincipal.Item2;
                        if (currentTeamMemberPrincipal.PrincipalType == PrincipalType.SecurityGroup)
                        {
                            securityGroupRowNumber = teamMemberPrincipal.Item1;
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.ErrorCodeSecurityGroupExists, ServiceConstantStrings.ErrorMessageSecurityGroupExists + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + userId[securityGroupRowNumber]);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectTeamMembersCode, ServiceConstantStrings.IncorrectTeamMembersMessage + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + userId[securityGroupRowNumber]);
                }
            }
            else
            {
                result = ConstantStrings.FALSE;
            }
            return result;
        }

        /// <summary>
        /// Checks users in team members list.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="userId">Id of the user's row</param>
        /// <returns>Security group existence</returns>
        internal static string ValidateTeamMembers(ClientContext clientContext, Matter matter, IList<string> userId)
        {
            string result = string.Empty;
            bool isInvalidUser = false;
            int iCounter = 0, teamMembersRowCount = matter.AssignUserEmails.Count(), iCount = 0;
            List<Principal> teamMemberPrincipalCollection = new List<Principal>();
            try
            {
                for (iCounter = 0; iCounter < teamMembersRowCount; iCounter++)
                {
                    IList<string> userList = matter.AssignUserEmails[iCounter].Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                    IList<string> userNameList = matter.AssignUserNames[iCounter].Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                    foreach (string userName in userList)
                    {
                        Principal teamMemberPrincipal = clientContext.Web.EnsureUser(userName.Trim());
                        clientContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title);
                        teamMemberPrincipalCollection.Add(teamMemberPrincipal);
                    }
                    clientContext.ExecuteQuery();
                    //// Check whether the name entered by the user and the name resolved by SharePoint is same.
                    foreach (string teamMember in userNameList)
                    {
                        if (!string.Equals(teamMember.Trim(), teamMemberPrincipalCollection[iCount].Title.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectTeamMembersCode, ServiceConstantStrings.IncorrectTeamMembersMessage + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + userId[iCounter]);
                            isInvalidUser = true;
                            break;
                        }
                        iCount++;
                    }
                    if (isInvalidUser)
                    {
                        break; // To break the outer loop as there is an invalid user
                    }
                }
            }
            catch (Exception)
            {
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, ServiceConstantStrings.IncorrectTeamMembersCode, ServiceConstantStrings.IncorrectTeamMembersMessage + ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR + userId[iCounter]);
            }
            return result;
        }

        /// <summary>
        /// Fetches the user name updating the matter.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <returns>Logged-in user name</returns>
        internal static string GetUserUpdatingMatter(ClientContext clientContext)
        {
            string userName = string.Empty;
            try
            {
                Users loggedInUser = UIUtility.GetLoggedInUserDetails(clientContext);
                userName = loggedInUser.Email;
            }
            catch (Exception)
            {
                throw;
            }
            return userName;
        }

        /// <summary>
        /// Fetches Matter Name from Matter Stamp properties.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="matterName">Matter Title</param>
        /// <returns>Matter Name related to Matter Title</returns>
        internal static string GetMatterName(ClientContext clientContext, string matterName)
        {
            try
            {

                PropertyValues properties = clientContext.Web.Lists.GetByTitle(matterName).RootFolder.Properties;
                clientContext.Load(properties);
                clientContext.ExecuteQuery();
                return properties.FieldValues.ContainsKey(ServiceConstantStrings.StampedPropertyMatterGUID) ? System.Web.HttpUtility.HtmlDecode(Convert.ToString(properties.FieldValues[ServiceConstantStrings.StampedPropertyMatterGUID], CultureInfo.InvariantCulture)) : matterName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Assign or Remove Full Control base on parameter given.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="matter">Matter object</param>
        /// <param name="loggedInUser">Name of logged in user</param>
        /// <param name="listItemId">ID of the list item</param>
        /// <param name="listExists">List of existed list</param>
        /// <param name="assignFullControl">Flag to determine Assign or Remove Permission</param>
        /// <param name="hasFullPermission">Flag to determine user has Full Permission or not</param>
        internal static void AssignRemoveFullControl(ClientContext clientContext, Matter matter, string loggedInUser, int listItemId, List<string> listExists, bool assignFullControl, bool hasFullPermission)
        {
            IList<IList<string>> currentUser = new List<IList<string>>();
            IList<string> currentLoggedInUser = new List<string>() { loggedInUser };
            currentUser.Add(currentLoggedInUser);

            IList<string> permission = new List<string>() { ConstantStrings.EditMatterAllowedPermissionLevel };

            if (assignFullControl)
            {
                //Assign full control to Matter
                if (listExists.Contains(matter.Name))
                {
                    Lists.SetPermission(clientContext, currentUser, permission, matter.Name);
                }
                //Assign full control to OneNote
                if (listExists.Contains(matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix))
                {
                    Lists.SetPermission(clientContext, currentUser, permission, matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix);
                }
                // Assign full control to Task list 
                if (listExists.Contains(matter.Name + ServiceConstantStrings.TaskNameSuffix))
                {
                    Lists.SetPermission(clientContext, currentUser, permission, matter.Name + ServiceConstantStrings.TaskNameSuffix);
                }
                //Assign full control to calendar 
                if (listExists.Contains(matter.Name + ServiceConstantStrings.CalendarNameSuffix))
                {
                    Lists.SetPermission(clientContext, currentUser, permission, matter.Name + ServiceConstantStrings.CalendarNameSuffix);
                }
                // Assign full control to Matter Landing page
                if (0 <= listItemId)
                {
                    Lists.SetItemPermission(clientContext, currentUser, ServiceConstantStrings.MatterLandingPageRepositoryName, listItemId, permission);
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
                    if (listExists.Contains(matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix))
                    {
                        RemoveFullControl(clientContext, matter.Name + ServiceConstantStrings.OneNoteLibrarySuffix, loggedInUser, false, -1);
                    }
                    // Remove full control to Task list 
                    if (listExists.Contains(matter.Name + ServiceConstantStrings.TaskNameSuffix))
                    {
                        RemoveFullControl(clientContext, matter.Name + ServiceConstantStrings.TaskNameSuffix, loggedInUser, false, -1);
                    }
                    //Remove full control to calendar 
                    if (listExists.Contains(matter.Name + ServiceConstantStrings.CalendarNameSuffix))
                    {
                        RemoveFullControl(clientContext, matter.Name + ServiceConstantStrings.CalendarNameSuffix, loggedInUser, false, -1);
                    }
                    if (0 <= listItemId)
                    {
                        RemoveFullControl(clientContext, ServiceConstantStrings.MatterLandingPageRepositoryName, loggedInUser, true, listItemId);
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
        internal static void RemoveFullControl(ClientContext clientContext, string listName, string currentLoggedInUser, bool isListItem, int matterLandingPageId)
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
                    clientContext.Load(listItem, listProperties => listProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member, roleAssignmentProperties => roleAssignmentProperties.Member.Title, roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)));
                    clientContext.ExecuteQuery();
                    roleCollection = listItem.RoleAssignments;
                }
            }
            else
            {
                clientContext.Load(list, listProperties => listProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member, roleAssignmentProperties => roleAssignmentProperties.Member.Title, roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name, roleDef => roleDef.BasePermissions)));
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
                            if (roleDef.Name == ConstantStrings.EditMatterAllowedPermissionLevel)
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

        /// <summary>
        /// Check Full Permission for logged in User.
        /// </summary>
        /// <param name="AssignUserNames">List of Assigned UserNames</param>
        /// <param name="Permissions">List of Permission</param>
        /// <param name="loggedInUserName">Name of logged in User</param>
        /// <returns>Status of Full Permission</returns>
        internal static bool CheckFullPermissionInAssignList(IList<IList<string>> assignUserEmails, IList<string> Permissions, string loggedInUserName)
        {
            bool result = false;
            if (null != Permissions && null != assignUserEmails && Permissions.Count == assignUserEmails.Count)
            {
                int position = 0;
                foreach (string roleName in Permissions)
                {
                    IList<string> AssignUserEmails = assignUserEmails[position];
                    if (!string.IsNullOrWhiteSpace(roleName) && null != AssignUserEmails)
                    {
                        foreach (string user in AssignUserEmails)
                        {
                            if (!string.IsNullOrWhiteSpace(user) && user.Trim().Equals(loggedInUserName.Trim()))
                            {
                                if (roleName == ConstantStrings.EditMatterAllowedPermissionLevel)
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

        /// <summary>
        /// Validates if there is at-least one user with full control in assign list.
        /// </summary>
        /// <param name="matter">Matter object</param>
        /// <returns>Status of Full Control permission</returns>
        internal static bool ValidateFullControlPermission(Matter matter)
        {

            bool hasFullConrol = false;
            if (null != matter && null != matter.Permissions && 0 != matter.Permissions.Count)
            {
                hasFullConrol = matter.Permissions.Contains(ConstantStrings.EditMatterAllowedPermissionLevel);
            }
            return hasFullConrol;
        }
    }
}