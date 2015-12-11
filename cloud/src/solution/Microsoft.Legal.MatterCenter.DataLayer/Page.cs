// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.DataLayer
// Author           : v-nikhid
// Created          : 06-16-2015
//
// ***********************************************************************
// <copyright file="Page.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains methods related to SharePoint page object.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.DataLayer
{
    #region using
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WebParts;
    using System;
    using System.IO;
    #endregion

    /// <summary>
    /// Performs operations related to SharePoint page.
    /// </summary>
    public static class Page
    {

        /// <summary>
        /// Deletes the page
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="pageUrl">Page URL</param>
        public static void Delete(ClientContext clientContext, string pageUrl)
        {
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageUrl))
            {
                Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);
                if (IsFileExists(clientContext, pageUrl))
                {
                    clientFile.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Checks the file at the specified location and return the file existence status.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="pageUrl">File URL</param>
        /// <returns>Success flag</returns>
        public static bool IsFileExists(ClientContext clientContext, string pageUrl)
        {
            bool success = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageUrl))
            {
                Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);
                clientContext.Load(clientFile, cf => cf.Exists);
                clientContext.ExecuteQuery();
                success = clientFile.Exists;
            }
            return success;
        }

        /// <summary>
        /// Create a web part page of matter in the document library.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="pageName">Web part page name</param>
        /// <param name="layout">Layout for the page</param>
        /// <param name="listName">List/library name</param>
        /// <param name="masterpagelistName">master page list name</param>
        /// <param name="pageTitle">Page title</param>
        /// <returns>Web part page id</returns>
        public static int CreateWebPartPage(ClientContext clientContext, string pageName, string layout, string masterpagelistName, string listName, string pageTitle)
        {
            int response = -1;
            if (null != clientContext && !string.IsNullOrWhiteSpace(pageName) && !string.IsNullOrWhiteSpace(layout) && !string.IsNullOrWhiteSpace(masterpagelistName) && !string.IsNullOrWhiteSpace(listName))
            {
                try
                {
                    //// Find Default Layout from Master Page Gallery to create Web Part Page                

                    Web web = clientContext.Web;
                    ListItemCollection collection = Lists.GetData(clientContext, masterpagelistName);
                    clientContext.Load(collection, listItemCollectionProperties => listItemCollectionProperties.Include(listItemProperties => listItemProperties.Id, listItemProperties => listItemProperties.DisplayName));
                    clientContext.ExecuteQuery();
                    ListItem fileName = null;
                    foreach (ListItem findLayout in collection)
                    {
                        if (findLayout.DisplayName.Equals(layout, StringComparison.OrdinalIgnoreCase))
                        {
                            fileName = findLayout;
                            break;
                        }
                    }
                    FileCreationInformation objFileInfo = new FileCreationInformation();
                    objFileInfo.Url = pageName;
                    Microsoft.SharePoint.Client.File fileLayout = fileName.File;
                    clientContext.Load(fileLayout);
                    clientContext.ExecuteQuery();
                    ClientResult<Stream> filedata = fileLayout.OpenBinaryStream();
                    List sitePageLib = web.Lists.GetByTitle(listName);
                    clientContext.Load(sitePageLib);
                    clientContext.ExecuteQuery();
                    StreamReader reader = new StreamReader(filedata.Value);
                    objFileInfo.Content = System.Text.Encoding.ASCII.GetBytes(reader.ReadToEnd());
                    Microsoft.SharePoint.Client.File matterLandingPage = sitePageLib.RootFolder.Files.Add(objFileInfo);
                    ListItem matterLandingPageDetails = matterLandingPage.ListItemAllFields;
                    // Update the title of the page
                    matterLandingPageDetails[Constants.TITLE] = pageTitle;
                    matterLandingPageDetails.Update();
                    clientContext.Load(matterLandingPageDetails, matterLandingPageProperties => matterLandingPageProperties[Constants.TITLE], matterLandingPageProperties => matterLandingPageProperties.Id);
                    clientContext.ExecuteQuery();
                    response = matterLandingPageDetails.Id;
                }
                catch (Exception)
                {
                    response = -1;
                }
            }
            return response;
        }

        /// <summary>
        /// Adds all web parts on matter landing page.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="limitedWebPartManager">LimitedWebPartManager object to import web parts</param>
        /// <param name="webPartDefinition">WebPartDefinition object to add web parts on page</param>
        /// <param name="webParts">Array of web parts that should be added on Matter Landing Page</param>
        /// <param name="zones">Array of Zone IDs</param>
        /// <returns>Success flag</returns>
        public static bool AddWebPart(ClientContext clientContext, LimitedWebPartManager limitedWebPartManager, WebPartDefinition webPartDefinition, string[] webParts, string[] zones)
        {
            bool result = false;
            if (null != clientContext && null != limitedWebPartManager && null != webParts && null != zones)
            {
                int index = 0;
                try
                {
                    for (index = 0; index < webParts.Length; index++)
                    {
                        if (!string.IsNullOrWhiteSpace(webParts[index]))
                        {
                            webPartDefinition = limitedWebPartManager.ImportWebPart(webParts[index]);
                            limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, zones[index], Constants.ZONEINDEX);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
