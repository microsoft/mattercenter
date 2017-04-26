
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
        /// <summary>
        /// Practice Group Term Set name that is configured
        /// </summary>
        public string PracticeGroupTermSetName { get; set; }
        /// <summary>
        /// Client Term Set Name that is configured
        /// </summary>
        public string ClientTermSetName { get; set; }
        public string PracticeGroupCustomPropertyFolderNames { get; set; }
        public string AreaCustomPropertyFolderNames { get; set; }
        public string SubAreaCustomPropertyFolderNames { get; set; }
        public string SubAreaCustomPropertyisNoFolderStructurePresent { get; set; }
        public string SubAreaOfLawDocumentTemplates { get; set; }
        public string ClientCustomPropertiesId { get; set; }
        /// <summary>
        /// Number of level that are configured - Taxonomy Hierarchy
        /// </summary>
        public int Levels { get; set; }
        /// <summary>
        /// Client term location in the taxonomy hierarchy
        /// </summary>
        public string ClientTermPath { get; set; }
        public string Level1Name { get; set; }
        public string Level2Name { get; set; }
        public string Level3Name { get; set; }
        public string Level4Name { get; set; }
        public string Level5Name { get; set; }

        public string MatterProvisionExtraPropertiesContentType { get; set; }
   
    }
}
