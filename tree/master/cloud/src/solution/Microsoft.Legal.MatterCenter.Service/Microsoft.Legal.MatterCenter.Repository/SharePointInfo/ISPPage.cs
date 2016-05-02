
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISPPage
    {
        bool UrlExists(Client client, string pageUrl);
        void Delete(ClientContext clientContext, string pageUrl);
        bool IsFileExists(ClientContext clientContext, string pageUrl);
        bool PageExists(string requestedUrl, ClientContext clientContext);
    }
}
