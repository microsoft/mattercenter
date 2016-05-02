
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IDocumentProvision
    {
        bool UploadAttachments(ServiceRequest serviceRequest, Client client);
        bool UploadEmails(ServiceRequest serviceRequest, Client client);
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM);
    }
}
