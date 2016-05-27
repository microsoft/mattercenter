
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold role meta-data. It includes role ID, name, and flag indicating whether role is mandatory or not.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the role identifier. Represents the role identifier.
        /// </summary>
        /// <value>The role identifier.</value>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the role. Represents the role name.
        /// </summary>
        /// <value>The name of the role.</value>

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Role"/> is mandatory.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>

        public bool Mandatory
        {
            get;
            set;
        }
    }
}
