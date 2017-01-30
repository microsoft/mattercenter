using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISearch
    {
        SearchResponseVM GetMatters(SearchRequestVM searchRequestVM, ClientContext clientContext);
        SearchResponseVM GetDocuments(SearchRequestVM searchRequestVM, ClientContext clientContext);
        SearchResponseVM GetPinnedData(SearchRequestVM searchRequestVM, string listName, string listColumnName, bool isShowDocument, ClientContext clientContext);
        bool PinMatter(PinRequestMatterVM pinRequestMatterVM);
        bool UnPinMatter(PinRequestMatterVM pinRequestMatterVM);
        bool PinDocument(PinRequestDocumentVM pinRequestDocumentVM);
        bool UnPinDocument(PinRequestDocumentVM pinRequestDocumentVM);
        List<ContextHelpData> GetMatterHelp(Client client, string selectedPage, string listName);
        List<RoleDefinition> GetWebRoleDefinitions(Client client);
        IList<PeoplePickerUser> SearchUsers(SearchRequestVM searchRequestVM);
        GenericResponseVM GetConfigurations(string siteCollectionUrl, string listName);
        bool PageExists(Client client, string requestedUrl);
    }
}
