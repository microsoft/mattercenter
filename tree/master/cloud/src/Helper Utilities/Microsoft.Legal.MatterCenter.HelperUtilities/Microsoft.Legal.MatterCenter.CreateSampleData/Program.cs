// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 08-27-2014
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This files creates sample matter(s).</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    #endregion

    /// <summary>
    /// This class is the entry point for the application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// A variable to save file path for logging error
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// Creates sample data based on the information provided
        /// </summary>
        /// <param name="listval">Matter details collection</param>
        /// <param name="clientDetails">Client details collection</param>
        /// <param name="configVal">Config values from Excel</param>        
        internal static void CreateData(List<DataStorage> listval, ClientTermSets clientDetails, Dictionary<string, string> configVal)
        {
            try
            {
                int successMatterNameCount = 0, alreadyExistsMatterCount = 0;
                Regex validateMatterId = new Regex(ConfigurationManager.AppSettings["SpecialCharacterExpressionMatterId"]),
                      validateMatterTitle = new Regex(ConfigurationManager.AppSettings["SpecialCharacterExpressionMatterTitle"]),
                      validateMatterDesc = new Regex(ConfigurationManager.AppSettings["SpecialCharacterExpressionMatterDescription"]);
                //Read data from term store
                TermSets terms = TermStoreOperations.FetchGroupTerms(configVal);
                if (null == terms)
                {
                    Utility.DisplayAndLogError(errorFilePath, "Failed to get Group Terms, skipping matter creation.");
                    return;
                }
                else
                {
                    MatterMetadata matterMetadata = new MatterMetadata();
                    //retrieve data from the list
                    for (int count = 0; count < listval.Count; count++)
                    {
                        string clientName = listval[count].ClientName;
                        /* Read from Term store */
                        Client client = clientDetails.ClientTerms.Where(item => item.ClientName.Equals(clientName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (null == client)
                        {
                            Console.WriteLine("Failed to get client Id and/or client Url from term store for '{0}' client.", clientName);
                            Console.WriteLine("-------------------------------------------------------------------------------");
                            continue;
                        }

                        List<string> practiceGroupsList = Utility.ProcessString(listval[count].PracticeGroup).Split(';').ToList();
                        List<string> areaOfLawsList = Utility.ProcessString(listval[count].AreaOfLaw).Split(';').ToList();
                        List<string> subAreaOfLawsList = Utility.ProcessString(listval[count].SubAreaOfLaw).Split(';').ToList();

                        string folders = string.Empty;
                        string documentTemplate = string.Empty;
                        bool flag = false;

                        AssociateTermStoreProperties(listval, terms, matterMetadata, count, practiceGroupsList, areaOfLawsList, subAreaOfLawsList, ref folders, ref documentTemplate, ref flag);
                        if (string.IsNullOrWhiteSpace(documentTemplate) || string.IsNullOrWhiteSpace(listval[count].DefaultContentType))
                        {
                            Console.WriteLine("Skipping matter creation as no matching document templates exists in term store corresponding to entry for '{0}' in the configuration Excel", client.ClientName);
                            Console.WriteLine("-------------------------------------------------------------------------------");
                            continue;
                        }

                        string[] contentTypes = documentTemplate.Split(';');
                        Matter matterObj = new Matter(listval[count]);
                        Console.WriteLine("Client details fetched");
                        Console.WriteLine("Client name: {0}", clientName);

                        using (ClientContext clientContext = MatterProvisionHelperUtility.GetClientContext(client.ClientUrl, configVal))
                        {
                            CheckMatterCreationStatus(configVal, ref successMatterNameCount, ref alreadyExistsMatterCount, validateMatterId, validateMatterTitle, validateMatterDesc, matterMetadata, clientName, client, folders, contentTypes, matterObj, clientContext);
                        }
                    }  // end of for

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(ConfigurationManager.AppSettings["MatterSuccess"], successMatterNameCount, listval.Count);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ConfigurationManager.AppSettings["MatterFound"], alreadyExistsMatterCount);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ConfigurationManager.AppSettings["MatterFailure"], Convert.ToString((listval.Count - (successMatterNameCount + alreadyExistsMatterCount)), CultureInfo.InvariantCulture), listval.Count);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Method to check matter creation status
        /// </summary>
        /// <param name="configVal">dictionary object</param>
        /// <param name="successMatterNameCount">success matter name</param>
        /// <param name="alreadyExistsMatterCount">count of existing matter</param>
        /// <param name="validateMatterId">validate matter id</param>
        /// <param name="validateMatterTitle">validate title</param>
        /// <param name="validateMatterDesc"> validate description</param>
        /// <param name="matterMetadata">matter metadata</param>
        /// <param name="clientName">client name</param>
        /// <param name="client">client object</param>
        /// <param name="folders">folder name</param>
        /// <param name="contentTypes">array of content type</param>
        /// <param name="matterObj">matter object</param>
        /// <param name="clientContext">client context</param>
        private static void CheckMatterCreationStatus(Dictionary<string, string> configVal, ref int successMatterNameCount, ref int alreadyExistsMatterCount, Regex validateMatterId, Regex validateMatterTitle, Regex validateMatterDesc, MatterMetadata matterMetadata, string clientName, Client client, string folders, string[] contentTypes, Matter matterObj, ClientContext clientContext)
        {
            string matterCreatedSuccessfully;
            if (Convert.ToInt32(ConfigurationManager.AppSettings["MatterIdMaxLength"], CultureInfo.InvariantCulture) < matterObj.MatterId.Length || (!validateMatterId.IsMatch(matterObj.MatterId)))
            {
                matterCreatedSuccessfully = Constants.InvalidMatterID;
            }
            else if (Convert.ToInt32(ConfigurationManager.AppSettings["MatterTitleMaxLength"], CultureInfo.InvariantCulture) < matterObj.MatterName.Length || (!validateMatterTitle.IsMatch(matterObj.MatterName)))
            {
                matterCreatedSuccessfully = Constants.InvalidMatterTitle;
            }
            else if (!validateMatterDesc.IsMatch(matterObj.MatterDescription))
            {
                matterCreatedSuccessfully = Constants.InvalidMatterDescription;
            }
            else if (Constants.InvalidConflictDate.Equals(matterObj.Conflict.ConflictCheckOn))
            {
                matterCreatedSuccessfully = Constants.InvalidConflictDate;
            }
            else
            {
                // Call to create matter
                matterCreatedSuccessfully = MatterProvisionHelper.CreateMatter(clientContext, client.ClientUrl, matterObj, folders);
            }
            if (Constants.MatterProvisionPrerequisitesSuccess == matterCreatedSuccessfully)
            {
                try
                {
                    successMatterNameCount = OnMatterCreationSuccess(configVal, successMatterNameCount, matterMetadata, clientName, client, contentTypes, matterObj, clientContext);
                }
                catch (Exception exception)
                {
                    Utility.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                    MatterProvisionHelper.DeleteMatter(clientContext, matterObj);
                }
            }
            else
            {
                switch (matterCreatedSuccessfully)
                {
                    case "1":
                        // matter is already present
                        Console.WriteLine("Matter: {0} is already present", matterObj.MatterName);
                        alreadyExistsMatterCount++;
                        break;
                    case "2":
                        // OneNote library is already present
                        Console.WriteLine("OneNote Library: {0} is already present", matterObj.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"]);
                        alreadyExistsMatterCount++;
                        break;
                    case "3":
                        // matter landing page is already present
                        Console.WriteLine("Matter Landing Page: {0} is already present", matterObj.MatterName);
                        alreadyExistsMatterCount++;
                        break;
                    case "4":
                        // Validation fail for matter ID
                        Console.WriteLine("Matter Id validation fail for Matter {0}", matterObj.MatterName);
                        break;
                    case "5":
                        // Validation fail for matter Name 
                        Console.WriteLine("Matter Name validation fail for Matter {0}", matterObj.MatterName);
                        break;
                    case "6":
                        // Validation fail for matter description
                        Console.WriteLine("Matter Description validation fail for Matter {0}", matterObj.MatterName);
                        break;
                    case "7":
                        // Validation fail for conflict date
                        Console.WriteLine("Conflict date validation fail for Matter {0}", matterObj.MatterName);
                        break;
                }
                Console.WriteLine("-------------------------------------------------------------------------------");
            }
        }

        /// <summary>
        /// Method to associate 
        /// </summary>
        /// <param name="listval">Value of list</param>
        /// <param name="terms">Termsets object</param>
        /// <param name="matterMetadata">Matter meta data</param>
        /// <param name="count">Integer count</param>
        /// <param name="practiceGroupsList">List of practice groups</param>
        /// <param name="areaOfLawsList">List of area of laws</param>
        /// <param name="subAreaOfLawsList">List of sub area of law</param>
        /// <param name="folders">Folder name</param>
        /// <param name="documentTemplate">Template of document</param>
        /// <param name="flag">Boolean flag</param>
        private static void AssociateTermStoreProperties(List<DataStorage> listval, TermSets terms, MatterMetadata matterMetadata, int count, List<string> practiceGroupsList, List<string> areaOfLawsList, List<string> subAreaOfLawsList, ref string folders, ref string documentTemplate, ref bool flag)
        {
            foreach (PracticeGroupTerm practiceGroup in terms.PGTerms)
            {
                foreach (AreaTerm areaTerm in practiceGroup.AreaTerms)
                {
                    foreach (SubareaTerm subAreaTerm in areaTerm.SubareaTerms)
                    {
                        if (subAreaOfLawsList.Contains(subAreaTerm.TermName, StringComparer.OrdinalIgnoreCase) && areaOfLawsList.Contains(areaTerm.TermName, StringComparer.OrdinalIgnoreCase) && practiceGroupsList.Contains(practiceGroup.TermName, StringComparer.OrdinalIgnoreCase))
                        {
                            if (Convert.ToBoolean(subAreaTerm.IsNoFolderStructurePresent, CultureInfo.InvariantCulture))
                            {
                                folders = string.IsNullOrWhiteSpace(areaTerm.FolderNames) ? practiceGroup.FolderNames : areaTerm.FolderNames;
                            }
                            else
                            {
                                folders = subAreaTerm.FolderNames;
                            }
                            documentTemplate = subAreaTerm.ContentTypeName + ";" + subAreaTerm.DocumentTemplates;
                            listval[count].DefaultContentType = subAreaTerm.ContentTypeName;
                            // Setting metadata properties for the Matter
                            matterMetadata.PracticeGroupTerm = practiceGroup;
                            matterMetadata.AreaTerm = areaTerm;
                            matterMetadata.SubareaTerm = subAreaTerm;
                            if (!string.IsNullOrWhiteSpace(subAreaTerm.DocumentTemplates))
                            {
                                listval[count].DocumentCount = Convert.ToString(subAreaTerm.DocumentTemplates.Split(';').Count(), CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                listval[count].DocumentCount = Constants.ZERO_DOCUMENT_COUNT;
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Created to refactor CreateData method
        /// </summary>
        /// <param name="configVal">Dictionary object</param>
        /// <param name="successMatterNameCount">Success matter name</param>
        /// <param name="matterMetadata">Matter metadata</param>
        /// <param name="clientName">Client name</param>
        /// <param name="client">Client object</param>
        /// <param name="contentTypes">Array of content type</param>
        /// <param name="matterObj">Matter object</param>
        /// <param name="clientContext">Client context</param>
        /// <returns>integer value</returns>
        private static int OnMatterCreationSuccess(Dictionary<string, string> configVal, int successMatterNameCount, MatterMetadata matterMetadata, string clientName, Client client, string[] contentTypes, Matter matterObj, ClientContext clientContext)
        {
            Console.WriteLine("Created Matter, OneNote library, calendar list and Task List");

            List<string> listResponsibleAttorneys = matterObj.TeamInfo.ResponsibleAttorneys.Trim().Split(';').ToList();
            List<string> listAttorneys = matterObj.TeamInfo.Attorneys.Trim().Split(';').ToList();
            List<string> listBlockedUploadUsers = matterObj.TeamInfo.BlockedUploadUsers.Trim().Split(';').ToList();
            IList<IList<string>> assignUserNames = new List<IList<string>>();
            assignUserNames.Add(listResponsibleAttorneys);
            assignUserNames.Add(listAttorneys);
            assignUserNames.Add(listBlockedUploadUsers);
            matterObj.AssignUserNames = assignUserNames;
            matterMetadata.Matter = matterObj;
            matterMetadata.Client = client;
            // Create Matter Landing page
            MatterProvisionHelper.CreateMatterLandingPage(clientContext, client, matterObj);
            Console.WriteLine("Created matter landing Page");

            // Step 4 Assign Content Types
            matterMetadata.ContentTypes = contentTypes.Distinct().ToList();
            MatterProvisionHelperUtility.AssignContentType(clientContext, matterMetadata);
            Console.WriteLine("Assigned content type");

            // Step 5 Assign Permissions
            bool isCalendarEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["CalendarCreationEnabled"], CultureInfo.InvariantCulture);
            bool isTaskEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["TaskListCreationEnabled"], CultureInfo.InvariantCulture);
            string calendarName = string.Empty, taskListName = string.Empty;
            if (isCalendarEnabled)
            {
                calendarName = string.Concat(matterObj.MatterName, ConfigurationManager.AppSettings["CalendarNameSuffix"]);
            }
            if (isTaskEnabled)
            {
                taskListName = string.Concat(matterObj.MatterName, ConfigurationManager.AppSettings["TaskListSuffix"]);
            }
            List<string> responsibleAttorneysList = matterObj.TeamInfo.ResponsibleAttorneys.Split(';').Where(responsibleAttorney => !string.IsNullOrWhiteSpace(responsibleAttorney.Trim())).Select(responsibleAttorney => responsibleAttorney.Trim()).ToList();
            MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, responsibleAttorneysList, ConfigurationManager.AppSettings["FullControl"]);
            MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], responsibleAttorneysList, ConfigurationManager.AppSettings["FullControl"]);
            if (isCalendarEnabled)
            {
                // If isCreateCalendar flag is enabled; assign FULL CONTROL permissions to calendar list
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, responsibleAttorneysList, ConfigurationManager.AppSettings["FullControl"], calendarName);
            }
            if (isTaskEnabled)
            {
                // If isTaskEnabled flag is enabled; assign FULL CONTROL permissions to task list
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, taskListName, responsibleAttorneysList, ConfigurationManager.AppSettings["FullControl"]);
            }
            List<string> attorneysList = new List<string>();
            string[] attorneys = matterObj.TeamInfo.Attorneys.Split(';');
            if (!string.IsNullOrWhiteSpace(attorneys[0].Trim()))
            {
                int attorneyCount = matterObj.TeamInfo.Attorneys.Split(';').Length;
                for (int iLength = 0; iLength < attorneyCount; iLength++)
                {
                    attorneysList.Add(attorneys[iLength].Trim());
                }
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, attorneysList, ConfigurationManager.AppSettings["Contribute"]);
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], attorneysList, ConfigurationManager.AppSettings["Contribute"]);
                if (isCalendarEnabled)
                {
                    //If isCreateCalendar flag is enabled; assign CONTRIBUTE permissions to calendar list
                    MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, attorneysList, ConfigurationManager.AppSettings["Contribute"], calendarName);
                }
                if (isTaskEnabled)
                {
                    //If isTaskEnabled flag is enabled; assign CONTRIBUTE permissions to task list
                    MatterProvisionHelperUtility.AssignUserPermissions(clientContext, taskListName, attorneysList, ConfigurationManager.AppSettings["Contribute"]);
                }
            }

            List<string> blockedUploadUserList = new List<string>();
            string[] blockedUploadUsers = matterObj.TeamInfo.BlockedUploadUsers.Split(';');
            if (!string.IsNullOrWhiteSpace(blockedUploadUsers[0].Trim()))
            {
                int blockUploadUsersCount = blockedUploadUsers.Length;
                for (int iLength = 0; iLength < blockUploadUsersCount; iLength++)
                {
                    blockedUploadUserList.Add(blockedUploadUsers[iLength].Trim());
                }
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, blockedUploadUserList, ConfigurationManager.AppSettings["Read"]);
                MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName + ConfigurationManager.AppSettings["OneNoteLibrarySuffix"], blockedUploadUserList, ConfigurationManager.AppSettings["Read"]);
                if (isCalendarEnabled)
                {
                    //If isCreateCalendar flag is enabled; assign READ permissions to calendar list
                    MatterProvisionHelperUtility.AssignUserPermissions(clientContext, matterObj.MatterName, blockedUploadUserList, ConfigurationManager.AppSettings["Read"], calendarName);
                }
                if (isTaskEnabled)
                {
                    //If isTaskEnabled flag is enabled; assign READ permissions to task list
                    MatterProvisionHelperUtility.AssignUserPermissions(clientContext, taskListName, blockedUploadUserList, ConfigurationManager.AppSettings["Read"]);
                }
            }
            Console.WriteLine("Assigned permission");

            // Step 6 Stamp properties 
            ListOperations.UpdateMetadataForList(clientContext, matterObj, client);
            Console.WriteLine("Updated matter properties");

            // Step 7 Add entry to list
            MatterProvisionHelper.InsertIntoMatterCenterMatters(configVal, clientName + "_" + matterObj.MatterName, matterObj, client);
            Console.WriteLine("{0} created successfully", matterObj.MatterName);
            Console.WriteLine("-------------------------------------------------------------------------------");
            successMatterNameCount++;
            return successMatterNameCount;
        }

        /// <summary>
        /// Reverts the changes for sample data based on the information provided
        /// </summary>
        /// <param name="matterDetailsCollection">Matter details collection</param>
        /// <param name="clientCollection">Client details collection</param>
        /// <param name="configVal">Configuration values from Excel</param>
        internal static void RevertData(List<DataStorage> matterDetailsCollection, ClientTermSets clientCollection, Dictionary<string, string> configVal)
        {
            try
            {
                if (null != matterDetailsCollection && null != clientCollection && null != configVal && 0 < matterDetailsCollection.Count)
                {
                    foreach (DataStorage matterDetails in matterDetailsCollection)
                    {
                        Client clientObject = clientCollection.ClientTerms.Where(item => item.ClientName.Equals(matterDetails.ClientName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (null != clientObject)
                        {
                            using (ClientContext clientContext = MatterProvisionHelperUtility.GetClientContext(clientObject.ClientUrl, configVal))
                            {
                                PropertyValues properties = clientContext.Web.Lists.GetByTitle(matterDetails.MatterPrefix).RootFolder.Properties;
                                clientContext.Load(properties);
                                clientContext.ExecuteQuery();
                                Matter matter = new Matter(matterDetails);
                                matter.MatterGuid = properties.FieldValues.ContainsKey("MatterGUID") ? System.Web.HttpUtility.HtmlDecode(Convert.ToString(properties.FieldValues["MatterGUID"], CultureInfo.InvariantCulture)) : matterDetails.MatterPrefix;
                                MatterProvisionHelper.DeleteMatter(clientContext, matter);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to get Client Url for client: {0}", matterDetails.ClientName);
                            Console.WriteLine("-------------------------------------------------------------------------------");
                            continue;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Main Method to initiate the execution
        /// </summary>
        /// <param name="args">Command line argument</param>
        public static void Main(string[] args)
        {
            try
            {
                if (null != args && 2 <= args.Length)
                {
                    bool createData = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);
                    if (!ExcelOperations.IsNullOrEmptyCredential(args[1], args[2]))
                    {
                        //// Read Configuration sheet and Sample data sheet from Excel
                        string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                        string sheetName = ConfigurationManager.AppSettings["sheetname"];
                        string configSheetName = ConfigurationManager.AppSettings["configsheetname"];
                        Dictionary<string, string> configVal = ExcelOperations.ReadFromExcel(filePath, configSheetName);
                        configVal.Add("Username", args[1].Trim());
                        configVal.Add("Password", args[2].Trim());
                        Collection<Collection<string>> dataValue = ExcelOperations.ReadSheet(filePath, sheetName);
                        List<DataStorage> matterDetails = MatterProvisionHelper.FetchMatterData(dataValue);
                        ClientTermSets clientDetails = TermStoreOperations.GetClientDetails(configVal);

                        if (createData)
                        {
                            CreateData(matterDetails, clientDetails, configVal);
                        }
                        else
                        {
                            RevertData(matterDetails, clientDetails, configVal);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Username and Password");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect command line argument was supplied. Kindly provide correct command line argument.");
                }
                Console.WriteLine("\n\n---Execution completed---");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Utility.DisplayAndLogError(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                Console.WriteLine("Error log will found at {0}", errorFilePath);
                Console.WriteLine("\n\n---Execution completed---");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}