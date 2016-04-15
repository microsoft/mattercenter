

using Microsoft.Extensions.OptionsModel;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class SPList:ISPList
    {
        #region Properties
        private SearchSettings searchSettings;
        private ISPOAuthorization spoAuthorization;
        private CamlQueries camlQueries;
        #endregion

        /// <summary>
        /// All the dependencies are injected 
        /// </summary>
        /// <param name="spoAuthorization"></param>
        /// <param name="generalSettings"></param>
        public SPList(ISPOAuthorization spoAuthorization,
            IOptions<CamlQueries> camlQueries,
            IOptions<SearchSettings> searchSettings)
        {
            this.searchSettings = searchSettings.Value;
            this.camlQueries = camlQueries.Value;
            this.spoAuthorization = spoAuthorization;
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
            bool returnValue = false;
            using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
            {
                if (!string.IsNullOrWhiteSpace(listName))
                {
                    ListCollection listCollection = clientContext.Web.Lists;
                    clientContext.Load(listCollection, lists => lists.Include(list => list.Title, list => list.EffectiveBasePermissions).Where(list => list.Title == listName));
                    clientContext.ExecuteQuery();
                    if (listCollection.Count>0)
                    {
                        returnValue = listCollection[0].EffectiveBasePermissions.Has(permission);
                    }
                }
            }            
            return returnValue;
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
            catch (Exception ex)
            {                
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
                ListItemCollection listItemCollection = null;
                using (ClientContext clientContext = spoAuthorization.GetClientContext(client.Url))
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
                throw;
            }
        }

        

        public List<FolderData> GetFolderHierarchy(MatterData matterData)
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

        /// <summary>
        /// Retrieves folder hierarchy from document library.
        /// </summary>
        /// <param name="list">SharePoint library</param>
        /// <param name="listItems">List items</param>
        /// <param name="allFolders">List of all folders of type folder data</param>
        /// <returns>List of folders of type folder data</returns>
        internal List<FolderData> GetFolderAssignment(List list, ListItemCollection listItems, List<FolderData> allFolders)
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

        /// <summary>
        /// Determines whether user has a particular permission on the list
        /// </summary>
        /// <param name="clientContext">Client context</param>
        /// <param name="listName">List name</param>
        /// <param name="permission">Permission to be checked</param>
        /// <returns>Success flag</returns>
        public bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission)
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

    }
}
