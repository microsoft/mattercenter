

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class SearchResponseVM
    {
        public IEnumerable<IDictionary<string, object>> SearchResults{ get; set; }
        public int TotalRows { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
    }
}
