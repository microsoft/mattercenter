// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProvisionWebDashboard
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file will create Matter Center Pages (Matter Center Home, Document Details and Settings).</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProvisionWebDashboard
{
    #region using

    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WebParts;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    #endregion using

    /// <summary>
    /// Class will create Matter Center Pages (Matter Center Home, Document Details and Settings).
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// To keep a check of which page to create
        /// </summary>
        internal enum MatterCenterPage
        {
            /// <summary>
            /// Matter center home page
            /// </summary>
            MatterCenterHome,

            /// <summary>
            /// Settings page
            /// </summary>
            Settings,

            /// <summary>
            /// Document Details page
            /// </summary>
            DocumentDetails,
        }

        /// <summary>
        /// XML definition of content editor web part for metadata
        /// </summary>
        private static string contentEditorWebpartMetadata = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                        <Title></Title>
                        <FrameType>None</FrameType>
                        <Description>Allows authors to enter rich text content.</Description>
                         <IsIncluded>true</IsIncluded>
                         <ZoneID>wpz</ZoneID>
                         <PartOrder>0</PartOrder>
                         <FrameState>Normal</FrameState>
                         <Height></Height>
                         <Width></Width>
                         <AllowRemove>true</AllowRemove>
                         <AllowZoneChange>true</AllowZoneChange>
                         <AllowMinimize>true</AllowMinimize>
                         <AllowConnect>true</AllowConnect>
                         <AllowEdit>true</AllowEdit>
                         <AllowHide>true</AllowHide>
                         <IsVisible>true</IsVisible>
                         <DetailLink />
                         <HelpLink />
                         <HelpMode>Modeless</HelpMode>
                         <Dir>Default</Dir>
                         <PartImageSmall />
                         <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                         <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                         <IsIncludedFilter />
                         <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                         <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                         <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                         <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                            <![CDATA[ {0} ]]>
                        </Content>
                        <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">Arguments from the command line</param>
        public static void Main(string[] args)
        {
            int argLength = null != args ? args.Length : 0;
            /*
             * Scenarios
             * 3 = false, Username and Password : Delete All pages
             * 3 = true, username and password : create all pages
             * 4 = false, <Page Name>, username, password  :  Delete specific page as mentioned in parameter
             * 4 = true, <Page Name>, username, password  :  Create specific page as mentioned in parameter
             * */

            if (null != args && 3 <= argLength)
            {
                bool isSpecificPageRequest = (3 < argLength) ? true : false;
                bool isCreatePageRequest = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);

                if (isSpecificPageRequest)
                {
                    string pageName = args[1];
                    string configSheet = ConfigurationManager.AppSettings["configsheetname"];
                    string filePath = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, Constants.Backslash, ConfigurationManager.AppSettings["filename"]);
                    Dictionary<string, string> configVal = ExcelOperations.ReadFromExcel(filePath, configSheet);

                    if (!ExcelOperations.IsNullOrEmptyCredential(args[1], args[2]))
                    {
                        configVal.Add("Username", args[1]);
                        configVal.Add("Password", args[2]);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Credentials. No inputs found");
                        return;
                    }
                    // Create specific page
                    if (isCreatePageRequest)
                    {
                        switch (pageName.ToUpperInvariant())
                        {
                            case Constants.MatterCenterHome:
                                {
                                    CreateProvisionPages(configVal, ConfigurationManager.AppSettings["urlConstant"], ConfigurationManager.AppSettings["sourcefilenametemplate"], ConfigurationManager.AppSettings["webDashboardUrl"], MatterCenterPage.MatterCenterHome); //Call for provisioning matter center home page
                                    break;
                                }
                            case Constants.Settings:
                                {
                                    CreateProvisionPages(configVal, ConfigurationManager.AppSettings["SettingsPageUrlConstant"], ConfigurationManager.AppSettings["SettingsHelperFileName"], ConfigurationManager.AppSettings["settingsUrl"], MatterCenterPage.Settings);        //Call for provisioning settings page
                                    break;
                                }
                            case Constants.DocumentDetails:
                                {
                                    CreateProvisionPages(configVal, string.Empty, ConfigurationManager.AppSettings["DocumentLandingFileName"], string.Empty, MatterCenterPage.DocumentDetails);       //Call for provisioning document landing page
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine(Constants.InvalidParameterFileName);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        // Delete specific page
                        switch (pageName.ToUpperInvariant())
                        {
                            case Constants.MatterCenterHome:
                                {
                                    DeleteSitePages(configVal, MatterCenterPage.MatterCenterHome); //Call for deleting matter center home page
                                    break;
                                }
                            case Constants.Settings:
                                {
                                    DeleteSitePages(configVal, MatterCenterPage.Settings);  //Call for deleting settings page
                                    break;
                                }
                            case Constants.DocumentDetails:
                                {
                                    DeleteSitePages(configVal, MatterCenterPage.DocumentDetails);   //Call for deleting document details page
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine(Constants.InvalidParameterFileName);
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    string configSheet = ConfigurationManager.AppSettings["configsheetname"];
                    string filePath = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, Constants.Backslash, ConfigurationManager.AppSettings["filename"]);
                    Dictionary<string, string> configVal = ExcelOperations.ReadFromExcel(filePath, configSheet);

                    if (!ExcelOperations.IsNullOrEmptyCredential(args[1], args[2]))
                    {
                        configVal.Add("Username", args[1]);
                        configVal.Add("Password", args[2]);
                    }
                    else
                    {
                        Console.WriteLine("Invalid credentials. No inputs found");
                        return;
                    }
                    // Create all pages
                    if (isCreatePageRequest)
                    {
                        CreateProvisionPages(configVal, ConfigurationManager.AppSettings["urlConstant"], ConfigurationManager.AppSettings["sourcefilenametemplate"], ConfigurationManager.AppSettings["webDashboardUrl"], MatterCenterPage.MatterCenterHome);       //Call for provisioning matter center home page
                        CreateProvisionPages(configVal, ConfigurationManager.AppSettings["SettingsPageUrlConstant"], ConfigurationManager.AppSettings["SettingsHelperFileName"], ConfigurationManager.AppSettings["settingsUrl"], MatterCenterPage.Settings);        //Call for provisioning settings page
                        CreateProvisionPages(configVal, string.Empty, ConfigurationManager.AppSettings["DocumentLandingFileName"], string.Empty, MatterCenterPage.DocumentDetails);       //Call for provisioning document landing page
                    }
                    else
                    {
                        //  Delete all pages
                        DeleteSitePages(configVal, MatterCenterPage.MatterCenterHome);   //Call for deleting matter center home page
                        DeleteSitePages(configVal, MatterCenterPage.Settings);   //Call for deleting settings page
                        DeleteSitePages(configVal, MatterCenterPage.DocumentDetails);   //Call for deleting document details page
                    }
                }
            }
            else
            {
                Console.Write(Constants.InvalidParameter);
            }
        }

        /// <summary>
        /// To create Provision matter pages
        /// </summary>
        /// <param name="configVal">values in the config sheet</param>
        /// <param name="urlConstantName">To get constant name of page</param>
        /// <param name="sourceFileTemplate">To get source file path</param>
        /// <param name="pageUrlName">To get page Url name</param>
        /// <param name="pageType">Page to be deleted</param>
        internal static void CreateProvisionPages(Dictionary<string, string> configVal, string urlConstantName, string sourceFileTemplate, string pageUrlName, MatterCenterPage pageType)
        {
            try
            {
                string login = configVal["Username"]; // Get the user name
                string password = configVal["Password"]; // Get the password
                bool isDeployedOnAzure = Convert.ToBoolean(configVal["IsDeployedOnAzure"].ToUpperInvariant(), CultureInfo.InvariantCulture);
                string tenantUrl = configVal["TenantURL"];
                List<string> files = new List<string>();
                string FileName = string.Empty;
                string pageFileName = string.Empty;
                string pageContent = string.Empty;
                string catalogSiteURL = configVal["CatalogSiteURL"]; // Get the URL for catalog site collection
                string catalogSiteConstant = ConfigurationManager.AppSettings["catalogSiteConstant"];
                string tenantSettingsFileName = string.Empty;
                string destinationFileName = string.Empty;
                ClientContext ClientContext = ConfigureSharePointContext.ConfigureClientContext(tenantUrl, login, password, isDeployedOnAzure);

                string pageUrlConstant = urlConstantName;
                string sourceFileTemplatePath = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, Constants.Backslash, ConfigurationManager.AppSettings["staticContentFolder"], Constants.Backslash, ConfigurationManager.AppSettings["htmlFolder"], Constants.Backslash, sourceFileTemplate);
                string pageUrl = configVal["UISiteURL"];
                // Read the content of helper file
                pageContent = System.IO.File.ReadAllText(sourceFileTemplatePath);
                // Set the Catalog site collection URL in the content of webdashboard helper page
                pageContent = pageContent.Replace(catalogSiteConstant, new Uri(catalogSiteURL).AbsolutePath);

                FileName = string.Concat(tenantUrl, ConfigurationManager.AppSettings["spWebDashboardPage"]);
                bool value = false;
                // App web part content
                switch (Convert.ToString(pageType, CultureInfo.InvariantCulture).ToUpperInvariant())
                {
                    case Constants.MatterCenterHome:
                        {
                            pageContent = pageContent.Trim();
                            pageContent = pageContent.Replace(pageUrlConstant, pageUrl);
                            pageFileName = FileName.Substring(FileName.LastIndexOf('/') + 1);
                            Console.WriteLine(string.Concat(Constants.DeletePageMessage, tenantUrl));
                            destinationFileName = FileName;
                            break;
                        }
                    case Constants.Settings:
                        {
                            pageContent = pageContent.Replace(pageUrlConstant, pageUrl);
                            tenantSettingsFileName = string.Concat(tenantUrl, ConfigurationManager.AppSettings["spSettingsPage"]);
                            pageFileName = ConfigurationManager.AppSettings["SettingsPageName"];
                            Console.WriteLine(string.Concat(Constants.DeletePageMessage, tenantUrl));
                            destinationFileName = tenantSettingsFileName;
                            value = true;
                            break;
                        }
                    case Constants.DocumentDetails:
                        {
                            pageFileName = ConfigurationManager.AppSettings["DocumentLandingPageName"];
                            ClientContext = ConfigureSharePointContext.ConfigureClientContext(catalogSiteURL, login, password, isDeployedOnAzure);
                            FileName = string.Concat(catalogSiteURL, ConfigurationManager.AppSettings["spDocumentLanding"]);
                            Console.WriteLine(string.Concat(Constants.DeletePageSuccessMessage, catalogSiteURL));
                            destinationFileName = pageFileName;
                            break;
                        }
                }

                files.Add(pageFileName);
                DeletePages(ClientContext, files);
                Folder DestinationFolder = CreateHelperPage(ClientContext, FileName);
                CreateMatterCenterPage(ClientContext, DestinationFolder, destinationFileName, pageContent, value);

                if (string.Equals(Constants.Settings, Convert.ToString(pageType, CultureInfo.InvariantCulture).ToUpperInvariant()))
                {
                    string groupName = ConfigurationManager.AppSettings["PracticeGroupName"],
                        termSetName = ConfigurationManager.AppSettings["TermSetName"],
                        clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"],
                        clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"];
                    ClientTermSets clients = TermStoreOperations.GetClientDetails(ClientContext, groupName, termSetName, clientIdProperty, clientUrlProperty); // Read client details from term store
                    // As Client level web dashboard will not be created, commenting code for provisioning client web dashboard page
                    foreach (Client client in clients.ClientTerms)
                    {
                        try
                        {
                            string clientUrl = client.ClientUrl;
                            // string fileName = clientUrl + ConfigurationManager.AppSettings["spWebDashboardPage"];
                            string settingsFileName = string.Concat(clientUrl, ConfigurationManager.AppSettings["spSettingsPage"]);
                            ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(clientUrl, login, password, isDeployedOnAzure);
                            // Deleting client web dashboard pages
                            Console.WriteLine(string.Concat(Constants.DeletePageMessage, clientUrl));
                            DeletePages(clientContext, files);
                            // Create helper file for site collections
                            // Folder destinationFolder = CreateHelperPage(clientContext, fileName);
                            Folder destinationFolder = CreateHelperPage(clientContext, settingsFileName);

                            // Upload web dashboard page
                            // CreateMatterCenterPage(clientContext, destinationFolder, fileName, helperFileContent, false);

                            // Upload settings page
                            CreateMatterCenterPage(clientContext, destinationFolder, settingsFileName, pageContent, true);
                            // Create list
                            CreateList(clientContext);
                        }
                        catch (Exception exception)
                        {
                            ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.ProvisioningPageExceptionMessage, exception.Message, exception.StackTrace));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.ProvisioningPageExceptionMessage, exception.Message, exception.StackTrace));
            }
        }

        /// <summary>
        /// Create helper page for web dashboard
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="fileName">SP Url for helper file</param>
        /// <returns>SP folder object</returns>
        internal static Folder CreateHelperPage(ClientContext clientContext, string fileName)
        {
            string folderPath = fileName.Substring(0, fileName.LastIndexOf('/'));
            Console.WriteLine(Constants.ProvisioningPageMessage + folderPath.Substring(0, folderPath.LastIndexOf('/')));
            Folder destinationFolder = clientContext.Web.GetFolderByServerRelativeUrl(folderPath);
            clientContext.Load(destinationFolder);
            clientContext.ExecuteQuery();
            return destinationFolder;
        }

        /// <summary>
        /// Create matter center page
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="destinationFolder">SP folder object</param>
        /// <param name="fileName">SP Url for web dashboard page</param>
        /// <param name="helperFileContent">Contents of helper file</param>
        /// <param name="hasUniquePermissions">Boolean value to check whether user has permissions or not</param>
        internal static void CreateMatterCenterPage(ClientContext clientContext, Folder destinationFolder, string fileName, string helperFileContent, bool hasUniquePermissions)
        {
            FileCreationInformation fileInfo = new FileCreationInformation();
            fileInfo.Url = fileName;
            fileInfo.Overwrite = true;
            fileInfo.Content = CreateWebPartPage(clientContext);
            Microsoft.SharePoint.Client.File MatterCenterPage = destinationFolder.Files.Add(fileInfo);
            clientContext.Load(MatterCenterPage);
            clientContext.ExecuteQuery();
            if (hasUniquePermissions)
            {
                ListItem MatterCenterPageDetails = MatterCenterPage.ListItemAllFields;
                clientContext.Load(MatterCenterPageDetails, MatterCenterPageProperties => MatterCenterPageProperties.Id);
                clientContext.ExecuteQuery();
                int id = MatterCenterPageDetails.Id;
                string listName = ConfigurationManager.AppSettings["SitePagesListName"];
                BreakItemPermission(clientContext, listName, id);
                AssignItemPermission(clientContext, listName, id);
            }
            LimitedWebPartManager webPartMgr = MatterCenterPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            if (!string.IsNullOrWhiteSpace(helperFileContent))
            {
                //// Add web part
                WebPartDefinition webPartDefinition = webPartMgr.ImportWebPart(string.Format(CultureInfo.InvariantCulture, contentEditorWebpartMetadata, helperFileContent));
                webPartMgr.AddWebPart(webPartDefinition.WebPart, Constants.WebPartZone, Constants.ZoneIndex);
            }
            clientContext.ExecuteQuery();
            Console.WriteLine(Constants.SuccessProvisionPageMessage + fileName.Substring(fileName.LastIndexOf('/') + 1));
        }

        /// <summary>
        /// Create a Web Part page for web dashboard
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <returns>array of bytes for the page content</returns>
        internal static byte[] CreateWebPartPage(ClientContext clientContext)
        {
            try
            {
                //// Find Default Layout from Master Page Gallery to create Web Part Page
                List sitePageLib = clientContext.Web.Lists.GetByTitle("Master Page Gallery");
                clientContext.Load(sitePageLib);
                clientContext.ExecuteQuery();
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = Constants.QueryAll;
                ListItemCollection collection = sitePageLib.GetItems(camlQuery);

                clientContext.Load(
                   collection,
                        items => items.Include(
                        item => item.DisplayName,
                        item => item.Id).Where(item => item.DisplayName == "DefaultLayout" || item.DisplayName == "BlankWebPartPage"));
                clientContext.ExecuteQuery();
                if (0 < collection.Count)
                {
                    ListItem fileName = collection[0];
                    Microsoft.SharePoint.Client.File fileLayout = fileName.File;
                    clientContext.Load(fileLayout);
                    clientContext.ExecuteQuery();

                    ClientResult<Stream> filedata = fileLayout.OpenBinaryStream();
                    clientContext.ExecuteQuery();
                    StreamReader reader = new StreamReader(filedata.Value);
                    return System.Text.Encoding.ASCII.GetBytes(reader.ReadToEnd());
                }
                else
                {
                    return Encoding.UTF8.GetBytes(Constants.ErrorMessage);
                }
            }
            catch (Exception)
            {
                return Encoding.UTF8.GetBytes(Constants.ErrorMessage);
            }
        }

        /// <summary>
        /// Delete web dashboard page and helper file
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="files">List of files to be deleted</param>
        internal static void DeletePages(ClientContext clientContext, List<string> files)
        {
            List list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["folder"]);
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = Constants.QueryGetSpecificFiles;
            ListItemCollection listCollection = list.GetItems(camlQuery);
            clientContext.Load(listCollection, items => items.Include(
                                item => item.DisplayName,
                                item => item.File.Name,
                                item => item.Id));
            clientContext.ExecuteQuery();

            if (null != listCollection)
            {
                List<ListItem> allItems = listCollection.ToList<ListItem>();

                foreach (var file in files)
                {
                    foreach (var item in allItems)
                    {
                        if (item.File.Name.ToUpperInvariant().Equals(file.ToUpperInvariant()))
                        {
                            item.DeleteObject();
                            list.Update();
                            Console.WriteLine(Constants.DeleteFileMessage + file);
                        }
                    }
                }
                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Delete matter center home page, settings page and document details page
        /// </summary>
        /// <param name="configVal">Values in Config sheet</param>
        /// <param name="pageType">Page to be deleted</param>
        internal static void DeleteSitePages(Dictionary<string, string> configVal, MatterCenterPage pageType)
        {
            try
            {
                string login = configVal["Username"]; // Get the user name
                string password = configVal["Password"]; // Get the password
                bool isDeployedOnAzure = Convert.ToBoolean(configVal["IsDeployedOnAzure"].ToUpperInvariant(), CultureInfo.InvariantCulture);
                List<string> files = new List<string>();
                string tenantUrl = configVal["TenantURL"];
                string tenantDashboardFileName, oldWebdashboard, catalogSiteURL, FileName = string.Empty;
                ClientContext siteClientContext = ConfigureSharePointContext.ConfigureClientContext(tenantUrl, login, password, isDeployedOnAzure);
                switch (Convert.ToString(pageType, CultureInfo.InvariantCulture).ToUpperInvariant())
                {
                    case Constants.MatterCenterHome:
                        {
                            tenantDashboardFileName = tenantUrl + ConfigurationManager.AppSettings["spWebDashboardPage"];
                            oldWebdashboard = ConfigurationManager.AppSettings["oldWebdashboardPage"];
                            FileName = tenantDashboardFileName.Substring(tenantDashboardFileName.LastIndexOf('/') + 1);
                            files.Add(oldWebdashboard);
                            Console.WriteLine(string.Concat(Constants.DeletePageSuccessMessage, tenantUrl));
                            break;
                        }
                    case Constants.Settings:
                        {
                            FileName = ConfigurationManager.AppSettings["SettingsPageName"];
                            Console.WriteLine(string.Concat(Constants.DeletePageSuccessMessage, tenantUrl));
                            break;
                        }
                    case Constants.DocumentDetails:
                        {
                            catalogSiteURL = configVal["CatalogSiteURL"];
                            siteClientContext = ConfigureSharePointContext.ConfigureClientContext(catalogSiteURL, login, password, isDeployedOnAzure);
                            FileName = ConfigurationManager.AppSettings["DocumentLandingPageName"];
                            Console.WriteLine(string.Concat(Constants.DeletePageSuccessMessage + catalogSiteURL));
                            break;
                        }
                }
                files.Add(FileName);
                DeletePages(siteClientContext, files);

                if (string.Equals(Constants.Settings, Convert.ToString(pageType, CultureInfo.InvariantCulture).ToUpperInvariant()))
                {
                    string groupName = ConfigurationManager.AppSettings["PracticeGroupName"],
                        termSetName = ConfigurationManager.AppSettings["TermSetName"],
                        clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"],
                        clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"];
                    try
                    {
                        ClientTermSets clients = TermStoreOperations.GetClientDetails(siteClientContext, groupName, termSetName, clientIdProperty, clientUrlProperty);
                        foreach (Client client in clients.ClientTerms)
                        {
                            try
                            {
                                string clientUrl = client.ClientUrl;
                                ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(clientUrl, login, password, isDeployedOnAzure);
                                Console.WriteLine(string.Concat(Constants.DeletePageSuccessMessage, clientUrl));
                                DeletePages(clientContext, files);
                                DeleteList(clientContext);
                            }
                            catch (Exception exception)
                            {
                                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.ProvisioningPageExceptionMessage, exception.Message, exception.StackTrace));
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.ProvisioningPageExceptionMessage, exception.Message, exception.StackTrace));
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.ProvisioningPageExceptionMessage, exception.Message, exception.StackTrace));
            }
        }

        /// <summary>
        /// Creates SharePoint list
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        internal static void CreateList(ClientContext clientContext)
        {
            try
            {
                // Check if list already present
                Web web = clientContext.Web;
                ListCollection listCollection = web.Lists;
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                string listName = ConfigurationManager.AppSettings["ConfigurationListTitle"];
                clientContext.Load(listCollection, items => items.Include(item => item.Title).Where(item => item.Title == listName));
                clientContext.ExecuteQuery();
                if (0 >= listCollection.Count)
                {
                    web = CreateConfigList(clientContext, web);
                }
                else
                {
                    Console.WriteLine(Constants.ListAlreadyPresent);
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.CreateConfigListExceptionMessage, exception.Message));
            }
        }

        /// <summary>
        /// Creates sharePoint list if not present
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="web">if list is present or no</param>
        /// <returns>Web object</returns>
        private static Web CreateConfigList(ClientContext clientContext, Web web)
        {
            List<string> userList = new List<string>();
            Console.WriteLine(Constants.CreateListMessage);
            ListCreationInformation listCreation = new ListCreationInformation();
            listCreation.Title = ConfigurationManager.AppSettings["ConfigurationListTitle"];
            listCreation.Description = ConfigurationManager.AppSettings["ConfigurationListDescription"];
            listCreation.TemplateType = Convert.ToInt32(ListTemplateType.GenericList, CultureInfo.InvariantCulture);
            web = clientContext.Web;
            List configurationList = web.Lists.Add(listCreation);
            configurationList.Hidden = true;
            configurationList.Update();
            clientContext.ExecuteQuery();
            // Add field to the list
            configurationList.Fields.AddFieldAsXml("<Field DisplayName=\'" + ConfigurationManager.AppSettings["ConfigurationColumn"] + "\' Type=\'Note\' />", true, AddFieldOptions.AddFieldToDefaultView);
            configurationList.Update();
            clientContext.ExecuteQuery();
            ListItemCreationInformation listItem = new ListItemCreationInformation();
            string columnName = ConfigurationManager.AppSettings["ConfigurationColumn"];
            ListItem item = configurationList.AddItem(listItem);
            item[columnName] = HttpUtility.HtmlEncode(ConfigurationManager.AppSettings["DefaultConfigurationItem"]);
            item[Constants.ColumnTitle] = Constants.TitleColumnValue;
            item.Update();
            clientContext.ExecuteQuery();
            userList = GetUsers(clientContext, configurationList);
            BreakPermission(clientContext, configurationList);
            AssignListPermissions(clientContext, configurationList, userList);
            return web;
        }

        /// <summary>
        /// Get the users who have permissions on the list
        /// </summary>
        /// <param name="clientContext">Client Context object</param>
        /// <param name="configurationList">Configuration list</param>
        /// <returns>User list</returns>
        internal static List<string> GetUsers(ClientContext clientContext, List configurationList)
        {
            List<string> userList = new List<string>();
            try
            {
                RoleAssignmentCollection roleAssignments = configurationList.RoleAssignments;
                clientContext.Load(roleAssignments, assignment => assignment.Include(role => role.Member, role => role.RoleDefinitionBindings));
                clientContext.ExecuteQuery();
                string limitedAccessRole = ConfigurationManager.AppSettings["LimitedAccessRole"];
                RoleAssignment(clientContext, userList, roleAssignments, limitedAccessRole);
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.CreateConfigListExceptionMessage, exception.Message));
            }
            return userList;
        }

        /// <summary>
        /// Static method to assign the role
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="userList">User list object</param>
        /// <param name="roleAssignments">Role assignments collection</param>
        /// <param name="limitedAccessRole">Limited to access role</param>
        private static void RoleAssignment(ClientContext clientContext, List<string> userList, RoleAssignmentCollection roleAssignments, string limitedAccessRole)
        {
            foreach (RoleAssignment roleAssignment in roleAssignments)
            {
                RoleDefinitionBindingCollection roleDefinitionsCollection = roleAssignment.RoleDefinitionBindings;
                clientContext.Load(roleDefinitionsCollection, roleDefinition => roleDefinition.Where(definitionItem => definitionItem.Name == limitedAccessRole));
                clientContext.ExecuteQuery();
                if (0 == roleDefinitionsCollection.Count)
                {
                    userList.Add(roleAssignment.Member.Title);
                }
            }
        }

        /// <summary>
        /// Deletes the list
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        internal static void DeleteList(ClientContext clientContext)
        {
            try
            {
                Console.WriteLine(Constants.DeleteListMessage);
                Web web = clientContext.Web;
                ListCollection listCollection = web.Lists;
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                string listName = ConfigurationManager.AppSettings["ConfigurationListTitle"];
                clientContext.Load(listCollection, items => items.Include(item => item.Title).Where(item => item.Title == listName));
                clientContext.ExecuteQuery();
                if (0 < listCollection.Count)
                {
                    List list = web.Lists.GetByTitle(listName);
                    list.DeleteObject();
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    Console.WriteLine(Constants.DeletedListMessage);
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.DeleteConfigListExceptionMessage, exception.Message));
            }
        }

        /// <summary>
        /// Break permissions of the list
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="list">List object</param>
        /// <returns>True or false</returns>
        internal static bool BreakPermission(ClientContext clientContext, List list)
        {
            bool flag = false;
            if (null != clientContext)
            {
                try
                {
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    if (!list.HasUniqueRoleAssignments)
                    {
                        list.BreakRoleInheritance(false, true);
                        list.Update();
                        clientContext.Load(list);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception exception)
                {
                    ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.BreakingPermissionExceptionMessage, Constants.ConfigurationList, exception.Message));
                }
            }

            return flag;
        }

        /// <summary>
        /// Assign permissions to the list
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="list">List object</param>
        /// <param name="userList">User list object</param>
        internal static void AssignListPermissions(ClientContext clientContext, List list, List<string> userList)
        {
            if (null != clientContext && null != list)
            {
                try
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web, item => item.SiteGroups);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments)
                    {
                        ClientRuntimeContext clientRuntimeContext = clientContext;
                        Principal userPrincipal = clientContext.Web.AssociatedOwnerGroup;
                        string roleName = ConfigurationManager.AppSettings["Role"];
                        RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                        /////add the role definition to the collection
                        roleDefinitionBindingCollection.Add(roleDefinition);
                        /////create a RoleAssigment with the owner group and role definition
                        list.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                        clientRuntimeContext.ExecuteQuery();
                    }

                    // Assign permissions to users
                    if (null != userList && 0 < userList.Count)
                    {
                        string roleName = ConfigurationManager.AppSettings["ReadRole"];
                        foreach (string userItem in userList)
                        {
                            try
                            {
                                Principal user = null;
                                user = web.SiteGroups.Where(groupItem => groupItem.Title == userItem).FirstOrDefault<Principal>();
                                if (null == user)
                                {
                                    user = web.EnsureUser(userItem);
                                }

                                if (null != user)
                                {
                                    RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                    RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientContext);
                                    // add the role definition to the collection
                                    roleDefinitionBindingCollection.Add(roleDefinition);
                                    list.RoleAssignments.Add(user, roleDefinitionBindingCollection);
                                    clientContext.ExecuteQuery();
                                }
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.AssigningPermissionsUsersExceptionMessage, userItem, exception.Message));
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.AssigningPermissionsExceptionMessage, Constants.ConfigurationList, exception.Message));
                }
            }
        }

        /// <summary>
        /// Used to break item level permissions
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">List item id</param>
        internal static void BreakItemPermission(ClientContext clientContext, string listName, int listItemId)
        {
            try
            {
                ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
                // Check if item has unique permissions
                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(false, true);
                    listItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.BreakingPermissionExceptionMessage, Constants.SettingsPage, exception.Message));
            }
        }

        /// <summary>
        /// Assigns item level permission
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">List item id</param>
        internal static void AssignItemPermission(ClientContext clientContext, string listName, int listItemId)
        {
            try
            {
                if (null != clientContext && !string.IsNullOrEmpty(listName))
                {
                    ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                    clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (listItem.HasUniqueRoleAssignments)
                    {
                        SecurableObject listItemObject = listItem;
                        ClientRuntimeContext clientRuntimeContext = clientContext;
                        // get the user object
                        Principal userPrincipal = clientContext.Web.AssociatedOwnerGroup;
                        string roleName = ConfigurationManager.AppSettings["Role"];
                        RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                        // create the role definition binding collection
                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                        // add the role definition to the collection
                        roleDefinitionBindingCollection.Add(roleDefinition);
                        // create a RoleAssigment with the user and role definition
                        listItemObject.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                        clientRuntimeContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.DisplayErrorMessage(string.Format(CultureInfo.InvariantCulture, Constants.AssigningPermissionsExceptionMessage, Constants.SettingsPage, exception.Message));
            }
        }
    }
}