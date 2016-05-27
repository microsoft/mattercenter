using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class CamlQueries
    {
        public string UserPinnedDetailsQuery { get; set; }
        public string AllFoldersQuery { get; set; }
        public string RetrieveContextualHelpSectionsQuery { get; set; }
        public string ContextualHelpQueryIncludeOrCondition { get; set; }
        public string RetrieveContextualHelpLinksQuery { get; set; }
        public string DMSRoleQuery { get; set; }
        public string MatterConfigurationsListQuery { get; set;}
        public string ViewOrderByQuery { get; set; }
        public string GetAllFilesInFolderQuery { get; set; }
    }
}
