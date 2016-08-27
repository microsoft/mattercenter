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
        public string MatterCenterDateFormat { get; set; }

        public string OneNoteLibrarySuffix { get; set; }
        public string CalendarNameSuffix { get; set; }
        public string TaskNameSuffix { get; set; }
        
        public string SendMailListName { get; set; }
        public string SpecialCharacterExpressionMatterTitle { get; set; }
        public string SpecialCharacterExpressionMatterId { get; set; }
        public string SpecialCharacterExpressionMatterDescription { get; set; }
        public string SpecialCharacterExpressionContentType { get; set; }
        public int MatterIdLength { get; set; }
        public int MatterNameLength { get; set; }
        public bool IsCreateCalendarEnabled { get; set; }

        public int ContentTypeLength { get; set; }
        public int MatterDescriptionLength { get; set; }
        public string UserPermissions { get; set; }
        
        public string MatterLandingPageRepositoryName { get; set; }


        public string StampedPropertyMatterGUID { get; set; }
        public string StampedPropertyMatterCenterUsers { get; set; }
        public string StampedPropertyMatterCenterPermissions { get; set; }
        public string StampedPropertyMatterCenterRoles { get; set; }
        public string StampedPropertyResponsibleAttorney { get; set; }
        public string StampedPropertyTeamMembers { get; set; }
        public string StampedPropertyBlockedUploadUsers { get; set; }
        public string StampedPropertyMatterCenterUserEmails { get; set; }

        public string StampedPropertyPracticeGroup { get; set; }
        public string StampedPropertyAreaOfLaw { get; set; }
        public string StampedPropertySubAreaOfLaw { get; set; }
        public string StampedPropertyMatterName { get; set; }
        public string StampedPropertyMatterID { get; set; }
        public string StampedPropertyClientName { get; set; }
        public string StampedPropertyClientID { get; set; }
        public string StampedPropertyIsMatter { get; set; }
        public string StampedPropertyOpenDate { get; set; }
        public string StampedPropertySecureMatter { get; set; }
        public string StampedPropertyMatterDescription { get; set; }
        public string StampedPropertyConflictCheckDate { get; set; }
        public string StampedPropertyConflictCheckBy { get; set; }
        public string StampedPropertyDefaultContentType { get; set; }
        public string StampedPropertyIsConflictIdentified { get; set; }
        public string StampedPropertyDocumentTemplateCount { get; set; }
        public string StampedPropertyBlockedUsers { get; set; }
        public string StampedPropertySuccess { get; set; }
        public string StampedPropertyResponsibleAttorneyEmail { get; set; }

        public string PropertyNameVtiIndexedPropertyKeys { get; set; }
        public string ValidDateFormat { get; set; }
        public string ShareListColumnMatterPath { get; set; }
        public string ShareListColumnMailList { get; set; }
        public string DeleteMatterCode { get; set; }
        public string MatterDeletedSuccessfully { get; set; }
        public string MatterNotPresent { get; set; }
        public bool IsContentCheck { get; set; }

        public bool IsMajorVersionEnable { get; set; }
        public bool IsMinorVersionEnable { get; set; }
        public bool IsForceCheckOut { get; set; }
        public string TitleListsPath { get; set; }

        public string MattersListColumnTitle { get; set; }
        public string MattersListColumnClientName { get; set; }
        public string MattersListColumnClientID { get; set; }
        public string MattersListColumnMatterName { get; set; }
        public string MattersListColumnMatterID { get; set; }

        public string MattersListColumnConflictCheckBy { get; set; }
        public string MattersListColumnConflictCheckOn { get; set; }
        public string MattersListColumnConflictIdentified { get; set; }
        public string MattersListColumnBlockUsers { get; set; }
        
        public string MattersListColumnManagingAttorney { get; set; }
        public string MattersListColumnSupport { get; set; }
        public string MatterLandingPageSections { get; set; }


        public string CommonCSSFileLink { get; set; }
        public string JQueryFileName { get; set; }
        public string CommonFolderName { get; set; }
        public string MatterLandingFolderName { get; set; }
        public string MatterLandingCSSFileName { get; set; }
        public string CommonJSFileLink { get; set; }
        public string MatterLandingJSFileName { get; set; }
    }
}
