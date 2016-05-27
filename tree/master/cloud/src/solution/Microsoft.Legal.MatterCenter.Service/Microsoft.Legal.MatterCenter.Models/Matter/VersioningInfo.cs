using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class VersioningInfo
    {
        public bool EnableVersioning
        {
            get;
            set;
        }
        public bool EnableMinorVersions
        {
            get;
            set;
        }
        public bool ForceCheckout
        {
            get;
            set;
        }

    }
}
