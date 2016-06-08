using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterVM
    {
        public Client Client { get; set; }
        public Matter Matter { get; set; }
        public MatterDetails MatterDetails { get; set; }
    }
}
