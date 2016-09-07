// Assembly         : Microsoft.Legal.MatterCenter.CreateContentTypes
// Author           : v-nikhid
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides functions to create content type in SharePoint.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateContentTypes
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    # endregion

    /// <summary>
    /// This class is the entry point for the application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Variable to store file path for logging errors
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorLogFile"];

        /// <summary>
        /// This method is the entry point for the application
        /// </summary>
        /// <param name="args">input from console</param>
        public static void Main(string[] args)
        {
            if (null != args && 2 <= args.Length)
            {
                string login = args[1].Trim(), password = args[2].Trim();
                bool createType = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);
                if (!ExcelOperations.IsNullOrEmptyCredential(login, password))
                {
                    string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                    string sheetName = ConfigurationManager.AppSettings["configsheetname"];
                    string termStoreSheetname = ConfigurationManager.AppSettings["termstoreSheetname"];
                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);

                    // Read from Excel
                    Collection<Collection<string>> termStoreVal = ExcelOperations.ReadSheet(filePath, termStoreSheetname);
                    List<string> contentTypeVal = new List<string>();
                    List<string> documentTemplateList = new List<string>();
                    int contentTypeIndex = termStoreVal[0].IndexOf(ConfigurationManager.AppSettings["contentTypeColumnName"]);
                    int documentTemplateIndex = termStoreVal[0].IndexOf(ConfigurationManager.AppSettings["documentTemplateColumnName"]);

                    for (int count = 1; count < termStoreVal.Count; count++)
                    {
                        if (!contentTypeVal.Contains(termStoreVal[count][contentTypeIndex]))
                        {
                            contentTypeVal.Add(termStoreVal[count][contentTypeIndex]);
                        }
                        documentTemplateList = documentTemplateList.Union(termStoreVal[count][documentTemplateIndex].Split(';')).ToList();
                    }
                    contentTypeVal = contentTypeVal.Union(documentTemplateList).ToList();

                    try
                    {
                        if (0 != listval.Count)
                        {
                            string targetSite = listval["ContentTypeHubURL"]; // Get the URL of site collection
                            ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password);

                            for (int count = 0; count < contentTypeVal.Count; count++)
                            {
                                string parentContentType = ConfigurationManager.AppSettings["ContentTypeValue"];
                                string contentType = contentTypeVal[count]; // Get Content Type DMS from Excel
                                string contentTypeGroup = ConfigurationManager.AppSettings["ContentTypeGroupValue"]; // Get Group of Content Type
                                if (createType)
                                {
                                    CreateContentType(clientContext, contentType, parentContentType, contentTypeGroup);
                                }
                                else
                                {
                                    DeleteContentType(clientContext, contentType);
                                }
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
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Invalid Credentials");
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Insufficient Parameters");
            }
        }

        /// <summary>
        /// Method to create Content Types
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="contentTypeName">Name of Content Type</param>
        /// <param name="parentContentTypeName">Name of Parent Content Type</param>
        /// <param name="contentTypeGroup">Name of group under which Content Type is to be added</param>
        public static void CreateContentType(ClientContext clientContext, string contentTypeName, string parentContentTypeName, string contentTypeGroup)
        {
            try
            {
                Console.WriteLine("Creating content type");

                Web web = clientContext.Web;
                ContentTypeCollection contentTypeCollection = web.ContentTypes;
                ContentType parentContentType = null;
                clientContext.Load(contentTypeCollection, contentTypes => contentTypes.Include(properties => properties.Name));
                clientContext.Load(web.Fields);
                clientContext.ExecuteQuery();
                parentContentType = (from contentTypes in contentTypeCollection where contentTypes.Name == parentContentTypeName select contentTypes).FirstOrDefault();
                ContentType contentType = (from contentTypes in contentTypeCollection where contentTypes.Name == contentTypeName select contentTypes).FirstOrDefault();

                if (null != contentType)
                {
                    contentType.DeleteObject();
                    web.Update();
                    clientContext.ExecuteQuery();
                }

                ContentTypeCreationInformation contentTypeCreationObj = new ContentTypeCreationInformation();
                contentTypeCreationObj.Name = contentTypeName;
                contentTypeCreationObj.Group = contentTypeGroup;
                if (parentContentType != null)
                {
                    contentTypeCreationObj.ParentContentType = parentContentType;
                }
                web.ContentTypes.Add(contentTypeCreationObj);
                web.Update();
                clientContext.ExecuteQuery();
                Console.WriteLine("Successfully created content type: " + contentTypeName);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while creating content type: " + exception.Message);
            }
        }

        /// <summary>
        /// Delete Content Types
        /// </summary>
        /// <param name="clientContext">SharePoint Client Context</param>
        /// <param name="contentTypeName">Name of content type to be deleted</param>
        public static void DeleteContentType(ClientContext clientContext, string contentTypeName)
        {
            try
            {
                Web web = clientContext.Web;
                ContentTypeCollection contentTypeCollection = web.ContentTypes;

                clientContext.Load(contentTypeCollection, contentTypes => contentTypes.Include(properties => properties.Name));
                clientContext.Load(web.Fields);
                clientContext.ExecuteQuery();
                ContentType contetType = (from contentTypes in contentTypeCollection where contentTypes.Name == contentTypeName select contentTypes).FirstOrDefault();
                if (null != contetType)
                {
                    contetType.DeleteObject();
                    web.Update();
                    clientContext.ExecuteQuery();
                    Console.WriteLine("Deleting content type: " + contentTypeName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception occurred while deleting content type: " + exception.Message);
            }
        }
    }
}
