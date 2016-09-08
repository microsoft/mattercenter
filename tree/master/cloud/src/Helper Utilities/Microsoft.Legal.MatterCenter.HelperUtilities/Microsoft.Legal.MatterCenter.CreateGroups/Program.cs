// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateGroups
// Author           : v-akdigh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used adds group.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateGroups
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
    using System.Net;
    using System.Security;
    using System.Threading;
    #endregion

    /// <summary>
    /// Class exposes properties for data storage
    /// </summary>
    public class DataStorage
    {
        /// <summary>
        /// Gets or sets the site url
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets group description
        /// </summary>
        public string GroupDesc { get; set; }

        /// <summary>
        /// Gets or sets permissions
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Gets or sets members
        /// </summary>
        public string Members { get; set; }
    }

    /// <summary>
    /// This class is the entry point for the application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// variable to save file path for logging errors
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorlog"];

        /// <summary>
        /// This method is the entry point for the application
        /// </summary>
        /// <param name="args">input from console</param>
        public static void Main(string[] args)
        {
            string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
            string sheetName = ConfigurationManager.AppSettings["configsheetname"];
            if (null != args && 3 <= args.Length)
            {
                string login = args[1], password = args[2];
                if (!ExcelOperations.IsNullOrEmptyCredential(login, password))
                {
                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                    listval.Add("Username", login);
                    listval.Add("Password", password);
                    Collection<Collection<string>> groupSheetValues = ExcelOperations.ReadSheet(filePath, ConfigurationManager.AppSettings["groupsheetname"]);
                    List<DataStorage> groupData = ReadGroupConfig(groupSheetValues, listval);
                    if (args.Length != 0 && !string.IsNullOrWhiteSpace(args[0]))
                    {
                        if (Convert.ToBoolean(args[0], CultureInfo.InvariantCulture))
                        {
                            AddGroups(listval, groupData);
                        }
                        else
                        {
                            DeleteGroups(listval, groupData);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Credentials.");
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception Details: Invalid Credentials.");
                }
            }
            else
            {
                Console.WriteLine("Insufficient Parameters.");
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception Details: Insufficient Parameters.");
            }
        }

        /// <summary>
        /// This method is used to delete groups
        /// </summary>
        /// <param name="listval">dictionary object</param>
        /// <param name="groupData">list of data storage objects</param>
        private static void DeleteGroups(Dictionary<string, string> listval, List<DataStorage> groupData)
        {
            try
            {
                // Get the URL of site collection
                string login = listval["Username"]; // Get the user name
                string password = listval["Password"]; // Get the password

                foreach (DataStorage item in groupData)
                {
                    using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(item.SiteUrl, login, password ))
                    {
                        //Deleting group if exists
                        Web site = clientContext.Web;
                        GroupCollection collGroup = site.SiteGroups;
                        clientContext.Load(collGroup, groups => groups.Include(properties => properties.Title));
                        clientContext.ExecuteQuery();
                        Console.WriteLine("Checking if group " + item.GroupName + " exists");
                        Group currentGrp = (from grp in collGroup where grp.Title == item.GroupName select grp).FirstOrDefault();
                        if (currentGrp != null)
                        {
                            Console.WriteLine("Group " + item.GroupName + " exists. Deleting the group");
                            collGroup.Remove(currentGrp);
                        }
                        site.Update();
                        clientContext.Load(collGroup);
                        clientContext.ExecuteQuery();
                        Console.WriteLine("Successfully deleted group " + item.GroupName);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Function to add SharePoint groups at App Catalog
        /// </summary>
        /// <param name="listval">Configuration values from configuration Excel</param>
        /// <param name="groupData">Group details</param>
        private static void AddGroups(Dictionary<string, string> listval, List<DataStorage> groupData)
        {
            try
            {
                // Get the URL of site collection
                string login = listval["Username"]; // Get the user name
                string password = listval["Password"]; // Get the password

                foreach (DataStorage item in groupData)
                {
                    using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(item.SiteUrl, login, password ))
                    {
                        Web site = clientContext.Web;
                        GroupCollection collGroup = site.SiteGroups;
                        clientContext.Load(collGroup, groups => groups.Include(properties => properties.Title));
                        clientContext.Load(site.RoleDefinitions);
                        clientContext.ExecuteQuery();

                        //Check if group already exists and create
                        Group currentGrp = CheckAndCreateGroup(collGroup, item, site, clientContext);

                        //Assigning permission to the current group
                        AssignPermission(item, clientContext, site, currentGrp);

                        //Adding users to the current group
                        AddUsersToCurrentGroup(clientContext, site, currentGrp, item);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                DeleteGroups(listval, groupData);
            }
        }
                
        /// <summary>
        /// Method to assign permissions
        /// </summary>
        /// <param name="item">data storage object</param>
        /// <param name="clientContext">client context</param>
        /// <param name="site">site object</param>
        /// <param name="currentGrp">group object</param>
        private static void AssignPermission(DataStorage item, ClientContext clientContext, Web site, Group currentGrp)
        {
            Console.WriteLine("Assigning " + item.Permissions + " permission to group " + item.GroupName);
            RoleDefinitionBindingCollection grpRole = new RoleDefinitionBindingCollection(clientContext);
            RoleDefinition grpRoleDef = null;
            switch (item.Permissions.ToLower(new CultureInfo("en-US", false)))
            {
                case "contribute":
                    grpRoleDef = site.RoleDefinitions.GetByType(RoleType.Contributor);
                    break;
                case "fullcontrol":
                case "full control":
                    grpRoleDef = site.RoleDefinitions.GetByType(RoleType.Administrator);
                    break;
                case "read":
                default:
                    grpRoleDef = site.RoleDefinitions.GetByType(RoleType.Reader);
                    break;
            }
            grpRole.Add(grpRoleDef);
            site.RoleAssignments.Add(currentGrp, grpRole);
            clientContext.Load(currentGrp);
            clientContext.Load(grpRole);
            clientContext.ExecuteQuery();
            Console.WriteLine("Successfully assigned " + item.Permissions + " to group " + item.GroupName);
        }
        
        /// <summary>
        /// Method to add users to current group
        /// </summary>
        /// <param name="clientContext">client context</param>
        /// <param name="site">site object</param>
        /// <param name="currentGrp">group object</param>
        /// <param name="item">data storage</param>
        private static void AddUsersToCurrentGroup(ClientContext clientContext, Web site, Group currentGrp, DataStorage item)
        {
            string[] allUserEmail = item.Members.Split(new char[] { ';' });

            Console.WriteLine("Adding users to this group");
            List<User> allUsers = new List<User>();
            foreach (string userEmail in allUserEmail)
            {
                if (!string.IsNullOrEmpty(userEmail))
                {
                    User user = clientContext.Web.EnsureUser(userEmail.Trim());
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();
                    allUsers.Add(user);
                }
            }
            foreach (User user in allUsers)
            {
                currentGrp.Users.AddUser(user);
            }
            site.Update();
            clientContext.Load(currentGrp);
            clientContext.ExecuteQuery();
            Console.WriteLine("Successfully added users to group " + currentGrp.Title);
        }
                
        /// <summary>
        /// Method to check if a group exists and create a new one
        /// </summary>
        /// <param name="collGroup">group collection</param>
        /// <param name="item">data storage</param>
        /// <param name="site">site object</param>
        /// <param name="clientContext">client context</param>
        /// <returns>returns group object</returns>
        private static Group CheckAndCreateGroup(GroupCollection collGroup, DataStorage item, Web site, ClientContext clientContext)
        {
            Group currentGrp = (from grp in collGroup where grp.Title == item.GroupName select grp).FirstOrDefault();
            if (currentGrp != null)
            {
                Console.WriteLine("Deleting group " + item.GroupName + " as it is already present");
                collGroup.Remove(currentGrp);
            }

            //Creating group
            Console.WriteLine("Creating group " + item.GroupName);
            GroupCreationInformation grpInfo = new GroupCreationInformation();
            grpInfo.Title = item.GroupName;
            grpInfo.Description = item.GroupDesc;
            collGroup.Add(grpInfo);
            site.Update();
            clientContext.Load(collGroup);
            clientContext.ExecuteQuery();
            Console.WriteLine("Successfully created group " + item.GroupName);
            currentGrp = (from grp in collGroup where grp.Title == item.GroupName select grp).FirstOrDefault();
            return currentGrp;
        }

        /// <summary>
        /// Method to read group config
        /// </summary>
        /// <param name="sheetValues">collection object</param>
        /// <param name="listval">dictionary object</param>
        /// <returns>list object</returns>
        private static List<DataStorage> ReadGroupConfig(Collection<Collection<string>> sheetValues, Dictionary<string, string> listval)
        {
            List<DataStorage> groupsData = new List<DataStorage>();
            int counter = 0;
            try
            {
                foreach (var row in sheetValues)
                {
                    if (counter == 0)
                    {
                        counter = 1;
                        continue;
                    }
                    DataStorage termInfoTuple = new DataStorage();
                    termInfoTuple.SiteUrl = Convert.ToString(listval["CatalogSiteURL"], CultureInfo.InvariantCulture);
                    termInfoTuple.GroupName = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                    termInfoTuple.GroupDesc = Convert.ToString(row[1], CultureInfo.InvariantCulture);
                    termInfoTuple.Permissions = Convert.ToString(row[2], CultureInfo.InvariantCulture);
                    termInfoTuple.Members = Convert.ToString(row[3], CultureInfo.InvariantCulture);
                    groupsData.Add(termInfoTuple);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception Details: " + exception.Message);
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception Details: " + exception.Message);
            }
            return groupsData;
        }
    }
}
