
using Microsoft.AspNet.Http;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IDocumentProvision
    {
        GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequestVM);
        GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequest);
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM);
        void UploadFiles(IFormFile uploadedFile, string fileExtension, string originalName, IList<GenericResponseVM> listResponse, string fileName, string clientUrl, string folder, string documentLibraryName);
    }
}
