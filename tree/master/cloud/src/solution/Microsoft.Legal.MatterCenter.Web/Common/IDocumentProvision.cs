
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IDocumentProvision
    {
        GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequest);
        GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequest);
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM);
    }
}
