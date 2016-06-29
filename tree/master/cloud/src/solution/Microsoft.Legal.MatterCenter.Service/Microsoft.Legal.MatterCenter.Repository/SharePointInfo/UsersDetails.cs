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
// <summary>This file provide methods to get information related to user</summary>
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Globalization;
using System.IO;
#endregion

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This class contains all the methods which are related to user information
    /// </summary>
    public class UsersDetails : IUsersDetails
    {
        private MatterSettings matterSettings;
        private ISPOAuthorization spoAuthorization;
        private ISPList spList;
        private ListNames listNames;
        private ICustomLogger customLogger;
        private LogTables logTables;
        /// <summary>
        /// Constructir where all the dependencies are injected
        /// </summary>
        /// <param name="spoAuthorization"></param>
        public UsersDetails(IOptionsMonitor<MatterSettings> matterSettings, IOptionsMonitor<ListNames> listNames, 
            ISPOAuthorization spoAuthorization, ISPList spList, ICustomLogger customLogger, IOptionsMonitor<LogTables> logTables)
        {
            this.matterSettings = matterSettings.CurrentValue;
            this.listNames = listNames.CurrentValue;
            this.spoAuthorization = spoAuthorization;
            this.spList = spList;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
        }

        public Users GetUserProfilePicture(Client client)
        {
            ClientContext clientContext = spoAuthorization.GetClientContext(client.Url);
            PeopleManager peopleManager = new PeopleManager(clientContext);            
            ClientResult<string> userProfile = peopleManager.GetUserProfilePropertyFor(spoAuthorization.AccountName, "PictureURL");            
            clientContext.ExecuteQuery();
            string personalURL = userProfile.Value;
            
            string smallProfilePicture = personalURL.Replace("MThumb.jpg", "SThumb.jpg");
            string mediumProfilePicture = personalURL;
            Users users = new Users();
            
            users.SmallPictureUrl = smallProfilePicture;
            users.LargePictureUrl = mediumProfilePicture;
            return users;
            //return null;
        }

        

        /// <summary>
        /// This method will return the user object who has currently logged into the system
        /// </summary>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public Users GetLoggedInUserDetails(ClientContext clientContext)
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
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
        }

        /// <summary>
        /// Over loaded method
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Users GetLoggedInUserDetails(Client client)
        {
            try
            {
                ClientContext clientContext = spoAuthorization.GetClientContext(client.Url);
                return GetLoggedInUserDetails(clientContext);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Function to check user full permission on document library
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="matter"></param>
        /// <returns></returns>
        public bool CheckUserFullPermission(ClientContext clientContext, Matter matter)
        {
            bool result = false;
            try
            {                
                if (null != matter)
                {
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(matter.Name);
                    Users userDetails = GetLoggedInUserDetails(clientContext);
                    Principal userPrincipal = web.EnsureUser(userDetails.Name);
                    RoleAssignment userRole = list.RoleAssignments.GetByPrincipal(userPrincipal);
                    clientContext.Load(userRole, userRoleProperties => userRoleProperties.Member,
                        userRoleProperties => userRoleProperties.RoleDefinitionBindings.Include(userRoleDefinition => userRoleDefinition.Name).Where(userRoleDefinitionName => userRoleDefinitionName.Name ==
                        matterSettings.EditMatterAllowedPermissionLevel));
                    clientContext.ExecuteQuery();
                    if (0 < userRole.RoleDefinitionBindings.Count)
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            
        }

        /// <summary>
        /// Gets the user access.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool GetUserAccess(Client client) => spList.CheckPermissionOnList(client, listNames.SendMailListName, PermissionKind.EditListItems);


        public IList<FieldUserValue> ResolveUserNames(ClientContext clientContext, IList<string> userNames)
        {
            List<FieldUserValue> userList = new List<FieldUserValue>();
            foreach (string userName in userNames)
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    User user = clientContext.Web.EnsureUser(userName.Trim());
                    ///// Only Fetch the User ID which is required
                    clientContext.Load(user, u => u.Id);
                    clientContext.ExecuteQuery();
                    ///// Add the user to the first element of the FieldUserValue array.
                    FieldUserValue tempUser = new FieldUserValue();
                    tempUser.LookupId = user.Id;
                    userList.Add(tempUser);
                }
            }           
            return userList;
        }
        /// <summary>
        /// Bulk resolves the specified users.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="userNames">Name of the users to be resolved</param>
        /// <returns>List of resolved users</returns>
        public IList<FieldUserValue> ResolveUserNames(Client client, IList<string> userNames)
        {
            try
            {
                
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    return ResolveUserNames(clientContext, userNames);
                    
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Get the principal associated to the user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="matter">Matter Information</param>
        /// <param name="userIds">The userids for which we need to return the principals</param>
        /// <returns></returns>
        public List<Tuple<int, Principal>> GetUserPrincipal(Client client, Matter matter, IList<string> userIds)
        {
            List<Tuple<int, Principal>> teamMemberPrincipalCollection = new List<Tuple<int, Principal>>();
            int securityGroupRowNumber = -1;
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
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
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
