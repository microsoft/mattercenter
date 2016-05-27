using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class ListInformation
    {
        public string name
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }

        public string templateType
        {
            get;
            set;
        }

        public bool isContentTypeEnable
        {
            get;
            set;
        }

        public IList<string> folderNames
        {
            get;
            set;
        }

        public VersioningInfo versioning
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
    }

    
}
