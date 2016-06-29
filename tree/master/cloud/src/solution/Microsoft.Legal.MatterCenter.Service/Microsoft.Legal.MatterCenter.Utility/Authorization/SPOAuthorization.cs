// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="SPOAuthorization.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Globalization;


#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Models;
using System.Reflection;
#endregion

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This class is used for reading authorization token which has been sent by the client.This
    /// class will validate the client token, get the token  for the service from Azure Active Directory
    /// and pass the service token to sharepoint
    /// </summary>
    public class SPOAuthorization: ISPOAuthorization
    {
        private GeneralSettings generalSettings;
        private ErrorSettings errorSettings;
        private ICustomLogger customLogger;
        private LogTables logTables;
        private string accessToken;
        private string accountName;
        /// <summary>
        /// Constructor where GeneralSettings and ErrorSettings are injected
        /// </summary>
        /// <param name="generalSettings"></param>
        /// <param name="errorSettings"></param>
        public SPOAuthorization(IOptionsMonitor<GeneralSettings> generalSettings, IOptionsMonitor<ErrorSettings> errorSettings, IOptionsMonitor<LogTables> logTables, 
            ICustomLogger customLogger)
        {            
            this.generalSettings = generalSettings.CurrentValue;
            this.errorSettings = errorSettings.CurrentValue;
            this.customLogger = customLogger;
            this.logTables = logTables.CurrentValue;
        }
        
        /// <summary>
        /// Property which will set or get the access token
        /// </summary>
        public string AccessToken
        {
            get
            {
                return accessToken;
            }
            set
            {
                accessToken = value.Split(' ')[1];
            }
        }

        /// <summary>
        /// Property which will set or get current logged in user
        /// </summary>
        public string AccountName
        {
            get
            {
                return accountName;
            }
            set
            {
                accountName = value;
            }
        }

        /// <summary>
        /// This method will get the access token for the service and creats SharePoint ClientContext object and returns that object
        /// </summary>
        /// <param name="url">The SharePoint Url for which the client context needs to be creatwed</param>
        /// <returns>ClientContext - Return SharePoint Client Context Object</returns>
        public ClientContext GetClientContext(string url)
        {
            try
            { 
                //ToDo: Try to get the service token from the client. Fi the client hasn't send the token
                //then try to get access token for service from AAD

                //ToDo: Try to validate the service token send by the client interms of expiry. If th token is expired, get the new token for the service 
                //from AAD

                string accessToken = GetAccessToken().Result;

                //ToDo: Once we get the access token from the service, try to send that service token back to the client so that
                //client will send that information for each and every request
                return GetClientContextWithAccessToken(Convert.ToString(url, CultureInfo.InvariantCulture), accessToken);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Helper method which will validate the token that has been sent by the client
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private ErrorResponse ValidateToken(string authToken)
        {
            try
            {
                //ToDo: Validate for token expiry
                //ToDo: Validate for token authenticity
                ErrorResponse authError = new ErrorResponse();
                var authBits = authToken.Split(' ');
                if (authBits.Length != 2)
                {
                    authError.IsTokenValid = false;
                    authError.Message = errorSettings.AuthorizationLengthError;
                    return authError;
                }
                if (!authBits[0].ToLowerInvariant().Equals("bearer"))
                {
                    authError.IsTokenValid = false;
                    authError.Message = errorSettings.NoBearerStringPresent;
                    return authError;
                }
                authError.IsTokenValid = true;
                authError.Message = "";
                return authError;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// This method will get access for the web api from the Azure Active Directory. 
        /// This method internally uses the Authorization token sent by the UI application
        /// </summary>
        /// <returns>Access Token for the web api service</returns>
        private async Task<string> GetAccessToken()
        {
            try
            {
                string clientId = generalSettings.ClientId;
                string appKey = generalSettings.AppKey;
                string aadInstance = generalSettings.AADInstance;
                string tenant = generalSettings.Tenant;
                string resource = generalSettings.Resource;                
                ClientCredential clientCred = new ClientCredential(clientId, appKey);
                UserAssertion userAssertion = new UserAssertion(AccessToken);
                string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
                //ToDo: Set the TokenCache to null. Need to implement custom token cache to support multiple users
                //If we dont have the custom cache, there will be some performance overhead.
                AuthenticationContext authContext = new AuthenticationContext(authority, null);
                AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred, userAssertion);
                return result.AccessToken;
            }
            catch(AggregateException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Uses the specified access token to create a client context. For each and every request to SPO
        /// an authorization header will be sent. With out authorization header, SPO will reject the request
        /// </summary>
        /// <param name="targetUrl">URL of the target SharePoint site</param>
        /// <param name="accessToken">Access token to be used when calling the specified targetUrl</param>
        /// <returns>A ClientContext ready to call targetUrl with the specified access token</returns>
        private ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        {
            try
            {
                ClientContext clientContext = new ClientContext(targetUrl);
                clientContext.AuthenticationMode = ClientAuthenticationMode.Anonymous;
                clientContext.FormDigestHandlingEnabled = false;
                clientContext.ExecutingWebRequest +=
                    delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
                    {
                        webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
                            "Bearer " + accessToken;
                    };
                return clientContext;
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }
    }
}
