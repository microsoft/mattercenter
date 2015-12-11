// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-prd
// Created          : 14-03-2014
//
// ***********************************************************************
// <copyright file="SendToOneDrive.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of SendToOneDrive.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality to send documents to OneDrive.
    /// </summary>
    public partial class SendToOneDrive : System.Web.UI.Page
    {
        /// <summary>
        /// Method called on page load.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string redirectURL = GenericFunctions.GetRedirectURL(this.Request);
                UIConstantStrings.SendToOneDriveQueryString = Request.Url.Query; // Stores the query string in a variable and used the variable for redirection as a fix for code analyzer warning.
                if (!string.IsNullOrWhiteSpace(Request.Url.Query))
                {
                    redirectURL = string.Concat(redirectURL, UIConstantStrings.SendToOneDriveQueryString);
                }
                GenericFunctions.SetConstantsResponse(this.Request, this.Response, redirectURL, ConstantStrings.ConstantObjectForBriefcase, ConstantStrings.ConstantFileForBriefcase);
            }
            catch (Exception exception)
            {
                string response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }
        }
    }
}