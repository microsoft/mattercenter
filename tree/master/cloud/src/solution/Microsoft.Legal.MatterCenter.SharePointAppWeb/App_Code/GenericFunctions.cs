// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-nikhid
// Created          : 06-16-2014
//
// ***********************************************************************
// <copyright file="GenericFunctions.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines methods related to resource files.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Web;
    #endregion

    /// <summary>
    /// Generic functions used in SharePoint application.
    /// </summary>
    public static class GenericFunctions
    {
        /// <summary>
        /// Gets the data from resource file and stores it in a JSON object.
        /// </summary>
        /// <param name="fileName">Name of resource file</param>
        /// <param name="resourceFileLocation">Location of resource file</param>
        /// <returns>
        /// JSON object which will have the data from resource file.
        /// </returns>
        public static string GetResourceData(string fileName, string resourceFileLocation)
        {
            ResourceSet resourceSet;
            StringBuilder scriptBuilder = new StringBuilder();
            try
            {
                using (ResXResourceReader resxReader = new ResXResourceReader(HttpContext.Current.Server.MapPath(@"~/" + resourceFileLocation + ConstantStrings.ForwardSlash + fileName + ConstantStrings.ResourceFileExtension)))
                {
                    resourceSet = new ResourceSet(resxReader);
                    foreach (DictionaryEntry entry in resourceSet)
                    {
                        string resourceKey = (string)entry.Key;
                        object resource = entry.Value;
                        scriptBuilder.Append("\"" + resourceKey + "\":" + "\"" + resource + "\",");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }

            return string.Concat(Convert.ToString(scriptBuilder, CultureInfo.InvariantCulture).TrimEnd(','));
        }

        /// <summary>
        /// Forms the global object with configurable data from Azure.
        /// </summary>
        /// <param name="resourceFile">The resource file containing Global Configurations details</param>
        /// <param name="jsonObject">JSON object which will have the data from resource file</param>
        /// <param name="resourceFileLocation">Location of resource file</param>
        /// <returns>
        /// JavaScript object as string
        /// </returns>
        public static string SetGlobalConfigurations(string resourceFile, string jsonObject, Enumerators.ResourceFileLocation resourceFileLocation)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            try
            {
                if (!string.IsNullOrWhiteSpace(resourceFile) && !string.IsNullOrWhiteSpace(jsonObject))
                {
                    // Add the script tag                
                    scriptBuilder.Append("<script type=\"text/javascript\">var " + jsonObject + "={");
                    scriptBuilder.Append(GetResourceData(resourceFile, Convert.ToString(resourceFileLocation, CultureInfo.InvariantCulture)));
                    scriptBuilder.Append("};</script>");
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }

            return Convert.ToString(scriptBuilder, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the error object to be stored on the page
        /// </summary>
        /// <param name="isErrorOccured">Property to check if the error has occurred</param>
        /// <param name="errorPopupData">The object having a code:value pair</param>
        /// <returns>JavaScript object as string</returns>
        public static string SetErrorResponse(string isErrorOccured, string errorPopupData)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            try
            {
                // Add the script tag                
                scriptBuilder.Append("<script type=\"text/javascript\">var oErrorObject={");
                scriptBuilder.Append("\"isErrorOccured\":" + "\"" + isErrorOccured + "\",");
                scriptBuilder.Append("\"errorPopupData\":" + errorPopupData);
                scriptBuilder.Append("};</script>");
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }

            return Convert.ToString(scriptBuilder, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Set the cookies required by Matter Center app
        /// </summary>
        /// <param name="request">Request Object</param>
        /// <param name="response">Response Object</param>
        /// <param name="redirectURL">Redirect URL</param>
        /// <param name="constantFileObject">Object to fetch constants from resource file</param>
        /// <param name="constantFile">Name of the app specific resource file</param>
        /// <param name="isTokenRequestFailure">Parameter to check if Token Request failed or not</param>
        /// <returns>Refresh Token</returns>
        private static string SetMatterCenterCookie(HttpRequest request, HttpResponse response, string redirectURL, string constantFileObject, string constantFile, bool isTokenRequestFailure)
        {
            string refreshToken = string.Empty;
            //// Redirect in case of the App Token is not set by SharePoint
            if (string.IsNullOrEmpty(request.Form[UIConstantStrings.SPAppToken]))
            {
                response.Redirect(redirectURL, false);
            }
            else
            {
                //// Regenerate the refresh token from Sp App Token
                refreshToken = UIUtility.GetRefreshToken(request.Form[UIConstantStrings.SPAppToken]);
                if (isTokenRequestFailure)
                {
                    //// Reset the cookie with new value of refresh token
                    response.Cookies[UIConstantStrings.refreshToken].Value = refreshToken;
                }
                else
                {
                    HttpCookie data = new HttpCookie(UIConstantStrings.refreshToken, refreshToken);
                    data.Secure = true;
                    data.Expires = DateTime.Now.AddHours(Convert.ToInt32(UIConstantStrings.RefreshTokenCookieExpiration, CultureInfo.InvariantCulture));
                    response.Cookies.Add(data);
                    SetResponse(request, response, constantFileObject, constantFile, refreshToken);
                    response.Write(UIUtility.SetSharePointResponse(refreshToken));
                }
            }
            return refreshToken;
        }

        /// <summary>
        /// Sets constants response from resource file and stores the refresh token in the cookie.
        /// </summary>
        /// <param name="request">Request Object</param>
        /// <param name="response">Response Object</param>
        /// <param name="queryStringCookieName">Cookie Name</param>
        /// <param name="redirectURL">Redirect URL</param>
        /// <param name="constantFileObject">Object to fetch constants from resource file</param>
        /// <param name="constantFile">Name of the app specific resource file</param>
        internal static void SetConstantsResponse(HttpRequest request, HttpResponse response, string redirectURL, string constantFileObject, string constantFile)
        {
            string refreshToken = string.Empty;
            try
            {
                bool environment = Convert.ToBoolean(UIConstantStrings.IsDeployedOnAzure, CultureInfo.InvariantCulture);
                if (environment)
                {
                    //// Check if page loaded due to Token Request failure issue (Query string will contain IsInvalidToken parameter)
                    if (request.Url.Query.ToString().ToUpperInvariant().Contains(UIConstantStrings.TokenRequestFailedQueryString.ToUpperInvariant()))
                    {
                        refreshToken = SetMatterCenterCookie(request, response, redirectURL, constantFileObject, constantFile, true);
                    }
                    else
                    {
                        refreshToken = (null != request.Cookies[UIConstantStrings.refreshToken]) ? request.Cookies[UIConstantStrings.refreshToken].Value : string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        refreshToken = SetMatterCenterCookie(request, response, redirectURL, constantFileObject, constantFile, false);
                    }
                    else
                    {
                        string DecryptedrefreshToken = string.Empty;
                        string key = ConfigurationManager.AppSettings["Encryption_Key"];
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            DecryptedrefreshToken = EncryptionDecryption.Decrypt(refreshToken, key);
                            if (!string.IsNullOrWhiteSpace(DecryptedrefreshToken))
                            {
                                SetResponse(request, response, constantFileObject, constantFile, refreshToken);
                                response.Write(UIUtility.SetSharePointResponse(refreshToken));
                            }
                            else
                            {
                                key = ConfigurationManager.AppSettings["Old_Encryption_Key"];
                                if (!string.IsNullOrWhiteSpace(key))
                                {
                                    DecryptedrefreshToken = EncryptionDecryption.Decrypt(refreshToken, key);
                                    if (!string.IsNullOrWhiteSpace(DecryptedrefreshToken))
                                    {
                                        request.Cookies[UIConstantStrings.refreshToken].Value = refreshToken = EncryptionDecryption.Encrypt(DecryptedrefreshToken);
                                        SetResponse(request, response, constantFileObject, constantFile, refreshToken);
                                        response.Write(UIUtility.SetSharePointResponse(refreshToken));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    SetResponse(request, response, constantFileObject, constantFile, refreshToken);
                    response.Write(UIUtility.SetSharePointResponse(refreshToken));
                }
            }
            catch (Exception exception)
            {
                string result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, result));
            }
        }

        /// <summary>
        /// Sets response on the page.
        /// </summary>
        /// <param name="request">Request Object</param>
        /// <param name="response">Response Object</param>
        /// <param name="constantFileObject">Name of the constant file object</param>
        /// <param name="constantFile">Name of the constant file</param>
        /// <param name="globalConstantFile">Name of the global constant file</param>
        /// <param name="environment">Deployed on Azure flag</param>
        /// <param name="refreshToken">Refresh Token for client context</param>
        private static void SetResponse(HttpRequest request, HttpResponse response, string constantFileObject, string constantFile, string refreshToken)
        {
            bool bContainsEdit = false, isSettingsPage = false;
            try
            {
                bContainsEdit = request.Url.Query.Contains(UIConstantStrings.IsEdit);
                string url = HttpUtility.UrlDecode(request.Url.Query);
                if (bContainsEdit)
                {
                    string clientUrl = HttpUtility.ParseQueryString(url).Get(UIConstantStrings.clientUrl);
                    string matterName = HttpUtility.ParseQueryString(url).Get(UIConstantStrings.matterName);
                    if (!UIUtility.GetUserAccess(refreshToken, new Uri(ConstantStrings.ProvisionMatterAppURL), request) || !UIUtility.CheckUserManagePermission(refreshToken, new Uri(clientUrl), matterName, request))
                    {
                        response.Write(UIConstantStrings.EditMatterAccessDeniedMessage);
                        response.End();
                    }
                }
                else if (string.Equals(constantFile, ConstantStrings.ConstantFileForSettings, StringComparison.OrdinalIgnoreCase))
                {
                    isSettingsPage = true;
                    string clientdetails = HttpUtility.ParseQueryString(url).Get(UIConstantStrings.clientDetails);
                    string clientUrl = string.IsNullOrWhiteSpace(clientdetails) ? UIConstantStrings.TenantUrl : clientdetails.Split(new string[] { ConstantStrings.DOLLAR + ConstantStrings.Pipe + ConstantStrings.DOLLAR }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (!ServiceUtility.GetUserGroup(refreshToken, new Uri(clientUrl), request))
                    {
                        response.Write(string.Format(CultureInfo.InvariantCulture, UIConstantStrings.SettingsPageAccessDeniedMessage, UIConstantStrings.MatterCenterSupportEmail));
                        response.End();
                    }
                }
                else
                {
                    if (string.Equals(constantFile, ConstantStrings.ConstantFileForProvision, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!UIUtility.GetUserAccess(refreshToken, new Uri(ConstantStrings.ProvisionMatterAppURL), request))
                        {
                            response.Redirect(UIConstantStrings.ServicePathFindMatter, false);
                        }
                    }
                }

                response.Write(SetGlobalConfigurations("Constants", UIConstantStrings.GlobalConstants, Enumerators.ResourceFileLocation.App_GlobalResources));
                response.Write(SetGlobalConfigurations(constantFile, constantFileObject, Enumerators.ResourceFileLocation.App_LocalResources));
            }
            catch (Exception exception)
            {
                if (!bContainsEdit && !isSettingsPage)
                {
                    Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                }
            }
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <returns>redirect URL</returns>
        internal static string GetRedirectURL(HttpRequest request)
        {
            try
            {
                string redirectURL = string.Empty;
                if (!string.IsNullOrWhiteSpace(request.Url.Query))
                {
                    redirectURL = string.Concat(UIConstantStrings.AppRedirectURL, ConstantStrings.QUESTION, ConstantStrings.ClientId, ConstantStrings.OperatorEqual, UIConstantStrings.ClientID, ConstantStrings.OperatorAmpersand, ConstantStrings.RedirectUrl, ConstantStrings.OperatorEqual, request.Url.AbsoluteUri.Replace(ConstantStrings.HTTP + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash, ConstantStrings.HTTPS + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash).Replace(request.Url.Query, string.Empty), ConstantStrings.QUESTION, ConstantStrings.StandardToken);
                }
                else
                {
                    redirectURL = string.Concat(UIConstantStrings.AppRedirectURL, ConstantStrings.QUESTION, ConstantStrings.ClientId, ConstantStrings.OperatorEqual, UIConstantStrings.ClientID, ConstantStrings.OperatorAmpersand, ConstantStrings.RedirectUrl, ConstantStrings.OperatorEqual, request.Url.AbsoluteUri.Replace(ConstantStrings.HTTP + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash, ConstantStrings.HTTPS + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash), ConstantStrings.QUESTION, ConstantStrings.StandardToken);
                }

                return redirectURL;
            }
            catch (Exception exception)
            {
                return Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Append appropriate query parameter to identify application type
        /// </summary>
        /// <param name="redirectURL">Redirect URL</param>
        /// <param name="request">HTTP request</param>
        /// <returns>Redirect URL with appropriate query parameter appended</returns>
        internal static string AppendQueryParameter(string redirectURL, HttpRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Url.Query))
            {
                redirectURL = string.Concat(redirectURL, ConstantStrings.QUESTION, UIConstantStrings.QUERY_PARAMETER_APPTYPE, ConstantStrings.OperatorEqual);
                string appType = request.QueryString[UIConstantStrings.QUERY_PARAMETER_APPTYPE];
                if (!string.IsNullOrEmpty(appType))
                {
                    if (string.Equals(appType, UIConstantStrings.IS_OUTLOOK, StringComparison.OrdinalIgnoreCase))
                    {
                        redirectURL = string.Concat(redirectURL, UIConstantStrings.IS_OUTLOOK);
                    }
                    else if (string.Equals(appType, UIConstantStrings.IS_OFFICE, StringComparison.OrdinalIgnoreCase))
                    {
                        redirectURL = string.Concat(redirectURL, UIConstantStrings.IS_OFFICE);
                    }
                }
            }
            return redirectURL;
        }
    }
}