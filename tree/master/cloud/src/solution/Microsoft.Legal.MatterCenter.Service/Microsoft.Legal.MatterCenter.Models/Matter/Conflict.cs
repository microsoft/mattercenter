using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold matter conflict meta-data. It includes user who performed the conflict check, the conflict check date, and security of matter from external users.
    /// </summary>
    public class Conflict
    {
        /// <summary>
        /// Gets or sets the user who performed the conflict check for matter.
        /// </summary>
        /// <value>The conflict check by.</value>

        public string CheckBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conflict check date for matter.
        /// </summary>
        /// <value>The conflict check on.</value>

        public string CheckOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the conflict identified option (Yes/No) for matter. Represents if the conflict was identified or not.
        /// </summary>
        /// <value>The conflict identified.</value>

        public string Identified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flag to mark the matter as secure or not.
        /// </summary>
        /// <value>The secure matter.</value>

        public string SecureMatter
        {
            get;
            set;
        }
    }
}
