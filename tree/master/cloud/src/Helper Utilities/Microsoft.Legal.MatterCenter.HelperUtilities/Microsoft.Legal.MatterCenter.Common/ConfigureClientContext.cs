// ***********************************************************************
// <copyright file="ConfigureClientContext.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods to configure SharePoint Client Context.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Common
// Author           : v-diajme
// Created          : 06-19-2014
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Common
{
    #region using
    using Microsoft.SharePoint.Client;
    using System.Net;
    using System.Security;
    #endregion

    /// <summary>
    /// A class to provides SharePoint related operations
    /// </summary>
    public static class ConfigureSharePointContext
    {
        /// <summary>
        /// Creating client context along with authentication
        /// </summary>
        /// <param name="url">Site URL</param>
        /// <param name="userId">User id</param>
        /// <param name="password">Password to authenticate</param>
        /// <param name="isDeployedOnAzure">To resolve what kind of credentials are to be use</param>
        /// <returns>Client context</returns>
        public static ClientContext ConfigureClientContext(string url, string userId, string password, bool isDeployedOnAzure)
        {
            using (var securePassword = new SecureString())
            {
                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(url))
                {
                    foreach (char character in password)
                    {
                        securePassword.AppendChar(character);
                    }
                    using (ClientContext clientContext = new ClientContext(url))
                    {
                        object onlineCredentials;
                        if (isDeployedOnAzure)
                        {
                            onlineCredentials = new SharePointOnlineCredentials(userId, securePassword);
                            clientContext.Credentials = (SharePointOnlineCredentials)onlineCredentials; // Secure the credentials and generate the SharePoint Online Credentials                    
                        }
                        else
                        {
                            onlineCredentials = new NetworkCredential(userId, securePassword);
                            clientContext.Credentials = (NetworkCredential)onlineCredentials; // Assign On Premise credentials to the Client Context
                        }
                        clientContext.ExecuteQuery();
                        return clientContext;
                    }
                }
                return null;
            }
        }
    }
}
