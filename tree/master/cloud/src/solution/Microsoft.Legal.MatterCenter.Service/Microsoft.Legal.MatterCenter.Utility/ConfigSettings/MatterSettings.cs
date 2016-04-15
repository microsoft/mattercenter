// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="MatterSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting the matter configuration information from the appSettings.json file
    /// These properties will subsequently used when getting the matter information from SPO
    /// </summary>
    public class MatterSettings
    {
        public string EditMatterAllowedPermissionLevel { get; set; }
        public string ColumnNameGuid { get; set; }
        public string RoleListColumnRoleName { get; set; }
        public string RoleListColumnIsRoleMandatory { get; set; }
        public string ColumnNameModifiedDate { get; set; }
        public string MatterConfigurationColumn { get; set; }
    }
}
