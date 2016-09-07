// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file updates taxonomy fields.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
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
    using System.Net;
    #endregion

    /// <summary>
    /// This class updates taxonomy fields.
    /// </summary>
    internal class Program
    {   
        /// <summary>
        /// String variable to store log file location
        /// </summary>
        private static string logfileLocation = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + Constants.BackSlash + ConfigurationManager.AppSettings["logfileLocation"];

        /// <summary>
        /// Object of SharePointOnlineCredentials class initialize to null
        /// </summary>
        private static SharePointOnlineCredentials credentials = null;

        /// <summary>
        /// Staic main method
        /// </summary>
        /// <param name="args">Command line arguement</param>
        public static void Main(string[] args)
        {
            if (2 == args.Length && !ExcelOperations.IsNullOrEmptyCredential(args[0], args[1]))
            {
                try
                {
                    using (ConsoleCopy console_copy = new ConsoleCopy(logfileLocation))
                    {
                        string termGroupName = ConfigurationManager.AppSettings["termGroupName"];
                        string termSetNameClient = ConfigurationManager.AppSettings["termSetNameClient"];
                        string termSetNamePracticeGroup = ConfigurationManager.AppSettings["termSetNamePracticeGroup"];
                        string practiceGroupFieldName = ConfigurationManager.AppSettings["practiceGroupFieldName"];
                        string areaOfLawFieldName = ConfigurationManager.AppSettings["areaOfLawFieldName"];
                        string subareaOfLawColumnName = ConfigurationManager.AppSettings["subareaOfLawColumnName"];
                        string userName = args[0].Trim();
                        string password = args[1].Trim();
                        List<PracticeGroup> practiceGroups = new List<PracticeGroup>();
                        List<Client> clients = new List<Client>();
                        ClientContext clientContext = GetClientContext(userName, password);
                        TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                        TermGroup termGroup = Utility.LoadTermGroup(clientContext, taxonomySession, termGroupName);
                        clients = Utility.IntializeClientObject(clientContext, termGroup, termSetNameClient);
                        practiceGroups = Utility.IntializePracticeGroupObject(clientContext, termGroup, termSetNamePracticeGroup);
                        foreach (Client client in clients)
                        {
                            UpdateAllClients(practiceGroupFieldName, areaOfLawFieldName, subareaOfLawColumnName, practiceGroups, client);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorMessage.ShowMessage(exception.Message, ErrorMessage.MessageType.Error);
                }
            }
            else
            {
                ErrorMessage.ShowMessage("Please enter the username and password", ErrorMessage.MessageType.Error);
            }
        }

        /// <summary>
        /// To update all clients
        /// </summary>
        /// <param name="practiceGroupFieldName">Practice Group field name</param>
        /// <param name="areaOfLawFieldName">Area of Law field name</param>
        /// <param name="subareaOfLawColumnName">Sub Area of Law column</param>
        /// <param name="practiceGroups">Practice Groups</param>
        /// <param name="client">Client object</param>
        /// <returns></returns>
        private static void UpdateAllClients(string practiceGroupFieldName, string areaOfLawFieldName, string subareaOfLawColumnName, List<PracticeGroup> practiceGroups, Client client)
        {
            if (!string.IsNullOrWhiteSpace(client.Url))
            {
                using (ClientContext clientContext = new ClientContext(client.Url))
                {
                    clientContext.Credentials = credentials;
                    int templateType = Convert.ToInt32(ListTemplateType.DocumentLibrary, CultureInfo.InvariantCulture);
                    ListCollection collection = clientContext.Web.Lists;
                    clientContext.Load(collection,
                          item => item.Where(items => (items.BaseTemplate == templateType)).Include(
                             items => items.Title,
                             items => items.RootFolder.Properties)
                          );

                    clientContext.ExecuteQuery();
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.ProcessingMessage, client.Name));
                    UpdateListCollection(practiceGroupFieldName, areaOfLawFieldName, subareaOfLawColumnName, practiceGroups, clientContext, client, collection);
                }
            }
        }

        /// <summary>
        /// To update List Collection
        /// </summary>
        /// <param name="practiceGroupFieldName">Practice Group field name</param>
        /// <param name="areaOfLawFieldName">Area of Law field name</param>
        /// <param name="subareaOfLawColumnName">Sub Area of Law column</param>
        /// <param name="practiceGroups">Practice groups</param>
        /// <param name="clientContext">Client Context</param>
        /// <param name="client">Client object</param>
        /// <param name="collection">List collection</param>
        private static void UpdateListCollection(string practiceGroupFieldName, string areaOfLawFieldName, string subareaOfLawColumnName, List<PracticeGroup> practiceGroups, ClientContext clientContext, Client client, ListCollection collection)
        {
            string matterCenterDefaultContentType = string.Empty;
            // Display the group name
            bool isUpdate = false;
            foreach (List list in collection)
            {
                try
                {
                    if (list.RootFolder.Properties.FieldValues.ContainsKey(Constants.MatterKey))
                    {
                        if (list.RootFolder.Properties.FieldValues.ContainsKey(Constants.MatterContentTypeKey))
                        {
                            matterCenterDefaultContentType = WebUtility.HtmlDecode(Convert.ToString(list.RootFolder.Properties.FieldValues[Constants.MatterContentTypeKey], CultureInfo.InvariantCulture));
                            isUpdate = Utility.UpdateMatterBasedOnContentType(clientContext, list, practiceGroups, matterCenterDefaultContentType, client.Name, 1);
                            if (isUpdate)
                            {
                                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.SuccessMessageMatter, list.Title));
                            }
                        }
                        else
                        {
                            string areaOfLawStringTrimed, practiceGroupStringTrimed;
                            string[] subAreaOfLawString = Convert.ToString(list.RootFolder.Properties.FieldValues[subareaOfLawColumnName], CultureInfo.InvariantCulture).Split(';');
                            string[] areaOfLawString = Convert.ToString(list.RootFolder.Properties.FieldValues[areaOfLawFieldName], CultureInfo.InvariantCulture).Split(';');
                            string[] practiceGroupString = Convert.ToString(list.RootFolder.Properties.FieldValues[practiceGroupFieldName], CultureInfo.InvariantCulture).Split(';');
                            ContentTypeCollection currentContentTypeOrder = list.ContentTypes;
                            clientContext.Load(currentContentTypeOrder);
                            clientContext.ExecuteQuery();
                            string subAreaOfLawStringTrimed = WebUtility.HtmlDecode(currentContentTypeOrder[0].Name.ToString());
                            int iPosition = Array.IndexOf(subAreaOfLawString, subAreaOfLawStringTrimed);
                            if (-1 == iPosition)
                            {
                                areaOfLawStringTrimed = WebUtility.HtmlDecode(areaOfLawString[0]);
                                practiceGroupStringTrimed = WebUtility.HtmlDecode(practiceGroupString[0]);
                            }
                            else
                            {
                                areaOfLawStringTrimed = WebUtility.HtmlDecode(areaOfLawString[iPosition]);
                                practiceGroupStringTrimed = WebUtility.HtmlDecode(practiceGroupString[iPosition]);
                            }
                            if (!(string.IsNullOrWhiteSpace(subAreaOfLawStringTrimed) || (string.IsNullOrWhiteSpace(areaOfLawStringTrimed)) || (string.IsNullOrWhiteSpace(practiceGroupStringTrimed))))
                            {
                                isUpdate = Utility.UpdateMatterBasedOnContentType(clientContext, list, practiceGroups, subAreaOfLawStringTrimed, client.Name, 2);
                                if (isUpdate)
                                {
                                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.SuccessMessageMatter, list.Title));
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.SkippedMatterMessageMatter, list.Title));
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.ClientFailureMessage, client.Name, exception.Message));
                }
            }
        }

        /// <summary>
        /// Storing Credentials For SharePoint
        /// </summary>
        /// <param name="login">Login detail</param>
        /// <param name="password">Pass word</param>
        /// <returns>Client context object</returns>
        public static ClientContext GetClientContext(string login, string password)
        {
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
            string sheetName = ConfigurationManager.AppSettings["sheetname"];
            // Read from Excel
            Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
            string catalogSiteURL = listval["TenantURL"];
            return ConfigureSharePointContext.ConfigureClientContext(catalogSiteURL, login, password );
        }
    }
}