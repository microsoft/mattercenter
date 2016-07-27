// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-rijadh
// Created          : 07-07-2016
//***************************************************************************
// History
// Modified         : 07-07-2016
// Modified By      : v-lapedd
// ***********************************************************************
// <copyright file="TaxonomyHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to get/update information from/in SP lists</summary>


using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
#endregion

namespace Microsoft.Legal.MatterCenter.Jobs
{
    /// <summary>
    /// This file provide methods to get/update information from/in SP lists
    /// </summary>
    public class SPList
    {
        #region Properties      
        private IHostingEnvironment hostingEnvironment;

        #endregion
        public static List<Tuple<int, Principal>> CheckUserSecurity(ClientContext clientContext, Matter matter, IList<string> userIds)
        {
            List<Tuple<int, Principal>> teamMemberPrincipalCollection = GetUserPrincipal(clientContext, matter, userIds);
            return teamMemberPrincipalCollection;

        }


        public static List<Tuple<int, Principal>> GetUserPrincipal(ClientContext clientContext, Matter matter, IList<string> userIds)
        {
            List<Tuple<int, Principal>> teamMemberPrincipalCollection = new List<Tuple<int, Principal>>();
            int securityGroupRowNumber = -1;
            try
            {
                
                int teamMembersRowCount = matter.AssignUserNames.Count;

                List<string> blockedUsers = matter.BlockUserNames.Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                if (0 < teamMembersRowCount)
                {
                    securityGroupRowNumber = -2; // Invalid user
                    for (int iterator = 0; iterator < teamMembersRowCount; iterator++)
                    {
                        List<string> currentRowTeamMembers = matter.AssignUserNames[iterator].Where(user => !string.IsNullOrWhiteSpace(user.Trim())).ToList();
                        foreach (string teamMember in currentRowTeamMembers)
                        {
                            Principal teamMemberPrincipal = clientContext.Web.EnsureUser(teamMember);
                            clientContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties =>
                                        teamMemberPrincipalProperties.PrincipalType,
                                        teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title);
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
                return teamMemberPrincipalCollection;
            }
            
            catch (Exception ex)
            {
                //customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Breaks the permissions of the list.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="libraryName">Name of list</param>        
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>Success flag</returns>
        public static bool BreakPermission(ClientContext clientContext, string libraryName, bool isCopyRoleAssignment)
        {
            bool flag = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
            {
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (!list.HasUniqueRoleAssignments)
                    {
                        list.BreakRoleInheritance(isCopyRoleAssignment, true);
                        list.Update();
                        clientContext.Load(list);
                        clientContext.ExecuteQuery();
                        flag = true;
                    }
                }
                catch (Exception)
                {
                    throw; // This will transfer control to catch block of parent function.
                }
            }
            return flag;
        }

        public static int RetrieveItemId(ClientContext clientContext, string libraryName, string originalMatterName)
        {
            int listItemId = -1;
            ListItemCollection listItemCollection = GetData(clientContext, libraryName);
            clientContext.Load(listItemCollection, listItemCollectionProperties =>
                                listItemCollectionProperties.Include(
                                    listItemProperties => listItemProperties.Id,
                                    listItemProperties => listItemProperties.DisplayName));
            clientContext.ExecuteQuery();

            ListItem listItem = listItemCollection.Cast<ListItem>().FirstOrDefault(listItemProperties => listItemProperties.DisplayName.ToUpper(CultureInfo.InvariantCulture).Equals(originalMatterName.ToUpper(CultureInfo.InvariantCulture)));

            if (null != listItem)
            {
                listItemId = listItem.Id;
            }
            return listItemId;
        }

        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public static List<string> MatterAssociatedLists(ClientContext clientContext, ReadOnlyCollection<string> listsNames)
        {
            List<string> existingLists = new List<string>();
            try
            {
                if (null != clientContext && null != listsNames)
                {
                    //ToDo: Chec
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    existingLists = (from listName in listsNames
                                     join item in lists
                                     on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                     select listName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return existingLists;
        }


        /// <summary>
        /// This method will return the user object who has currently logged into the system
        /// </summary>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public static Users GetLoggedInUserDetails(ClientContext clientContext)
        {
            Users currentUserDetail = new Users();
            try
            {
                clientContext.Load(clientContext.Web.CurrentUser, userInfo => userInfo.Title, userInfo => userInfo.Email, userInfo => userInfo.LoginName);
                clientContext.ExecuteQuery();
                currentUserDetail.Name = clientContext.Web.CurrentUser.Title;
                currentUserDetail.Email = clientContext.Web.CurrentUser.Email;

                //Check if email is available to get account name, if not use login name
                if (!String.IsNullOrEmpty(currentUserDetail.Email))
                {
                    currentUserDetail.LogOnName = currentUserDetail.Email;
                }
                else
                {
                    currentUserDetail.LogOnName = clientContext.Web.CurrentUser.LoginName;
                }

                //Retrieve user name from login
                int splitPositionStart;
                int splitPositionEnd = currentUserDetail.LogOnName.LastIndexOf(ServiceConstants.SYMBOL_AT, StringComparison.OrdinalIgnoreCase);
                if (splitPositionEnd == -1)  //The user is an on-premise account
                {
                    splitPositionStart = currentUserDetail.LogOnName.LastIndexOf(ServiceConstants.BACKWARD_SLASH, StringComparison.OrdinalIgnoreCase) + 1;
                    splitPositionEnd = currentUserDetail.LogOnName.Length;
                }
                else
                {
                    splitPositionStart = currentUserDetail.LogOnName.LastIndexOf(ServiceConstants.PIPE, StringComparison.OrdinalIgnoreCase) + 1;
                }
                currentUserDetail.LogOnName = currentUserDetail.LogOnName.Substring(splitPositionStart, splitPositionEnd - splitPositionStart);
                return currentUserDetail;
            }

            catch (Exception exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Converts the project users emails in a form that can be stamped to library.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="matter">Matter object</param>
        /// <returns>Users that can be stamped</returns>
        public static string GetMatterAssignedUsersEmail(ClientContext clientContext, Matter matter)
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
                                if (Regex.IsMatch(userName.Trim(), "^[\\s]*\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*[\\s]*$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
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
                        currentUsers += separator + string.Join(ServiceConstants.SEMICOLON, userEmails);
                        separator = ServiceConstants.DOLLAR + ServiceConstants.PIPE + ServiceConstants.DOLLAR;
                    }
                }
            }

            return currentUsers;
        }

        /// <summary>
        /// Removes the users' permission from list or list item.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="usersToRemove">List of users</param>
        /// <param name="isListItem">ListItem or list</param>
        /// <param name="list">List object</param>
        /// <param name="matterLandingPageId">List item id</param>
        public static void RemoveSpecificUsers(ClientContext clientContext, List<string> usersToRemove, string loggedInUserTitle,
            bool isListItem, string listName, int matterLandingPageId)
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
                            clientContext.Load(listItem, listItemProperties =>
                            listItemProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member,
                            roleAssignmentProperties => roleAssignmentProperties.Member.Title,
                            roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name,
                                                                                                                roleDef => roleDef.BasePermissions)));
                            clientContext.ExecuteQuery();
                            roleCollection = listItem.RoleAssignments;
                        }
                    }
                    else
                    {
                        clientContext.Load(list, listProperties =>
                        listProperties.RoleAssignments.Include(roleAssignmentProperties => roleAssignmentProperties.Member,
                        roleAssignmentProperties => roleAssignmentProperties.Member.Title,
                        roleAssignmentProperties => roleAssignmentProperties.RoleDefinitionBindings.Include(roleDef => roleDef.Name,
                                                                                                            roleDef => roleDef.BasePermissions)));
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
                                    if (role.Member.Title == user && !((role.Member.Title == loggedInUserTitle) && (roleDef.Name ==
                                        "Full Control")))
                                    {
                                        roleDefinationList.Add(roleDef);
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
        /// Validates and breaks the item level permission for the specified list item under the list/library. 
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>String stating success flag</returns>
        public bool BreakItemPermission(ClientContext clientContext, string listName, int listItemId, bool isCopyRoleAssignment)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();

                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(isCopyRoleAssignment, true);
                    listItem.Update();
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        

        


        

        

        


        

        /// <summary>
        /// Gets the list items of specified list based on CAML query.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public static ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null)
        {
            try
            {
                ListItemCollection listItemCollection = null;
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {
                    CamlQuery query = new CamlQuery();
                    if (!string.IsNullOrWhiteSpace(camlQuery))
                    {
                        query.ViewXml = camlQuery;
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(query);
                    }
                    else
                    {
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(CamlQuery.CreateAllItemsQuery());
                    }
                    clientContext.Load(listItemCollection);
                    clientContext.ExecuteQuery();
                }
                return listItemCollection;
            }
            catch (Exception exception)
            {
                throw;
            }
        }







        

        public static string GetMatterName(ClientContext clientContext, string matterName)
        {
            PropertyValues propertyValues = SPList.GetListProperties(clientContext, matterName);
            return propertyValues.FieldValues.ContainsKey("MatterGUID") ?
                System.Net.WebUtility.HtmlDecode(Convert.ToString(propertyValues.FieldValues["MatterGUID"], CultureInfo.InvariantCulture)) : matterName;
        }


        public static PropertyValues GetListProperties(ClientContext clientContext, string libraryName)
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
            catch (Exception ex)
            {
                throw; //// This will transfer control to catch block of parent function.
            }

            return stampedProperties;
        }


        public static IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname)
        {
            IEnumerable<RoleAssignment> userPermissionCollection = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryname))
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryname);
                    userPermissionCollection = clientContext.LoadQuery(list.RoleAssignments.Include(listRoleAssignment => 
                        listRoleAssignment.PrincipalId, listRoleAssignment => listRoleAssignment.Member, 
                        listRoleAssignment => listRoleAssignment.Member.Title, 
                        listRoleAssignment => listRoleAssignment.Member.PrincipalType, 
                        listRoleAssignment => listRoleAssignment.RoleDefinitionBindings.Include(userRoles => userRoles.BasePermissions, 
                                                                                                userRoles => userRoles.Name, 
                                                                                                userRoles => userRoles.Id)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType)));
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                
            }
            return userPermissionCollection;
        }

        /// <summary>
        /// Sets permissions for the list.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <param name="listName">List name</param>
        /// <returns>String stating success flag</returns>
        public static bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ClientRuntimeContext clientRuntimeContext = clientContext;
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
                                {
                                    if (!string.IsNullOrWhiteSpace(user))
                                    {
                                        /////get the user object
                                        Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                        /////create the role definition binding collection
                                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                        /////add the role definition to the collection
                                        roleDefinitionBindingCollection.Add(roleDefinition);
                                        /////create a RoleAssigment with the user and role definition
                                        list.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                    }
                                }
                                /////execute the query to add everything
                                clientRuntimeContext.ExecuteQuery();
                            }
                            position++;
                        }
                        ///// Success. Return a success code
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            return result;
        }

        /// <summary>
        /// Set permission to the specified list item 
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <returns>Status of the unique item level permission assignment operation</returns>
        public static bool SetItemPermission(ClientContext clientContext, IList<IList<string>> assignUserName, string listName, int listItemId, IList<string> permissions)
        {
            bool result = false;
            try
            {
                if (null != clientContext)
                {
                    ClientRuntimeContext clientRuntimeContext = clientContext;
                    ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                    clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (listItem.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
                                {

                                    if (!string.IsNullOrWhiteSpace(user))
                                    {
                                        /////get the user object
                                        Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                        /////create the role definition binding collection
                                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                        /////add the role definition to the collection
                                        roleDefinitionBindingCollection.Add(roleDefinition);
                                        /////create a RoleAssigment with the user and role definition
                                        listItem.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                    }
                                }
                                /////execute the query to add everything
                                clientRuntimeContext.ExecuteQuery();
                            }
                            position++;
                        }
                        ///// Success. Return a success code
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property Bag</param>
        /// <param name="matterName">Name of matter to which property is to be attached</param>
        /// <param name="propertyList">List of properties</param>
        public static void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            try
            { 
                if (null != clientContext && !string.IsNullOrWhiteSpace(matterName) && null != props && null != propertyList)
                {
                    List list = clientContext.Web.Lists.GetByTitle(matterName);

                    foreach (var item in propertyList)
                    {
                        props[item.Key] = item.Value;
                        list.Update();
                    }

                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
