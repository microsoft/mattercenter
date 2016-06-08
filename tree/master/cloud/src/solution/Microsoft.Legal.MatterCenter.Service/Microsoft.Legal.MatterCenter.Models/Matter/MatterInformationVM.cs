using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterInformationVM: MatterVM
    {        
        public bool EditMode { get; set; }
        public IList<string> UserIds { get; set; }
    }
}
