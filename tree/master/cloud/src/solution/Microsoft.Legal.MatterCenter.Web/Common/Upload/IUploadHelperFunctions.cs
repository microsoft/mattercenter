using Microsoft.AspNetCore.Http;
using Microsoft.Legal.MatterCenter.Models;
using System.IO;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
   

    /// <summary>
    /// Provide methods to perform document transfer functionalities.
    /// </summary>
    public interface IUploadHelperFunctions
    {

        GenericResponseVM Upload(Client client, ServiceRequest serviceRequest, string soapRequest, string attachmentOrMailID,
            bool isMailUpload, string fileName, string folderPath, bool isFirstCall, ref string message, string originalFileName);
        DuplicateDocument DocumentExists(string clientUrl, ContentCheckDetails contentCheck, string documentLibraryName, string folderName, bool isMail);
        GenericResponseVM PerformContentCheck(string clientUrl, string folderUrl, IFormFile uploadedFile, string fileName);
        GenericResponseVM UploadAttachmentOfEmail(Client client, ServiceRequest serviceRequest,
                 AttachmentDetails attachment, string folderPath,
                ref string message);      
        GenericResponseVM Upload(Client client, ServiceRequest serviceRequest, ref string message);
        


        }
}
