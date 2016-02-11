// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.DataLayer
// Author           : v-rijadh
// Created          : 06-16-2015
//
// ***********************************************************************
// <copyright file="Lists.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains methods related to SharePoint list object.</summary>
// ***********************************************************************

//// Keeping using System over here because of usage of CLSComplaint attribute for namespace
using System;
[assembly: CLSCompliant(false)]
namespace Microsoft.Legal.MatterCenter.DataLayer
{
    #region using
    using Microsoft.SharePoint.Client;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using IO = System.IO;
    #endregion

    /// <summary>
    /// Performs operation to update SharePoint list
    /// </summary>
    public static class Lists
    {
        /// <summary>
        /// Function to create document library for Matter and OneNote
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listInfo">List information</param>
        /// <returns>Success flag</returns>

        public static bool Create(ClientContext clientContext, ListInformation listInfo)
        {
            bool result = true;
            if (null != clientContext && null != listInfo && !string.IsNullOrWhiteSpace(listInfo.name))
            {
                Web web = clientContext.Web;
                ListTemplateCollection listTemplates = web.ListTemplates;
                ListCreationInformation creationInfo = new ListCreationInformation();
                creationInfo.Title = listInfo.name;
                creationInfo.Description = listInfo.description;
                // To determine changes in URL we specified below condition as this function is common
                if (!string.IsNullOrWhiteSpace(listInfo.Path))
                {
                    creationInfo.Url = listInfo.Path;
                }
                if (!string.IsNullOrWhiteSpace(listInfo.templateType))
                {
                    string templateType = listInfo.templateType;
                    clientContext.Load(listTemplates, item => item.Include(currentTemplate => currentTemplate.Name, currentTemplate => currentTemplate.ListTemplateTypeKind).Where(selectedTemplate => selectedTemplate.Name == templateType));
                    clientContext.ExecuteQuery();
                    if (null != listTemplates && 0 < listTemplates.Count)
                    {
                        creationInfo.TemplateType = listTemplates.FirstOrDefault().ListTemplateTypeKind;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
                }
                if (result)
                {
                    List list = web.Lists.Add(creationInfo);
                    list.ContentTypesEnabled = listInfo.isContentTypeEnable;
                    if (null != listInfo.folderNames && listInfo.folderNames.Count > 0)
                    {
                        list = Lists.AddFolders(clientContext, list, listInfo.folderNames);
                    }
                    if (null != listInfo.versioning)
                    {
                        list.EnableVersioning = listInfo.versioning.EnableVersioning;
                        list.EnableMinorVersions = listInfo.versioning.EnableMinorVersions;
                        list.ForceCheckout = listInfo.versioning.ForceCheckout;
                    }
                    list.Update();
                    clientContext.Load(list, l => l.DefaultViewUrl);
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the OneNote in List/Library
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="clientAddressPath">Client URL</param>
        /// <param name="oneNoteLocation">OneNote URL</param>
        /// <param name="oneNoteTitle">OneNote Title</param>
        /// <param name="listName">List/library name</param>
        /// <returns></returns>
        public static string AddOneNote(ClientContext clientContext, string clientAddressPath, string oneNoteLocation, string listName, string oneNoteTitle)
        {
            string returnValue = String.Empty;
            if (null != clientContext && !string.IsNullOrWhiteSpace(clientAddressPath) && !string.IsNullOrWhiteSpace(oneNoteLocation) && !string.IsNullOrWhiteSpace(listName))
            {
                Uri clientUrl = new Uri(clientAddressPath);
                string oneNotePath = HttpContext.Current.Server.MapPath(Constants.ONENOTERELATIVEFILEPATH);
                byte[] oneNoteFile = System.IO.File.ReadAllBytes(oneNotePath);
                Web web = clientContext.Web;
                Microsoft.SharePoint.Client.File file = web.GetFolderByServerRelativeUrl(oneNoteLocation).Files.Add(new FileCreationInformation()
                {
                    Url = string.Concat(listName, Constants.EXTENSIONONENOTETABLEOFCONTENT),
                    Overwrite = true,
                    ContentStream = new IO.MemoryStream(oneNoteFile)
                });
                web.Update();
                clientContext.Load(file);
                clientContext.ExecuteQuery();
                ListItem oneNote = file.ListItemAllFields;
                oneNote["Title"] = oneNoteTitle;
                oneNote.Update();
                returnValue = string.Concat(clientUrl.Scheme, Constants.COLON, Constants.FORWARDSLASH, Constants.FORWARDSLASH, clientUrl.Authority, file.ServerRelativeUrl, Constants.WEBSTRING);
            }
            return returnValue;
        }

        /// <summary>
        /// Adds all the folders from Content type in matter library.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">List of folders</param>
        /// <param name="folderNames">The folder names.</param>
        /// <returns>Microsoft SharePoint Client List</returns>
        public static List AddFolders(ClientContext clientContext, List list, IList<string> folderNames)
        {
            if (null != clientContext && null != list && null != folderNames)
            {
                FolderCollection listFolders = list.RootFolder.Folders;
                Folder listRootFolder = list.RootFolder;
                clientContext.Load(listFolders);
                clientContext.ExecuteQuery();
                if (0 < folderNames.Count)
                {
                    foreach (string folderName in folderNames)
                    {
                        // Check for empty folder names
                        if (!string.IsNullOrEmpty(folderName))
                        {
                            listFolders.Add(folderName);
                            listRootFolder.Update();
                        }
                    }
                    list.Update();
                }
            }
            return list;
        }

        /// <summary>
        /// Adds list item in specified list.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="list">Name of the list</param>
        /// <param name="columns">List of column names</param>
        /// <param name="values">Values for corresponding columns</param>
        /// <returns>String stating success flag</returns>
        public static bool AddItem(ClientContext clientContext, List list, IList<string> columns, IList<object> values)
        {
            bool result = false;
            if (null != clientContext && null != list && null != columns && null != values && columns.Count == values.Count)
            {
                // Add the Matter URL in list
                ListItemCreationInformation listItemCreateInfo = new ListItemCreationInformation();
                ListItem newListItem = list.AddItem(listItemCreateInfo);
                int position = 0;
                foreach (string column in columns)
                {
                    newListItem[column] = values[position++];
                }
                ///// Update the list
                newListItem.Update();
                clientContext.ExecuteQuery();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Breaks the permissions of the list.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="libraryName">Name of list</param>        
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>Success flag</returns>
        public static bool BreakPermission(ClientContext clientContext, string libraryName, bool isCopyRoleAssignment)
        {
            bool flag = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
            {
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (!list.HasUniqueRoleAssignments)
                    {
                        list.BreakRoleInheritance(isCopyRoleAssignment, true);
                        list.Update();
                        clientContext.Load(list);
                        clientContext.ExecuteQuery();
                        flag = true;
                    }
                }
                catch (Exception)
                {
                    throw; // This will transfer control to catch block of parent function.
                }
            }
            return flag;
        }

        /// <summary>
        /// Sets permissions for the list.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="AssignUserEmails">List of User emails to give permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <param name="listName">List name</param>
        /// <returns>String stating success flag</returns>
        public static bool SetPermission(ClientContext clientContext, IList<IList<string>> AssignUserEmails, IList<string> permissions, string listName)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ClientRuntimeContext clientRuntimeContext = clientContext;
                try
                {
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments && null != permissions && null != AssignUserEmails && permissions.Count == AssignUserEmails.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> assignUserEmails = AssignUserEmails[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != assignUserEmails)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in assignUserEmails)
                                {
                                    if (!string.IsNullOrWhiteSpace(user))
                                    {
                                        /////get the user object
                                        Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                        /////create the role definition binding collection
                                        RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                        /////add the role definition to the collection
                                        roleDefinitionBindingCollection.Add(roleDefinition);
                                        /////create a RoleAssigment with the user and role definition
                                        list.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                    }
                                }
                                /////execute the query to add everything
                                clientRuntimeContext.ExecuteQuery();
                            }
                            position++;
                        }
                        ///// Success. Return a success code
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Validates and breaks the item level permission for the specified list item under the list/library. 
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="isCopyRoleAssignment">Flag to copy permission from parent</param>
        /// <returns>String stating success flag</returns>
        public static bool BreakItemPermission(ClientContext clientContext, string listName, int listItemId, bool isCopyRoleAssignment)
        {
            bool result = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();

                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(isCopyRoleAssignment, true);
                    listItem.Update();
                    clientContext.ExecuteQuery();
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Set permission to the specified list item 
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="AssignUserEmails">User emails to give permission</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <returns>Status of the unique item level permission assignment operation</returns>
        public static bool SetItemPermission(ClientContext clientContext, IList<IList<string>> AssignUserEmails, string listName, int listItemId, IList<string> permissions)
        {
            bool result = false;
            if (null != clientContext)
            {
                ClientRuntimeContext clientRuntimeContext = clientContext;
                ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                clientContext.ExecuteQuery();
                if (listItem.HasUniqueRoleAssignments && null != permissions && null != AssignUserEmails && permissions.Count == AssignUserEmails.Count)
                {
                    int position = 0;
                    foreach (string roleName in permissions)
                    {
                        IList<string> assignUserEmails = AssignUserEmails[position];
                        if (!string.IsNullOrWhiteSpace(roleName) && null != assignUserEmails)
                        {
                            RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                            foreach (string user in assignUserEmails)
                            {

                                if (!string.IsNullOrWhiteSpace(user))
                                {
                                    /////get the user object
                                    Principal userPrincipal = clientContext.Web.EnsureUser(user.Trim());
                                    /////create the role definition binding collection
                                    RoleDefinitionBindingCollection roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(clientRuntimeContext);
                                    /////add the role definition to the collection
                                    roleDefinitionBindingCollection.Add(roleDefinition);
                                    /////create a RoleAssigment with the user and role definition
                                    listItem.RoleAssignments.Add(userPrincipal, roleDefinitionBindingCollection);
                                }
                            }
                            /////execute the query to add everything
                            clientRuntimeContext.ExecuteQuery();
                        }
                        position++;
                    }
                    ///// Success. Return a success code
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the list items of specified list based on CAML query.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public static ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null)
        {
            ListItemCollection listItemCollection = null;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                try
                {
                    CamlQuery query = new CamlQuery();
                    if (!string.IsNullOrWhiteSpace(camlQuery))
                    {
                        query.ViewXml = camlQuery;
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(query);
                    }
                    else
                    {
                        listItemCollection = clientContext.Web.Lists.GetByTitle(listName).GetItems(CamlQuery.CreateAllItemsQuery());
                    }
                    clientContext.Load(listItemCollection);
                    clientContext.ExecuteQuery();
                }
                catch (Exception)
                {
                    listItemCollection = null;
                    throw;
                }
            }
            return listItemCollection;
        }

        /// <summary>
        ///  Creates a new view for the list
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="matterList">List name</param>
        /// <param name="viewColumnList">Name of the columns in view</param>
        /// <param name="viewName">View name</param>
        /// <param name="strQuery">View query</param>
        /// <returns>String stating success flag</returns>
        public static bool AddView(ClientContext clientContext, List matterList, string[] viewColumnList, string viewName, string strQuery)
        {
            bool result = true;
            if (null != clientContext && null != matterList && null != viewColumnList && !string.IsNullOrWhiteSpace(viewName) && !string.IsNullOrWhiteSpace(strQuery))
                try
                {
                    View outlookView = matterList.Views.Add(new ViewCreationInformation
                    {
                        Title = viewName,
                        ViewTypeKind = ViewType.Html,
                        ViewFields = viewColumnList,
                        Paged = true
                    });
                    outlookView.ViewQuery = strQuery;
                    outlookView.Update();
                    clientContext.ExecuteQuery();
                }
                catch (Exception)
                {
                    result = false;
                }
            return result;
        }

        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public static List<string> Exists(ClientContext clientContext, ReadOnlyCollection<string> listsNames)
        {
            List<string> existingLists = new List<string>();
            if (null != clientContext && null != listsNames)
            {
                ListCollection lists = clientContext.Web.Lists;
                clientContext.Load(lists);
                clientContext.ExecuteQuery();
                existingLists = (from listName in listsNames
                                 join item in lists
                                 on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                 select listName).ToList();
            }
            return existingLists;
        }

        /// <summary>
        /// Function to delete the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public static bool Delete(ClientContext clientContext, IList<string> listsNames)
        {
            bool result = false;
            if (null != clientContext && null != listsNames)
            {
                ListCollection allLists = clientContext.Web.Lists;
                clientContext.Load(allLists);
                clientContext.ExecuteQuery();
                foreach (string listName in listsNames)
                {
                    List list = allLists.Cast<List>().FirstOrDefault(item => item.Title.ToUpperInvariant().Equals(listName.ToUpperInvariant()));
                    if (null != list)
                    {
                        result = true;
                        list.DeleteObject();
                    }

                }
                clientContext.ExecuteQuery();
            }
            return result;
        }

        /// <summary>
        /// Retrieves the list item ID.
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="libraryName">Library name</param>
        /// <param name="pageName">Page name</param>      
        /// <returns>List item id</returns>
        public static int RetrieveItemId(ClientContext clientContext, string libraryName, string pageName)
        {
            int listItemId = -1;
            if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName) && !string.IsNullOrWhiteSpace(pageName))
            {
                ListItemCollection listItemCollection = GetData(clientContext, libraryName);
                clientContext.Load(listItemCollection, listItemCollectionProperties => listItemCollectionProperties.Include(listItemProperties => listItemProperties.Id, listItemProperties => listItemProperties.DisplayName));
                clientContext.ExecuteQuery();

                ListItem listItem = listItemCollection.Cast<ListItem>().FirstOrDefault(listItemProperties => listItemProperties.DisplayName.ToUpper(CultureInfo.InvariantCulture).Equals(pageName.ToUpper(CultureInfo.InvariantCulture)));

                if (null != listItem)
                {
                    listItemId = listItem.Id;
                }
            }
            return listItemId;
        }

        /// <summary>
        /// To check if the folder is present in the list
        /// </summary>
        /// <param name="clientContext">Client context object</param>      
        /// <param name="folderPath">Folder path</param>
        /// <param name="documentLibraryName">Document library name</param>          
        /// <returns>true or false</returns>
        public static bool CheckFolderPresent(ClientContext clientContext, string folderPath, string documentLibraryName)
        {
            bool folderFound = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(documentLibraryName) && !string.IsNullOrWhiteSpace(folderPath))
            {
                List list = clientContext.Web.Lists.GetByTitle(documentLibraryName);
                ListItemCollection folderList = list.GetItems(CamlQuery.CreateAllFoldersQuery());
                clientContext.Load(folderList, item => item.Include(items => items.Folder.Name, items => items.Folder.ServerRelativeUrl).Where(items => items.Folder.ServerRelativeUrl == folderPath));
                clientContext.ExecuteQuery();
                folderFound = 0 < folderList.Count;
            }
            return folderFound;
        }

        /// <summary>
        /// Returns stream data of the file.
        /// </summary>
        /// <param name="clientContext">ClientContext object</param>
        /// <param name="documentUrl">Document URL of the document whose stream is to be retrieved</param>
        /// <returns>Stream of the file</returns>
        public static ClientResult<System.IO.Stream> GetStreamFromFile(ClientContext clientContext, string documentUrl)
        {
            ClientResult<System.IO.Stream> data = null;
            if (null != clientContext && !string.IsNullOrWhiteSpace(documentUrl))
            {
                File file = clientContext.Web.GetFileByServerRelativeUrl(documentUrl);
                data = file.OpenBinaryStream();
                // Load the Stream data for the file
                clientContext.Load(file);
                clientContext.ExecuteQuery();
            }
            return data;
        }

        /// <summary>
        /// Checks if item is modified after it is loaded on the client side
        /// </summary>        
        /// <param name="collection">List item collection</param>
        /// <param name="cachedItemModifiedDate">Date time when current user loaded the page to see/update configuration values.</param>
        /// <returns>Success flag</returns>
        public static bool CheckItemModified(ListItemCollection collection, string cachedItemModifiedDate)
        {
            bool response = false;
            int errorModifiedDate = 0;  // If there is new list item being created then 'cachedItemModifiedDate' will be 0
            if (null != collection && !string.IsNullOrWhiteSpace(cachedItemModifiedDate))
            {
                if (String.Equals(Convert.ToString(errorModifiedDate, CultureInfo.InvariantCulture), cachedItemModifiedDate) && collection.Count.Equals(0)) // Verify if new item flag is true and no list item is present in the Matter Configuration list
                {
                    response = true;
                }
                else if (0 < collection.Count)
                {
                    ListItem settingsListItem = collection.FirstOrDefault();
                    DateTime cachedDate;
                    if (DateTime.TryParse(cachedItemModifiedDate, out cachedDate))
                    {
                        DateTime itemModifiedDate = Convert.ToDateTime(settingsListItem[Constants.MODIFIED_DATE_COLUMN], CultureInfo.InvariantCulture);
                        if (0 == DateTime.Compare(cachedDate, itemModifiedDate))
                        {
                            response = true;
                        }
                    }

                }
            }
            return response;
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property Bag</param>
        /// <param name="matterName">Name of matter to which property is to be attached</param>
        /// <param name="propertyList">List of properties</param>
        public static void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            if (null != clientContext && !string.IsNullOrWhiteSpace(matterName) && null != props && null != propertyList)
            {
                List list = clientContext.Web.Lists.GetByTitle(matterName);

                foreach (var item in propertyList)
                {
                    props[item.Key] = item.Value;
                    list.Update();
                }

                clientContext.ExecuteQuery();
            }
        }

        /// <summary>
        /// Fetches the values of property for specified matter.
        /// </summary>
        /// <param name="context">Client context</param>
        /// <param name="matterName">Name of matter</param>
        /// <param name="propertyList">List of properties</param>
        /// <returns>Property list stamped to the matter</returns>
        public static string GetPropertyValueForList(ClientContext context, string matterName, string propertyList)
        {
            string value = string.Empty;
            if (null != context && !string.IsNullOrWhiteSpace(matterName) && null != propertyList)
            {
                ListCollection allLists = context.Web.Lists;
                context.Load(allLists);
                context.ExecuteQuery();
                List list = allLists.Cast<List>().FirstOrDefault(item => item.Title.ToUpperInvariant().Equals(matterName.ToUpperInvariant()));
                if (null != list)
                {
                    var props = list.RootFolder.Properties;
                    context.Load(props);
                    context.ExecuteQuery();
                    if (null != props)
                    {
                        if (props.FieldValues.ContainsKey(propertyList))
                        {
                            value = Convert.ToString(props.FieldValues[propertyList], CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public static bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission)
        {
            bool returnValue = false;
            if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
            {
                ListCollection listCollection = clientContext.Web.Lists;
                clientContext.Load(listCollection, lists => lists.Include(list => list.Title, list => list.EffectiveBasePermissions).Where(list => list.Title == listName));
                clientContext.ExecuteQuery();
                if (0 < listCollection.Count)
                {
                    returnValue = listCollection[0].EffectiveBasePermissions.Has(permission);
                }
            }
            return returnValue;
        }
    }
}
