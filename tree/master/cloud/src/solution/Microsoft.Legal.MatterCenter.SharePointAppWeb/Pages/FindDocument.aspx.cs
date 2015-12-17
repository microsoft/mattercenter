// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-prd
// Created          : 13-01-2015
//
// ***********************************************************************
// <copyright file="FindDocument.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of FindDocument.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality for Search Document
    /// </summary>
    public partial class SearchDocument : System.Web.UI.Page
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
                GenericFunctions.SetConstantsResponse(this.Request, this.Response, redirectURL, ConstantStrings.ConstantObjectForDocument, ConstantStrings.ConstantFileForDocument);
            }
            catch (Exception exception)
            {
                string response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }

        }
    }
}