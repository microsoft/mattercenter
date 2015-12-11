// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-nikhid
// Created          : 06-03-2014
//
// ***********************************************************************
// <copyright file="BriefcaseOperations.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of BriefcaseOperations.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality for all briefcase operations.
    /// </summary>
    public partial class BriefcaseOperations : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string redirectURL = GenericFunctions.GetRedirectURL(this.Request);
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