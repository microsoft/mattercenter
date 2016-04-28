using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;


namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public interface IDocumentProvision
    {
        bool UploadAttachments(ServiceRequest serviceRequest, Client client);
        bool UploadEmails(ServiceRequest serviceRequest, Client client);
    }
}
