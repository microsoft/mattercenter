using Microsoft.Legal.MatterCenter.Models;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
   

    /// <summary>
    /// Provide methods to perform document transfer functionalities.
    /// </summary>
    public interface IUploadHelperFunctions
    {

        GenericResponseVM Upload(Client client, ServiceRequest serviceRequest, string soapRequest, string attachmentOrMailID,
            bool isMailUpload, string fileName, string folderPath, bool isFirstCall, ref string message, string originalFileName);
        
    }
}
