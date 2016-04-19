

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterInformationVM
    {
        public Client Client { get; set; }
        public Matter Matter { get; set; }
        public MatterDetails MatterDetails { get; set; }
        public bool EditMode { get; set; }
        public IList<string> UserIds { get; set; }
    }
}
