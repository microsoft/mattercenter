using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class SearchResponseVM
    {
        public IEnumerable<IDictionary<string, object>> SearchResults{ get; set; }
        public dynamic MatterDataList { get; set; }
        public dynamic DocumentDataList { get; set; }
        public int TotalRows { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
        public string NoPinnedMessage { get; set; }
    }
}
