using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class SearchResponseVM
    {
        public IEnumerable<IDictionary<string, object>> SearchResults{ get; set; }
        public IEnumerable<MatterData> MatterDataList { get; set; }
        public IEnumerable<DocumentData> DocumentDataList { get; set; }
        public int TotalRows { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
        public string NoPinnedMessage { get; set; }
    }
}
