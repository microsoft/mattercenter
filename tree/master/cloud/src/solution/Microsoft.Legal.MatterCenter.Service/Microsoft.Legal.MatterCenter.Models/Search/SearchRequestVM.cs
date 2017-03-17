using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class SearchRequestVM
    {
        public SearchObject SearchObject { get; set; }
        /// <summary>
        /// Client info object
        /// </summary>
        public virtual Client Client { get; set; }
    }
}
