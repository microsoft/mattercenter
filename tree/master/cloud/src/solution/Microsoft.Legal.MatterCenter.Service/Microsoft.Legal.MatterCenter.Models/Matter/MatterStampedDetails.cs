using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure required to hold matter property bag information (stamped properties).
    /// </summary>
    public class MatterStampedDetails
    {
        /// <summary>
        /// Indicates if the matter is old or new
        /// </summary>
        public string IsNewMatter { get; set; }

        /// <summary>
        /// Provides the structure required to hold matter meta-data. It includes matter ID, name, description, conflict details, allowed teams, blocked users, permissions, and content type.
        /// </summary>
        public Matter MatterObject { get; set; }

        /// <summary>
        /// Provides the structure required to hold additional matter meta-data. It includes matter practice group, area of law, sub area of law, responsible attorney, team members, and blocked users.
        /// </summary>
        public MatterDetails MatterDetailsObject { get; set; }

        /// <summary>
        /// Provides the structure required to hold client meta-data. It includes client unique ID, client name, and client URL.
        /// </summary>
        public Client ClientObject { get; set; }
    }
}
