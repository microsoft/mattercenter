// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ConfigureLists
// Author           : v-akdigh
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="ConfigureLists.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides functions to create lists on SharePoint</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ConfigureLists
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
    #endregion

    /// <summary>
    /// A class to configure lists
    /// </summary>
    public static class ConfigureList
    {
        /// <summary>
        /// Main method - Start of the program
        /// </summary>
        /// <param name="args">Input from console</param>
        public static void Main(string[] args)
        {
            bool revert = false;
            string login, password;
            if (null != args && 2 <= args.Length)
            {
                revert = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture);
                login = args[1];
                password = args[2];

                if (!ExcelOperations.IsNullOrEmptyCredential(login, password))
                {
                    Console.WriteLine("Reading inputs from Excel...");
                    string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + ConfigurationManager.AppSettings["filename"];
                    string sheetName = ConfigurationManager.AppSettings["sheetname"];
                    Collection<Collection<string>> groupSheetValues = ExcelOperations.ReadSheet(filePath, ConfigurationManager.AppSettings["groupsheetname"]);
                    string groupName = Convert.ToString(groupSheetValues[1][0], CultureInfo.InvariantCulture);
                    string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorLogFile"];
                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                    if (listval.Count() > 0)
                    {
                        string targetSite = listval["CatalogSiteURL"]; // Get the URL of site collection
                        bool isDeployedOnAzure = Convert.ToBoolean(listval["IsDeployedOnAzure"].ToUpperInvariant(), CultureInfo.InvariantCulture); // Get Is Deployed on Azure parameter                                  
                        using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password, isDeployedOnAzure))
                        {
                            int listCount = 0;
                            try
                            {
                                listCount = Convert.ToInt32(ConfigurationManager.AppSettings["ListCount"], CultureInfo.InvariantCulture); // Get the total number of lists to be created
                                RevertCreatedList(clientContext, listCount, errorFilePath);
                                RemovePermissionLevelIfExists(clientContext, ConfigurationManager.AppSettings["MatterCenterContributePermission"]);
                                if (!revert)
                                {
                                    CreateSharePointList(clientContext, listCount, groupName); // Create SharePoint List
                                    AddRoleDetails(clientContext);
                                }
                            }
                            catch (Exception exception)
                            {
                                RevertCreatedList(clientContext, listCount, errorFilePath);
                                RemovePermissionLevelIfExists(clientContext, ConfigurationManager.AppSettings["MatterCenterContributePermission"]);
                                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: No inputs found");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Credentials.");
                }
            }
        }

        /// <summary>
        /// A method to delete a list, if it exists
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listCount">Count of lists</param>
        /// <param name="errorFilePath">File path for logging error</param>
        private static void RevertCreatedList(ClientContext clientContext, int listCount, string errorFilePath)
        {
            try
            {
                Console.WriteLine("Checking if lists already exists...");
                Web web = clientContext.Web;
                clientContext.Load(web.Lists);
                clientContext.ExecuteQuery();
                for (int iIterator = 0; iIterator < listCount; iIterator++)   // For each list
                {
                    string listName = ConfigurationManager.AppSettings["ListName" + iIterator];
                    List isList = web.Lists.Cast<List>().FirstOrDefault(list => list.Title == listName);  // If list already present, delete the list
                    if (null != isList)
                    {
                        Console.WriteLine("Deleting list " + listName + "...");
                        isList.DeleteObject();
                    }
                }
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Method to create list in site collection
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listCount">Total number of lists to be created</param>
        /// <param name="groupName">Name of group for which permissions are to be assigned</param>
        private static void CreateSharePointList(ClientContext clientContext, int listCount, string groupName)
        {
            Console.WriteLine("Creating Config lists...");
            Web web = clientContext.Web;
            for (int iIterator = 0; iIterator < listCount; iIterator++)   // For each list
            {
                string listName = ConfigurationManager.AppSettings["ListName" + iIterator];    // Retrieve the list details
                string listColumn = ConfigurationManager.AppSettings["ColumnList" + iIterator];
                string listColumnDataType = ConfigurationManager.AppSettings["DataType" + iIterator];
                string description = ConfigurationManager.AppSettings["Description" + iIterator];
                string[] listColumns = listColumn.Split(new string[] { "$|$" }, StringSplitOptions.RemoveEmptyEntries);
                string[] listColumnDataTypes = listColumnDataType.Split(new string[] { "$|$" }, StringSplitOptions.RemoveEmptyEntries);
                string[] displayHiddenLists = ConfigurationManager.AppSettings["DisplayHiddenLists"].Split(new string[] { "$|$" }, StringSplitOptions.RemoveEmptyEntries);

                ListCreationInformation creationInfo = new ListCreationInformation(); // Create list
                creationInfo.Title = listName;
                creationInfo.Description = description;
                creationInfo.TemplateType = (int)ListTemplateType.GenericList;
                List list = web.Lists.Add(creationInfo); // Add the list
                for (int iCount = 0; iCount < listColumns.Length; iCount++) // Create columns inside list
                {
                    Field column = list.Fields.AddFieldAsXml("<Field Type='" + listColumnDataTypes[iCount] + "' DisplayName='" + listColumns[iCount] + "' Name='" + listColumns[iCount] + "' UserSelectionMode='0' Mult='TRUE' />", true, AddFieldOptions.AddFieldToDefaultView);
                    int pos = Array.IndexOf(displayHiddenLists, listName);
                    if (-1 < pos)
                    {
                        column.SetShowInEditForm(true);
                        column.SetShowInNewForm(true);
                    }
                    else
                    {
                        column.SetShowInEditForm(false);
                        column.SetShowInNewForm(false);
                    }
                    list.Hidden = true;
                    list.OnQuickLaunch = false;
                    list.Update();
                    if (string.Equals(listColumnDataTypes[iCount], "Boolean"))
                    {
                        column.DefaultValue = "False";   // Set the default value of Mandatory field as No in MatterCenterRoles list
                    }
                }
                Field fldTitle = list.Fields.GetByTitle("Title");
                fldTitle.Required = false;  // Make the Title column of the list as Not Required
                fldTitle.Update();
                clientContext.Load(list);
            }
            Group matterCenterGroup = web.SiteGroups.GetByName(groupName);
            clientContext.Load(matterCenterGroup);
            clientContext.ExecuteQuery();  // Execute the Client Context
            Console.WriteLine("Successfully created config lists");

            string permissionLevel = ConfigurationManager.AppSettings["MatterCenterContributePermission"];
            string permissionLevelDescription = ConfigurationManager.AppSettings["MatterCenterContributePermissionDescription"];
            string[] customPermissionLists = { ConfigurationManager.AppSettings["ListName0"], ConfigurationManager.AppSettings["ListName1"] };
            bool newPermissionExists = CreateNewPermissionLevel(clientContext, permissionLevel, permissionLevelDescription);
            bool isCustomPermission = false;

            for (int iIterator = 0; iIterator < listCount; iIterator++)   // For each list
            {
                string listName = ConfigurationManager.AppSettings["ListName" + iIterator];
                List list = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(list, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();

                isCustomPermission = (0 < customPermissionLists.Count() && customPermissionLists.Contains(listName) && newPermissionExists) ? true : false;

                if (AssignPermission(clientContext, matterCenterGroup, list, listName, isCustomPermission))
                {
                    Console.WriteLine("Assigned Permissions for " + listName + " list");
                }
                else
                {
                    Console.WriteLine("Failed to assign Permissions for " + listName + " list");
                }
            }
            AddValuesToChoiceColumn(clientContext);
            CreateLookUpField(clientContext);
        }

        /// <summary>
        /// Creates a new permission level in specified web context
        /// </summary>
        /// <param name="context">Client Context</param>
        /// <param name="permissionLevelName">Name of permission level to be created</param>
        /// <param name="description">Description of permission level to be created</param>
        /// <returns>Boolean value indicating success of permission level creation</returns>
        private static bool CreateNewPermissionLevel(ClientContext context, string permissionLevelName, string description)
        {
            string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "/" + "ErrorLog.txt";
            bool isPermissionLevelCreated = false;
            try
            {
                RemovePermissionLevelIfExists(context, permissionLevelName);

                BasePermissions permissions = new BasePermissions();

                // List Permissions
                permissions.Set(PermissionKind.AddListItems);
                permissions.Set(PermissionKind.EditListItems);
                permissions.Set(PermissionKind.DeleteListItems);
                permissions.Set(PermissionKind.ViewListItems);
                permissions.Set(PermissionKind.OpenItems);
                permissions.Set(PermissionKind.ViewVersions);
                permissions.Set(PermissionKind.DeleteVersions);
                permissions.Set(PermissionKind.CreateAlerts);

                // Site Permissions
                permissions.Set(PermissionKind.BrowseDirectories);
                permissions.Set(PermissionKind.CreateSSCSite);
                permissions.Set(PermissionKind.BrowseDirectories);
                permissions.Set(PermissionKind.ViewPages);
                permissions.Set(PermissionKind.BrowseUserInfo);
                permissions.Set(PermissionKind.UseRemoteAPIs);
                permissions.Set(PermissionKind.UseClientIntegration);
                permissions.Set(PermissionKind.Open);
                permissions.Set(PermissionKind.EditMyUserInfo);

                // Personal Permissions
                permissions.Set(PermissionKind.ManagePersonalViews);
                permissions.Set(PermissionKind.AddDelPrivateWebParts);
                permissions.Set(PermissionKind.UpdatePersonalWebParts);

                // Extra Permissions            
                permissions.Set(PermissionKind.ManagePermissions);
                permissions.Set(PermissionKind.EnumeratePermissions);

                RoleDefinitionCreationInformation roleDefinitionCreationInfo = new RoleDefinitionCreationInformation();
                roleDefinitionCreationInfo.BasePermissions = permissions;
                roleDefinitionCreationInfo.Name = permissionLevelName;
                roleDefinitionCreationInfo.Description = description;
                context.Web.RoleDefinitions.Add(roleDefinitionCreationInfo);
                context.Web.Update();
                context.ExecuteQuery();
                isPermissionLevelCreated = true;
                Console.WriteLine("New permission level created [{0}]", permissionLevelName);
            }
            catch (Exception exception)
            {
                isPermissionLevelCreated = false;
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                RemovePermissionLevelIfExists(context, permissionLevelName);
            }
            return isPermissionLevelCreated;
        }

        /// <summary>
        /// Removes Permission level if exists in web context
        /// </summary>
        /// <param name="context">Client context</param>
        /// <param name="permissionLevelName">Permission level name to remove</param>
        private static void RemovePermissionLevelIfExists(ClientContext context, string permissionLevelName)
        {
            Web web = context.Web;
            context.Load(web, items => items.RoleDefinitions);
            context.ExecuteQuery();
            RoleDefinition role = web.RoleDefinitions.Where(item => item.Name == permissionLevelName).FirstOrDefault();
            try
            {
                if (null != role)
                {
                    context.Load(role);
                    context.ExecuteQuery();
                    role.DeleteObject();
                    context.ExecuteQuery();
                    Console.WriteLine("Permission Level [{0}] Removed", permissionLevelName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Assigns permission to group
        /// </summary>
        /// <param name="clientcontext">Client Context</param>
        /// <param name="matterCenterGroup">Group for which permission is to be assigned</param>
        /// <param name="list">List at which permission is to be assigned</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="isCustomPermission">Custom permission flag</param>
        /// <returns>Status of operation</returns>
        private static bool AssignPermission(ClientContext clientcontext, Group matterCenterGroup, List list, string listName, bool isCustomPermission)
        {
            string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "/" + "ErrorLog.txt";
            bool result = false;
            try
            {
                if (null != clientcontext && null != list && null != matterCenterGroup)
                {
                    // first breaking permissions 
                    Console.WriteLine("Breaking Permissions for " + listName + " list...");
                    if (!list.HasUniqueRoleAssignments)
                    {
                        list.BreakRoleInheritance(true, true);
                        list.Update();
                        clientcontext.Load(list);
                        clientcontext.ExecuteQuery();
                    }

                    //// assigning permissions
                    Principal principal = matterCenterGroup;
                    RoleDefinition role = null;
                    int permission = Convert.ToInt32(ConfigurationManager.AppSettings["Permission"], CultureInfo.InvariantCulture);
                    permission = isCustomPermission ? 3 : permission;
                    switch (permission)
                    {
                        case 0:
                            role = clientcontext.Web.RoleDefinitions.GetByType(RoleType.Contributor);
                            break;
                        case 1:
                            role = clientcontext.Web.RoleDefinitions.GetByType(RoleType.Administrator);
                            break;
                        case 3:
                            string permissionLevel = ConfigurationManager.AppSettings["MatterCenterContributePermission"];
                            RoleDefinitionCollection roles = clientcontext.Web.RoleDefinitions;
                            clientcontext.Load(roles);
                            clientcontext.ExecuteQuery();
                            role = roles.Where(item => item.Name == permissionLevel).FirstOrDefault();
                            break;
                        case 2:
                        default:
                            role = clientcontext.Web.RoleDefinitions.GetByType(RoleType.Reader);
                            break;
                    }

                    RoleDefinitionBindingCollection grpRole = new RoleDefinitionBindingCollection(clientcontext);
                    if (null != role)
                    {
                        grpRole.Add(role);
                    }
                    list.RoleAssignments.Add(principal, grpRole);
                    list.Update();
                    clientcontext.ExecuteQuery();
                    result = true;
                }
            }
            catch (Exception exception)
            {
                result = false;
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Function to include Role details into Roles List
        /// </summary>
        /// <param name="clientContext">Client context</param>
        private static void AddRoleDetails(ClientContext clientContext)
        {
            Console.WriteLine("Adding Role details...");
            string listName = ConfigurationManager.AppSettings["RoleListName"];    // Retrieve the list details
            string roleNames = ConfigurationManager.AppSettings["RoleName"];
            string mandatory = ConfigurationManager.AppSettings["Mandatory"];
            string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + "/" + "ErrorLog.txt";

            if (!(string.IsNullOrEmpty(listName) || string.IsNullOrEmpty(roleNames) || string.IsNullOrEmpty(mandatory)))
            {
                string[] listColumns = roleNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] mandatoryColumnValues = mandatory.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                List oList = clientContext.Web.Lists.GetByTitle(listName);

                for (int iCount = 0; iCount < listColumns.Length; iCount++)
                {
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem oListItem = oList.AddItem(itemCreateInfo);
                    oListItem["RoleName"] = listColumns[iCount];
                    oListItem["Mandatory"] = mandatoryColumnValues[iCount];
                    oListItem.Update();
                }
                clientContext.ExecuteQuery();
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: No inputs found");
            }
        }

        /// <summary>
        /// Creates choice field for contextual help list
        /// </summary>
        /// <param name="clientcontext">Client Context</param>
        private static void AddValuesToChoiceColumn(ClientContext clientcontext)
        {
            // Basic code to load the list
            Web myWeb = clientcontext.Web;
            clientcontext.Load(myWeb);
            List myList = myWeb.Lists.GetByTitle(ConfigurationManager.AppSettings["ContextualHelpSectionListName"]);
            FieldCollection allFields = myList.Fields;
            clientcontext.Load(allFields);
            clientcontext.ExecuteQuery();
            //The field must be cast to a FieldChoice using context.CastTo

            for (int iIterator = 0; iIterator < Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfColumns"], CultureInfo.InvariantCulture); iIterator++)   // For each list
            {
                string columnName = ConfigurationManager.AppSettings["ChoiceFieldColumnName" + iIterator];
                string choiceFields = ConfigurationManager.AppSettings["ChoiceFieldValues" + iIterator];
                string choiceFieldValue = string.Empty;

                string[] listChioceFields = choiceFields.Split(new string[] { "$|$" }, StringSplitOptions.RemoveEmptyEntries);
                for (int iCount = 0; iCount < listChioceFields.Length; iCount++) // Create columns inside list
                {
                    choiceFieldValue = string.Format(CultureInfo.InvariantCulture, "{0}<CHOICE>{1}</CHOICE>", choiceFieldValue, listChioceFields[iCount]);
                }
                choiceFieldValue = string.Format(CultureInfo.InvariantCulture, "<CHOICES>{0}</CHOICES>", choiceFieldValue);

                string choiceFieldDefaultvalue = string.Format(CultureInfo.InvariantCulture, "<Default>{0}</Default>", ConfigurationManager.AppSettings["DefaultChoiceField" + iIterator]);
                string choicefieldXml = "<Field Type='Choice' DisplayName='{0}' Required='FALSE' EnforceUniqueValues='FALSE' Format='Dropdown' FillInChoice='FALSE'> {1}{2}</Field>";
                Field choiceField = myList.Fields.AddFieldAsXml(string.Format(CultureInfo.InvariantCulture, choicefieldXml, columnName, choiceFieldDefaultvalue, choiceFieldValue), true, AddFieldOptions.DefaultValue);
                FieldNumber fldNumberTaskStatus = clientcontext.CastTo<FieldNumber>(choiceField);
                fldNumberTaskStatus.Update();
                clientcontext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Creates lookup field for contextual help list
        /// </summary>
        /// <param name="clientcontext">Client Context</param>
        private static void CreateLookUpField(ClientContext clientcontext)
        {
            string sourceList = ConfigurationManager.AppSettings["ContextualHelpList1"];
            string lookupList = ConfigurationManager.AppSettings["ContextualHelpList2"];
            string lookupColumnName = ConfigurationManager.AppSettings["ContextualHelpLookUpColumnName"];
            List objSourceList = clientcontext.Web.Lists.GetByTitle(sourceList);
            List objLookupList = clientcontext.Web.Lists.GetByTitle(lookupList);
            clientcontext.Load(objSourceList);
            clientcontext.Load(objLookupList);
            clientcontext.ExecuteQuery();

            Field lookupFieldXML = objLookupList.Fields.AddFieldAsXml("<Field Type='" + "Lookup" + "' DisplayName='" + lookupColumnName + "' Name='" + lookupColumnName + "' />", true, AddFieldOptions.AddFieldToDefaultView);
            FieldLookup lookupField = clientcontext.CastTo<FieldLookup>(lookupFieldXML);
            lookupField.LookupList = objSourceList.Id.ToString();
            lookupField.LookupField = lookupColumnName;
            lookupFieldXML.Update();
            clientcontext.ExecuteQuery();
        }
    }
}
