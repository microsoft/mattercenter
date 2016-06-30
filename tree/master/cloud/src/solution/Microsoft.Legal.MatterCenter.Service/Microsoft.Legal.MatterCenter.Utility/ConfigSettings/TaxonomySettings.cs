
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="TaxonomySettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the taxonomy configuration information from the appSettings.json file
    /// These properties will subsequently used when getting the taxonomy information from SPO
    /// </summary>
    public class TaxonomySettings
    {        
        public string PracticeGroupTermSetName { get; set; }
        public string ClientTermSetName { get; set; }
        public string PracticeGroupCustomPropertyFolderNames { get; set; }
        public string AreaCustomPropertyFolderNames { get; set; }
        public string SubAreaCustomPropertyFolderNames { get; set; }
        public string SubAreaCustomPropertyisNoFolderStructurePresent { get; set; }
        public string SubAreaOfLawDocumentTemplates { get; set; }
        public string ClientCustomPropertiesId { get; set; }
        public int Levels { get; set; }
        
    }
}
