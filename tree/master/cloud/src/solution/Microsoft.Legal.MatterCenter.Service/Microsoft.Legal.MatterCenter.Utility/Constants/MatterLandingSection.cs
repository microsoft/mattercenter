using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// Matter Landing page section mapping
    /// </summary>
    public enum MatterLandingSection
    {
        /// <summary>
        /// Task panel mapping
        /// </summary>
        TaskPanel,

        /// <summary>
        /// Calendar panel mapping
        /// </summary>
        CalendarPanel,

        /// <summary>
        /// Footer panel mapping
        /// </summary>
        FooterPanel,

        /// <summary>
        /// Information panel mapping
        /// </summary>
        InformationPanel,

        /// <summary>
        /// Header panel mapping
        /// </summary>
        HeaderPanel,

        /// <summary>
        /// RSS Title panel mapping
        /// </summary>
        RSSTitlePanel,

        /// <summary>
        /// OneNote panel mapping
        /// </summary>
        OneNotePanel
    }
}
