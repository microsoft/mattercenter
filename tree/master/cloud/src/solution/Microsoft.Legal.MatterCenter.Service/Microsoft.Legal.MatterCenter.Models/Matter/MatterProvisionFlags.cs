using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary> 
    /// Provides the structure required to hold flag values
    /// </summary>   
    public class MatterProvisionFlags
    {
        /// <summary>
        /// Gets or sets value for matter landing flag
        /// </summary>
        /// <value>Matter landing</value>

        public string MatterLandingFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets boolean value for send email flag
        /// </summary>
        /// <value>Send email</value>

        public bool SendEmailFlag
        {
            get;
            set;
        }
    }
}
