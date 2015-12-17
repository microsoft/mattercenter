// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-nikhid
// Created          : 02-01-2015
//
// ***********************************************************************
// <copyright file="Home.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of Home.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality to app landing page.
    /// </summary>
    public partial class Home : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string redirectURL = GenericFunctions.GetRedirectURL(this.Request);
                redirectURL = GenericFunctions.AppendQueryParameter(redirectURL, this.Request);
                GenericFunctions.SetConstantsResponse(this.Request, this.Response, redirectURL, null, null);
                /* Code to hide the Provision Matter icon if user is not having access Provision Matter App */
                // Get the refresh token which is saved in the cookies
                string refreshToken = null != this.Request.Cookies[UIConstantStrings.refreshToken] ? this.Request.Cookies[UIConstantStrings.refreshToken].Value : string.Empty;
                // Check whether the user is having access to Provision Matter App
                UIConstantStrings.ProvisionMatterAccess = UIUtility.GetUserAccess(refreshToken, new Uri(ConstantStrings.ProvisionMatterAppURL), this.Request);
                if (!UIConstantStrings.ProvisionMatterAccess)
                {
                    CreateMatterLink.Visible = false;
                }
            }
            catch (Exception exception)
            {
                string response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }
        }
    }
}