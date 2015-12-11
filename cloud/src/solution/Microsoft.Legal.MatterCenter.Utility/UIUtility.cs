// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-rijadh
// Created          : 03-14-2014
//
// ***********************************************************************
// <copyright file="UIUtility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines common function used by UI layer.</summary>
// ***********************************************************************
//// Keeping using System over here because of usage of CLSComplaint attribute for namespace
using System;
[assembly: CLSCompliant(true)]
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using Microsoft.Legal.MatterCenter.DataLayer;
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.SharePoint.Client;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    #endregion

    /// <summary>
    /// Provides methods used by the UI.
    /// </summary>
    [CLSCompliant(false)]
    public static class UIUtility
    {
        /// <summary>
        /// Gets the refresh token from the specified content token.
        /// </summary>
        /// <param name="contextToken">SharePoint App Token as context token</param>
        /// <returns>
        /// Refresh Token as string
        /// </returns>
        public static string GetRefreshToken(string contextToken)
        {
            string refreshToken = string.Empty;
            if (!string.IsNullOrWhiteSpace(contextToken))
            {
                SharePointContextToken contextTokenData = TokenHelper.ReadAndValidateContextToken(contextToken);
                refreshToken = null != contextTokenData ? EncryptionDecryption.Encrypt(contextTokenData.RefreshToken) : string.Empty;
            }
            return refreshToken;
        }

        /// <summary>
        /// Forms the global object with SharePoint context.
        /// </summary>
        /// <param name="refreshToken">Refresh Token</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>
        /// JS object as string
        /// </returns>
        public static string SetSharePointResponse(string refreshToken)
        {
            string scriptContent = string.Empty;
            refreshToken = !string.IsNullOrWhiteSpace(refreshToken) ? refreshToken : string.Empty;
            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendFormat("<script type=\"text/javascript\">var oSharePointContext={{\"SPAppToken\":\"\",\"RefreshToken\":" + "\"{0}\"", refreshToken);

            // Remove the final comma from the object
            scriptContent = Convert.ToString(scriptBuilder, CultureInfo.InvariantCulture);

            // Close the script tag
            scriptContent += "};</script>";
            return scriptContent;
        }

        /// <summary>
        /// Gets the user access.
        /// </summary>
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="clientUrl">The client URL for Client Context</param>
        /// <param name="request">The HTTP request.</param>
        /// <param name="environment">environment identifier</param>
        /// <returns>User has access</returns>
        public static bool GetUserAccess(string refreshToken, Uri clientUrl, HttpRequest request)
        {
            bool flag = false;
            if (request != null)
            {
                try
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(null, clientUrl, refreshToken, request))
                    {
                        flag = Lists.CheckPermissionOnList(clientContext, ConstantStrings.SendMailListName, PermissionKind.EditListItems);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
                    throw;
                }
            }

            return flag;
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
        public static bool CheckUserFullPermission(ClientContext clientContext, Matter matter)
        {
            bool result = false;
            try
            {
                if (null != clientContext && null != matter)
                {
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(matter.Name);
                    Users userDetails = GetLoggedInUserDetails(clientContext);
                    Principal userPrincipal = web.EnsureUser(userDetails.Name);
                    RoleAssignment userRole = list.RoleAssignments.GetByPrincipal(userPrincipal);
                    clientContext.Load(userRole, userRoleProperties => userRoleProperties.Member, userRoleProperties => userRoleProperties.RoleDefinitionBindings.Include(userRoleDefinition => userRoleDefinition.Name).Where(userRoleDefinitionName => userRoleDefinitionName.Name == ConstantStrings.EditMatterAllowedPermissionLevel));
                    clientContext.ExecuteQuery();
                    if (0 < userRole.RoleDefinitionBindings.Count)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Gets the logged in user details.
        /// </summary>
        /// <param name="context">The Client Context</param>
        /// <returns>User object with name, logon name and email address.</returns>
        public static Users GetLoggedInUserDetails(ClientContext context)
        {
            Users currentUserDetail = new Users();
            try
            {
                if (null != context)
                {
                    context.Load(context.Web.CurrentUser, userInfo => userInfo.Title, userInfo => userInfo.Email, userInfo => userInfo.LoginName);
                    context.ExecuteQuery();
                    currentUserDetail.Name = context.Web.CurrentUser.Title;
                    currentUserDetail.Email = context.Web.CurrentUser.Email;

                    //Check if email is available to get account name, if not use login name
                    if (!String.IsNullOrEmpty(currentUserDetail.Email))
                    {
                        currentUserDetail.LogOnName = currentUserDetail.Email;
                    }
                    else
                    {
                        currentUserDetail.LogOnName = context.Web.CurrentUser.LoginName;
                    }

                    //Retrieve user name from login
                    int splitPositionStart;
                    int splitPositionEnd = currentUserDetail.LogOnName.LastIndexOf(ConstantStrings.SymbolAt, StringComparison.OrdinalIgnoreCase);
                    if (splitPositionEnd == -1)  //The user is an on-premise account
                    {
                        splitPositionStart = currentUserDetail.LogOnName.LastIndexOf(ConstantStrings.BackwardSlash, StringComparison.OrdinalIgnoreCase) + 1;
                        splitPositionEnd = currentUserDetail.LogOnName.Length;
                    }
                    else
                    {
                        splitPositionStart = currentUserDetail.LogOnName.LastIndexOf(ConstantStrings.Pipe, StringComparison.OrdinalIgnoreCase) + 1;
                    }
                    currentUserDetail.LogOnName = currentUserDetail.LogOnName.Substring(splitPositionStart, splitPositionEnd - splitPositionStart);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return currentUserDetail;
        }

        /// <summary>
        /// Downloads the image and converts it to base64 strings
        /// </summary>
        /// <param name="ClientContext">client context.</param>        
        /// <param name="imagePath">Includes the image path.</param>
        public static string GetImageInBase64Format(ClientContext clientContext, string imagePath)
        {
            string result = string.Empty;
            try
            {
                if (null != clientContext)
                {
                    Microsoft.SharePoint.Client.File userImage = clientContext.Web.GetFileByServerRelativeUrl(imagePath);
                    ClientResult<Stream> userImageStream = userImage.OpenBinaryStream();
                    clientContext.Load(userImage);
                    clientContext.ExecuteQuery();
                    if (userImage.Exists)
                    {
                        using (var newStream = new MemoryStream())
                        {
                            userImageStream.Value.CopyTo(newStream);
                            byte[] bytes = newStream.ToArray();
                            result = ConstantStrings.base64ImageFormat + Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return result;
        }

        /// <summary>
        /// Checks if the folder or the document library exists.
        /// </summary>
        /// <param name="folderPath">Relative path of folder</param>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="documentLibraryName">Name of the document library no which upload operation is being performed</param>
        /// <returns>Folder exists or not</returns>
        public static bool FolderExists(string folderPath, ClientContext clientContext, string documentLibraryName)
        {
            bool folderFound = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(documentLibraryName) && null != clientContext)
                {
                    string folderName = folderPath.Substring(folderPath.LastIndexOf(ConstantStrings.ForwardSlash, StringComparison.OrdinalIgnoreCase) + 1);
                    List docLibList = clientContext.Web.Lists.GetByTitle(documentLibraryName);
                    ListItemCollection folderList = docLibList.GetItems(CamlQuery.CreateAllFoldersQuery());
                    clientContext.Load(clientContext.Web, web => web.ServerRelativeUrl);
                    clientContext.Load(docLibList, list => list.Title);
                    clientContext.Load(folderList, item => item.Include(currentItem => currentItem.Folder.Name, currentItem => currentItem.Folder.ServerRelativeUrl).Where(currentItem => currentItem.Folder.ServerRelativeUrl == folderPath));
                    clientContext.ExecuteQuery();

                    if (null != docLibList)
                    {
                        string rootFolderURL = string.Concat(clientContext.Web.ServerRelativeUrl, ConstantStrings.ForwardSlash + folderName);
                        if (string.Equals(rootFolderURL, folderPath, StringComparison.OrdinalIgnoreCase))
                        {
                            //// Upload is performed on root folder
                            folderFound = null != docLibList && docLibList.Title.ToUpperInvariant().Equals(documentLibraryName.ToUpperInvariant());
                        }
                        else
                        {
                            //// Upload is performed on different folder other than root folder
                            folderFound = 0 < folderList.Count;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return folderFound;
        }

        /// <summary>
        /// Function to check user manage permission on document library
        /// </summary>
        /// <param name="refreshToken">The refresh token for Client Context</param>
        /// <param name="clientUrl">The client URL for Client Context</param>
        /// <param name="matterName">Document library name</param>
        /// <param name="request">The HTTP request</param>
        /// <returns>A Boolean variable indicating whether user has manage permission on the matter</returns>
        public static bool CheckUserManagePermission(string refreshToken, Uri clientUrl, string matterName, HttpRequest request)
        {
            bool result = false;
            try
            {
                if (null != request)
                {
                    using (ClientContext clientContext = ServiceUtility.GetClientContext(null, clientUrl, refreshToken, request))
                    {
                        result = Lists.CheckPermissionOnList(clientContext, matterName, PermissionKind.ManagePermissions);
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}