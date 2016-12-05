// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateAppConfig
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="UpdateConfig.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file updates build with appropriate configurations required before publishing the Site and Service.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateAppConfig
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Search.Administration;
    using Microsoft.SharePoint.Client.Search.Portability;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    #endregion

    /// <summary>
    /// Class UpdateConfig updates build with appropriate configurations required before publishing the Site and Service.
    /// </summary>
    internal class UpdateConfig
    {
        /// <summary>
        /// Declaring the static string variable errorPath 
        /// </summary>
        private static string errorPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + ConstantStrings.BACKSLASH + ConfigurationManager.AppSettings["errorFile"];

        /// <summary>
        /// Generates search term to be used in result source
        /// </summary>
        /// <param name="clientTermsets">Client termsets object</param>
        /// <returns>Search Term</returns>
        private static string GetSearchTerm(ClientTermSets clientTermsets)
        {
            string searchterm = ConfigurationManager.AppSettings["SearchTermTemplate"],
                searchTermField = ConfigurationManager.AppSettings["SearchTermField"],
                concatenation = ConfigurationManager.AppSettings["Concatenation"],
                url;
            foreach (Client client in clientTermsets.ClientTerms)
            {
                url = client.ClientUrl;
                if (!string.IsNullOrEmpty(url))
                {
                    if (!client.Equals(clientTermsets.ClientTerms.First()))
                    {
                        searchterm = string.Concat(searchterm, concatenation, ConstantStrings.SPACE, string.Format(CultureInfo.InvariantCulture, searchTermField, url), ConstantStrings.SPACE);
                    }
                    else
                    {
                        searchterm = string.Concat(searchterm, ConstantStrings.BRACKETOPEN, ConstantStrings.SPACE, string.Format(CultureInfo.InvariantCulture, searchTermField, url), ConstantStrings.SPACE);
                    }
                }
            }
            searchterm = string.Concat(searchterm, ConstantStrings.SPACE, ConstantStrings.BRACKETCLOSE);
            return searchterm.Trim();
        }

        /// <summary>
        /// Update Search Configuration file and set new Search Term for restricting search scope
        /// </summary>
        /// <param name="filePath">path of the configuration Excel</param>
        /// <param name="login">Login detail</param>
        /// <param name="password">Password</param>
        public static void UpdateSearchConfig(string filePath, string login, string password)
        {
            // 1. Read Configuration Excel and form Search Term            
            string clientSheetName = ConfigurationManager.AppSettings["manifestSheetname"], xmlQueryTemplate = ConfigurationManager.AppSettings["XMLNodeQueryTemplate"];
            Dictionary<string, string> constantsList = ExcelOperations.ReadFromExcel(filePath, clientSheetName);

            string updatedSearchTerm;
            using (ClientContext context = ConfigureSharePointContext.ConfigureClientContext(constantsList[ConstantStrings.TENANT_ADMIN_URL], login, password))
            {
                string groupName = ConfigurationManager.AppSettings["PracticeGroupName"];
                string termSetName = ConfigurationManager.AppSettings["TermSetName"];
                string clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"];
                string clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"];
                ClientTermSets clientTermSets = TermStoreOperations.GetClientDetails(context, groupName, termSetName, clientIdProperty, clientUrlProperty);
                updatedSearchTerm = GetSearchTerm(clientTermSets);
            }

            // 2. Read Search Configuration XML file
            string resultSourceSearchConfigFilename = ConfigurationManager.AppSettings["ResultSourceXML"], resultSourceName = ConfigurationManager.AppSettings["ResultSourceName"],
            mangedPropertySearchConfigFileName = ConfigurationManager.AppSettings["SearchConfigXML"];
            string xmlFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.GetDirectories(ConfigurationManager.AppSettings["StaticContentFolder"]).FirstOrDefault().GetDirectories(ConfigurationManager.AppSettings["XMLFolderName"]).FirstOrDefault().FullName;
            string resultSourceSearchConfigPath = string.Concat(xmlFolderPath, ConstantStrings.BACKSLASH, resultSourceSearchConfigFilename);
            string managedPropertySearchConfigPath = string.Concat(xmlFolderPath, ConstantStrings.BACKSLASH, mangedPropertySearchConfigFileName);
            XDocument resultSourceXml = XDocument.Load(resultSourceSearchConfigPath);
            XDocument managedPropertyXml = XDocument.Load(managedPropertySearchConfigPath);
            XElement resultSource = null;

            GetResultSource(resultSourceName, resultSourceXml, ref resultSource); // Existing result source

            if (null != resultSource)
            {
                XElement queryTemplate = resultSource.Descendants().Where(item => item.Name.LocalName.Equals(xmlQueryTemplate)).FirstOrDefault();
                // 3. Update XML file and set updated search term
                if (null != queryTemplate)
                {
                    queryTemplate.Value = updatedSearchTerm;
                    resultSourceXml.Save(resultSourceSearchConfigPath);
                }
                else
                {
                    string errorMessage = "Query Template not found in Search Configuration";
                    ErrorLogger.DisplayErrorMessage(errorMessage);
                    ErrorLogger.LogErrorToTextFile(errorPath, errorMessage);
                }
            }

            try
            {
                // 4. Upload Search Configuration to SharePoint, Import search configuration For creating Result Source at Catalog Level
                string configSheet = ConfigurationManager.AppSettings["manifestSheetname"];
                Dictionary<string, string> configDetails = ExcelOperations.ReadFromExcel(filePath, configSheet);
                string url = configDetails[ConfigurationManager.AppSettings["TenantSiteUrlKey"]].TrimEnd();
                using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(url, login, password))
                {
                    ClientRuntimeContext context = clientContext;
                    SearchConfigurationPortability searchConfigPortability = new SearchConfigurationPortability(context);
                    SearchObjectOwner owner = new SearchObjectOwner(context, SearchObjectLevel.SPSite);
                    searchConfigPortability.ImportSearchConfiguration(owner, resultSourceXml.ToString());
                    context.ExecuteQuery();
                    Console.WriteLine("Imported search configuration and created result source.");
                }

                // 5. Upload Search Configuration tenant level and create managed properties
                url = configDetails[ConstantStrings.TENANT_ADMIN_URL].Trim();
                using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(url, login, password))
                {
                    ClientRuntimeContext context = clientContext;
                    SearchConfigurationPortability searchConfigPortability = new SearchConfigurationPortability(context);
                    SearchObjectOwner owner = new SearchObjectOwner(context, SearchObjectLevel.SPSiteSubscription);
                    searchConfigPortability.ImportSearchConfiguration(owner, managedPropertyXml.ToString());
                    context.ExecuteQuery();
                    Console.WriteLine("Imported search configuration and created search schema.");
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
            }
        }

        /// <summary>
        /// Get result source from Search Configuration
        /// </summary>
        /// <param name="resultSourceName">Name of the result source</param>
        /// <param name="xmlDoc">XML Document</param>
        /// <param name="element">XML element result source</param>        
        private static void GetResultSource(string resultSourceName, XDocument xmlDoc, ref XElement element)
        {
            string searchQueryConfigSettings = ConfigurationManager.AppSettings["XMLNodeSearchQueryConfigurationSettings"],
                sources = ConfigurationManager.AppSettings["XMLNodeSources"],
                source = ConfigurationManager.AppSettings["XMLNodeSource"],
                name = ConfigurationManager.AppSettings["XMLNodeName"];
            element = xmlDoc.Elements().Descendants().Where(item => item.Name.LocalName.Equals(searchQueryConfigSettings)).FirstOrDefault().Descendants().Where(item => item.Name.LocalName.Equals(searchQueryConfigSettings)).FirstOrDefault().Descendants().Where(item => item.Name.LocalName.Equals(sources)).Descendants().Where(item => (item.Name.LocalName.Equals(source) && item.Descendants().Where(nodeName => nodeName.Name.LocalName.Equals(name)).FirstOrDefault().Value.Equals(resultSourceName))).FirstOrDefault();
        }

        /// <summary>
        /// Returns result source id for Matter Center result source
        /// </summary>
        /// <param name="login">Login detail</param>
        /// <param name="password">Password</param>
        /// <returns>Result source id</returns>
        private static string GetResultSourceId(string login, string password)
        {
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + ConstantStrings.BACKSLASH + ConfigurationManager.AppSettings["filename"];
            string configSheet = ConfigurationManager.AppSettings["manifestSheetname"];
            Dictionary<string, string> ConfigDetails = ExcelOperations.ReadFromExcel(filePath, configSheet);
            string url = ConfigDetails[ConfigurationManager.AppSettings["TenantSiteUrlKey"]].TrimEnd(ConstantStrings.FRONTSLASH);
            string resultSourceID = null;

            try
            {
                using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(url, login, password))
                {
                    ClientRuntimeContext context = clientContext;
                    SearchConfigurationPortability searchConfigPortability = new SearchConfigurationPortability(context);
                    SearchObjectOwner owner = new SearchObjectOwner(context, SearchObjectLevel.SPSite);
                    ClientResult<string> searchConfigXML = searchConfigPortability.ExportSearchConfiguration(owner);
                    context.ExecuteQuery();

                    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(searchConfigXML.Value)))
                    {
                        string id = ConfigurationManager.AppSettings["XMLNodeId"], resultSourceName = ConfigurationManager.AppSettings["ResultSourceName"];

                        if (0 < stream.Length)
                        {
                            XDocument xmlDoc = XDocument.Load(stream);
                            XElement element = null;
                            GetResultSource(resultSourceName, xmlDoc, ref element);
                            if (null != element)
                            {
                                resultSourceID = element.Descendants().Where(x => x.Name.LocalName.Equals(id)).FirstOrDefault().Value;
                            }
                            else
                            {
                                string errorMessage = "Result Source ID not found.";
                                ErrorLogger.DisplayErrorMessage(errorMessage);
                                throw new Exception(errorMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
            }
            return resultSourceID;
        }

        /// <summary>
        /// Method to update the manifest file
        /// </summary>
        /// <param name="inputXMLFile">XML file to be uploaded</param>
        /// <param name="excelInput">List of values to be updated</param>
        /// <param name="sourceLocation">Key of Source Location</param>
        /// <param name="defaultValue">Key of Default Value</param>
        /// <param name="startPageElement">Key of Start Page Element</param>
        /// <param name="flag">is XML update of constants update</param>
        /// <returns>Updated document</returns>
        public static XmlDocument UpdateManifest(string inputXMLFile, Dictionary<string, string> excelInput, string sourceLocation, string defaultValue, string startPageElement, bool flag)
        {
            XmlDocument doc = new XmlDocument();

            if (null != excelInput && !string.IsNullOrEmpty(inputXMLFile) && !string.IsNullOrEmpty(sourceLocation) && !string.IsNullOrEmpty(defaultValue) && !string.IsNullOrEmpty(startPageElement))
            {
                try
                {
                    using (XmlReader xmlReader = XmlReader.Create(inputXMLFile))
                    {
                        doc.Load(xmlReader);
                        string clientElementTag = ConfigurationManager.AppSettings[sourceLocation];
                        string clientAttribute = ConfigurationManager.AppSettings[defaultValue];

                        if (!string.IsNullOrEmpty(clientElementTag) && !string.IsNullOrEmpty(clientAttribute))
                        {
                            XmlNodeList clientElement = doc.GetElementsByTagName(clientElementTag);

                            if (null != clientElement && clientElement.Count > 0)
                            {
                                for (int iterator = 0; iterator < clientElement.Count; iterator++)
                                {
                                    string previousValue = clientElement[iterator].Attributes[clientAttribute].Value;

                                    clientElement[iterator].Attributes[clientAttribute].Value = excelInput[startPageElement];

                                    if ("iconUrl" == sourceLocation)
                                    {
                                        clientElement[iterator].Attributes[clientAttribute].Value = excelInput[startPageElement] + previousValue.Substring(previousValue.IndexOf(ConstantStrings.IMAGES_ICON, StringComparison.OrdinalIgnoreCase));
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_XML_UPDATE);
                        }
                    }
                }
                catch (ArgumentNullException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                }
                catch (DirectoryNotFoundException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
            }
            return doc;
        }

        /// <summary>
        /// Method to update the XML files in the solution
        /// </summary>
        /// <param name="excelInput">List of values to be updated</param>
        /// <returns>true if success else false</returns>
        public static bool UpdateXML(Dictionary<string, string> excelInput)
        {
            if (null != excelInput)
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
                    Console.WriteLine(directoryInfo.FullName);
                    DirectoryInfo[] directory = directoryInfo.GetDirectories();

                    foreach (DirectoryInfo dir in directory)
                    {
                        if (dir.Name.Contains(ConstantStrings.XML_OFFICE) || dir.Name.Contains(ConstantStrings.XML_OUTLOOK))
                        {
                            List<FileInfo> manifestFile = dir.GetFiles(ConstantStrings.XML_EXTENSION).ToList();
                            if (manifestFile.Count > 0)
                            {
                                Console.WriteLine("Updating " + manifestFile[0].Name + " file");
                                string inputXMLFile = manifestFile[0].DirectoryName + ConstantStrings.BACKSLASH + manifestFile[0].Name;
                                XmlDocument doc = UpdateManifest(inputXMLFile, excelInput, "sourceLocation", "defaultValue", "UISiteURL", false);
                                doc.Save(inputXMLFile);
                                doc = UpdateManifest(inputXMLFile, excelInput, "iconUrl", "defaultValue", "UISiteURL", false);
                                // Update App Domains
                                string appDomainTag = ConfigurationManager.AppSettings["appDomain"];
                                XmlNodeList clientElement = doc.GetElementsByTagName(appDomainTag);

                                if (null != clientElement && clientElement.Count > 0 && clientElement.Count == 4)
                                {
                                    int iAppDomain = 1;
                                    foreach (XmlNode appDomain in clientElement)
                                    {
                                        switch (iAppDomain)
                                        {
                                            case 2:
                                                appDomain.InnerText = excelInput[ConstantStrings.AZURE_UI_SITE_URL];
                                                break;                                            
                                            case 4:
                                                appDomain.InnerText = excelInput[ConstantStrings.TENANT_URL];
                                                break;
                                        }
                                        iAppDomain++;
                                    }
                                }

                                doc.Save(inputXMLFile);
                                Console.WriteLine("Successfully updated " + manifestFile[0].Name + " file");
                            }
                        }
                    }
                }
                catch (ArgumentNullException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
                catch (DirectoryNotFoundException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to update the values from Excel into constants file
        /// </summary>
        /// <param name="updateList">List of values to be updated</param>
        /// <param name="folderName">Folder name in the solution directory</param>
        /// <returns>true if success else false</returns>
        public static bool UpdateConstants(Dictionary<string, string> updateList, string folderName)
        {
            if (null != updateList && !string.IsNullOrEmpty(folderName))
            {
                try
                {
                    string constantElement = ConfigurationManager.AppSettings["constantElement"];
                    string constantAttribute = ConfigurationManager.AppSettings["constantAttribute"];

                    if (!string.IsNullOrEmpty(constantElement) && !string.IsNullOrEmpty(constantAttribute))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
                        if (null != directoryInfo)
                        {
                            List<FileInfo> manifestFile = directoryInfo.GetDirectories(folderName).FirstOrDefault().GetFiles(ConstantStrings.RESOURCE_EXTENSION, SearchOption.AllDirectories).ToList();
                            if (manifestFile.Count > 0)
                            {
                                UpdateResourceConstants(updateList, constantElement, constantAttribute, manifestFile);
                            }
                        }
                    }
                    else
                    {
                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_CONSTANTS_KEY);
                    }
                }
                catch (ArgumentNullException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
                catch (DirectoryNotFoundException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    return false;
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to update the resource constants
        /// </summary>
        /// <param name="updateList">Excel inputs</param>
        /// <param name="constantElement">Update element</param>
        /// <param name="constantAttribute">Update attribute</param>
        /// <param name="manifestFile">File to be updated</param>
        private static void UpdateResourceConstants(Dictionary<string, string> updateList, string constantElement, string constantAttribute, List<FileInfo> manifestFile)
        {
            List<string> UpdateHardCoded = AddToList();

            string tenant = updateList["Site_Url"];
            string catalog = updateList["Central_Repository_Url"].TrimEnd(ConstantStrings.FRONTSLASH);
            string[] catalogUrl = catalog.Split(ConstantStrings.FRONTSLASH);
            string catalogSiteName = catalogUrl[catalogUrl.Length - 2] + ConstantStrings.FRONTSLASH + catalogUrl[catalogUrl.Length - 1];
            string serviceURL = updateList["ServiceSiteURL"];
            string delveLink = updateList["DelveLink"];
            string contentTypeGroupName = ConfigurationManager.AppSettings["ContentTypeGroupValue"];
            string replace = string.Empty;
            string emptyPlaceHolder = ConfigurationManager.AppSettings["EmptyPlaceHolder"];

            foreach (FileInfo manifest in manifestFile)
            {
                Console.WriteLine("Updating " + manifest.Name + " file");
                string inputXMLFile = manifest.DirectoryName + ConstantStrings.BACKSLASH + manifest.Name;
                XDocument document = XDocument.Load(inputXMLFile);

                UpdateList(updateList, constantElement, constantAttribute, document);

                replace = CheckValueAndReplace(updateList, constantElement, constantAttribute, UpdateHardCoded, tenant, catalog, catalogSiteName, serviceURL, delveLink, contentTypeGroupName, replace, emptyPlaceHolder, document);

                document.Save(inputXMLFile);
                Console.WriteLine("Successfully updated " + manifest.Name + " file");
            }
        }

        /// <summary>
        /// String method to check value and replace
        /// </summary>
        /// <param name="updateList">List of values to be updated</param>
        /// <param name="constantElement">Update element</param>
        /// <param name="constantAttribute">Update attribute</param>
        /// <param name="UpdateHardCoded">Update hard coded</param>
        /// <param name="tenant">Tenant URL</param>
        /// <param name="catalog">Catalog Name</param>
        /// <param name="catalogSiteName">Catalog site name</param>
        /// <param name="serviceURL">Service URL</param>
        /// <param name="delveLink">Delve Link</param>
        /// <param name="contentTypeGroupName">Name of content type group</param>
        /// <param name="replace">Replace value</param>
        /// <param name="emptyPlaceHolder">Empty placeholder</param>
        /// <param name="document">Document object</param>
        /// <returns>Replaced value</returns>
        private static string CheckValueAndReplace(Dictionary<string, string> updateList, string constantElement, string constantAttribute, List<string> UpdateHardCoded, string tenant, string catalog, string catalogSiteName, string serviceURL, string delveLink, string contentTypeGroupName, string replace, string emptyPlaceHolder, XDocument document)
        {
            foreach (string value in UpdateHardCoded)
            {
                replace = string.Empty;
                var element = document.Descendants(constantElement).Where(e => e.Attribute(constantAttribute).Value == value);
                foreach (XElement stepOne in element)
                {
                    string previousValue = ((XElement)((((XContainer)(stepOne)).FirstNode).NextNode)).Value;
                    Uri preVal;
                    switch (value)
                    {
                        case "Image_Document_Icon":
                        case "Image_General_Document":
                            preVal = new Uri(previousValue);
                            replace = tenant + preVal.LocalPath;
                            break;
                        case "App_Redirect_URL":
                            preVal = new Uri(previousValue);
                            string layouts = preVal.LocalPath.Substring(preVal.LocalPath.IndexOf("/_layouts", StringComparison.OrdinalIgnoreCase));
                            replace = catalog + layouts;
                            break;
                        case "Matter_Provision_Service_Url":
                            replace = serviceURL + ConstantStrings.PROVISION_MATTER;
                            break;
                        case "Search_Service_Url":
                            replace = serviceURL + ConstantStrings.SEARCH_MATTER;
                            break;
                        case "Legal_Briefcase_Service_Url":
                            replace = serviceURL + ConstantStrings.LEGAL_BRIEFCASE;
                            break;
                        case "One_Drive_Content_Type_Group":
                            replace = contentTypeGroupName;
                            break;
                        case "TemplatesURL":
                            replace = ConstantStrings.TEMPLATE_URL;
                            break;
                        case "TenantWebDashboard":
                            replace = ConfigurationManager.AppSettings["sitePages"] + ConfigurationManager.AppSettings["webDashBoardPage"];
                            break;
                        case "Matter_Landing_Page_jQuery_File_Name":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingPagejQueryFileNameValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Matter_Landing_Page_Script_File_Name":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingPageScriptFileNameValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Matter_Landing_Page_CSS_File_Name":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingPageCSSFileNameValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Common_CSS_File_Location":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["CommonCSSLocation"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Common_JS_File_Location":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["CommonJSLocation"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "OneNote_Image_Url":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["OneNoteImageUrlValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Pin_Image_Url":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["PinImageUrlValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Share_Image_Url":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ShareImageUrlValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Unpin_Image_Url":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["UnpinImageUrlValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Microsoft_Logo_Location":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MicrosoftLogoLocationValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Matter_Landing_Page_Icon":
                            replace = string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["MatterLandingPageIconValue"], emptyPlaceHolder, catalogSiteName);
                            break;
                        case "Tenant_Url":
                            replace = tenant;
                            break;
                        case "Search_Result_Source_ID":
                            replace = GetResultSourceId(updateList[ConstantStrings.USERNAME], updateList[ConstantStrings.PASSWORD]);
                            break;
                        case "Delve_Link":
                            replace = delveLink;
                            break;
                    }
                    ((XElement)((((XContainer)(stepOne)).FirstNode).NextNode)).Value = replace;
                }
            }

            return replace;
        }

        /// <summary>
        /// Method to update the list
        /// </summary>
        /// <param name="updateList">List of values to be updated</param>
        /// <param name="constantElement">Update element</param>
        /// <param name="constantAttribute">Update attribute</param>
        /// <param name="document">Document object</param>
        private static void UpdateList(Dictionary<string, string> updateList, string constantElement, string constantAttribute, XDocument document)
        {
            foreach (var element in updateList)
            {
                var stepOnes = document.Descendants(constantElement).Where(e => e.Attribute(constantAttribute).Value == element.Key);

                foreach (XElement stepOne in stepOnes)
                {
                    ((XElement)((((XContainer)(stepOne)).FirstNode).NextNode)).Value = element.Value;
                }
            }
        }

        /// <summary>
        /// Static list to add elements
        /// </summary>
        /// <returns>List object</returns>
        private static List<string> AddToList()
        {
            List<string> UpdateHardCoded = new List<string>();
            UpdateHardCoded.Add("Image_Document_Icon");
            UpdateHardCoded.Add("Image_General_Document");
            UpdateHardCoded.Add("App_Redirect_URL");
            UpdateHardCoded.Add("Matter_Provision_Service_Url");
            UpdateHardCoded.Add("Search_Service_Url");
            UpdateHardCoded.Add("Legal_Briefcase_Service_Url");
            UpdateHardCoded.Add("One_Drive_Content_Type_Group");
            UpdateHardCoded.Add("TemplatesURL");
            UpdateHardCoded.Add("TenantWebDashboard");
            UpdateHardCoded.Add("Matter_Landing_Page_jQuery_File_Name");
            UpdateHardCoded.Add("Matter_Landing_Page_Script_File_Name");
            UpdateHardCoded.Add("Matter_Landing_Page_CSS_File_Name");
            UpdateHardCoded.Add("OneNote_Image_Url");
            UpdateHardCoded.Add("Pin_Image_Url");
            UpdateHardCoded.Add("Share_Image_Url");
            UpdateHardCoded.Add("Unpin_Image_Url");
            UpdateHardCoded.Add("Microsoft_Logo_Location");
            UpdateHardCoded.Add("Matter_Landing_Page_Icon");
            UpdateHardCoded.Add("Tenant_Url");
            UpdateHardCoded.Add("Search_Result_Source_ID");
            UpdateHardCoded.Add("Delve_Link");
            UpdateHardCoded.Add("Common_CSS_File_Location");
            UpdateHardCoded.Add("Common_JS_File_Location");
            return UpdateHardCoded;
        }

        /// <summary>
        /// Method to update the resource files
        /// </summary>
        /// <param name="updateList">List of values to be updated</param>
        /// <param name="folderName">Folder name in the solution directory</param>
        public static void UpdateResource(Dictionary<string, string> updateList, string folderName)
        {
            if (null != updateList && !string.IsNullOrEmpty(folderName))
            {
                try
                {
                    if (updateList.Count > 0)
                    {
                        bool isSuccessfullyWritten = false;
                        isSuccessfullyWritten = UpdateConstants(updateList, folderName);

                        if (isSuccessfullyWritten)
                        {
                            Console.WriteLine(ConstantStrings.CONSTANT_SUCCESS_MESSAGE);
                        }
                        else
                        {
                            ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_XML_UPDATE);
                            Console.WriteLine(ConstantStrings.CONSTANT_FAILURE_MESSAGE);
                        }
                    }
                    else
                    {
                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
                    }
                }
                catch (System.IO.IOException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
            }
        }

        /// <summary>
        /// Update the web.config file and ApplicationInsights.config
        /// </summary>
        /// <param name="configUpdateList">Excel inputs</param>
        /// <param name="deployConfigName">Name of configuration file to update</param>
        /// <param name="folderName">Name of folder (UI or Service)</param>
        public static void UpdateWebConfig(Dictionary<string, string> configUpdateList, string deployConfigName, string folderName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            if (null != directoryInfo)
            {
                DirectoryInfo directory = directoryInfo.GetDirectories(folderName).FirstOrDefault();
                if (null != directory)
                {
                    List<FileInfo> manifestFile = directory.GetFiles(ConstantStrings.CONFIG_EXTENSION).ToList();
                    // Logic for Updating Web.Config
                    FileInfo mainConfigFile = directory.GetFiles(ConstantStrings.WEB_CONFIG + ConstantStrings.CONFIG).FirstOrDefault();

                    string configAppSettings = ConfigurationManager.AppSettings["configAppSettings"];
                    string configAdd = ConfigurationManager.AppSettings["configAdd"];
                    string configKey = ConfigurationManager.AppSettings["configKey"];
                    string customHeaders = ConfigurationManager.AppSettings["customHeaders"];
                    string constantAttribute = ConfigurationManager.AppSettings["constantAttribute"];
                    string xmlNodeInstrumentationKey = ConfigurationManager.AppSettings["xmlInstrumentationKey"];
                    string appInsightsConfigKey = ConfigurationManager.AppSettings["ConfigKeyAppInsight"];

                    if (manifestFile.Count > 0)
                    {
                        foreach (FileInfo configFile in manifestFile)
                        {
                            if (configFile.Name == deployConfigName + ConstantStrings.CONFIG)
                            {
                                string inputConfigFile = configFile.FullName;
                                XDocument document = XDocument.Load(inputConfigFile);

                                // If updating ApplicationInsights.config
                                if (deployConfigName.Equals(ConstantStrings.APP_INSIGHTS, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (null != document)
                                    {
                                        XElement intrumentationKeyXmlElement = document.Descendants().Where(item => item.Name.LocalName.Equals(xmlNodeInstrumentationKey)).FirstOrDefault();
                                        intrumentationKeyXmlElement.Value = string.IsNullOrWhiteSpace(configUpdateList[appInsightsConfigKey]) ? ConstantStrings.NA : configUpdateList[appInsightsConfigKey];
                                        document.Save(inputConfigFile);
                                    }
                                    break;
                                }

                                foreach (var element in configUpdateList)
                                {
                                    XElement stepOnes = document.Descendants(configAppSettings).Descendants(configAdd).Where(e => e.Attribute(configKey).Value.ToUpper(CultureInfo.InvariantCulture) == element.Key.ToUpper(CultureInfo.InvariantCulture)).FirstOrDefault();
                                    XElement stepTwos = document.Descendants(customHeaders).Descendants(configAdd).Where(e => e.Attribute(constantAttribute).Value.ToUpper(CultureInfo.InvariantCulture) == element.Key.ToUpper(CultureInfo.InvariantCulture)).FirstOrDefault();

                                    if (null != stepOnes)
                                    {
                                        if (stepOnes.Attribute(configKey).Value == "HostedAppHostNameOverride")
                                        {
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = configUpdateList["Access-Control-Allow-Origin"].Substring(configUpdateList["Access-Control-Allow-Origin"].IndexOf("//", StringComparison.Ordinal) + 2);
                                        }
                                        else if (stepOnes.Attribute(configKey).Value == "Access-Control-Allow-Origin")
                                        {
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = configUpdateList["Access-Control-Allow-Origin"];
                                        }
                                        else if (stepOnes.Attribute(configKey).Value == "ClientSigningCertificatePath")
                                        {
                                            string temp = configUpdateList["ClientSigningCertificatePath"];
                                            temp = temp.Substring(0, temp.LastIndexOf(".", StringComparison.Ordinal)) + ".pfx";
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = temp;
                                        }
                                        else if (stepOnes.Attribute(configKey).Value == "Mail_Cart_Mail_User_Name")
                                        {
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = configUpdateList["MailCartUserName"];
                                        }
                                        else if (stepOnes.Attribute(configKey).Value == "Mail_Cart_Mail_Password")
                                        {
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = configUpdateList["MailCartPassword"];
                                        }
                                        else
                                        {
                                            stepOnes.Attribute(ConstantStrings.VALUE).Value = element.Value;
                                        }
                                    }
                                    if (null != stepTwos)
                                    {
                                        stepTwos.Attribute(ConstantStrings.VALUE).Value = element.Value;
                                    }
                                }
                                document.Save(inputConfigFile);
                                configFile.CopyTo(mainConfigFile.FullName, true);       // Copy the file into original configuration file
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Main function of the utility
        /// </summary>
        /// <param name="args">args[0] = Operation to be performed, 1: updating schema files, 2: updating constant files, 3: updating Search Configuration XML</param>
        public static void Main(string[] args)
        {
            if (3 <= args.Length)
            {
                bool isTrustConfigurationCall = (args[0].Length > 32) ? true : false;   // Compare with GUID length (Issuer Id)
                ushort operation = 0;
                if (!isTrustConfigurationCall)
                {
                    operation = ushort.Parse(args[0], CultureInfo.InvariantCulture);
                }
                string username = args[1], password = args[2];
                if (!ExcelOperations.IsNullOrEmptyCredential(username, password))
                {
                    try
                    {
                        string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + ConstantStrings.BACKSLASH + ConfigurationManager.AppSettings["filename"];

                        if (1 == operation) // Update schema files
                        {
                            #region Update app schema files

                            // Code for updating schema files
                            Console.WriteLine("Updating app schema files");


                            string manifestSheetname = ConfigurationManager.AppSettings["manifestSheetname"];

                            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(manifestSheetname))
                            {
                                Dictionary<string, string> xmlUpdateList = ExcelOperations.ReadFromExcel(filePath, manifestSheetname);
                                //Dictionary<string, string> xmlUpdateList = new Dictionary<string, string>();

                                string azureSiteURL = args[3];
                                xmlUpdateList.Add(ConstantStrings.AZURE_UI_SITE_URL, azureSiteURL);
                                xmlUpdateList.Add(ConstantStrings.USERNAME, username);
                                xmlUpdateList.Add(ConstantStrings.PASSWORD, password);
                                if (xmlUpdateList.Count > 0)
                                {
                                    bool isSuccessfullyWritten = false;
                                    isSuccessfullyWritten = UpdateXML(xmlUpdateList);
                                    if (isSuccessfullyWritten)
                                    {
                                        Console.WriteLine(ConstantStrings.APP_SUCCESS_MESSAGE);
                                    }
                                    else
                                    {
                                        Console.WriteLine(ConstantStrings.APP_FAILURE_MESSAGE);
                                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_XML_UPDATE);
                                    }
                                }
                                else
                                {
                                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
                                }
                            }
                            else
                            {
                                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_EXCEL_INPUTS);
                            }
                            #endregion
                        }
                        else if (2 == operation)    // Update resource files
                        {
                            #region Update resource files
                            UpdateResourceFiles(args, username, password, filePath);
                            #endregion
                        }
                        else if (3 == operation)    // For updating search configuration xml
                        {
                            #region Update search configuration file
                            UpdateSearchConfig(filePath, username, password);
                            #endregion
                        }
                        else if (4 == operation)
                        {
                            Console.WriteLine(GetResultSourceId(username, password));
                        }
                        else if (0 == operation)
                        {
                            #region Update Issuer Id
                            // Add issue id to the web.config in case of On Premise scenario
                            // This issuer id is generated while configuring trust for your apps in One Click deployment steps
                            string constantsSheetName = ConfigurationManager.AppSettings["manifestSheetname"];
                            Dictionary<string, string> constantUpdateListold = ExcelOperations.ReadFromExcel(filePath, constantsSheetName);
                            constantUpdateListold.Add(ConstantStrings.USERNAME, username);
                            constantUpdateListold.Add(ConstantStrings.PASSWORD, password);
                            constantUpdateListold.Add("MailCartUserName", username);
                            constantUpdateListold.Add("MailCartPassword", password);

                            Dictionary<string, string> configUpdateList = new Dictionary<string, string>();
                            configUpdateList.Add("IssuerId", args[0]);
                            #endregion
                        }
                    }
                    catch (ArgumentNullException exception)
                    {
                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    }
                    catch (Exception exception)
                    {
                        ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.MESSAGE + exception.Message + ConstantStrings.STACKTRACE + exception.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Created method to refractor Main method
        /// </summary>
        /// <param name="args">Command line argument</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password of user</param>
        /// <param name="filePath">Path of the file</param>
        private static void UpdateResourceFiles(string[] args, string username, string password, string filePath)
        {
            // Code for updating resource files
            Console.WriteLine("Updating resource files");

            // Created a dictionary for mapping between keys present in Excel and actual keys in resource file
            Dictionary<string, string> keyMapping = new Dictionary<string, string>()
                            {
                                { "CatalogSiteURL", "Central_Repository_Url" },
                                { "TenantURL", "Site_Url" },
                                { "ImageDocumentIcon", "Image_Document_Icon" },
                                { "ImageGeneralDocument", "Image_General_Document" },
                                { "MailCartUserName", "Mail_Cart_Mail_User_Name" },
                                { "MailCartPassword", "Mail_Cart_Mail_Password" },
                                { "AppRedirectURL", "App_Redirect_URL" },
                                { "ApplicationInsightAppId", "Application_Insight_App_Id" },
                                { "LogTableName", "LogTableName,UILogTableName,UtilityLogTableName" }
                            };

            string constantsSheetName = ConfigurationManager.AppSettings["manifestSheetname"];

            if (!string.IsNullOrEmpty(filePath))
            {
                Dictionary<string, string> constantUpdateListold = ExcelOperations.ReadFromExcel(filePath, constantsSheetName);
                Dictionary<string, string> constantUpdateList = new Dictionary<string, string>();
                constantUpdateList.Add(ConstantStrings.USERNAME, username);
                constantUpdateList.Add(ConstantStrings.PASSWORD, password);
                if (5 <= args.Length)
                {
                    constantUpdateList.Add("MailCartUserName", args[3].Trim());
                    constantUpdateList.Add("MailCartPassword", args[4].Trim());
                }
                else
                {
                    ErrorMessage.ShowMessage("Failed to update exchange credentials into Web.Config", ErrorMessage.MessageType.Error);
                }

                foreach (string updateMapping in constantUpdateListold.Keys)
                {
                    if (keyMapping.ContainsKey(updateMapping))
                    {
                        if (updateMapping.Equals(ConfigurationManager.AppSettings["logTableNameKey"]))
                        {
                            constantUpdateList.Add(keyMapping[updateMapping].Split(',')[0], constantUpdateListold[updateMapping] + ConfigurationManager.AppSettings["logTableService"]);
                            constantUpdateList.Add(keyMapping[updateMapping].Split(',')[1], constantUpdateListold[updateMapping] + ConfigurationManager.AppSettings["logTableUI"]);
                            constantUpdateList.Add(keyMapping[updateMapping].Split(',')[2], constantUpdateListold[updateMapping] + ConfigurationManager.AppSettings["logTableUtility"]);
                        }
                        else
                        {
                            constantUpdateList.Add(keyMapping[updateMapping], constantUpdateListold[updateMapping]);
                        }
                    }
                    else
                    {
                        constantUpdateList.Add(updateMapping, constantUpdateListold[updateMapping]);
                    }
                }

                constantUpdateList.Add(ConfigurationManager.AppSettings["CommonTermSetGroupNameKey"], ConfigurationManager.AppSettings["CommonTermSetGroupNameValue"]);
                constantUpdateList.Add(ConfigurationManager.AppSettings["ContentTypeKey"], ConfigurationManager.AppSettings["ContentTypeValue"]);
                constantUpdateList.Add(ConfigurationManager.AppSettings["ContentTypeGroupKey"], ConfigurationManager.AppSettings["ContentTypeGroupValue"]);
                constantUpdateList.Add(ConfigurationManager.AppSettings["IsTenantDeploymentKey"], ConfigurationManager.AppSettings["IsTenantDeploymentValue"]);
                constantUpdateList.Add(ConfigurationManager.AppSettings["MatterProvision_GroupKey"], ConfigurationManager.AppSettings["MatterProvision_GroupValue"]);
                constantUpdateList.Add(ConfigurationManager.AppSettings["MatterProvisionAppURLKey"], string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", constantUpdateList["Central_Repository_Url"], "/", ConfigurationManager.AppSettings["MatterProvisionAppName"]));

                if (constantUpdateList.Count > 0)
                {
                    UpdateResource(constantUpdateList, ConstantStrings.UI_FOLDER_NAME);
                    UpdateResource(constantUpdateList, ConstantStrings.SERVICE_FOLDER_NAME);
                }
                else
                {
                    ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_NO_INPUTS);
                }

                // Update the web.config                                
                Dictionary<string, string> configUpdateList = new Dictionary<string, string>();

                // Created a dictionary for mapping between keys present in Excel and actual keys in resource file
                Dictionary<string, string> keyMappingConfig = new Dictionary<string, string>()
                            {
                                { "TenantURL", "HostedAppHostNameOverride" },
                                { "UISiteURL", "Access-Control-Allow-Origin" },
                                { "MailCartUserName", "Mail_Cart_Mail_User_Name" },
                                { "MailCartPassword", "Mail_Cart_Mail_Password" }
                            };

                foreach (string updateMapping in constantUpdateListold.Keys)
                {
                    if (keyMappingConfig.ContainsKey(updateMapping))
                    {
                        configUpdateList.Add(keyMappingConfig[updateMapping], constantUpdateListold[updateMapping]);
                    }
                    else
                    {
                        configUpdateList.Add(updateMapping, constantUpdateListold[updateMapping]);
                    }
                }

                // Update the configuration for Azure
                UpdateWebConfig(configUpdateList, ConstantStrings.WEB_CONFIG_CLOUD, ConstantStrings.UI_FOLDER_NAME);
                UpdateWebConfig(configUpdateList, ConstantStrings.WEB_CONFIG_CLOUD, ConstantStrings.SERVICE_FOLDER_NAME);
                UpdateWebConfig(configUpdateList, ConstantStrings.APP_INSIGHTS, ConstantStrings.UI_FOLDER_NAME);

                Console.WriteLine("Updated Web.config");
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorPath, ConstantStrings.ERROR_EXCEL_INPUTS);
            }
        }
    }
}
