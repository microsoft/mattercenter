
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISPPage
    {
        bool UrlExists(Client client, string pageUrl);
        void Delete(ClientContext clientContext, string pageUrl);
        bool IsFileExists(ClientContext clientContext, string pageUrl);
        bool PageExists(string requestedUrl, ClientContext clientContext);
        int CreateWebPartPage(ClientContext clientContext, string pageName, string layout, string masterpagelistName, string listName, string pageTitle);
        string[] ConfigureXMLCodeOfWebParts(Client client, Matter matter, ClientContext clientContext, string pageName, Uri uri,
            Web web, MatterConfigurations matterConfigurations);
        bool AddWebPart(ClientContext clientContext, LimitedWebPartManager limitedWebPartManager, WebPartDefinition webPartDefinition,
            string[] webParts, string[] zones);
    }
}
