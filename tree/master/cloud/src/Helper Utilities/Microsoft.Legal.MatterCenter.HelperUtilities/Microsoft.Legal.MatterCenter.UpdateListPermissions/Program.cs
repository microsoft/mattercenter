// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateListPermissions
// Author           : v-shpate
// Created          : 24-7-2015
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file updates list permission.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateListPermissions
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
    /// Class to update the list permission.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// String variable to specify the error file path
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorLog"];

        /// <summary>
        /// String variable to specify filePath
        /// </summary>
        private static string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["fileName"];

        /// <summary>
        /// Static main method
        /// </summary>
        /// <param name="args">Command line argument</param>
        public static void Main(string[] args)
        {
            if (2 <= args.Length && !ExcelOperations.IsNullOrEmptyCredential(args[0], args[1]))
            {
                Dictionary<string, string> configVal = ExcelOperations.ReadFromExcel(filePath, ConfigurationManager.AppSettings["configSheetName"]);
                string username = args[0].Trim();
                string password = args[1].Trim();
                try
                {
                    string targetSite = configVal["CatalogSiteURL"];
                    using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, username, password))
                    {
                        CreateUpdateProvisionMatterListPermissions(clientContext, configVal);
                    }
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                }
            }
            else
            {
                ErrorMessage.ShowMessage("Please enter username and password", ErrorMessage.MessageType.Error);
            }
        }

        /// <summary>
        /// Function to update permissions on send mail list
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="configVal">Configuration values from configuration excel</param>
        internal static void CreateUpdateProvisionMatterListPermissions(ClientContext clientContext, Dictionary<string, string> configVal)
        {
            Dictionary<string, string> groupValues = ExcelOperations.ReadFromExcel(filePath, ConfigurationManager.AppSettings["groupSheetName"]);
            Console.Write("Creating Provision Matter List...");
            List provisionMatterList;
            //This is a dummy list that is getting created to check whether the current login user can create matter or not
            //Otherwise this list has no significance
            string listName = ConfigurationManager.AppSettings["listName"];
            Web web = clientContext.Web;

            ListCollection listcoll = web.Lists;

            clientContext.Load(listcoll, lists => lists.Include(list => list.Title)); //Refresh the collection
            clientContext.ExecuteQuery();
            provisionMatterList = listcoll.Cast<List>().FirstOrDefault(list => list.Title == listName);

            if (null == provisionMatterList)
            {
                ListCreationInformation listCreationInfo = new ListCreationInformation();
                listCreationInfo.Title = listName;
                listCreationInfo.TemplateType = (int)ListTemplateType.Announcements;
                provisionMatterList = web.Lists.Add(listCreationInfo);
                clientContext.Load(provisionMatterList);
                clientContext.ExecuteQuery();
            }

            clientContext.Load(listcoll, lists => lists.Include(list => list.Title, list => list.HasUniqueRoleAssignments)); //Refresh the collection
            clientContext.ExecuteQuery();
            provisionMatterList = listcoll.Cast<List>().FirstOrDefault(list => list.Title == listName);



            //Update list permissions
            if (provisionMatterList.HasUniqueRoleAssignments)
            {
                provisionMatterList.ResetRoleInheritance();
            }

            provisionMatterList.BreakRoleInheritance(false, true);
            clientContext.Load(provisionMatterList);
            clientContext.ExecuteQuery();

            RoleAssignmentCollection roleAssignment = provisionMatterList.RoleAssignments;
            List<string> groupNames = groupValues.Keys.ToList();
            if (3 <= groupNames.Count)
            {
                Group provisionGroup = web.SiteGroups.GetByName(groupNames[2]);
                RoleDefinition contributeRole = web.RoleDefinitions.GetByName(ConfigurationManager.AppSettings["roleType"]);
                clientContext.Load(roleAssignment);
                clientContext.Load(provisionGroup);
                clientContext.Load(contributeRole);
                clientContext.ExecuteQuery();

                RoleDefinitionBindingCollection collRdb = new Microsoft.SharePoint.Client.RoleDefinitionBindingCollection(clientContext);
                collRdb.Add(contributeRole);
                roleAssignment.Add(provisionGroup, collRdb);
                clientContext.ExecuteQuery();
            }
        }
    }
}