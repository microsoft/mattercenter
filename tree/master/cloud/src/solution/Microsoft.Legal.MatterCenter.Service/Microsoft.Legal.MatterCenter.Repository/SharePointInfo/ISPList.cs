
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISPList
    {
        
        ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null);
        ListItemCollection GetData(Client client, string listName, string camlQuery = null);
        ListItemCollection GetData(string url, string listName, string camlQuery = null);
        dynamic GetDocumentAndClientGUID(Client client);
        List<FolderData> GetFolderHierarchy(MatterData matterData);
        bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission);
        bool CheckPermissionOnList(Client client, string listName, PermissionKind permission);
        bool CheckPermissionOnList(string url, string listName, PermissionKind permission);
        bool Exists(Client client, ReadOnlyCollection<string> lists);
        List<string> MatterAssociatedLists(ClientContext clientContext, ReadOnlyCollection<string> lists);
        PropertyValues GetListProperties(ClientContext clientContext, string libraryname);
        IEnumerable<RoleAssignment> FetchUserPermissionForLibrary(ClientContext clientContext, string libraryname);
        bool SetPermission(ClientContext clientContext, IList<IList<string>> assignUserName, IList<string> permissions, string listName);
        bool SetItemPermission(ClientContext clientContext, IList<IList<string>> assignUserName, string listName, int listItemId, IList<string> permissions);
        void SetPropertBagValuesForList(ClientContext clientContext, PropertyValues props, string matterName, Dictionary<string, string> propertyList);
        bool AddItem(ClientContext clientContext, List list, IList<string> columns, IList<object> values);
        string GetPropertyValueForList(ClientContext clientContext, string matterName, string propertyList);
        bool Delete(ClientContext clientContext, IList<string> lists);
        bool CheckItemModified(ListItemCollection collection, string cachedItemModifiedDate);
    }
}
