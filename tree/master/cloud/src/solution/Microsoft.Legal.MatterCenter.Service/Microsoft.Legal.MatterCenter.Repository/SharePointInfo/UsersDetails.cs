

using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
using System.Reflection;
using System.Collections.Generic;

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
        /// 
        /// </summary>
        /// <param name="spoAuthorization"></param>
        public UsersDetails(IOptions<MatterSettings> matterSettings, IOptions<ListNames> listNames, 
            ISPOAuthorization spoAuthorization, ISPList spList, ICustomLogger customLogger, IOptions<LogTables> logTables)
        {
            this.matterSettings = matterSettings.Value;
            this.listNames = listNames.Value;
            this.spoAuthorization = spoAuthorization;
            this.spList = spList;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
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

        public Users GetLoggedInUserDetails(Client client)
        {
            ClientContext clientContext = spoAuthorization.GetClientContext(client.Url);
            return GetLoggedInUserDetails(clientContext);
        }

        /// <summary>
        /// Function to check user full permission on document library
        /// </summary>
        /// <param name="environment">environment identifier</param>
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="clientUrl">The client URL for Client Context</param>
        /// <param name="matterName">Document library name</param>
        /// <param name="request">The HTTP request</param>
        /// <returns>A Boolean variable indicating whether user has full permission on the matter</returns>
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
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="clientUrl">The client URL for Client Context</param>
        /// <param name="request">The HTTP request.</param>
        /// <param name="environment">environment identifier</param>
        /// <returns>User has access</returns>
        public bool GetUserAccess(Client client) => spList.CheckPermissionOnList(client, listNames.SendMailListName, PermissionKind.EditListItems);

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
                List<FieldUserValue> userList = new List<FieldUserValue>();
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    {
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
                    }
                    return userList;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="currentRowTeamMembers"></param>
        /// <param name="blockedUsers"></param>
        /// <returns></returns>
        public List<Tuple<int, Principal>> GetUserPrincipal(Client client, Matter matter, IList<string> userIds)
        {
            List<Tuple<int, Principal>> teamMemberPrincipalCollection = new List<Tuple<int, Principal>>();
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    int teamMembersRowCount = matter.AssignUserNames.Count;
                    int securityGroupRowNumber = -1;
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
