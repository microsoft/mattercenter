// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateLists
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="UpdateLists.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to update lists in project.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateLists
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

    #endregion using

    /// <summary>
    /// Class is used to update lists in project.
    /// </summary>
    internal class UpdateLists
    {   
        /// <summary>
        /// String variable to specify the error file path
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["ErrorFilePath"];

        /// <summary>
        /// Main method - Start of the program
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            if (2 == args.Length && !ExcelOperations.IsNullOrEmptyCredential(args[0], args[1]))
            {
                Console.Title = "Update List Permission";
                ErrorMessage.ShowMessage(ConfigurationManager.AppSettings["ExcelMessage"], ErrorMessage.MessageType.Success);
                try
                {
                    string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                    string sheetName = ConfigurationManager.AppSettings["SheetName"];
                    Collection<Collection<string>> groupSheetValues = ExcelOperations.ReadSheet(filePath, ConfigurationManager.AppSettings["GroupSheetName"]);
                    string groupName = Convert.ToString(groupSheetValues[1][0], CultureInfo.InvariantCulture);
                    string username = args[0].Trim();
                    string password = args[1].Trim();
                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                    if (listval.Count() > 0)
                    {
                        // Get Is Deployed on Azure parameter
                        bool isDeployedOnAzure = Convert.ToBoolean(listval["IsDeployedOnAzure"].ToUpperInvariant(), CultureInfo.InvariantCulture);
                        // Get the Client Context
                        using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(listval["CatalogSiteURL"], username, password, isDeployedOnAzure))  
                        {
                            try
                            {
                                UpdateListPermissions(groupName, clientContext);
                                ErrorMessage.ShowMessage(ConfigurationManager.AppSettings["ListSuccess"], ErrorMessage.MessageType.Success);
                            }
                            catch (Exception exception)
                            {
                                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ListFailure"], errorFilePath), ErrorMessage.MessageType.Error);
                            }
                        }
                    }
                    else
                    {
                        ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: No inputs found");
                    }
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, string.Concat(exception.Message, "\n", exception.StackTrace));
                }
            }
            else
            {
                ErrorMessage.ShowMessage(ConfigurationManager.AppSettings["Invalidcredential"], ErrorMessage.MessageType.Error);
            }
        }

        /// <summary>
        /// Updates permissions on existing lists
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="clientContext">Client context</param>
        private static void UpdateListPermissions(string groupName, ClientContext clientContext)
        {
            try
            {
                string permissionLevelName = ConfigurationManager.AppSettings["MatterCenterContributePermission"];
                string permissionLevelDescription = ConfigurationManager.AppSettings["MatterCenterContributePermissionDescription"];
                if (CreateNewPermissionLevel(clientContext, permissionLevelName, permissionLevelDescription))
                {
                    ErrorMessage.ShowMessage(ConfigurationManager.AppSettings["UpdateList"], ErrorMessage.MessageType.Notification);
                    string[] lists = ConfigurationManager.AppSettings["ListTitles"].Split(',');
                    int listCount = lists.Count();
                    for (int iterator = 0; iterator < listCount; iterator++)
                    {
                        string listName = lists[iterator];
                        List list = GetListIfExists(clientContext, listName);

                        // Check if list already exists
                        if (null != list)
                        {
                            if (!list.HasUniqueRoleAssignments)
                            {
                                list.BreakRoleInheritance(true, true);
                                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["RemovedPermission"], listName), ErrorMessage.MessageType.Notification);
                                clientContext.ExecuteQuery();
                            }

                            // remove group (Matter Center Users) from list
                            RoleAssignmentCollection collection = list.RoleAssignments;
                            clientContext.Load(collection, items => items.Include(item => item.Member.Title).Where(item => item.Member.Title == groupName));
                            clientContext.ExecuteQuery();

                            // Check if group is exists in list, then only remove it
                            if (0 < collection.Count())
                            {
                                collection.FirstOrDefault().DeleteObject();
                                clientContext.ExecuteQuery();
                                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["RemovedGroup"], groupName, listName), ErrorMessage.MessageType.Notification);
                            }
                            else
                            {
                                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["GroupFailure"], groupName, listName), ErrorMessage.MessageType.Notification);
                            }

                            // Assign new permission to group(Matter Center Users) & add that group to list

                            // 1. select new permission level
                            RoleDefinition matterCenterContribute = GetPermissionLevel(clientContext, permissionLevelName);

                            // 2. select Group (Matter Center Users)
                            RoleAssignment group = GetGroup(clientContext, groupName);

                            // 3. Check if permission level already added to group, if not then assign new permission level(Matter Center Contribute) to group
                            AssignPermissionLevelToGroup(clientContext, matterCenterContribute, group);

                            // 4. Add group into list
                            AddGroupToList(groupName, clientContext, list, matterCenterContribute);
                            ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["AddGroup"], group.Member.Title, listName), ErrorMessage.MessageType.Notification);

                            // 5. get list item and User who created it and assign full control to that user
                            AssignFullControltoListItem(clientContext, list);
                        }
                        else
                        {
                            ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ListNotFound"], listName), ErrorMessage.MessageType.Error);
                        }
                    }
                }
                else
                {
                    ErrorMessage.ShowMessage(ConfigurationManager.AppSettings["PermissionFailure"], ErrorMessage.MessageType.Error);
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, string.Concat(exception.Message, "\n", exception.StackTrace));
            }
        }

        /// <summary>
        /// Add a SharePoint group into list
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">List to which group added</param>
        /// <param name="matterCenterContribute">RoleDefinition/PermissionLevel that assigned to group</param>
        private static void AddGroupToList(string groupName, ClientContext clientContext, List list, RoleDefinition matterCenterContribute)
        {
            Group matterCenterGroup = clientContext.Web.SiteGroups.GetByName(groupName);
            clientContext.Load(matterCenterGroup);
            clientContext.ExecuteQuery();
            Principal MCGroup = matterCenterGroup;
            RoleDefinitionBindingCollection grpRole = new RoleDefinitionBindingCollection(clientContext);
            grpRole.Add(matterCenterContribute);
            list.RoleAssignments.Add(MCGroup, grpRole);
            list.Update();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Assign existing permission level to group
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="matterCenterContribute">Role to assign a group</param>
        /// <param name="group">RoleAssignment to which permission added</param>
        private static void AssignPermissionLevelToGroup(ClientContext clientContext, RoleDefinition matterCenterContribute, RoleAssignment group)
        {
            clientContext.Load(group.RoleDefinitionBindings, item => item.Include(items => items.Name, items => items.BasePermissions));
            clientContext.ExecuteQuery();

            RoleDefinitionBindingCollection rolesAssignedToGroup = group.RoleDefinitionBindings;
            clientContext.Load(rolesAssignedToGroup, role => role.Include(item => item.Name, item => item.BasePermissions));
            clientContext.ExecuteQuery();
            RoleDefinition roleDefinition = rolesAssignedToGroup.Select(item => item).Where(item => item.Name == matterCenterContribute.Name).FirstOrDefault();
            if (null == roleDefinition)
            {
                group.RoleDefinitionBindings.Add(matterCenterContribute);
                group.Update();
                clientContext.ExecuteQuery();
                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["PermissionAssign"], matterCenterContribute.Name, group.Member.Title), ErrorMessage.MessageType.Notification);
            }
        }

        /// <summary>
        /// Get user or group of specified name
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="groupName">Group or user name</param>
        /// <returns>Role Assignment</returns>
        private static RoleAssignment GetGroup(ClientContext clientContext, string groupName)
        {
            RoleAssignmentCollection roles = clientContext.Web.RoleAssignments;
            clientContext.Load(roles, item => item.Include(items => items.Member.Title));
            clientContext.ExecuteQuery();
            RoleAssignment group = roles.Where(role => role.Member.Title == groupName).FirstOrDefault();
            clientContext.Load(group, item => item.Member.Title);
            clientContext.ExecuteQuery();
            return group;
        }

        /// <summary>
        /// Get permissions of specified permission level
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="permissionLevelName">Name of permission level</param>
        /// <returns>RoleDefinition object that represents specified permission level</returns>
        private static RoleDefinition GetPermissionLevel(ClientContext clientContext, string permissionLevelName)
        {
            RoleDefinitionCollection roleDefCollection = clientContext.Web.RoleDefinitions;
            clientContext.Load(roleDefCollection, item => item);
            clientContext.ExecuteQuery();
            RoleDefinition matterCenterContribute = roleDefCollection.Where(item => item.Name == permissionLevelName).FirstOrDefault();
            return matterCenterContribute;
        }

        /// <summary>
        /// Assign Full control permission to items of specified list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">SharePoint List object to assign full control over list-items</param>
        private static void AssignFullControltoListItem(ClientContext clientContext, List list)
        {
            try
            {
                CamlQuery query = new CamlQuery();
                query.ViewXml = ConfigurationManager.AppSettings["ListQuery"];
                ListItemCollection listItems = list.GetItems(query);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                clientContext.Load(list, lst => lst.EntityTypeName);
                clientContext.ExecuteQuery();

                RoleDefinition fullControl = clientContext.Web.RoleDefinitions.GetByType(RoleType.Administrator);
                clientContext.Load(fullControl);
                clientContext.ExecuteQuery();
                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["ItemNotFound"], listItems.Count, list.EntityTypeName), ErrorMessage.MessageType.Notification);

                UpdateItems(clientContext, listItems, fullControl);
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, string.Concat(exception.Message, "\n", exception.StackTrace));
            }
        }

        /// <summary>
        /// Method to update items
        /// </summary>
        /// <param name="clientContext">client context</param>
        /// <param name="listItems">list item collection</param>
        /// <param name="fullControl">role definition</param>
        private static void UpdateItems(ClientContext clientContext, ListItemCollection listItems, RoleDefinition fullControl)
        {
            foreach (ListItem item in listItems)    // For each list item
            {
                User itemOwner = GetAuthor(clientContext, item);

                // 2. break permissions for list item
                item.BreakRoleInheritance(false, true);

                // 3. Assign full control to user on list item
                Principal user = itemOwner;
                RoleDefinitionBindingCollection roleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                roleDefinitionBinding.Add(fullControl);
                item.RoleAssignments.Add(user, roleDefinitionBinding);
                item.Update();
                clientContext.ExecuteQuery();
                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["PermissionUpdate"]), ErrorMessage.MessageType.Notification);
            }
        }

        /// <summary>
        /// Get author of list item
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="item">List item</param>
        /// <returns>User who created list item</returns>
        private static User GetAuthor(ClientContext clientContext, ListItem item)
        {
            FieldUserValue itemAuthor = (FieldUserValue)item.FieldValues["Author"];
            User itemOwner = clientContext.Web.GetUserById(itemAuthor.LookupId);
            clientContext.Load(itemOwner);
            clientContext.ExecuteQuery();
            return itemOwner;
        }

        /// <summary>
        /// Verifies and returns a list if exists
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name to retrieve</param>
        /// <returns>List object that represents SharePoint List</returns>
        private static List GetListIfExists(ClientContext clientContext, string listName)
        {
            List tempList = null;
            try
            {
                tempList = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(tempList, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                tempList = null;
            }
            return tempList;
        }

        /// <summary>
        /// Creates a new permission level in specified web context
        /// </summary>
        /// <param name="context">Client Context</param>
        /// <param name="permissionLevelName">Name of permission level to be created</param>
        /// <param name="description">Description of permission level to be created</param>
        /// <returns>Boolean value whether permission level created or not</returns>
        private static bool CreateNewPermissionLevel(ClientContext context, string permissionLevelName, string description)
        {
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
                Console.WriteLine();
                ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["NewPermission"], permissionLevelName), ErrorMessage.MessageType.Success);
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
        /// <param name="permissionLevelName">permission level name to remove</param>
        private static void RemovePermissionLevelIfExists(ClientContext context, string permissionLevelName)
        {
            try
            {
                Web web = context.Web;
                context.Load(web, items => items.RoleDefinitions);
                context.ExecuteQuery();
                RoleDefinition role = web.RoleDefinitions.Where(item => item.Name == permissionLevelName).FirstOrDefault();
                if (null != role)
                {
                    context.Load(role);
                    context.ExecuteQuery();
                    role.DeleteObject();
                    context.ExecuteQuery();
                    ErrorMessage.ShowMessage(string.Format(CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["PermissionLevelRemoved"], permissionLevelName), ErrorMessage.MessageType.Notification);
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }
    }
}