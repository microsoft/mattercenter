using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class GenericResponseVM
    {
        public string Code { get; set; }
        public string Value { get; set;  }
        public bool IsError { get; set; }
        public string Description { get; set; }
    }
}
