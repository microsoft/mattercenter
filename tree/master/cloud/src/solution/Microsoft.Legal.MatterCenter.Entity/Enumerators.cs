// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Entity
// Author           : v-akvira
// Created          : 04-01-2014
//
// ***********************************************************************
// <copyright file="Enumerators.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines enumerators used under current project.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Entity
{
    /// <summary>
    /// Provides set of enumerators available in Matter Center.
    /// </summary>
    public static class Enumerators
    {
        /// <summary>
        /// Supports selection of enumerator to refer to local/global resource file.
        /// </summary>
        public enum ResourceFileLocation
        {
            /// <summary>
            /// The resource file refers to App_GlobalResource folder.
            /// </summary>
            App_GlobalResources,

            /// <summary>
            /// The resource file refers to App_LocalResource folder.
            /// </summary>
            App_LocalResources,

            /// <summary>
            /// The resource file refers to bin folder.
            /// </summary>
            bin
        }

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
}
