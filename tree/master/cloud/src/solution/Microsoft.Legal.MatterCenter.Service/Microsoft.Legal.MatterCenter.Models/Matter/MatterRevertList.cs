using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// Provides the structure needed to revert the matter permission update operation.
    /// </summary>
    [CLSCompliant(false)]
    public class MatterRevertList
    {
        /// <summary>
        /// Gets or sets a matter library instance 
        /// </summary>
        public string MatterLibrary { get; set; }

        /// <summary>
        /// Gets or sets a matter OneNote library instance
        /// </summary>
        public string MatterOneNoteLibrary { get; set; }

        /// <summary>
        /// Gets or sets a matter calendar instance
        /// </summary>
        public string MatterCalendar { get; set; }

        /// <summary>
        /// Gets or sets a matter site pages instance
        /// </summary>
        public string MatterSitePages { get; set; }

        /// <summary>
        /// Gets or sets a matter task instance
        /// </summary>
        public string MatterTask { get; set; }
    }
}
