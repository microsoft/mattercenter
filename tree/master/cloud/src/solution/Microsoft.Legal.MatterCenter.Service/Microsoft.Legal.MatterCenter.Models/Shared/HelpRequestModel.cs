using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class HelpRequestModel
    {
        public Client Client { get; set; }
        public string SelectedPage { get; set; }
    }
}
