// ***********************************************************************
// <copyright file="MatterProvisionHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : MAQ Software
// Created          : 04-27-2015
//
// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.SharePoint.Client;
    using SharePoint.Client.WebParts;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    #endregion

    /// <summary>
    /// This class provides meta data related information for matter provision.
    /// </summary>
    public static class MatterProvisionHelper
    {
        /// <summary>
        /// Creates the OneNote.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="clientUrl">The client URL.</param>
        /// <param name="matter">Matter object containing Matter data</param>
        /// <returns>Returns the URL of the OneNote</returns>
        internal static string CreateOneNote(ClientContext clientContext, Uri clientUrl, Matter matter)
        {
            string returnValue = string.Empty;
            try
            {
                byte[] oneNoteFile = System.IO.File.ReadAllBytes("./Open Notebook.onetoc2");

                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                Microsoft.SharePoint.Client.File file = web.GetFolderByServerRelativeUrl(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", clientUrl.AbsolutePath, Constants.Backslash, matter.MatterGuid + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], Constants.Backslash, matter.MatterGuid)).Files.Add(new FileCreationInformation()
                {
                    Url = string.Format(CultureInfo.InvariantCulture, "{0}{1}", matter.MatterGuid, ConfigurationManager.AppSettings["ExtensionOneNoteTableOfContent"]),
                    Overwrite = true,
                    ContentStream = new MemoryStream(oneNoteFile)
                });
                web.Update();
                clientContext.Load(file);
                clientContext.ExecuteQuery();
                ListItem oneNote = file.ListItemAllFields;
                oneNote["Title"] = matter.MatterName;
                oneNote.Update();
                returnValue = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}{6}", clientUrl.Scheme, Constants.COLON, Constants.Backslash, Constants.Backslash, clientUrl.Authority, file.ServerRelativeUrl, "?Web=1");
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + "Matter name: " + matter.MatterName + "\n" + exception.Message + "\nStacktrace: " + exception.StackTrace);
                throw;
            }

            return returnValue;
        }

        /// <summary>
        /// Deletes Matter if exception occur post creation
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void DeleteMatter(ClientContext clientContext, Matter matter)
        {
            //Delete matter library
            DeleteMatterObject(clientContext, matter.MatterName, true);
            //Delete Task List
            DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["TaskListSuffix"], true);
            //Delete OneNote library
            DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], true);
            //Delete Calendar list
            DeleteMatterObject(clientContext, matter.MatterName + ConfigurationManager.AppSettings["CalendarNameSuffix"], true);
            //Delete Matter landing page
            DeleteMatterObject(clientContext, matter.MatterGuid, false);
            Console.WriteLine(ConfigurationManager.AppSettings["dashedLine"]);
        }

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
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "creating Matter Landing page"));
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + ex.Message + "\nStacktrace: " + ex.StackTrace);
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
                headerWebPartSection = string.Concat(string.Format(CultureInfo.InvariantCulture, Constants.ScriptTagWithContents, string.Format(CultureInfo.InvariantCulture, Constants.MatterLandingStampProperties, matter.MatterName, matter.MatterGuid)), headerWebPartSection);
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
                MatterProvisionHelper.DeleteMatter(clientContext, matter);
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return result;
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
                                    Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
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
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "assigning Permission"));
                    Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                    throw;
                }
                return returnvalue;
            }
        }

        /// <summary>
        /// Breaks item level permission.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="listName">Site pages library</param>
        /// <param name="copyPermissionsFromParent">Copy parent permissions</param>
        /// <param name="listItemId">List item Id to break permission</param>
        /// <returns>Boolean flag value</returns>
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
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
            return flag;
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
        /// Deletes the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="objectName">Object name</param>
        /// <param name="isList">Flag to specify is matter object is list</param>
        internal static void DeleteMatterObject(ClientContext clientContext, string objectName, bool isList)
        {
            try
            {
                if (isList)
                {
                    List list = clientContext.Web.Lists.GetByTitle(objectName);
                    list.DeleteObject();
                    clientContext.ExecuteQuery();
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["Deleted"], objectName));
                }
                else
                {
                    clientContext.Load(clientContext.Web, webDetails => webDetails.ServerRelativeUrl);
                    clientContext.ExecuteQuery();
                    string matterLandingPageUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientContext.Web.ServerRelativeUrl, Constants.Backslash, ConfigurationManager.AppSettings["MatterLandingPageRepository"].Replace(Constants.SPACE, string.Empty), Constants.Backslash, objectName, Constants.AspxExtension);
                    Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(matterLandingPageUrl);
                    clientFile.DeleteObject();
                    clientContext.ExecuteQuery();
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["DeletedMatter"], objectName));
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Fetches matter details from excel sheet
        /// </summary>
        /// <param name="dataValue">Data value</param>
        /// <returns>List of data storage</returns>
        internal static List<DataStorage> FetchMatterData(Collection<Collection<string>> dataValue)
        {
            List<DataStorage> kvp = new List<DataStorage>();
            int xlRange = dataValue.Count;
            if (0 != xlRange)
            {
                int rowCount;
                for (rowCount = 1; rowCount <= xlRange - 1; rowCount++)
                {
                    if (!string.IsNullOrWhiteSpace(dataValue[rowCount][0]))
                    {
                        DataStorage tuple = new DataStorage();
                        tuple.ClientName = Convert.ToString(dataValue[rowCount][0], CultureInfo.InvariantCulture).Trim();
                        tuple.MatterPrefix = Convert.ToString(dataValue[rowCount][1], CultureInfo.InvariantCulture).Trim();
                        tuple.MatterDescription = Convert.ToString(dataValue[rowCount][2], CultureInfo.InvariantCulture).Trim();
                        tuple.MatterIdPrefix = Convert.ToString(dataValue[rowCount][3], CultureInfo.InvariantCulture).Trim();
                        tuple.PracticeGroup = Convert.ToString(dataValue[rowCount][4], CultureInfo.InvariantCulture).Trim();
                        tuple.AreaOfLaw = Convert.ToString(dataValue[rowCount][5], CultureInfo.InvariantCulture).Trim();
                        tuple.SubAreaOfLaw = Convert.ToString(dataValue[rowCount][6], CultureInfo.InvariantCulture).Trim();
                        tuple.ConflictCheckConductedBy = Convert.ToString(dataValue[rowCount][7], CultureInfo.InvariantCulture).Trim();
                        tuple.ConflictDate = Convert.ToString(dataValue[rowCount][8], CultureInfo.InvariantCulture).Trim();
                        tuple.BlockedUser = Convert.ToString(dataValue[rowCount][9], CultureInfo.InvariantCulture).Trim();
                        tuple.ResponsibleAttorneys = Convert.ToString(dataValue[rowCount][10], CultureInfo.InvariantCulture).Trim();
                        tuple.Attorneys = Convert.ToString(dataValue[rowCount][11], CultureInfo.InvariantCulture).Trim();
                        tuple.BlockedUploadUsers = Convert.ToString(dataValue[rowCount][12], CultureInfo.InvariantCulture).Trim();
                        tuple.ConflictIdentified = Convert.ToString(dataValue[rowCount][14], CultureInfo.InvariantCulture).Trim();
                        tuple.CopyPermissionsFromParent = Convert.ToBoolean(tuple.ConflictIdentified, CultureInfo.InvariantCulture) ? Constants.FALSE.ToUpperInvariant() : Convert.ToString(dataValue[rowCount][13], CultureInfo.InvariantCulture).Trim();
                        kvp.Add(tuple);
                    }
                }
            }
            return kvp;
        }

        /// <summary>
        /// Creates matter
        /// </summary>
        /// <param name="clientContext">Client context object for sharePoint</param>
        /// <param name="url">URL of tenant</param>
        /// <param name="matterData">Matter data</param>
        /// <param name="folders">Folder name</param>
        /// <returns>String result</returns>
        internal static string CreateMatter(ClientContext clientContext, string url, Matter matterData, string folders)
        {
            string result = string.Empty;
            string oneNoteLibraryName = matterData.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"];
            try
            {
                Microsoft.SharePoint.Client.Web web = clientContext.Web;
                clientContext.Load(web.Lists);
                clientContext.ExecuteQuery();
                List matterLibrary = (from list in web.Lists where list.Title.ToString().ToUpper() == matterData.MatterName.ToUpper() select list).FirstOrDefault();
                List oneNoteLibrary = (from list in web.Lists where list.Title.ToString().ToUpper() == oneNoteLibraryName.ToUpper() select list).FirstOrDefault();
                Uri clientUri = new Uri(url);
                string requestedUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", clientUri.AbsolutePath, Constants.Backslash, ConfigurationManager.AppSettings["MatterLandingPageRepository"].Replace(Constants.SPACE, string.Empty), Constants.Backslash, matterData.MatterName, Constants.AspxExtension);
                bool isMatterLandingPageExists = Utility.PageExists(requestedUrl, clientContext);
                // Matter exists
                if (null != matterLibrary || null != oneNoteLibrary || isMatterLandingPageExists)
                {
                    if (null != matterLibrary)
                    {
                        result = Constants.MatterLibraryExists; // return if matter library exists
                    }
                    else if (null != oneNoteLibrary)
                    {
                        result = Constants.OneNoteLibraryExists; // return if OneNote library exists
                    }
                    else if (isMatterLandingPageExists)
                    {
                        result = Constants.MatterLandingPageExists; // return if matter landing page exists
                    }
                }
                else
                {
                    //Create Document library
                    MatterProvisionHelper.CreateDocumentLibrary(matterData.MatterName, clientContext, folders, false, matterData);

                    //Create OneNote library 
                    MatterProvisionHelper.CreateDocumentLibrary(oneNoteLibraryName, clientContext, folders, true, matterData);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["CalendarCreationEnabled"], CultureInfo.InvariantCulture))
                    {
                        Utility.AddCalendarList(clientContext, matterData);
                    }
                    MatterProvisionHelper.CreateOneNote(clientContext, new Uri(url), matterData);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture))
                    {
                        ListOperations.CreateTaskList(clientContext, matterData);
                    }
                    result = Constants.MatterProvisionPrerequisitesSuccess;
                }
            }
            catch (Exception exception)
            {
                MatterProvisionHelper.DeleteMatter(clientContext, matterData);
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "Matter name: " + matterData.MatterName + "\nStacktrace: " + exception.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Creates a new view for the document library (Matter)
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="matterList">Name of the list</param>
        /// <returns>True if success else False</returns>
        internal static void CreateView(ClientContext clientContext, List matterList)
        {
            try
            {
                string viewName = ConfigurationManager.AppSettings["ViewName"];
                string[] viewColumnList = ConfigurationManager.AppSettings["ViewColumnList"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(listEntry => listEntry.Trim()).ToArray();
                View outlookView = matterList.Views.Add(new ViewCreationInformation
                {
                    Title = viewName,
                    ViewTypeKind = ViewType.Html,
                    ViewFields = viewColumnList,
                    Paged = true
                });
                string strQuery = string.Format(CultureInfo.InvariantCulture, Constants.ViewOrderByQuery, ConfigurationManager.AppSettings["ViewOrderByColumn"]);
                outlookView.ViewQuery = strQuery;
                outlookView.Update();
                clientContext.ExecuteQuery();
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Created {0}", viewName));
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "creating Outlook View"));
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Function to create document library for Matter and OneNote
        /// </summary>
        /// <param name="libraryName">Matter library name</param>
        /// <param name="clientContext">client context information</param>
        /// <param name="folders">folders to create in document library</param>
        /// <param name="isOneNoteLibrary">Flag to determine OneNote library or Matter Library</param>
        /// <param name="matter">Matter object containing Matter data</param>
        internal static void CreateDocumentLibrary(string libraryName, ClientContext clientContext, string folders, bool isOneNoteLibrary, Matter matter)
        {
            IList<string> folderNames = new List<string>();
            Microsoft.SharePoint.Client.Web web = clientContext.Web;
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = libraryName;
            creationInfo.Description = matter.MatterDescription;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            // Added library GUID for URL consolidation changes
            creationInfo.Url = matter.MatterGuid + (isOneNoteLibrary ? ConfigurationManager.AppSettings["OneNoteLibrarySuffix"] : string.Empty);
            List list = web.Lists.Add(creationInfo);
            list.ContentTypesEnabled = true;
            // Version setting for OneNote document library
            if (isOneNoteLibrary)
            {
                list.EnableVersioning = false;
                string oneNoteFolderName = string.Empty;
                oneNoteFolderName = matter.MatterGuid;
                // create folder
                folderNames = new List<string>() { oneNoteFolderName };
            }
            else
            {
                list.EnableVersioning = true;
                list.EnableMinorVersions = false;
                list.ForceCheckout = false;
                //Addition of Email folder
                folderNames = folders.Split(';').Where(folder => !string.IsNullOrWhiteSpace(folder.Trim())).ToList();
            }
            list.Update();
            clientContext.ExecuteQuery();
            Utility.AddFolders(clientContext, list, folderNames);
            MatterProvisionHelperUtility.BreakPermission(clientContext, libraryName, matter.CopyPermissionsFromParent);
        }

        /// <summary>
        /// Assign field values for specified content types to the specified matter (document library)
        /// </summary>
        /// <param name="matterMetadata">Object containing metadata for Matter</param>
        /// <param name="fields">Field Collection object</param>
        internal static void SetFieldValues(MatterMetadata matterMetadata, FieldCollection fields)
        {
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).DefaultValue = matterMetadata.Client.ClientId;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientId"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).DefaultValue = matterMetadata.Client.ClientName;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnClientName"]).Update();

            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).DefaultValue = matterMetadata.Matter.MatterId;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterId"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).DefaultValue = matterMetadata.Matter.MatterName;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).ReadOnlyField = true;
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnMatterName"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.PracticeGroupTerm.WssId, matterMetadata.PracticeGroupTerm.TermName, matterMetadata.PracticeGroupTerm.Id);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnPracticeGroup"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.AreaTerm.WssId, matterMetadata.AreaTerm.TermName, matterMetadata.AreaTerm.Id);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnAreaOfLaw"]).Update();
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).DefaultValue = string.Format(CultureInfo.InvariantCulture, Constants.MetadataDefaultValue, matterMetadata.SubareaTerm.WssId, matterMetadata.SubareaTerm.TermName, matterMetadata.SubareaTerm.Id);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).SetShowInDisplayForm(true);
            fields.GetByInternalNameOrTitle(ConfigurationManager.AppSettings["ContentTypeColumnSubareaOfLaw"]).Update();
        }

        /// <summary>
        /// Inserts details into LegalDMSMatters list
        /// </summary>
        /// <param name="configVal">Dictionary object</param>
        /// <param name="title">Title of matter</param>
        /// <param name="matter">Matter object</param>
        /// <param name="client">client object</param>
        /// <returns>Boolen value</returns>
        internal static string InsertIntoMatterCenterMatters(Dictionary<string, string> configVal, string title, Matter matter, Client client)
        {
            try
            {
                using (ClientContext clientContext = MatterProvisionHelperUtility.GetClientContext(configVal["CatalogSiteURL"], configVal))
                {
                    bool isConflict = Convert.ToBoolean(matter.Conflict.ConflictIdentified, CultureInfo.InvariantCulture);
                    List<FieldUserValue> conflictByUsers = MatterProvisionHelperUtility.ResolveUserNames(configVal, matter.Conflict.ConflictCheckBy).ToList<FieldUserValue>(),
                        blockedUsers = !string.IsNullOrEmpty(matter.TeamInfo.BlockedUsers.Trim()) ? MatterProvisionHelperUtility.ResolveUserNames(configVal, matter.TeamInfo.BlockedUsers).ToList<FieldUserValue>() : null,
                        managingUsers = MatterProvisionHelperUtility.ResolveUserNames(configVal, matter.TeamInfo.ResponsibleAttorneys).ToList<FieldUserValue>(),
                        supportUsers = !string.IsNullOrEmpty(matter.TeamInfo.Attorneys.Trim()) ? MatterProvisionHelperUtility.ResolveUserNames(configVal, matter.TeamInfo.Attorneys).ToList<FieldUserValue>() : null;

                    List list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["MatterCenterList"]);
                    ListItemCreationInformation listCreationInformation = new ListItemCreationInformation();
                    ListItem listItem = list.AddItem(listCreationInformation);
                    listItem["Title"] = title; // "MyTitle";
                    listItem["ClientName"] = client.ClientName;
                    listItem["ClientID"] = client.ClientId; // "MyID";
                    listItem["MatterName"] = matter.MatterName;
                    listItem["MatterID"] = matter.MatterId;

                    listItem["ConflictCheckBy"] = conflictByUsers;
                    listItem["ConflictCheckOn"] = matter.Conflict.ConflictCheckOn;
                    listItem["ConflictIdentified"] = isConflict;

                    if (isConflict)
                    {
                        listItem["BlockUsers"] = blockedUsers;
                    }
                    listItem["ManagingAttorney"] = managingUsers;
                    listItem["Support"] = supportUsers;

                    listItem.Update();
                    clientContext.ExecuteQuery();
                }
                return "true";
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(Utility.ErrorFilePath, string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ErrorMessage"], "while updating Metadata"));
                Utility.DisplayAndLogError(Utility.ErrorFilePath, "Message: " + exception.Message + "Matter name: " + matter.MatterName + "\nStacktrace: " + exception.StackTrace);
                throw;
            }
        }
    }
}