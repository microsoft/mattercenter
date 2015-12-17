// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-akdigh
// Created          : 06-16-2014
//
// ***********************************************************************
// <copyright file="UIConstantStrings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file holds code for common operations to be used across various pages.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb
{
    #region using
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;
    #endregion

    /// <summary>
    /// Master page class for common operations.
    /// </summary>
    public partial class MatterCenterMaster : MasterPage
    {
        // <summary>
        /// Handles the Load event of the Master Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string refreshToken = null != this.Request.Cookies[UIConstantStrings.refreshToken] ? this.Request.Cookies[UIConstantStrings.refreshToken].Value : string.Empty;
                ProvisionMatterLink.InnerText = UIConstantStrings.ProvisionMatterAccess ? UIConstantStrings.ProvisionMatterName : string.Empty;
                if (null == Request.Cookies[UIConstantStrings.URLReferrerCookieName] && null != Request.UrlReferrer)
                {
                    HttpCookie referrer = new HttpCookie(UIConstantStrings.URLReferrerCookieName, Convert.ToString(Request.UrlReferrer.Host, CultureInfo.InvariantCulture));
                    Response.Cookies.Add(referrer);
                }
                if (null == Request.Cookies[UIConstantStrings.RequestTokenCookieName] && String.IsNullOrWhiteSpace(Request.Form[this.requestvalidator.Name]))
                {
                    string newGuid = Convert.ToString(Guid.NewGuid(), CultureInfo.InvariantCulture);
                    this.requestvalidator.Value = newGuid;
                    HttpCookie cookie = new HttpCookie(UIConstantStrings.RequestTokenCookieName, newGuid);
                    cookie.HttpOnly = false;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    if (null != Request.Cookies[UIConstantStrings.RequestTokenCookieName])
                    {
                        if (null != Request.UrlReferrer && Request.UrlReferrer.Host == Request.Cookies[UIConstantStrings.URLReferrerCookieName].Value)
                        {
                            this.requestvalidator.Value = Request.Cookies[UIConstantStrings.RequestTokenCookieName].Value;
                        }
                        else if (null != Request.Url)
                        {
                            this.requestvalidator.Value = Request.Cookies[UIConstantStrings.RequestTokenCookieName].Value;
                        }
                    }
                }
                //// This code is required to load Office.js when app is opened in Outlook
                if (Request.Url.Query.ToString().ToUpper(CultureInfo.InvariantCulture).Contains(UIConstantStrings.IS_OUTLOOK.ToUpper(CultureInfo.InvariantCulture)))
                {
                    LiteralControl javascriptRef = new LiteralControl("<script type='text/javascript' src=" + UIConstantStrings.OfficeJSPath + "></script>");
                    Page.Header.Controls.Add(javascriptRef);
                }
                // Set User Details
                using (ClientContext clientContext = ServiceUtility.GetClientContext(null, new Uri(UIConstantStrings.CentralRepository), refreshToken, this.Request))
                {
                    ShowProfilePicture(clientContext, refreshToken);
                }
            }
            catch (Exception ex)
            {
                string response = Logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, UIConstantStrings.LogTableName);
                Response.Write(GenericFunctions.SetErrorResponse(ConstantStrings.TRUE, response));
            }
        }

        /// <summary>
        /// Handles the click event of the sign out link. Clear cookies and sign out the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void SignOutLink_Click(object sender, EventArgs e)
        {
            HttpCookie refreshTokenCookie = new HttpCookie(UIConstantStrings.refreshToken);
            refreshTokenCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(refreshTokenCookie);
            Response.Redirect(UIConstantStrings.SiteURL + UIConstantStrings.SignOutURL);
        }

        /// <summary>
        /// Shows the profile picture on the app header and persona flyout.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="refreshToken">The refresh token</param>
        private void ShowProfilePicture(ClientContext clientContext, string refreshToken)
        {
            PeopleManager peopleManager = new PeopleManager(clientContext);
            PersonProperties personProperties = peopleManager.GetMyProperties();
            ///// Load users my site URL
            clientContext.Load(personProperties);
            clientContext.ExecuteQuery();
            string personalURL = personProperties.PersonalUrl.ToUpperInvariant().TrimEnd('/');
            string[] pictureInfo = personalURL.Split(new string[] { UIConstantStrings.PersonalURLSeparator.ToUpperInvariant() }, StringSplitOptions.None);
            PersonaTitle.InnerHtml = personProperties.DisplayName;
            PersonaTitle.Attributes.Add(UIConstantStrings.titleAttribute, personProperties.DisplayName);
            PersonaEmail.InnerHtml = personProperties.Email;
            PersonaEmail.Attributes.Add(UIConstantStrings.titleAttribute, personProperties.Email);
            if (null != refreshToken && 2 == pictureInfo.Length)
            {
                using (ClientContext oneDriveClientContext = ServiceUtility.GetClientContext(null, new Uri(pictureInfo[0]), refreshToken, this.Request))
                {
                    string profilePictureURL = String.Format(CultureInfo.InvariantCulture, UIConstantStrings.UserPhotoURL, pictureInfo[1], UIConstantStrings.userPhotoSmall);
                    string smallImageContent = UIUtility.GetImageInBase64Format(oneDriveClientContext, profilePictureURL);
                    if (!string.IsNullOrWhiteSpace(smallImageContent))
                    {
                        AppHeaderPersona.Src = smallImageContent;
                    }
                    profilePictureURL = String.Format(CultureInfo.InvariantCulture, UIConstantStrings.UserPhotoURL, pictureInfo[1], UIConstantStrings.userPhotoMedium);
                    string mediumImageContent = UIUtility.GetImageInBase64Format(oneDriveClientContext, profilePictureURL);
                    if (!string.IsNullOrWhiteSpace(mediumImageContent))
                    {
                        PersonaPicture.Src = mediumImageContent;
                    }
                }
            }
        }
    }
}