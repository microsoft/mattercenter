// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 04-27-2015
//
// ***********************************************************************
// <copyright file="Utility.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides meta data related information for matter provision.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
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
    ///This class provides meta data related information for matter provision.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// A variable to save file path for logging error
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\" + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// Gets or sets path for error file
        /// </summary>
        public static string ErrorFilePath
        {
            get
            {
                return errorFilePath;
            }

            set
            {
                errorFilePath = value;
            }
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
                MatterProvisionHelperUtility.BreakPermission(clientContext, matter.MatterName, matter.CopyPermissionsFromParent, calendarName);
            }
            catch (Exception exception)
            {
                //// Generic Exception
                MatterProvisionHelper.DeleteMatter(clientContext, matter);
                DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        ///  Adds folders inside the document library
        /// </summary>
        /// <param name="clientContext">Context of client</param>
        /// <param name="list">List object</param>
        /// <param name="folderNames">Names of folder</param>
        /// <returns>List of folder added</returns>
        internal static List AddFolders(ClientContext clientContext, List list, IList<string> folderNames)
        {
            // Addition of Folders
            FolderCollection listFolders = list.RootFolder.Folders;
            Microsoft.SharePoint.Client.Folder listRootFolder = list.RootFolder;
            clientContext.Load(listFolders);
            if (0 < folderNames.Count)
            {
                foreach (string folderName in folderNames)
                {
                    // Check for empty folder names
                    if (!string.IsNullOrWhiteSpace(folderName))
                    {
                        listFolders.Add(folderName);
                        listRootFolder.Update();
                    }
                }
                list.Update();
                clientContext.ExecuteQuery();
            }
            return list;
        }

        /// <summary>
        /// Checks if the requested page exists or not.
        /// </summary>
        /// <param name="requestedUrl">URL of the page, for which check is to be performed</param>
        /// <param name="clientContext">ClientContext for SharePoint</param>
        /// <returns>true or false string based upon the existence of the page, referred in requestedUrl</returns>
        internal static bool PageExists(string requestedUrl, ClientContext clientContext)
        {
            Microsoft.SharePoint.Client.File clientFile = clientContext.Web.GetFileByServerRelativeUrl(requestedUrl);
            clientContext.Load(clientFile, cf => cf.Exists);
            clientContext.ExecuteQuery();
            return clientFile.Exists;
        }

        /// <summary>
        /// Validate and change the date format to universal sortable format 
        /// </summary>
        /// <param name="currentDate">Current date</param>
        /// <returns>String object</returns>
        internal static string ValidateDateFormat(string currentDate)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(currentDate))
                {
                    DateTime todaysDate = DateTime.Today;
                    DateTime dt = DateTime.FromOADate(Convert.ToDouble(currentDate, CultureInfo.InvariantCulture));
                    if (DateTime.Compare(dt, todaysDate) < 0)
                    {
                        result = dt.ToString(System.Configuration.ConfigurationManager.AppSettings["DateFormat"], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        result = Constants.InvalidConflictDate;
                    }
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(ErrorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                result = Constants.InvalidConflictDate;
            }
            return result;
        }

        /// <summary>
        /// Converts string to Pascal case
        /// </summary>
        /// <param name="str">String Object</param>
        /// <returns>String object</returns>
        internal static string UpperFirst(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : char.ToUpper(str[0], CultureInfo.InvariantCulture) + str.Substring(1).ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Log and display error
        /// </summary>
        /// <param name="errorFilePath">Error file path</param>
        /// <param name="errorMessage">Error message to display or write into file</param>
        internal static void DisplayAndLogError(string errorFilePath, string errorMessage)
        {
            ConsoleColor currentColor = Console.ForegroundColor;    // Save current foreground color
            Console.ForegroundColor = ConsoleColor.Red;
            ErrorLogger.LogErrorToTextFile(errorFilePath, errorMessage);
            Console.ForegroundColor = currentColor;
        }

        /// <summary>
        /// Validates the semicolon separate string
        /// </summary>
        /// <param name="receivedString">Received string object</param>
        /// <returns>String object</returns>
        internal static string ProcessString(string receivedString)
        {
            string processedString = string.Empty;
            List<string> listOfValues = receivedString.Trim().Split(';').Where(input => !string.IsNullOrWhiteSpace(input.Trim())).ToList();
            processedString = string.Join(";", listOfValues);
            return processedString.Trim();
        }
    }
}
