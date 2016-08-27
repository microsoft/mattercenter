using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    public class ManagedColumn
    {
        /// <summary>
        /// Gets or sets the name of the term
        /// </summary>
        /// <value>The name of the term.</value>
        /// 
        public string TermName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the term (practice group). This is a GUID value (string representation) and is used as the Id property of the Term.
        /// </summary>
        /// <value>The id of the term.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WssId of the term This is an integer value being used as the Id of the Term in the TaxonomyHiddenList.
        /// </summary>
        /// <value>The WssId of the term.</value>
        public int WssId
        {
            get;
            set;
        }
    }
}
