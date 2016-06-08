using System.Collections.Generic;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterMetdataVM : MatterVM
    {
        public MatterConfigurations MatterConfigurations { get; set; }
        public MatterProvisionFlags MatterProvisionFlags { get; set; }
    }
}
