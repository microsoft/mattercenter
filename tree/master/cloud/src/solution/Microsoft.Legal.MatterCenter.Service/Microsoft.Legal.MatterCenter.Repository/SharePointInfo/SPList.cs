// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Repository
// Author           : v-rijadh
// Created          : 07-07-2016
//***************************************************************************
// History
// Modified         : 07-07-2016
// Modified By      : v-lapedd
// ***********************************************************************
// <copyright file="TaxonomyHelper.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provide methods to get/update information from/in SP lists</summary>


using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

#region Matter Namespaces
using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using System.Reflection;
#endregion

namespace Microsoft.Legal.MatterCenter.Repository
{
    /// <summary>
    /// This file provide methods to get/update information from/in SP lists
    /// </summary>
    public class SPList:ISPList
    {
        #region Properties
        private SearchSettings searchSettings;
        private ISPOAuthorization spoAuthorization;
        private CamlQueries camlQueries;
        private ICustomLogger customLogger;
        private LogTables logTables;
        #endregion

        /// <summary>
        /// All the dependencies are injected into the constructor
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        public SPList(ISPOAuthorization spoAuthorization,
            IOptions<CamlQueries> camlQueries,
            IOptions<SearchSettings> searchSettings,
            ICustomLogger customLogger, IOptions<LogTables> logTables)
        {
            this.searchSettings = searchSettings.Value;
            this.camlQueries = camlQueries.Value;
            this.spoAuthorization = spoAuthorization;
            this.customLogger = customLogger;
            this.logTables = logTables.Value;
        }


        /// <summary>
        /// Method will check the permission of the list that has been provided
        /// </summary>
        /// <param name="url"></param>
        /// <param name="listName"></param>
        /// <param name="permission"></param>
        /// <returns>True or false</returns>
        public bool CheckPermissionOnList(string url, string listName, PermissionKind permission)
        {           
            try
            {
                return CheckPermissionOnList(url, listName, permission);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }            
        }

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public bool CheckPermissionOnList(Client client, string listName, PermissionKind permission)
        {
            try
            {
                return CheckPermissionOnList(client.Url, listName, permission);
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission)
        {
            try
            {
                bool returnValue = false;
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
                {
                    ListCollection listCollection = clientContext.Web.Lists;
                    clientContext.Load(listCollection, lists => lists.Include(list => list.Title, list =>
                                                    list.EffectiveBasePermissions).Where(list => list.Title == listName));
                    clientContext.ExecuteQuery();
                    if (0 < listCollection.Count)
                    {
                        returnValue = listCollection[0].EffectiveBasePermissions.Has(permission);
                    }
                }
                return returnValue;
            }

            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Gets the list items of specified list based on CAML query.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null)
        {
            try
            {
                ListItemCollection listItemCollection = null;
                if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
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
                return listItemCollection;
            }
            catch (Exception exception)
            {
                customLogger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        public ListItemCollection GetData(string url, string listName, string camlQuery = null)
        {
            try
            {
                ListItemCollection listItemCollection = null;
                using (ClientContext clientContext = spoAuthorization.GetClientContext(url))
                {
                    if (null != clientContext && !string.IsNullOrWhiteSpace(listName))
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
                    return listItemCollection;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Gets the list items of specified list based on CAML query. 
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="camlQuery">CAML Query that need to be executed on list</param>
        /// <returns>Collection of list items</returns>
        public ListItemCollection GetData(Client client, string listName, string camlQuery = null)
        {
            try
            { 
                return GetData(client.Url, listName, camlQuery);
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the document and List GUID
        /// </summary>
        /// <param name="client">Client object containing list data</param>        
        /// <param name="clientContext">Client Context</param>     
        /// <returns>Returns the document and List GUID</returns>
        public dynamic GetDocumentAndClientGUID(Client client)
        {
            try
            {
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    string listInternalName = string.Empty;
                    string documentGUID = string.Empty;
                    string result = string.Empty;
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists, list => list.Include(listItem => listItem.Id, listItem => listItem.RootFolder.ServerRelativeUrl));
                    File file = clientContext.Web.GetFileByServerRelativeUrl(client.Id);
                    clientContext.Load(file, files => files.ListItemAllFields);
                    clientContext.ExecuteQuery();
                    if (0 < file.ListItemAllFields.FieldValues.Count)
                    {
                        documentGUID = Convert.ToString(file.ListItemAllFields.FieldValues[ServiceConstants.DOCUMENT_GUID_COLUMN_NAME], CultureInfo.InvariantCulture);
                    }

                    List retrievedList = (from list in lists
                                          where list.RootFolder.ServerRelativeUrl.ToUpperInvariant().Equals(client.Name.ToUpperInvariant())
                                          select list).FirstOrDefault();
                    if (null != retrievedList)
                    {
                        listInternalName = Convert.ToString(retrievedList.Id, CultureInfo.InvariantCulture);
                    }
                    var documentAsset = new
                    {
                        ListInternalName = listInternalName,
                        DocumentGuid = documentGUID
                    };
                    return documentAsset;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        
        /// <summary>
        /// Method which will give matter folder hierarchy
        /// </summary>
        /// <param name="matterData"></param>
        /// <returns></returns>
        public List<FolderData> GetFolderHierarchy(MatterData matterData)
        {
            try
            { 
                using (ClientContext clientContext = spoAuthorization.GetClientContext(matterData.MatterUrl))
                {
                    List list = clientContext.Web.Lists.GetByTitle(matterData.MatterName);
                    clientContext.Load(list.RootFolder);
                    ListItemCollection listItems = GetData(clientContext, matterData.MatterName, 
                        string.Format(CultureInfo.InvariantCulture, camlQueries.AllFoldersQuery, matterData.MatterName));
                    List<FolderData> allFolders = new List<FolderData>();
                    allFolders = GetFolderAssignment(list, listItems, allFolders);

                    return allFolders;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        /// <summary>
        /// Retrieves folder hierarchy from document library.
        /// </summary>
        /// <param name="list">SharePoint library</param>
        /// <param name="listItems">List items</param>
        /// <param name="allFolders">List of all folders of type folder data</param>
        /// <returns>List of folders of type folder data</returns>
        internal List<FolderData> GetFolderAssignment(List list, ListItemCollection listItems, List<FolderData> allFolders)
        {
            try
            {
                FolderData folderData = new FolderData()
                {
                    Name = list.RootFolder.Name,
                    URL = list.RootFolder.ServerRelativeUrl,
                    ParentURL = null
                };
                allFolders.Add(folderData);
                foreach (var listItem in listItems)
                {
                    folderData = new FolderData()
                    {
                        Name = Convert.ToString(listItem[searchSettings.ColumnNameFileLeafRef], CultureInfo.InvariantCulture),
                        URL = Convert.ToString(listItem[searchSettings.ColumnNameFileRef], CultureInfo.InvariantCulture),
                        ParentURL = Convert.ToString(listItem[searchSettings.ColumnNameFileDirRef], CultureInfo.InvariantCulture)
                    };

                    allFolders.Add(folderData);
                }
                return allFolders;            
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

        

        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public bool Exists(Client client, ReadOnlyCollection<string> listsNames)
        {
            try
            { 
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
                {
                    List<string> existingLists = new List<string>();
                    if (null != clientContext && null != listsNames)
                    {
                        //ToDo: Chec
                        ListCollection lists = clientContext.Web.Lists;
                        clientContext.Load(lists);
                        clientContext.ExecuteQuery();
                        existingLists = (from listName in listsNames
                                         join item in lists
                                         on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                         select listName).ToList();
                    }
                    return existingLists.Count>0;
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }


        /// <summary>
        /// Function to check whether list is present or not.
        /// </summary>
        /// <param name="clientContext">Client context object for SharePoint</param>
        /// <param name="listsNames">List name</param>
        /// <returns>Success flag</returns>
        public List<string> MatterAssociatedLists(ClientContext clientContext, ReadOnlyCollection<string> listsNames)
        {            
            List<string> existingLists = new List<string>();
            try
            { 
                if (null != clientContext && null != listsNames)
                {
                    //ToDo: Chec
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();
                    existingLists = (from listName in listsNames
                                        join item in lists
                                        on listName.ToUpper(CultureInfo.InvariantCulture) equals item.Title.ToUpper(CultureInfo.InvariantCulture)
                                        select listName).ToList();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return existingLists;
        }

        public PropertyValues GetListProperties(ClientContext clientContext, string libraryName)
        {
            
            PropertyValues stampedProperties = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryName))
                {
                    stampedProperties = clientContext.Web.Lists.GetByTitle(libraryName).RootFolder.Properties;
                    clientContext.Load(stampedProperties);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw; //// This will transfer control to catch block of parent function.
            }

            return stampedProperties;
        }

        public IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname)
        {
            IEnumerable<RoleAssignment> userPermissionCollection = null;
            try
            {
                if (null != clientContext && !string.IsNullOrWhiteSpace(libraryname))
                {
                    List list = clientContext.Web.Lists.GetByTitle(libraryname);
                    userPermissionCollection = clientContext.LoadQuery(list.RoleAssignments.Include(listRoleAssignment => 
                        listRoleAssignment.PrincipalId, listRoleAssignment => listRoleAssignment.Member, 
                        listRoleAssignment => listRoleAssignment.Member.Title, 
                        listRoleAssignment => listRoleAssignment.Member.PrincipalType, 
                        listRoleAssignment => listRoleAssignment.RoleDefinitionBindings.Include(userRoles => userRoles.BasePermissions, 
                                                                                                userRoles => userRoles.Name, 
                                                                                                userRoles => userRoles.Id)).Where(listUsers => (PrincipalType.User == listUsers.Member.PrincipalType) || (PrincipalType.SecurityGroup == listUsers.Member.PrincipalType)));
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw; //// This will transfer control to catch block of parent function.
            }
            return userPermissionCollection;
        }

        /// <summary>
        /// Sets permissions for the list.
        /// </summary>
        /// <param name="clientContext">Client Context</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <param name="listName">List name</param>
        /// <returns>String stating success flag</returns>
        public bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName)
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
                    if (list.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
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
                catch (Exception ex)
                {
                    customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Set permission to the specified list item 
        /// </summary>
        /// <param name="clientContext">Client context object</param>
        /// <param name="assignUserName">Users to give permission</param>
        /// <param name="listName">List name</param>
        /// <param name="listItemId">Unique list item id to break item level permission</param>
        /// <param name="permissions">Permissions for the users</param>
        /// <returns>Status of the unique item level permission assignment operation</returns>
        public bool SetItemPermission(ClientContext clientContext, IList<IList<string>> assignUserName, string listName, int listItemId, IList<string> permissions)
        {
            bool result = false;
            try
            {
                if (null != clientContext)
                {
                    ClientRuntimeContext clientRuntimeContext = clientContext;
                    ListItem listItem = clientContext.Web.Lists.GetByTitle(listName).GetItemById(listItemId);
                    clientContext.Load(listItem, item => item.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (listItem.HasUniqueRoleAssignments && null != permissions && null != assignUserName && permissions.Count == assignUserName.Count)
                    {
                        int position = 0;
                        foreach (string roleName in permissions)
                        {
                            IList<string> userName = assignUserName[position];
                            if (!string.IsNullOrWhiteSpace(roleName) && null != userName)
                            {
                                RoleDefinition roleDefinition = clientContext.Web.RoleDefinitions.GetByName(roleName);
                                foreach (string user in userName)
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
            }
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="props">Property Bag</param>
        /// <param name="matterName">Name of matter to which property is to be attached</param>
        /// <param name="propertyList">List of properties</param>
        public void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList)
        {
            try
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
            catch (Exception ex)
            {
                customLogger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, logTables.SPOLogTable);
                throw;
            }
        }

    }
}
