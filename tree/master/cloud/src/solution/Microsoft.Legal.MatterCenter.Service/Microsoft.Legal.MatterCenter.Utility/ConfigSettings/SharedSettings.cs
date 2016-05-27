// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="SharedSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the shared settings information from the appSettings.json file
    /// These properties will subsequently at multiple places
    /// </summary>
    public class SharedSettings
    {
        public string MatterCenterPages { get; set; }
        public string ContextualHelpSectionColumnSectionID { get; set; }
        public string ContextualHelpLinksColumnLinkOrder { get; set; }
        public string ContextualHelpLinksColumnLinkTitle { get; set; }
        public string ContextualHelpLinksColumnLinkURL { get; set; }
        public string ContextualHelpSectionColumnPageName { get; set; }
        public string ContextualHelpSectionColumnNumberOfColumns { get; set; }
        public string ContextualHelpSectionColumnSectionOrder { get; set; }
        public string ContextualHelpSectionColumnSectionTitle { get; set; }
        public string ContextualHelpLinksColumnSectionID { get; set; }

    }
}
