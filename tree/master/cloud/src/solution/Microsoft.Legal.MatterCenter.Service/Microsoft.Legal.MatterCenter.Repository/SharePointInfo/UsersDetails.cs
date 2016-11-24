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
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        private IHttpContextAccessor httpContextAccessor;
        private ListNames listNames;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private GeneralSettings generalSettings;
        /// <summary>
        /// Constructir where all the dependencies are injected
        /// </summary>
        /// <param name="spoAuthorization"></param>
        public UsersDetails(IOptions<MatterSettings> matterSettings, 
            IOptions<ListNames> listNames,
            ISPOAuthorization spoAuthorization, 
            ICustomLogger customLogger, 
            IOptions<LogTables> logTables,
            IHttpContextAccessor httpContextAccessor,
            IOptions<GeneralSettings> generalSettings)
        {
            this.matterSettings = matterSettings.Value;
            this.listNames = listNames.Value;
            this.spoAuthorization = spoAuthorization;
            //this.spList = spList;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.generalSettings = generalSettings.Value;
        }


        /// <summary>
        /// Get HttpContext object from IHttpContextAccessor to read Http Request Headers
        /// </summary>
        private HttpContext Context => httpContextAccessor.HttpContext;

        /// <summary>
        /// This method will get user profile picture of the login user
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<Users> GetUserProfilePicture(Client client)
        {
            Users users = new Users();
            try
            {
                string accessToken = spoAuthorization.GetGraphAccessToken();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);                    
                    var url = $"{generalSettings.GraphUrl}/v1.0/me/photo/$value";
                    var resultAsString = await httpClient.GetStreamAsync(url);

                    using (var newStream = new MemoryStream())
                    {
                        resultAsString.CopyTo(newStream);
                        byte[] bytes = newStream.ToArray();
                        users.LargePictureUrl = ServiceConstants.BASE64_IMAGE_FORMAT + Convert.ToBase64String(bytes);
                    }
                    return users;
                }
            }
            catch(Exception ex) when(ex.Message.Contains("404"))
            {
                users.LargePictureUrl = "";
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        /// <returns></returns>
        public bool CheckUserPresentInMatterCenter(ClientContext clientContext, string email)
        {
            try
            {
                //If the current email is part of current organization, no need to check for validity of the user email
                if (email.Trim().ToLower().IndexOf(generalSettings.Tenant.Trim().ToLower()) > 0)
                {
                    return true;
                }
                string userAlias = email;
                ClientPeoplePickerQueryParameters queryParams = new ClientPeoplePickerQueryParameters();
                queryParams.AllowMultipleEntities = false;
                queryParams.MaximumEntitySuggestions = 500;
                queryParams.PrincipalSource = PrincipalSource.All;
                queryParams.PrincipalType = PrincipalType.User | PrincipalType.SecurityGroup;
                queryParams.QueryString = userAlias;
                ClientResult<string> clientResult = ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(clientContext, queryParams);
                clientContext.ExecuteQuery();
                string results = clientResult.Value;
                int peoplePickerMaxRecords = 30;
                IList<PeoplePickerUser> foundUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeoplePickerUser>>(results).Where(result => (string.Equals(result.EntityType, ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER,
                        StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.Description)) || (!string.Equals(result.EntityType,
                        ServiceConstants.PEOPLE_PICKER_ENTITY_TYPE_USER, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(result.EntityData.Email))).Take(peoplePickerMaxRecords).ToList();
                return foundUsers.Count > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        /// <summary>
        /// This method will check whether user exists in a sharepoint site or not
        /// </summary>
        /// <param name="clientUrl"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckUserPresentInMatterCenter(string clientUrl, string email)
        {
            try
            {
                //If the current email is part of current organization, no need to check for validity of the user
                if(email.Trim().ToLower().IndexOf(generalSettings.Tenant.Trim().ToLower()) >0)
                {
                    return true;
                }
                var clientContext = spoAuthorization.GetClientContext(clientUrl);
                return CheckUserPresentInMatterCenter(clientContext, email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will check whether the given login user is present in a 
        /// sharepoint group or not
        /// </summary>
        /// <param name="client"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        //public bool CheckUserPermissionsInGroup(Client client, string groupName)
        //{
        //    try
        //    {                   
        //        ClientContext clientContext = spoAuthorization.GetClientContext(client.Url);
        //        Web web = clientContext.Web;
        //        //Get the group name in which to check the user
        //        Group group = web.SiteGroups.GetByName(groupName);
        //        clientContext.Load(group, grp => grp.Title, grp => grp.Users);
        //        clientContext.ExecuteQuery();
        //        //Loop through all the users in that group
        //        foreach (User usr in group.Users)
        //        {
        //            //If the user is found in that group, return true else return false
        //            if (usr.Email.ToLower().Trim() == Context.User.Identity.Name.ToLower().Trim())
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        //If the user doesn't belong to the user, the code will throw the exception and the user may not belong to the group and 
        //        //log the message and return false from the method
        //        customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
        //        return false;
                
        //    }
        //}

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
        //public bool GetUserAccess(Client client) => spList.CheckPermissionOnList(client, listNames.SendMailListName, PermissionKind.EditListItems);


        public IList<FieldUserValue> ResolveUserNames(ClientContext clientContext, IList<string> userNames)
        {
            List<FieldUserValue> userList = new List<FieldUserValue>();
            foreach (string userName in userNames)
            {
                //Check has been made to check whether the user is present in the system as part of external sharing implementation
                if (!string.IsNullOrWhiteSpace(userName) && 
                    CheckUserPresentInMatterCenter(clientContext, userName))
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
                                //Check has been made to check whether the user is present in the system as part of external sharing implementation
                                if (CheckUserPresentInMatterCenter(clientContext, teamMember))
                                { 
                                    Principal teamMemberPrincipal = clientContext.Web.EnsureUser(teamMember);
                                    clientContext.Load(teamMemberPrincipal, teamMemberPrincipalProperties =>
                                                teamMemberPrincipalProperties.PrincipalType,
                                                teamMemberPrincipalProperties => teamMemberPrincipalProperties.Title);
                                    teamMemberPrincipalCollection.Add(new Tuple<int, Principal>(iterator, teamMemberPrincipal));
                                }
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


        /// <summary>
        /// This method will check if the current login user is part of site owners group or not
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsLoginUserOwner(Client client)
        {
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    UserCollection siteOwners = clientContext.Web.AssociatedOwnerGroup.Users;
                    clientContext.Load(siteOwners, owners => owners.Include(owner => owner.Title));
                    clientContext.Load(clientContext.Web.CurrentUser);
                    clientContext.ExecuteQuery();
                    return siteOwners.Any(owners => owners.Title.Equals(clientContext.Web.CurrentUser.Title));
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
