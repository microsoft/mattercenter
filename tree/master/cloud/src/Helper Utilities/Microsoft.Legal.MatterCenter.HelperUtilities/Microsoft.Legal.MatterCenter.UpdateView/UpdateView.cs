// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateView
// Author           : v-sauve
// Created          : 10-09-2015
//
// ***********************************************************************
// <copyright file="UpdateView.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file updates the client list with fields.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateView
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
    /// Update the list of the client with the title field
    /// </summary>
   internal class UpdateView
    {
        /// <summary>
        /// Main method - Start of the program
        /// </summary>
        /// <param name="args"> argument as parameter </param>
        public static void Main(string[] args)
        {
            if (2 <= args.Length && !ExcelOperations.IsNullOrEmptyCredential(args[0], args[1]))
            {
                try
                {
                    // Read login credentials from excel
                    string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                    string sheetName = ConfigurationManager.AppSettings["SheetName"];
                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                    string username = args[0].Trim();
                    string password = args[1].Trim();

                    // Read client context for Tenant URL
                    using (ClientContext userClientContext = ConfigureSharePointContext.ConfigureClientContext(listval["CatalogSiteURL"], username, password ))
                    {
                        // Reading SharePoint properties
                        string groupName = ConfigurationManager.AppSettings["PracticeGroupName"]; // Get Practice Group Name
                        string termSetName = ConfigurationManager.AppSettings["TermSetName"]; // Get Term Set Name
                        string clientIdProperty = ConfigurationManager.AppSettings["ClientIDProperty"]; // Get Client ID 
                        string clientUrlProperty = ConfigurationManager.AppSettings["ClientUrlProperty"]; // Get Client Url 
                        string clientUrl = null; // Store Client URL
                        string selectedField = null;

                        // Reading client term sets
                        ClientTermSets clientTermSets = TermStoreOperations.GetClientDetails(userClientContext, groupName, termSetName, clientIdProperty, clientUrlProperty);

                        // Iterating over clients in term store to get client URL and update lists
                        foreach (Client client in clientTermSets.ClientTerms)
                        {
                            try
                            {
                                clientUrl = client.ClientUrl;
                                if (!string.IsNullOrEmpty(clientUrl))
                                {
                                    ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(clientUrl, username, password );
                                    List list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings["ListName"]);
                                    ViewCollection viewFields = list.Views;
                                    View targetView = viewFields.GetByTitle(ConfigurationManager.AppSettings["ViewName"]);
                                    clientContext.Load(targetView.ViewFields);
                                    clientContext.ExecuteQuery();

                                    // Update fields to list only if title field is not present already
                                    selectedField = UpdateFields(selectedField, client, clientContext, list, targetView);
                                }

                                ErrorMessage.ShowMessage(client.ClientName + " site collection view updated with field", ErrorMessage.MessageType.Success);
                            }
                            catch (Exception exception)
                            {
                                ErrorMessage.ShowMessage(exception.Message + client.ClientName, ErrorMessage.MessageType.Error);
                            }
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
                ErrorMessage.ShowMessage("Please enter the Username and Password", ErrorMessage.MessageType.Error);
            }
        }

        /// <summary>
        /// String method to updating the view for site collection
        /// </summary>
        /// <param name="selectedField">Field which is being selected</param>
        /// <param name="client">Client object</param>
        /// <param name="clientContext">Context of client</param>
        /// <param name="list">List object</param>
        /// <param name="targetView">Targeted view</param>
        /// <returns>Selected field</returns>
        private static string UpdateFields(string selectedField, Client client, ClientContext clientContext, List list, View targetView)
        {
            List<string> updateFields = ConfigurationManager.AppSettings["ColumnName"].Split(';').ToList();
            ErrorMessage.ShowMessage("\nUpdating View for site collection " + client.ClientName, ErrorMessage.MessageType.Notification);
            foreach (string field in updateFields)
            {
                try
                {
                    if (!string.IsNullOrEmpty(field))
                    {
                        selectedField = field;
                        if (!targetView.ViewFields.Contains(field))
                        {
                            targetView.ViewFields.Add(field);
                            targetView.Update();
                            list.Update();

                            clientContext.Load(list);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
                catch (Exception)
                {
                    ErrorMessage.ShowMessage(selectedField + " field failed to update/unavailable for site collection " + client.ClientName, ErrorMessage.MessageType.Error);
                }
            }

            return selectedField;
        }
    }
}
