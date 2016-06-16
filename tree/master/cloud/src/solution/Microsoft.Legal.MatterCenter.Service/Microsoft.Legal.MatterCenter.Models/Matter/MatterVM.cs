using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
namespace Microsoft.Legal.MatterCenter.Models
{
    public class MatterVM:TableEntity
    {
        public Client Client { get; set; }
        public Matter Matter { get; set; }
        public MatterDetails MatterDetails { get; set; }
        public IList<string> UserIds { get; set; }
        public string SerializeMatter { get; set; }
        public string Status { get; set; }
    }
}
