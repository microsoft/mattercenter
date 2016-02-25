// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSiteColumns
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="CreateSiteColumns.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file creates site columns.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSiteColumns
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    #endregion

    /// <summary>
    /// Class for creating site columns
    /// </summary>
    internal static class CreateSiteColumns
    {
        /// <summary>
        /// A variable to save file path for logging error
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorLogFile"];

        /// <summary>
        /// Method to get data from Resource file
        /// </summary>
        /// <param name="fileName">Name of resource file</param>
        /// <param name="keyName">Key whose value is to be retrieved</param>
        /// <returns>Value of specified key</returns>
        public static string GetConfigDataFromResource(string fileName, string keyName)
        {
            string resourceValue = string.Empty;
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string[] appendResourceAssembly = new string[] { assemblyName, fileName };
            fileName = string.Join(".", appendResourceAssembly);
            try
            {
                ResourceManager resourceManager = new ResourceManager(fileName, Assembly.Load(assemblyName));
                resourceValue = resourceManager.GetString(keyName);
                return resourceValue;
            }
            catch (ArgumentNullException exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return null;
        }

        /// <summary>
        /// Method to revert creation of site column and Content Type
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="siteColumns">List of site columns</param>
        /// <param name="contentType">Name of Content Type</param>
        /// <param name="contentTypegroup">Name of Content Type group</param>
        /// <returns>Success or Message</returns>
        internal static void RevertSiteColumns(ClientContext clientContext, List<string> siteColumns, string contentType, string contentTypegroup)
        {
            try
            {
                Console.WriteLine("Deleting existing site columns and content type");
                Web web = clientContext.Web;
                ContentTypeCollection contentTypeCollection = web.ContentTypes;
                ContentType parentContentType = null;
                clientContext.Load(contentTypeCollection, contentTypeItem => contentTypeItem.Include(type => type.Group, type => type.Name).Where(f => f.Group == contentTypegroup));
                FieldCollection fieldCollection = web.Fields;
                clientContext.Load(fieldCollection, field => field.Include(attribute => attribute.Group, attribute => attribute.Title).Where(p => p.Group == contentTypegroup));
                clientContext.ExecuteQuery();
                parentContentType = (from contentTypes in contentTypeCollection where contentTypes.Name == contentType select contentTypes).FirstOrDefault();
                if (null != parentContentType)
                {
                    parentContentType.DeleteObject();
                }
                foreach (string columns in siteColumns)
                {
                    Field revertColumn = (from field in fieldCollection where field.Title == columns select field).FirstOrDefault();
                    if (null != revertColumn)
                    {
                        revertColumn.DeleteObject();
                    }
                }
                web.Update();
                clientContext.ExecuteQuery();
                Console.WriteLine("Deleted existing site columns and content type");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to delete existing site columns.");
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Method to create site column
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="siteColumns">List of site columns</param>
        /// <param name="contentTypeGroup">content type group</param>
        /// <returns>Success or Failure</returns>
        internal static bool CreateSiteColumn(ClientContext clientContext, List<string> siteColumns, string contentTypeGroup)
        {
            try
            {
                int totalColumnsCreated = 0;
                Console.WriteLine("Creating site columns");
                Web web = clientContext.Web;
                FieldCollection fieldCollection = web.Fields;
                clientContext.Load(fieldCollection, field => field.Include(fieldAttribute => fieldAttribute.Title).Where(fieldAttribute => fieldAttribute.Group == contentTypeGroup));
                clientContext.ExecuteQuery();
                List<Field> existingFields = fieldCollection.ToList<Field>();
                foreach (string columns in siteColumns)
                {
                    Field field = (from fields in existingFields where (fields.Title.Equals(columns, StringComparison.OrdinalIgnoreCase)) select fields).FirstOrDefault();
                    if (null == field)
                    {
                        string siteColumnXML = string.Format(CultureInfo.InvariantCulture, GetConfigDataFromResource("SiteColumn_Config", columns), contentTypeGroup);
                        web.Fields.AddFieldAsXml(siteColumnXML, true, AddFieldOptions.DefaultValue);
                        totalColumnsCreated++;
                    }
                    else
                    {
                        Console.WriteLine(string.Concat(GetConfigDataFromResource("SiteColumn_Config", "SiteColumnValue") + " " + columns + " " + GetConfigDataFromResource("SiteColumn_Config", "Exists")));
                    }
                }
                web.Update();
                clientContext.ExecuteQuery();
                if (0 < totalColumnsCreated)
                {
                    Console.WriteLine("Site columns created successfully");
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Method to add columns to Content Type
        /// </summary>
        /// <param name="web">Object of site</param>
        /// <param name="siteColumns">List of site columns</param>
        /// <param name="finalObj">Content type to which columns are to be added</param>
        /// <returns>Success or Failure</returns>
        private static bool AddColumnsToContentType(Web web, List<string> siteColumns, ContentType finalObj)
        {
            try
            {
                FieldCollection fieldCol = web.Fields;
                foreach (string columns in siteColumns)
                {
                    bool isColumnExists = (null != finalObj.Fields.Where(field => field.Title.Equals(columns, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()) ? true : false;
                    if (!isColumnExists)
                    {
                        Field customDoc = fieldCol.GetByTitle(columns);
                        FieldLinkCreationInformation fieldCreationInformation = new FieldLinkCreationInformation();
                        fieldCreationInformation.Field = customDoc;
                        finalObj.FieldLinks.Add(fieldCreationInformation);
                    }
                }
                finalObj.Update(true);
                return true;
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Method to create Content Types
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="siteColumns">List if site columns</param>
        /// <param name="contentTypeName">Name of Content Type</param>
        /// <param name="contentTypegroup">Name of group under which Content Type is to be added</param>
        /// <returns>Success or Failure</returns>
        internal static bool CreateContentType(ClientContext clientContext, List<string> siteColumns, string contentTypeName, string contentTypegroup)
        {
            try
            {
                int totalSiteColumnsAdded = 0;
                string taxonomyFieldType = ConfigurationManager.AppSettings["TaxonomyFieldType"];
                Console.WriteLine("Adding site columns to content type");
                Web web = clientContext.Web;
                ContentTypeCollection contentTypeCollection = web.ContentTypes;
                ContentType parentContentType = null;
                FieldCollection fieldCol = web.Fields;
                clientContext.Load(contentTypeCollection, contentTypes => contentTypes.Include(properties => properties.Name));
                clientContext.Load(web.Fields);
                clientContext.Load(fieldCol, field => field.Include(fieldAttribute => fieldAttribute.Title, fieldAttr => fieldAttr.TypeAsString).Where(fieldAttribute => (fieldAttribute.Group == contentTypegroup) && (fieldAttribute.TypeAsString == taxonomyFieldType)));
                clientContext.ExecuteQuery();
                parentContentType = (from contentTypes in contentTypeCollection where contentTypes.Name == ConfigurationManager.AppSettings["parentcontenttype"] select contentTypes).FirstOrDefault();
                ContentType contentType = (from contentTypes in contentTypeCollection where contentTypes.Name == contentTypeName select contentTypes).FirstOrDefault();

                // Check if content type is not present, then only create new "Matter Center" content type
                if (null == contentType)
                {
                    ContentTypeCreationInformation contentTypeCreationInformation = new ContentTypeCreationInformation();
                    contentTypeCreationInformation.Name = contentTypeName;
                    contentTypeCreationInformation.Group = contentTypegroup;
                    if (parentContentType != null)
                    {
                        contentTypeCreationInformation.ParentContentType = parentContentType;
                    }
                    contentType = web.ContentTypes.Add(contentTypeCreationInformation);
                    totalSiteColumnsAdded++;
                }

                clientContext.Load(contentType.Fields, field => field.Include(fieldAttribute => fieldAttribute.Title));
                clientContext.ExecuteQuery();
                MapMetadataColumns(clientContext, fieldCol);
                AddColumnsToContentType(web, siteColumns, contentType);
                web.Update();
                clientContext.ExecuteQuery();
                if (0 < totalSiteColumnsAdded)
                    Console.WriteLine("Successfully added site columns to content type");
                return true;
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Function to map metadata columns with specific term set
        /// </summary>
        /// <param name="clientContext">SP client context</param>
        /// <param name="fieldCol">Field Collection object</param>
        internal static void MapMetadataColumns(ClientContext clientContext, FieldCollection fieldCol)
        {
            string termsetName = ConfigurationManager.AppSettings["DefaultTermSetName"];
            string taxonomyFieldType = ConfigurationManager.AppSettings["TaxonomyFieldType"];
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            TermSetCollection termSetCollection = termStore.GetTermSetsByName(termsetName, 1033);       // Setting LCID=1033, as the default language is English
            clientContext.Load(termStore);
            clientContext.Load(termSetCollection);
            clientContext.ExecuteQuery();
            string termStoreId = Convert.ToString(termStore.Id, CultureInfo.InvariantCulture);
            string termSetId = Convert.ToString(termSetCollection[0].Id, CultureInfo.InvariantCulture);

            TaxonomyField taxonomyField = null;
            foreach (Field field in fieldCol)
            {
                if (field.TypeAsString.Equals(taxonomyFieldType, StringComparison.OrdinalIgnoreCase))
                {
                    taxonomyField = clientContext.CastTo<TaxonomyField>(field);
                    taxonomyField.SspId = new Guid(termStoreId);
                    taxonomyField.TermSetId = new Guid(termSetId);
                    taxonomyField.AnchorId = Guid.Empty;
                    taxonomyField.Update();
                }
            }
        }

        /// <summary>
        /// Function to create Site columns in content type hub
        /// </summary>
        /// <param name="revert">Flag to create or remove site columns</param>
        /// <param name="login">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        internal static void AddSiteColumns(bool revert, string login, string password)
        {
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
            string sheetName = ConfigurationManager.AppSettings["sheetname"];

            // Read from Excel
            Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);

            if (listval.Count != 0)
            {
                string targetSite = listval["ContentTypeHubURL"]; // Get the URL of site collection
                string contentType = ConfigurationManager.AppSettings["ContentTypeValue"]; // Get Content Type DMS from Excel
                string contentTypegroup = ConfigurationManager.AppSettings["ContentTypeGroupValue"]; // Get Group of Content Type
                bool isDeployedOnAzure = Convert.ToBoolean(listval["IsDeployedOnAzure"].ToUpperInvariant(), CultureInfo.InvariantCulture); // Get Is Deployed on Azure parameter

                try
                {
                    string siteColNames = GetConfigDataFromResource("SiteColumn_Config", "SiteColumnNames");
                    List<string> siteColumns = siteColNames.Split(new char[] { ',' }).ToList();

                    using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password, isDeployedOnAzure))
                    {
                        try
                        {
                            if (revert)
                            {
                                // Revert the creation of Site Columns
                                RevertSiteColumns(clientContext, siteColumns, contentType, contentTypegroup);
                            }
                            else
                            {
                                bool nextStep = CreateSiteColumn(clientContext, siteColumns, contentTypegroup);
                                if (nextStep)
                                {
                                    bool typeCreated = CreateContentType(clientContext, siteColumns, contentType, contentTypegroup);
                                    if (typeCreated)
                                    {
                                        Console.WriteLine(GetConfigDataFromResource("SiteColumn_Config", "MsgContentTypeCreated"));
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: No inputs from Excel file...");
            }
        }

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            if (null != args && 2 <= args.Length && !ExcelOperations.IsNullOrEmptyCredential(args[1], args[2]))
            {
                bool revert = Convert.ToBoolean(args[0].ToUpperInvariant(), CultureInfo.InvariantCulture);
                Console.WriteLine("Reading inputs from Excel...");
                AddSiteColumns(revert, args[1], args[2]);
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Insufficient Parameters");
            }
        }
    }
}
