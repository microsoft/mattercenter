// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-rijadh
// Created          : 14-03-2014
//
// ***********************************************************************
// <copyright file="MatterProvision.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of MatterProvision.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    /// <summary>
    /// Provides functionality to provision matter.
    /// </summary>
    public partial class MatterProvision : System.Web.UI.Page
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
                GenericFunctions.SetConstantsResponse(this.Request, this.Response, redirectURL, ConstantStrings.ConstantObjectForProvision, ConstantStrings.ConstantFileForProvision);
            }
            catch (Exception exception)
            {
                string response = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }
        }
    }
}