
using Microsoft.AspNetCore.Http;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IDocumentProvision: ISharedProvision
    {
        GenericResponseVM UploadAttachments(AttachmentRequestVM attachmentRequestVM);
        GenericResponseVM UploadEmails(AttachmentRequestVM attachmentRequest);
        Task<SearchResponseVM> GetDocumentsAsync(SearchRequestVM searchRequestVM);
        GenericResponseVM UploadFiles(IFormFile uploadedFile, string fileExtension, string originalName, 
            string folderName, string fileName, string clientUrl, string folder, string documentLibraryName);
        Stream DownloadAttachments(MailAttachmentDetails mailAttachmentDetails);
        GenericResponseVM CheckDuplicateDocument(string clientUrl, string folderName, string documentLibraryName, string fileName, ContentCheckDetails contentCheck, bool allowContentCheck);
        GenericResponseVM PerformContentCheck(string clientUrl, string folderUrl, IFormFile uploadedFile, string fileName);
    }
}
