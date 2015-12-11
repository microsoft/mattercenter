// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-prd
// Created          : 07-01-2014
//
// ***********************************************************************
// <copyright file="MatterLandingHelperFunction.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides methods involved in matter landing page.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService.CommonHelper
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web;
    #endregion

    /// <summary>
    /// Provides methods involved in matter landing page.
    /// </summary>
    internal static class MatterLandingHelperFunction
    {

        /// <summary>
        /// Configures XML of web parts.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="uri">To get URL segments</param>
        /// <param name="web">Web object of the current context</param>
        /// <returns>List of Web Parts</returns>
        internal static string[] ConfigureXMLCodeOfWebParts(RequestObject requestObject, Client client, Matter matter, ClientContext clientContext, string pageName, Uri uri, Web web, MatterConfigurations matterConfigurations)
        {
            string[] result = null;
            try
            {
                List sitePageLib = web.Lists.GetByTitle(matter.Name);
                clientContext.Load(sitePageLib);
                clientContext.ExecuteQuery();

                ////Configure list View Web Part XML
                string listViewWebPart = ConfigureListViewWebPart(requestObject, sitePageLib, clientContext, pageName, client, matter, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, ConstantStrings.ForwardSlash, matter.Name, ConstantStrings.ForwardSlash, pageName));
                string[] contentEditorSectionIds = WebpartConstants.MatterLandingPageSections.Split(Convert.ToChar(ConstantStrings.Comma, CultureInfo.InvariantCulture));

                ////Configure content Editor Web Part of user information XML
                string contentEditorWebPartTasks = string.Empty;
                if (matterConfigurations.IsTaskSelected)
                {
                    contentEditorWebPartTasks = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.TaskPanel, CultureInfo.InvariantCulture)]));
                }

                string calendarWebpart = string.Empty, rssFeedWebPart = string.Empty, rssTitleWebPart = string.Empty;
                if (matterConfigurations.IsRSSSelected)
                {
                    rssFeedWebPart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.RssFeedWebpart, HttpUtility.UrlEncode(matter.Name));
                    rssTitleWebPart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.RSSTitlePanel, CultureInfo.InvariantCulture)]));
                }

                ////Configure calendar Web Part XML
                if (matterConfigurations.IsCalendarSelected)
                {
                    ////If create calendar is enabled configure calendar Web Part XML; else dont configure
                    calendarWebpart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.CalendarPanel, CultureInfo.InvariantCulture)]));
                }

                string matterInformationSection = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.InformationPanel, CultureInfo.InvariantCulture)]));
                string cssLink = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MatterLandingCSSFileName, ServiceConstantStrings.MatterLandingFolderName);
                string commonCssLink = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.CommonCSSFileLink, ServiceConstantStrings.CommonFolderName);
                string jsLinkMatterLandingPage = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.MatterLandingJSFileName, ServiceConstantStrings.MatterLandingFolderName);
                string jsLinkJQuery = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.JQueryFileName, ServiceConstantStrings.CommonFolderName);
                string jsLinkCommon = string.Format(CultureInfo.InvariantCulture, ServiceConstantStrings.CommonJSFileLink, ServiceConstantStrings.CommonFolderName);
                string headerWebPartSection = string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.HeaderPanel, CultureInfo.InvariantCulture)]);
                string footerWebPartSection = string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.FooterPanel, CultureInfo.InvariantCulture)]);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.StyleTag, cssLink), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.StyleTag, commonCssLink), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.ScriptTagWithContents, string.Format(CultureInfo.InvariantCulture, WebpartConstants.matterLandingStampProperties, matter.Name, matter.MatterGuid)), headerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.ScriptTag, jsLinkMatterLandingPage), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.ScriptTag, jsLinkCommon), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, WebpartConstants.ScriptTag, jsLinkJQuery), footerWebPartSection);
                string headerWebPart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, headerWebPartSection);
                string footerWebPart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, footerWebPartSection);
                string oneNoteWebPart = string.Format(CultureInfo.InvariantCulture, WebpartConstants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, WebpartConstants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Enumerators.MatterLandingSection.OneNotePanel, CultureInfo.InvariantCulture)]));
                string[] webParts = { headerWebPart, matterInformationSection, oneNoteWebPart, listViewWebPart, rssFeedWebPart, rssTitleWebPart, footerWebPart, calendarWebpart, contentEditorWebPartTasks };
                result = webParts;
            }
            catch (Exception exception)
            {
                //// Generic Exception
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                string[] arr = new string[1];
                arr[0] = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
                result = arr;
            }
            return result;
        }

        /// <summary>
        /// Configures XML code of List View web part.
        /// </summary>
        /// <param name="requestObject">Request Object</param>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="titleUrl">Segment of URL</param>
        /// <returns>Configured ListView Web Part</returns>
        internal static string ConfigureListViewWebPart(RequestObject requestObject, List sitePageLib, ClientContext clientContext, string pageName, Client client, Matter matter, string titleUrl)
        {
            string viewName = string.Empty;
            string result = string.Empty;
            try
            {
                Uri uri = new Uri(client.Url);
                ViewCollection viewColl = sitePageLib.Views;
                clientContext.Load(
                    viewColl,
                    views => views.Include(
                        view => view.Title,
                        view => view.Id));
                clientContext.ExecuteQuery();
                foreach (View view in viewColl)
                {
                    viewName = Convert.ToString(view.Id, CultureInfo.InvariantCulture);
                    break;
                }
                viewName = string.Concat(ConstantStrings.OpeningCurlyBrace, viewName, ConstantStrings.ClosingCurlyBrace);
                string listViewWebPart = WebpartConstants.ListviewWebpart;
                listViewWebPart = string.Format(CultureInfo.InvariantCulture, listViewWebPart,
                    Convert.ToString(sitePageLib.Id, CultureInfo.InvariantCulture), titleUrl,
                    string.Concat(ConstantStrings.OpeningCurlyBrace, Convert.ToString(sitePageLib.Id, CultureInfo.InvariantCulture),
                    ConstantStrings.ClosingCurlyBrace), viewName, string.Concat(uri.AbsolutePath, ConstantStrings.ForwardSlash,
                    matter.Name, ConstantStrings.ForwardSlash, pageName));
                result = listViewWebPart;
            }
            catch (Exception exception)
            {
                ////Generic Exception
                ProvisionHelperFunctions.DeleteMatter(requestObject, client, matter);
                result = Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ServiceConstantStrings.LogTableName);
            }
            return result;
        }
    }
}