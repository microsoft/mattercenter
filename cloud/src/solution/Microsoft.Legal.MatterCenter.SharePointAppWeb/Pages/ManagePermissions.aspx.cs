// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-shpate
// Created          : 03-02-2015
//
// ***********************************************************************
// <copyright file="ManagePermissions.aspx.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains code behind of ManagePermissions.aspx page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb.Pages
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using System;
    using System.Reflection;
    #endregion

    public partial class ManagePermissions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string redirectURL = string.Empty;
                redirectURL = GenericFunctions.GetRedirectURL(this.Request);
                UIConstantStrings.EditMatterQueryString = Request.Url.Query; // Stored the query string in a variable and used the variable for redirection as a fix for code analyzer warning.
                if (!string.IsNullOrWhiteSpace(Request.Url.Query))
                {
                    redirectURL = string.Concat(redirectURL, UIConstantStrings.EditMatterQueryString);
                }
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