using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class TaxonomyResponseVM
    {
        public string TermSets { get; set; }
        public ClientTermSets ClientTermSets { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
    }
}
