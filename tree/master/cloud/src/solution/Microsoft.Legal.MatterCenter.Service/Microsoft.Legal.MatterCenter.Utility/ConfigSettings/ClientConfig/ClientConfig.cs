using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility.ConfigSettings
{
    public class ClientConfig
    {
        public Uri Uri { get; set; }
        public Adal Adal { get; set; }
        public Upload Upload { get; set; }
        
        public SharePointOnline SharePointOnline { get; set; }
    }

    public class Uri
    {
        public string SPOsiteURL { get; set; }
        public string Tenant { get; set; }
    }

    public class Adal
    {
        public string ClientId { get; set; }
    }

    public class Upload
    {
        public string PNGIconExtensions { get; set; }
        public string ImageDocumentIcon { get; set; }
    }  
    
    public class SharePointOnline
    {
        public string CentralRepositoryUrl { get; set; }
        public string ProvisionMatterAppURL { get; set; }
    }
    
}
