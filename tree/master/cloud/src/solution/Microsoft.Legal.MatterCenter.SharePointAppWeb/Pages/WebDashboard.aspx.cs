// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-shpate
// Created          : 14-03-2015
//
// ***********************************************************************
// <copyright file="WebDashboard.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of WebDashboard.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides web dashboard functionalities.
    /// </summary>
    public partial class WebDashboard : System.Web.UI.Page
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
                string redirectURL = this.GetRedirectURL();
                GenericFunctions.SetConstantsResponse(this.Request, this.Response, redirectURL, ConstantStrings.ConstantObjectForWebDashboard, ConstantStrings.ConstantFileForWebDashboard);
            }
            catch (Exception exception)
            {
                string response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <returns>
        /// redirect URL
        /// </returns>
        private string GetRedirectURL()
        {
            string redirectURL = string.Empty;
            string result = string.Empty;
            try
            {
                redirectURL = string.Concat(UIConstantStrings.AppRedirectURL, ConstantStrings.QUESTION, ConstantStrings.ClientId, ConstantStrings.OperatorEqual, UIConstantStrings.ClientID, ConstantStrings.OperatorAmpersand, ConstantStrings.RedirectUrl, ConstantStrings.OperatorEqual, Request.Url.AbsoluteUri.Replace(ConstantStrings.HTTP + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash, ConstantStrings.HTTPS + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash), ConstantStrings.QUESTION, ConstantStrings.StandardToken);
                if (!string.IsNullOrWhiteSpace(Request.Url.Query) && !Request.Url.Query.ToUpperInvariant().Contains("?SECTION="))   // Not to clear query string if Section is present
                {
                    redirectURL = string.Concat(UIConstantStrings.AppRedirectURL, ConstantStrings.QUESTION, ConstantStrings.ClientId, ConstantStrings.OperatorEqual, UIConstantStrings.ClientID, ConstantStrings.OperatorAmpersand, ConstantStrings.RedirectUrl, ConstantStrings.OperatorEqual, Request.Url.AbsoluteUri.Replace(ConstantStrings.HTTP + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash, ConstantStrings.HTTPS + ConstantStrings.COLON + ConstantStrings.ForwardSlash + ConstantStrings.ForwardSlash).Replace(Request.Url.Query, string.Empty), ConstantStrings.QUESTION, ConstantStrings.StandardToken);
                }
                result = redirectURL;
            }
            catch (Exception exception)
            {
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
            }
            return result;
        }
    }
}