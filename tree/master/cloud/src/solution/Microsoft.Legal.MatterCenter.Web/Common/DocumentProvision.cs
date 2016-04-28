using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Legal.MatterCenter.Models;

namespace Microsoft.Legal.MatterCenter.Web.Common
{
    public class DocumentProvision : IDocumentProvision
    {
        public bool UploadAttachments(ServiceRequest serviceRequest, Client client)
        {
            return true;
        }

        public bool UploadEmails(ServiceRequest serviceRequest, Client client)
        {
            return true;
        }
    }
}
