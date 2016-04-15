

using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
using System.Reflection;

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
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
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
        
    }
}
