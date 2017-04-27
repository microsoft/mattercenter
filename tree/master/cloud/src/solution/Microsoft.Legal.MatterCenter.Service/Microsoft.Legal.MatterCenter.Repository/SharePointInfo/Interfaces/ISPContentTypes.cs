using Microsoft.Legal.MatterCenter.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public interface ISPContentTypes
    {
        IList<ContentType> GetContentTypeData(ClientContext clientContext, IList<string> contentTypes, Client client, Matter matter);
        GenericResponseVM AssignContentTypeHelper(MatterMetadata matterMetadata, ClientContext clientContext, IList<ContentType> contentTypeCollection, Client client, Matter matter);
        void AssignContentType(ClientContext clientContext, string contentTypeName, List matterDocumentLibrary);
    }
}
