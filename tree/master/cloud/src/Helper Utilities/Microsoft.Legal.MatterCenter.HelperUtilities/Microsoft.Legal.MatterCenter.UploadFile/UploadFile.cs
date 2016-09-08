// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UploadFile
// Author           : v-akdigh
// Created          : 10-09-2015
//
// ***********************************************************************
// <copyright file="UploadFile.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file uploads file(s) to Document library.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UploadFile
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    #endregion

    /// <summary>
    /// Class to upload file(s) to Document library.
    /// </summary>
    internal class UploadFile
    {
        /// <summary>
        /// String variable to define error file path.
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// Function to verify if Folder already exists in Site Assets
        /// </summary>
        /// <param name="matterCenterAssetsFolder">Matter Center Assets Folder</param>
        /// <param name="clientContext">Client Context</param>
        /// <param name="siteAssets">Site Assets</param>
        /// <param name="matterLandingFolder">Matter Landing Folder</param>
        /// <param name="listFolders">List Folder</param>
        /// <returns>List of items in folder</returns>
        private static ListItemCollection CheckFolderExists(string matterCenterAssetsFolder, ClientContext clientContext, out List siteAssets, out ListItemCollection matterLandingFolder, out FolderCollection listFolders)
        {
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='FileLeafRef' /><Value Type='Folder'>" + matterCenterAssetsFolder + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='FileLeafRef' /></ViewFields></View>";
            siteAssets = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["LibraryName"]);
            matterLandingFolder = siteAssets.GetItems(camlQuery);
            listFolders = siteAssets.RootFolder.Folders;
            clientContext.Load(matterLandingFolder);
            clientContext.Load(siteAssets.RootFolder);
            clientContext.Load(listFolders);
            clientContext.ExecuteQuery();
            return matterLandingFolder;
        }

        /// <summary>
        /// Upload files to SharePoint assets library
        /// </summary>
        /// <param name="matterCenterAssetsFolder">Matter Center Assets Folder</param>
        /// <param name="clientContext">Client Context</param>
        /// <param name="siteAssets">Site Assets</param>
        /// <param name="listFolders">List Folders</param>
        /// <param name="listval">Configuration values from Configuration Excel</param>
        private static void UploadFilesToFolder(string matterCenterAssetsFolder, ClientContext clientContext,
            List siteAssets, FolderCollection listFolders, Dictionary<string, string> listval, string azureWebsiteUrl, string appInsightsId)
        {
            try
            {
                // Add matter landing folder
                listFolders.Add(matterCenterAssetsFolder);
                siteAssets.RootFolder.Update();
                siteAssets.Update();
                clientContext.Load(listFolders, folders => folders.Include(folderProperty => folderProperty.Name, folderProperty => folderProperty.ServerRelativeUrl).Where(folder => folder.Name == matterCenterAssetsFolder));
                clientContext.ExecuteQuery();

                Console.WriteLine("\n " + matterCenterAssetsFolder + " folder created...");
                string matterCenterAssetsUrl = listFolders.FirstOrDefault().ServerRelativeUrl;
                string staticContentFolder = ConfigurationManager.AppSettings["StaticContentFolder"];
                if (!string.IsNullOrWhiteSpace(matterCenterAssetsUrl))
                {
                    string parentDirectory = Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, CultureInfo.InvariantCulture) + "\\" + staticContentFolder + "\\" + matterCenterAssetsFolder;
                    string matterLandingAssetsFolder = ConfigurationManager.AppSettings["MatterLandingAssets"];
                    string webDashboardAssetsFolder = ConfigurationManager.AppSettings["WebDashboardAssets"];
                    string commonAssetsFolder = ConfigurationManager.AppSettings["CommonAssets"];
                    string documentLandingAssetsFolder = ConfigurationManager.AppSettings["DocumentLandingAssets"];

                    string images = ConfigurationManager.AppSettings["Images"];
                    string scripts = ConfigurationManager.AppSettings["Scripts"];
                    string styles = ConfigurationManager.AppSettings["Styles"];

                    // Create matter landing assets and web dashboard assets folder
                    listFolders.Add(matterCenterAssetsUrl + "/" + matterLandingAssetsFolder);
                    listFolders.Add(matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + images);
                    listFolders.Add(matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + scripts);
                    listFolders.Add(matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + styles);

                    // Add web dashboard folder
                    listFolders.Add(matterCenterAssetsUrl + "/" + webDashboardAssetsFolder);
                    listFolders.Add(matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + images);
                    listFolders.Add(matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + scripts);
                    listFolders.Add(matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + styles);

                    // Add common assets folder
                    listFolders.Add(matterCenterAssetsUrl + "/" + commonAssetsFolder);
                    listFolders.Add(matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + images);
                    listFolders.Add(matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + scripts);
                    listFolders.Add(matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + styles);

                    // Add document landing folder
                    listFolders.Add(matterCenterAssetsUrl + "/" + documentLandingAssetsFolder);
                    listFolders.Add(matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + images);
                    listFolders.Add(matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + scripts);
                    listFolders.Add(matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + styles);

                    siteAssets.Update();
                    clientContext.ExecuteQuery();

                    Console.WriteLine("Created matter landing and web dashboard assets folders.");

                    Console.WriteLine(" ------- Starting to upload assets ------- ");

                    // Upload matter landing assets
                    UploadAssets(parentDirectory + "\\" + matterLandingAssetsFolder, "*", matterCenterAssetsUrl + "/" + matterLandingAssetsFolder, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload web dashboard images
                    UploadAssets(parentDirectory + "\\" + webDashboardAssetsFolder + "\\" + images, "*", matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + images, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload web dashboard scripts
                    UploadAssets(parentDirectory + "\\" + webDashboardAssetsFolder + "\\" + scripts, "*", matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + scripts, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload web dashboard styles
                    UploadAssets(parentDirectory + "\\" + webDashboardAssetsFolder + "\\" + styles, "*", matterCenterAssetsUrl + "/" + webDashboardAssetsFolder + "/" + styles, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload matter landing images
                    UploadAssets(parentDirectory + "\\" + matterLandingAssetsFolder + "\\" + images, "*", matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + images, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload matter landing scripts
                    UploadAssets(parentDirectory + "\\" + matterLandingAssetsFolder + "\\" + scripts, "*", matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + scripts, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload matter landing styles
                    UploadAssets(parentDirectory + "\\" + matterLandingAssetsFolder + "\\" + styles, "*", matterCenterAssetsUrl + "/" + matterLandingAssetsFolder + "/" + styles, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload common images
                    UploadAssets(parentDirectory + "\\" + commonAssetsFolder + "\\" + images, "*", matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + images, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload common scripts
                    UploadAssets(parentDirectory + "\\" + commonAssetsFolder + "\\" + scripts, "*", matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + scripts, clientContext, listval, azureWebsiteUrl, appInsightsId);
                    // Upload common styles
                    UploadAssets(parentDirectory + "\\" + commonAssetsFolder + "\\" + styles, "*", matterCenterAssetsUrl + "/" + commonAssetsFolder + "/" + styles, clientContext, listval, azureWebsiteUrl, appInsightsId);

                    UploadAssets(parentDirectory + "\\" + documentLandingAssetsFolder + "\\" + scripts, "*", matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + scripts, clientContext, listval, azureWebsiteUrl, appInsightsId);

                    UploadAssets(parentDirectory + "\\" + documentLandingAssetsFolder + "\\" + images, "*", matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + images, clientContext, listval, azureWebsiteUrl, appInsightsId);

                    UploadAssets(parentDirectory + "\\" + documentLandingAssetsFolder + "\\" + styles, "*", matterCenterAssetsUrl + "/" + documentLandingAssetsFolder + "/" + styles, clientContext, listval, azureWebsiteUrl, appInsightsId);
                }
                else
                {
                    Console.WriteLine("\n Something went wrong. The matter center assets folder is not created or unable to browse.");
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Reads the files from directory based on search pattern and upload files to respective SharePoint folder
        /// </summary>
        /// <param name="directory">Location of directory to get files for upload</param>
        /// <param name="searchPattern">Get files from directory based on pattern</param>
        /// <param name="matterCenterAssetsUrl">SharePoint assets URL to upload file</param>
        /// <param name="clientContext">SharePoint context to upload the files</param>
        /// <param name="listval">Value in list</param>
        private static void UploadAssets(string directory, string searchPattern, string matterCenterAssetsUrl,
            ClientContext clientContext, Dictionary<string, string> listval, string azureWebsiteUrl, string appInsightsId)
        {
            try
            {
                string[] filesToUpload = Directory.GetFiles(directory, searchPattern);

                foreach (string uploadFilePath in filesToUpload)
                {
                    string currFileUpload = uploadFilePath.Substring(uploadFilePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1), content;
                    string[] configurableFiles = ConfigurationManager.AppSettings["ConfigurableFiles"].Split(',');
                    // Configure all the configurable files with the appropriate configurations
                    if (configurableFiles.Contains(currFileUpload))
                    {
                        using (StreamReader fileStream = new StreamReader(uploadFilePath))
                        {
                            string siteUrl = azureWebsiteUrl;
                            string managePermissionUrl = siteUrl + ConfigurationManager.AppSettings["ManagePermissionPageName"];
                            string tenantWebDashboardUrl = ConfigurationManager.AppSettings["WebDashboardPageName"];
                            string catalogsiteUrl = listval["CatalogSiteURL"];
                            string appInsightsID = appInsightsId;
                            string managePermissionUrlPlaceHolder = ConfigurationManager.AppSettings["ManagePermissionUrl"];
                            string tenantWebDashboardUrlPlaceHolder = ConfigurationManager.AppSettings["TenantWebDashboardUrl"];
                            string catalogsiteUrlPlaceHolder = ConfigurationManager.AppSettings["CatalogsiteUrl"];
                            string catalogSiteRelativeUrlPlaceHolder = ConfigurationManager.AppSettings["CatalogsiteRelativeUrl"];
                            string appInsightsIDPlaceHolder = ConfigurationManager.AppSettings["AppInsightsID"];
                            string settingsPageUrl = ConfigurationManager.AppSettings["SettingsPageName"];
                            string settingsPageUrlPlaceHolder = ConfigurationManager.AppSettings["TenantSettingsUrl"];
                            string managePermissionPlaceHolder = ConfigurationManager.AppSettings["ManagePermissionRelativeUrl"];
                            string matterLandingFolderPlaceHolder = ConfigurationManager.AppSettings["MatterLandingFolderRelativeUrl"];
                            string matterLandingFolderRelativeUrl = "/" + ConfigurationManager.AppSettings["LibraryUrl"] + "/" + ConfigurationManager.AppSettings["AssetsFolder"] + "/" + ConfigurationManager.AppSettings["MatterLandingAssets"] + "/";
                            string azureSiteUrlPlaceHolder = ConfigurationManager.AppSettings["AzureSiteUrl"];
                            string commonAssetsUrlPlaceHolder = ConfigurationManager.AppSettings["CommonAssetsUrl"];
                            string commonAssetsUrl = "/" + ConfigurationManager.AppSettings["LibraryUrl"] + "/" + ConfigurationManager.AppSettings["AssetsFolder"] + "/" + ConfigurationManager.AppSettings["CommonAssets"] + "/";
                            string SearchPagePlaceHolder = ConfigurationManager.AppSettings["SearchPagePlaceHolder"];
                            string SearchPage = ConfigurationManager.AppSettings["SearchPage"];
                            string SendToOneDrivePlaceHolder = ConfigurationManager.AppSettings["SendToOneDrivePlaceHolder"];
                            string SendToOneDrive = ConfigurationManager.AppSettings["SendToOneDrive"];
                            string FeedbackLinkPlaceHolder = ConfigurationManager.AppSettings["FeedbackLinkPlaceHolder"];
                            string FeedbackLink = ConfigurationManager.AppSettings["FeedbackLink"];
                            string TermsOfUseLinkPlaceHolder = ConfigurationManager.AppSettings["TermsOfUseLinkPlaceHolder"];
                            string TermsOfUseLink = ConfigurationManager.AppSettings["TermsOfUseLink"];
                            string PrivacyLinkPlaceHolder = ConfigurationManager.AppSettings["PrivacyLinkPlaceHolder"];
                            string PrivacyLink = ConfigurationManager.AppSettings["PrivacyLink"];
                            string LogoLinkPlaceHolder = ConfigurationManager.AppSettings["LogoLinkPlaceHolder"];
                            string LogoLink = ConfigurationManager.AppSettings["LogoLink"];

                            content = fileStream.ReadToEnd();
                            content = content.Replace(managePermissionUrlPlaceHolder, managePermissionUrl)
                                .Replace(tenantWebDashboardUrlPlaceHolder, tenantWebDashboardUrl)
                                .Replace(catalogsiteUrlPlaceHolder, catalogsiteUrl)
                                .Replace(appInsightsIDPlaceHolder, appInsightsID)
                                .Replace(catalogSiteRelativeUrlPlaceHolder, new Uri(catalogsiteUrl).AbsolutePath + "/")
                                .Replace(settingsPageUrlPlaceHolder, settingsPageUrl);
                            content = content.Replace(managePermissionPlaceHolder, ConfigurationManager.AppSettings["ManagePermissionPageName"])
                                .Replace(matterLandingFolderPlaceHolder, matterLandingFolderRelativeUrl)
                                .Replace(azureSiteUrlPlaceHolder, siteUrl).Replace(commonAssetsUrlPlaceHolder, commonAssetsUrl)
                                .Replace(SearchPagePlaceHolder, SearchPage)
                                .Replace(SendToOneDrivePlaceHolder, SendToOneDrive)
                                .Replace(FeedbackLinkPlaceHolder, FeedbackLink)
                                .Replace(TermsOfUseLinkPlaceHolder, TermsOfUseLink)
                                .Replace(PrivacyLinkPlaceHolder, PrivacyLink)
                                .Replace(LogoLinkPlaceHolder, LogoLink);
                        }
                        using (StreamWriter writer = new StreamWriter(uploadFilePath))
                        {
                            writer.Write(content);
                        }
                    }
                    using (FileStream fileStream = new FileStream(uploadFilePath, FileMode.Open))
                    {
                        var fileUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", matterCenterAssetsUrl, currFileUpload);
                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, fileStream, true);
                        Console.WriteLine("\n Uploaded " + currFileUpload + " successfully...");
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Program Entry Point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            try
            {
                if (5 <= args.Length)
                {
                    bool uploadAssets = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);
                    if (!ExcelOperations.IsNullOrEmptyCredential(args[1], args[2]))  // Validate Username and Password and azure website url
                    {
                        Console.WriteLine("Reading inputs from Excel...");
                        string parentPath = Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, CultureInfo.InvariantCulture);
                        Console.WriteLine(parentPath);
                        string filePath = parentPath + "\\" + ConfigurationManager.AppSettings["filename"];
                        Console.WriteLine(filePath);
                        string sheetName = ConfigurationManager.AppSettings["configsheetname"];
                        Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                        string login = args[1]; // Get the user name
                        string password = args[2]; // Get the password
                        string azureWebsiteUrl = args[3];
                        string appInsightsId = args[4];
                        string catalogURL = listval["CatalogSiteURL"];
                        string matterCenterAssetsFolder = ConfigurationManager.AppSettings["AssetsFolder"];                        
                        parentPath = Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, CultureInfo.InvariantCulture);

                        using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(catalogURL, login, password))
                        {
                            List siteAssets;
                            ListItemCollection matterCenterFolder;
                            FolderCollection listFolders;
                            matterCenterFolder = CheckFolderExists(matterCenterAssetsFolder, clientContext, out siteAssets, out matterCenterFolder, out listFolders);

                            if (0 < matterCenterFolder.Count)
                            {
                                Console.WriteLine("\n " + matterCenterAssetsFolder + " already present... Deleting...");
                                matterCenterFolder[0].DeleteObject();
                                clientContext.ExecuteQuery();
                                Console.WriteLine("Successfully deleted " + matterCenterAssetsFolder + " from SharePoint.");
                            }

                            if (uploadAssets)
                            {
                                UploadFilesToFolder(matterCenterAssetsFolder, clientContext, siteAssets,
                                    listFolders, listval, azureWebsiteUrl, appInsightsId);
                            }
                        }
                    }
                }
                else
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Insufficient Parameters");
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }
    }
}
