
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
        GenericResponseVM UploadFiles(IFormFile uploadedFile, string fileExtension, string originalName, string folderName, string fileName, string clientUrl, string folder, string documentLibraryName);
    }
}
