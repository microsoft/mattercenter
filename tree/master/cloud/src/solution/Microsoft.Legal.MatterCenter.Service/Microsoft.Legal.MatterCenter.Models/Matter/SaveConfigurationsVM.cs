using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class SaveConfigurationsVM:MatterVM
    {
        public string SiteCollectionPath { get; set; }
        public MatterConfigurations MatterConfigurations { get; set; }
        public IList<string> UserId { get; set; }
        public string CachedItemModifiedDate { get; set; }
    }   
}
