
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISPList
    {
        bool CheckPermissionOnList(Client client, string listName, PermissionKind permission);
        ListItemCollection GetData(ClientContext clientContext, string listName, string camlQuery = null);
        ListItemCollection GetData(Client client, string listName, string camlQuery = null);
        dynamic GetDocumentAndClientGUID(Client client);
        List<FolderData> GetFolderHierarchy(MatterData matterData);
        bool CheckPermissionOnList(ClientContext clientContext, string listName, PermissionKind permission);
    }
}
