using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IUploadHelperFunctionsUtility
    {
        string GetAttachmentID(XmlDocument xmlDocument, string originalFileName);
        HttpWebResponse GetWebResponse(Uri ewsUrl, string attachmentToken, string soapRequest, string attachmentOrMailID);
        string RemoveSpecialChar(string fileName);
        byte[] GetStream(XmlDocument xmlDocument, XmlNamespaceManager nsmgr, bool isMailUpload, string extension, bool isMsg);
        GenericResponseVM CheckDuplicateDocument(ClientContext clientContext, string documentLibraryName, bool isMailUpload, string folderPath, ContentCheckDetails contentCheck, string uploadFileName, bool allowContentCheck, ref string message);
        GenericResponseVM PerformContentCheckUtility(bool isMailUpload, string folderPath, bool isMsg, XmlDocument xmlDocument, XmlNamespaceManager nsmgr, string extension, string uploadFileName, ClientContext clientContext);
    }
}
