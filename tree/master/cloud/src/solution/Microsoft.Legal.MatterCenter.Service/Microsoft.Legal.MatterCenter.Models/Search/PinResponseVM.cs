using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class PinResponseVM
    {
        public int TotalCount { get; set; }
        public IEnumerable<MatterData> UserPinnedMattersList { get; set; }
        public IEnumerable<DocumentData> UserPinnedDocumentsList { get; set; }
        public string NoPinnedMessage { get; set; }
    }
}
