// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 08-27-2014
//
// ***********************************************************************
// <copyright file="MatterLandingHelperFunction.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains Matter Landing page helper functions.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WebParts;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    #endregion

    /// <summary>
    /// Main class of Matter Landing page helper functions
    /// </summary>
    internal static class MatterLandingHelperFunction
    {
        internal static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\" + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// Creates Matter Landing Page on matter creation
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>true if success else false</returns>
        internal static string CreateMatterLandingPage(ClientContext clientContext, Client client, Matter matter)
        {
            string response = string.Empty;

            if (null != clientContext && null != client && null != matter)
            {
                try
                {
                    using (clientContext)
                    {
                        Uri uri = new Uri(client.ClientUrl);
                        Web web = clientContext.Web;
                        FileCreationInformation objFileInfo = new FileCreationInformation();
                        List sitePageLib = null;
                        //// Create Matter Landing Web Part Page
                        objFileInfo.Url = string.Format(CultureInfo.InvariantCulture, "{0}{1}", matter.MatterGuid, Constants.AspxExtension);
                        response = CreateWebPartPage(sitePageLib, clientContext, objFileInfo, matter, web);
                        if (Constants.TRUE == response)
                        {
                            //// Configure All Web Parts
                            string[] webParts = ConfigureXMLCodeOfWebParts(client, matter, clientContext, sitePageLib, objFileInfo, uri, web);
                            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, Constants.Backslash, ConfigurationManager.AppSettings["MatterLandingPageRepository"].Replace(Constants.SPACE, string.Empty), Constants.Backslash, objFileInfo.Url));
                            clientContext.Load(file);
                            clientContext.ExecuteQuery();

                            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
                            WebPartDefinition webPartDefinition = null;

                            string[] zones = { Constants.HeaderZone, Constants.TopZone, Constants.RightZone, Constants.TopZone, Constants.RightZone, Constants.RightZone, Constants.FooterZone, Constants.RightZone, Constants.RightZone };
                            AddWebPart(clientContext, limitedWebPartManager, webPartDefinition, webParts, zones);
                        }
                        else
                        {
                            response = Constants.FALSE;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ////Generic Exception                    
                    MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "creating Matter Landing page"));
                    MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + ex.Message + "\nStacktrace: " + ex.StackTrace);
                    throw;
                }

                return response;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, Constants.ServiceResponse, string.Empty, Constants.MessageNoInputs);
            }
        }

        /// <summary>
        /// Create a Web Part page of matter in its document library
        /// </summary>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="web">Web object containing Web data</param>
        /// <returns>true if success else false</returns>
        internal static string CreateWebPartPage(List sitePageLib, ClientContext clientContext, FileCreationInformation objFileInfo, Matter matter, Web web)
        {
            string response = string.Empty;
            //// Find Default Layout from Master Page Gallery to create Web Part Page
            sitePageLib = web.Lists.GetByTitle(Constants.MasterPageGallery);
            clientContext.Load(sitePageLib);
            clientContext.ExecuteQuery();
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = Constants.DMSRoleQuery;
            ListItemCollection collection = sitePageLib.GetItems(camlQuery);
            clientContext.Load(
               collection,
                    items => items.Include(
                    item => item.DisplayName,
                    item => item.Id));
            clientContext.ExecuteQuery();
            response = WebPartsCreation(sitePageLib, clientContext, objFileInfo, matter, web, collection);
            return response;
        }

        /// <summary>
        /// Breaks, assigns user permission and create Web Parts layout in Matter Landing Page
        /// </summary>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="web">Web object containing Web data</param>
        /// <param name="collection">ListItemCollection object consist of items in Master Page Library</param>        
        /// <returns>true or exception value</returns>
        internal static string WebPartsCreation(List sitePageLib, ClientContext clientContext, FileCreationInformation objFileInfo, Matter matter, Web web, ListItemCollection collection)
        {
            ListItem fileName = null;
            foreach (ListItem findLayout in collection)
            {
                if (findLayout.DisplayName.Equals(Constants.DefaultLayout, StringComparison.OrdinalIgnoreCase))
                {
                    fileName = findLayout;
                    break;
                }
            }
            Microsoft.SharePoint.Client.File fileLayout = fileName.File;
            clientContext.Load(fileLayout);
            clientContext.ExecuteQuery();
            ClientResult<Stream> filedata = fileLayout.OpenBinaryStream();
            sitePageLib = web.Lists.GetByTitle(ConfigurationManager.AppSettings["MatterLandingPageRepository"]);
            clientContext.Load(sitePageLib);
            clientContext.ExecuteQuery();
            StreamReader reader = new StreamReader(filedata.Value);
            objFileInfo.Content = System.Text.Encoding.ASCII.GetBytes(reader.ReadToEnd());
            int matterLandingPageId = AddMatterLandingPageFile(sitePageLib, clientContext, objFileInfo, matter.MatterName);
            BreakItemPermission(clientContext, ConfigurationManager.AppSettings["MatterLandingPageRepository"], matter.CopyPermissionsFromParent, matterLandingPageId);
            List<string> responsibleAttorneyList = matter.TeamInfo.ResponsibleAttorneys.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            List<string> attorneyList = matter.TeamInfo.Attorneys.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            List<string> blockedUserList = matter.TeamInfo.BlockedUploadUsers.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            if (0 < responsibleAttorneyList.Count)
            {
                AssignUserPermissionsToItem(clientContext, ConfigurationManager.AppSettings["MatterLandingPageRepository"], responsibleAttorneyList, ConfigurationManager.AppSettings["FullControl"], matterLandingPageId);
            }
            if (0 < attorneyList.Count)
            {
                AssignUserPermissionsToItem(clientContext, ConfigurationManager.AppSettings["MatterLandingPageRepository"], attorneyList, ConfigurationManager.AppSettings["Contribute"], matterLandingPageId);
            }
            if (0 < blockedUserList.Count)
            {
                AssignUserPermissionsToItem(clientContext, ConfigurationManager.AppSettings["MatterLandingPageRepository"], blockedUserList, ConfigurationManager.AppSettings["Read"], matterLandingPageId);
            }
            return Constants.TRUE;
        }

        /// <summary>
        /// Add Calendar Web Part to client site
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void AddCalendarList(ClientContext clientContext, Matter matter)
        {
            string calendarName = string.Concat(matter.MatterName, ConfigurationManager.AppSettings["CalendarNameSuffix"]);
            try
            {
                Web web = clientContext.Web;
                clientContext.Load(web, item => item.ListTemplates);
                clientContext.ExecuteQuery();
                ListTemplate listTemplate = null;
                foreach (var calendar in web.ListTemplates)
                {
                    if (calendar.Name == Constants.CalendarName)
                    {
                        listTemplate = calendar;
                    }
                }

                ListCreationInformation listCreationInformation = new ListCreationInformation();
                listCreationInformation.TemplateType = listTemplate.ListTemplateTypeKind;
                listCreationInformation.Title = calendarName;
                // Added URL property for URL consolidation changes
                listCreationInformation.Url = Constants.TitleListPath + matter.MatterGuid + ConfigurationManager.AppSettings["CalendarNameSuffix"];
                web.Lists.Add(listCreationInformation);
                web.Update();
                clientContext.ExecuteQuery();
                MatterProvisionHelperFunction.BreakPermission(clientContext, matter.MatterName, matter.CopyPermissionsFromParent, calendarName);
            }
            catch (Exception exception)
            {
                //// Generic Exception
                MatterProvisionHelperFunction.DeleteMatter(clientContext, matter);
                MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Function is used to Configure XML of web parts
        /// </summary>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="uri">To get URL segments</param>
        /// <param name="web">Web object of the current context</param>
        /// <returns>List of Web Parts</returns>                              
        internal static string[] ConfigureXMLCodeOfWebParts(Client client, Matter matter, ClientContext clientContext, List sitePageLib, FileCreationInformation objFileInfo, Uri uri, Web web)
        {
            string[] result = null;
            try
            {
                sitePageLib = web.Lists.GetByTitle(matter.MatterName);
                clientContext.Load(sitePageLib);
                clientContext.ExecuteQuery();

                ////Configure list View Web Part XML
                string listViewWebPart = ConfigureListViewWebPart(sitePageLib, clientContext, objFileInfo, client, matter, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, Constants.Backslash, matter.MatterName, Constants.Backslash, objFileInfo.Url));
                string[] contentEditorSectionIds = ConfigurationManager.AppSettings["MatterLandingPageSections"].Split(',');
                ////Configure content Editor Web Part of user information XML
                string contentEditorWebPartTasks = string.Empty;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture))
                {
                    contentEditorWebPartTasks = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.TaskPanel, CultureInfo.InvariantCulture)]));
                }

                string calendarWebpart = string.Empty, rssFeedWebPart = string.Empty, rssTitleWebPart = string.Empty;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture))
                {
                    rssFeedWebPart = string.Format(CultureInfo.InvariantCulture, Constants.RssFeedWebpart, HttpUtility.UrlEncode(matter.MatterName));
                    rssTitleWebPart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.RSSTitlePanel, CultureInfo.InvariantCulture)]));
                }

                ////Configure calendar Web Part XML
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture))
                {
                    ////If create calendar is enabled configure calendar Web Part XML; else don't configure
                    calendarWebpart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.CalendarPanel, CultureInfo.InvariantCulture)]));
                }

                string matterInformationSection = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.InformationPanel, CultureInfo.InvariantCulture)]));
                string cssLink = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingCSSFileName"], ConfigurationManager.AppSettings["MatterLandingFolderName"]);
                string cssLinkCommon = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["CommonCSSFileName"], ConfigurationManager.AppSettings["CommonFolderName"]);
                string jsLinkMatterLandingPage = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingJSFileName"], ConfigurationManager.AppSettings["MatterLandingFolderName"]);
                string jsLinkJQuery = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["JQueryFileName"], ConfigurationManager.AppSettings["CommonFolderName"]);
                string jsCommonLink = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["CommonJSFileName"], ConfigurationManager.AppSettings["CommonFolderName"]);
                string headerWebPartSection = string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.HeaderPanel, CultureInfo.InvariantCulture)]);
                string footerWebPartSection = string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.FooterPanel, CultureInfo.InvariantCulture)]);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.StyleTag, cssLink), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.StyleTag, cssLinkCommon), headerWebPartSection);
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.ScriptTagWithContents, string.Format(CultureInfo.InvariantCulture, Constants.matterLandingStampProperties, matter.MatterName, matter.MatterGuid)), headerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.ScriptTag, jsLinkMatterLandingPage), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.ScriptTag, jsCommonLink), footerWebPartSection);
                footerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.ScriptTag, jsLinkJQuery), footerWebPartSection);
                string headerWebPart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, headerWebPartSection);
                string footerWebPart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, footerWebPartSection);
                string oneNoteWebPart = string.Format(CultureInfo.InvariantCulture, Constants.ContentEditorWebPart, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingSectionContent, contentEditorSectionIds[Convert.ToInt32(Constants.MatterLandingSection.OneNotePanel, CultureInfo.InvariantCulture)]));
                string[] webParts = { headerWebPart, matterInformationSection, oneNoteWebPart, listViewWebPart, rssFeedWebPart, rssTitleWebPart, footerWebPart, calendarWebpart, contentEditorWebPartTasks };
                result = webParts;
            }
            catch (Exception exception)
            {
                MatterProvisionHelperFunction.DeleteMatter(clientContext, matter);
                MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Configure XML Code of List View Web Part
        /// </summary>
        /// <param name="sitePageLib">SharePoint List of matter library</param>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="objFileInfo">Object of FileCreationInformation</param>
        /// <param name="client">Client object containing Client data</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <param name="titleUrl">Segment of URL</param>
        /// <returns>Configured ListView Web Part</returns>
        internal static string ConfigureListViewWebPart(List sitePageLib, ClientContext clientContext, FileCreationInformation objFileInfo, Client client, Matter matter, string titleUrl)
        {
            string listViewWebPart = Constants.ListviewWebpart;

            Uri uri = new Uri(client.ClientUrl);
            ViewCollection viewColl = sitePageLib.Views;
            clientContext.Load(
                viewColl,
                views => views.Include(
                    view => view.Title,
                    view => view.Id));
            clientContext.ExecuteQuery();
            string viewName = string.Empty;

            foreach (View view in viewColl)
            {
                viewName = view.Id.ToString();
                break;
            }

            viewName = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Constants.OpeningCurlyBrace, viewName, Constants.ClosingCurlyBrace);

            listViewWebPart = string.Format(CultureInfo.InvariantCulture, listViewWebPart, sitePageLib.Id.ToString(), titleUrl, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Constants.OpeningCurlyBrace, sitePageLib.Id.ToString(), Constants.ClosingCurlyBrace), viewName, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", uri.AbsolutePath, Constants.Backslash, matter.MatterName, Constants.Backslash, objFileInfo.Url));

            return listViewWebPart;
        }

        /// <summary>
        /// Adding All Web Parts on Matter Landing Page
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="limitedWebPartManager">LimitedWebPartManager object to import web parts</param>
        /// <param name="webPartDefinition">WebPartDefinition object to add web parts on page.</param>
        /// <param name="webParts">Array of web parts that should be added on Matter Landing Page</param>
        /// <param name="zones">Array of Zone IDs</param>
        /// <returns>true if success else false</returns>
        internal static string AddWebPart(ClientContext clientContext, LimitedWebPartManager limitedWebPartManager, WebPartDefinition webPartDefinition, string[] webParts, string[] zones)
        {
            int index = 0;
            int ZoneIndex = 1;
            for (index = 0; index < webParts.Length; index++)
            {
                if (!string.IsNullOrWhiteSpace(webParts[index]))
                {
                    webPartDefinition = limitedWebPartManager.ImportWebPart(webParts[index]);
                    limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, zones[index], ZoneIndex);
                    clientContext.ExecuteQuery();
                }
            }
            return Constants.TRUE;
        }

        /// <summary>
        /// Adds the matter landing page to the site pages library of client site collection and returns the unique list item Id.
        /// </summary>
        /// <param name="sitePageLib">Site pages library</param>
        /// <param name="clientContext">Client context object</param>
        /// <param name="objFileInfo">Matter landing page file</param>
        /// <param name="matterName">Name of the Matter</param>
        /// <returns>Unique list item id from site pages library for the matter landing page file uploaded.</returns>
        internal static int AddMatterLandingPageFile(List sitePageLib, ClientContext clientContext, FileCreationInformation objFileInfo, string matterName)
        {
            Microsoft.SharePoint.Client.File matterLandingPage = sitePageLib.RootFolder.Files.Add(objFileInfo);
            ListItem matterLandingPageDetails = matterLandingPage.ListItemAllFields;
            matterLandingPageDetails["Title"] = matterName;
            matterLandingPageDetails.Update();
            clientContext.Load(matterLandingPageDetails, matterLandingPageProperties => matterLandingPageProperties.Id);
            clientContext.ExecuteQuery();
            return matterLandingPageDetails.Id;
        }

        /// <summary>
        /// Breaks item level permission.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="listName">Site pages library</param>
        /// <param name="copyPermissionsFromParent">Copy parent permissions</param>
        /// <param name="listItemId">List item Id to break permission</param>
        /// <returns></returns>
        internal static bool BreakItemPermission(ClientContext clientContext, string listName, bool copyPermissionsFromParent, int listItemId)
        {
            bool flag = false;
            try
            {
                Web web = clientContext.Web;
                ListItem listItem = web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(copyPermissionsFromParent, true);
                    listItem.Update();
                    clientContext.ExecuteQuery();
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return flag;
        }

        /// <summary>
        /// Assigns permissions to users on the specified list item.
        /// </summary>
        /// <param name="clientcontext">Client context object</param>
        /// <param name="listName">Site pages library</param>
        /// <param name="users">List of users</param>
        /// <param name="permission">Permission to grant</param>
        /// <param name="listItemId">Unique list item Id for permissions assignment</param>
        /// <returns>Status of permission assignment</returns>
        internal static string AssignUserPermissionsToItem(ClientContext clientcontext, string listName, List<string> users, string permission, int listItemId)
        {
            {
                string returnvalue = "false";
                try
                {
                    List<string> permissions = new List<string>();
                    permissions.Add(permission);
                    Web web = clientcontext.Web;
                    clientcontext.Load(web.RoleDefinitions);
                    ListItem listItem = web.Lists.GetByTitle(listName).GetItemById(listItemId);
                    clientcontext.Load(listItem, item => item.HasUniqueRoleAssignments);
                    clientcontext.ExecuteQuery();
                    if (listItem.HasUniqueRoleAssignments)
                    {
                        if (null != permissions && null != users) //matter.permissions=read/limited access/contribute/ full control/ view only
                        {
                            foreach (string rolename in permissions)
                            {
                                try
                                {
                                    RoleDefinitionCollection roleDefinitions = clientcontext.Web.RoleDefinitions;
                                    RoleDefinition role = (from roleDef in roleDefinitions
                                                           where roleDef.Name == rolename
                                                           select roleDef).First();

                                    foreach (string user in users)
                                    {
                                        //get the user object
                                        Principal userprincipal = clientcontext.Web.EnsureUser(user);
                                        //create the role definition binding collection
                                        RoleDefinitionBindingCollection roledefinitionbindingcollection = new RoleDefinitionBindingCollection(clientcontext);
                                        //add the role definition to the collection
                                        roledefinitionbindingcollection.Add(role);
                                        //create a role assignment with the user and role definition
                                        listItem.RoleAssignments.Add(userprincipal, roledefinitionbindingcollection);
                                    }
                                    //execute the query to add everything
                                    clientcontext.ExecuteQuery();
                                }
                                catch (Exception exception)
                                {
                                    MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                                    throw; // Check 
                                }
                            }
                        }
                        // success. return a success code
                        returnvalue = "true";
                    }
                }
                catch (Exception exception)
                {
                    MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning Permission"));
                    MatterProvisionHelperFunction.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                    throw;
                }
                return returnvalue;
            }
        }
    }
}
