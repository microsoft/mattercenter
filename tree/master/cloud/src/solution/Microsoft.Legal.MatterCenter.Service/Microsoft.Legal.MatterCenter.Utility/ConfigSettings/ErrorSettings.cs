
// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="ErrorSettings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// This file is used for getting all the error messages that are used in matter center from the appSettings.json file
    /// These properties will subsequently used where ever exceptions messages needs to be set
    /// </summary>
    public class ErrorSettings
    {
        public string IncorrectInputClientUrlCode { get; set; }
        public string IncorrectInputClientUrlMessage { get; set; }
        public string AuthorizationLengthError { get; set; }
        public string NoBearerStringPresent { get; set; }
        public string MessageNoInputs { get; set; }
        public string MessageNoResult { get; set; }
        public string PeoplePickerNoResults { get; set; }



        #region Error Code
        public string UserNotSiteOwnerCode { get; set; }
        public string UserNotSiteOwnerMessage { get; set; }
        public string IncorrectInputUserAccessCode { get; set; }
        public string IncorrectInputUserAccessMessage { get; set; }
        public string IncorrectInputClientIdCode { get; set; }
        public string IncorrectInputClientIdMessage { get; set; }

        public string IncorrectInputClientNameCode { get; set; }
        public string IncorrectInputClientNameMessage { get; set; }

        public string IncorrectInputMatterNameCode { get; set; }
        public string IncorrectInputMatterNameMessage { get; set; }

        public string IncorrectInputMatterIdCode { get; set; }
        public string IncorrectInputMatterIdMessage { get; set; }

        public string IncorrectInputMatterDescriptionCode { get; set; }
        public string IncorrectInputMatterDescriptionMessage { get; set; }

        public string IncorrectInputConflictDateCode { get; set; }
        public string IncorrectInputConflictDateMessage { get; set; }
        public string IncorrectInputConflictIdentifiedCode { get; set; }
        public string IncorrectInputConflictIdentifiedMessage { get; set; }
        public string IncorrectInputBlockUserNamesCode { get; set; }
        public string IncorrectInputBlockUserNamesMessage { get; set; }
        public string IncorrectInputConflictCheckByCode { get; set; }
        public string IncorrectInputConflictCheckByMessage { get; set; }
        public string IncorrectInputUserRolesCode { get; set; }
        public string IncorrectInputUserRolesMessage { get; set; }

        public string IncorrectInputUserNamesCode { get; set; }
        public string IncorrectInputUserNamesMessage { get; set; }
        public string IncorrectInputUserPermissionsCode { get; set; }
        public string IncorrectInputUserPermissionsMessage { get; set; }
        public string IncorrectInputContentTypeCode { get; set; }
        public string IncorrectInputContentTypeMessage { get; set; }
        public string ErrorEditMatterMandatoryPermission { get; set; }


        public string IncorrectInputPracticeGroupCode { get; set; }
        public string IncorrectInputPracticeGroupMessage { get; set; }
        public string IncorrectInputAreaOfLawCode { get; set; }
        public string IncorrectInputAreaOfLawMessage { get; set; }
        public string IncorrectInputSubareaOfLawCode { get; set; }
        public string IncorrectInputSubareaOfLawMessage { get; set; }
        public string IncorrectInputResponsibleAttorneyCode { get; set; }
        public string IncorrectInputResponsibleAttorneyMessage { get; set; }

        public string ErrorCodeSecurityGroupExists { get; set; }
        public string ErrorMessageSecurityGroupExists { get; set; }

        public string IncorrectTeamMembersCode { get; set; }
        public string IncorrectTeamMembersMessage { get; set; }
        public string IncorrectInputSelfPermissionRemoval { get; set; }
        #endregion
    }
}
