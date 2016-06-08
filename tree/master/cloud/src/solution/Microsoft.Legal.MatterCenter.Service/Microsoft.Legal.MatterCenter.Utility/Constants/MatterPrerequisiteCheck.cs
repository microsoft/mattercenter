using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// Represents existence of matter library, OneNote library, Calendar library, Task library and Matter landing page
    /// </summary>
    public enum MatterPrerequisiteCheck
    {
        /// <summary>
        /// Represents matter library, OneNote library, calendar library or task library existence
        /// </summary>
        LibraryExists,

        /// <summary>
        /// Represents matter landing page existence
        /// </summary>
        MatterLandingPageExists
    }
}
