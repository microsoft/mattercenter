using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class UIConfigSettings
    {
        public virtual string Partitionkey { get; set; }
        public virtual string Rowkey { get; set; }
        public virtual string ConfigGroup { get; set; } 
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }   
        public virtual string MatterCenterConfiguration { get; set; }
    }
}
