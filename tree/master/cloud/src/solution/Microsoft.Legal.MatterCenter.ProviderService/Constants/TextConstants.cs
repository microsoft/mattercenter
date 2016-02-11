// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-akdigh
// Created          : 11-28-2014
//
// ***********************************************************************
// <copyright file="TextConstants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains messages displayed to user.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    #endregion

    /// <summary>
    /// Provides Text constants used in Matter Center.
    /// </summary>
    internal static class TextConstants
    {
        /// <summary>
        /// The message no inputs
        /// </summary>
        internal static readonly string MessageNoInputs = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Message_No_Inputs", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        internal static readonly string ShareListColumnMailBody = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_Body", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The share list column mail list
        /// </summary>
        internal static readonly string ShareListColumnMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Share_List_Column_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// HTML chunk of the Default content type for Matter creation mail
        /// </summary>
        internal static readonly string MatterMailDefaultContentTypeHtmlChunk = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Default_Content_Type_Html_Chunk", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Subject of the matter creation mail
        /// </summary>
        internal static readonly string MatterMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Request Object message
        /// </summary>
        internal static readonly string IncorrectInputRequestObjectMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Request_Object_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Request Object code
        /// </summary>
        internal static readonly string IncorrectInputRequestObjectCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Request_Object_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client URL message
        /// </summary>
        internal static readonly string IncorrectInputClientUrlMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Url_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client URL code
        /// </summary>
        internal static readonly string IncorrectInputClientUrlCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Url_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Id message
        /// </summary>
        internal static readonly string IncorrectInputClientIdMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Id_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client Id code
        /// </summary>
        internal static readonly string IncorrectInputClientIdCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Id_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client name message
        /// </summary>
        internal static readonly string IncorrectInputClientNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// REGEX pattern for email validation
        /// </summary>
        internal static readonly string EmailValidationRegex = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Email_Validation_Regex", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Client name code
        /// </summary>
        internal static readonly string IncorrectInputClientNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Client_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Practice Group message
        /// </summary>
        internal static readonly string IncorrectInputPracticeGroupMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Practice_Group_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Practice Group code
        /// </summary>
        internal static readonly string IncorrectInputPracticeGroupCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Practice_Group_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Area Of Law message
        /// </summary>
        internal static readonly string IncorrectInputAreaOfLawMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Area_Of_Law_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Area Of Law code
        /// </summary>
        internal static readonly string IncorrectInputAreaOfLawCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Area_Of_Law_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Sub Area Of Law message
        /// </summary>
        internal static readonly string IncorrectInputSubareaOfLawMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Subarea_Of_Law_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Sub Area Of Law code
        /// </summary>
        internal static readonly string IncorrectInputSubareaOfLawCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Subarea_Of_Law_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Responsible Attorney message
        /// </summary>
        internal static readonly string IncorrectInputResponsibleAttorneyMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Responsible_Attorney_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Responsible Attorney code
        /// </summary>
        internal static readonly string IncorrectInputResponsibleAttorneyCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Responsible_Attorney_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter name message
        /// </summary>
        internal static readonly string IncorrectInputMatterNameMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Name_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter name code
        /// </summary>
        internal static readonly string IncorrectInputMatterNameCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Name_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter Id message 
        /// </summary>
        internal static readonly string IncorrectInputMatterIdMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Id_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter Id code
        /// </summary>
        internal static readonly string IncorrectInputMatterIdCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Id_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user name message
        /// </summary>
        internal static readonly string IncorrectInputUserNamesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Names_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user name code
        /// </summary>
        internal static readonly string IncorrectInputUserNamesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Names_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user permission message
        /// </summary>
        internal static readonly string IncorrectInputUserPermissionsMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Permissions_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user permission code
        /// </summary>
        internal static readonly string IncorrectInputUserPermissionsCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Permissions_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Content Type message
        /// </summary>
        internal static readonly string IncorrectInputContentTypeMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Content_Type_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Content Type code
        /// </summary>
        internal static readonly string IncorrectInputContentTypeCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Content_Type_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter description message
        /// </summary>
        internal static readonly string IncorrectInputMatterDescriptionMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Description_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect Matter description code
        /// </summary>
        internal static readonly string IncorrectInputMatterDescriptionCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Matter_Description_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict date message
        /// </summary>
        internal static readonly string IncorrectInputConflictDateMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Date_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict date code
        /// </summary>
        internal static readonly string IncorrectInputConflictDateCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Date_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict identified message
        /// </summary>
        internal static readonly string IncorrectInputConflictIdentifiedMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Identified_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict identified code
        /// </summary>
        internal static readonly string IncorrectInputConflictIdentifiedCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Identified_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user roles message
        /// </summary>
        internal static readonly string IncorrectInputUserRolesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Roles_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect user roles code
        /// </summary>
        internal static readonly string IncorrectInputUserRolesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_User_Roles_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict check by message
        /// </summary>
        internal static readonly string IncorrectInputConflictCheckByMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Check_By_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect conflict check by code
        /// </summary>
        internal static readonly string IncorrectInputConflictCheckByCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Conflict_Check_By_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect block users message
        /// </summary>
        internal static readonly string IncorrectInputBlockUserNamesMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Block_User_Names_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Incorrect block users code
        /// </summary>
        internal static readonly string IncorrectInputBlockUserNamesCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Incorrect_Input_Block_User_Names_Code", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The Is Read Only User key name, used to determine if user is read-only user for particular matter
        /// </summary>
        internal static readonly string IsReadOnlyUser = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Is_Read_Only_User", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The no mail subject
        /// </summary>
        internal static readonly string NoMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "No_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The missing parameters message
        /// </summary>
        internal static readonly string MissingParametersMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Parameter_Missing_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The invalid parameters message
        /// </summary>
        internal static readonly string InvalidParametersMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Parameter_Invalid_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The mail cart mail subject
        /// </summary>
        internal static readonly string MailCartMailSubject = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Mail_Cart_Mail_Subject", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The mail cart mail body
        /// </summary>
        internal static readonly string MailCartMailBody = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Mail_Cart_Mail_Body", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Link to templates document library on content type hub
        /// </summary>
        internal static readonly string FileNotAvailableMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "File_Not_Available_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Success message on deletion of matter
        /// </summary>
        internal static readonly string MatterDeletedSuccessfully = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Deletion_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Holds the message for no data returned from people picker control based on the search term.
        /// </summary>
        internal static readonly string PeoplePickerNoResults = ConstantStrings.GetConfigurationFromResourceFile("Constants", "People_Picker_No_Results_Found", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created
        /// </summary>
        /// <value>Error code when for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created</value>
        internal static readonly string ErrorCodeContentTypes = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code for issue when creation of site column while sending document to OneDrive
        /// </summary>
        /// <value>Error code for issue when creation of site column while sending document to OneDrive</value>
        internal static readonly string ErrorCodeCreateSiteColumn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeCreateSiteColumn", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code for issue when creation of site content type while sending document to OneDrive
        /// </summary>
        /// <value>Error code for issue when creation of site content type while sending document to OneDrive</value>
        internal static readonly string ErrorCodeCreateSiteContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeCreateSiteContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code for issue when assigning default site content type while sending document to OneDrive
        /// </summary>
        /// <value>Error code for issue when assigning default site content type while sending document to OneDrive</value>
        internal static readonly string ErrorCodeAssignDefaultContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeAssignDefaultContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code for issue when OneDrive is not configured for user
        /// </summary>
        /// <value>Error code for issue when OneDrive is not configured for user</value>
        internal static readonly string ErrorCodeOneDriveNotConfigured = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeOneDriveNotConfigured", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message for issue when creation of site column while sending document to OneDrive
        /// </summary>
        /// <value>Error message for issue when creation of site column while sending document to OneDrive</value>
        internal static readonly string ErrorMessageCreateSiteColumn = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageCreateSiteColumn", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message for issue when creation of site content type while sending document to OneDrive
        /// </summary>
        /// <value>Error message for issue when creation of site content type while sending document to OneDrive</value>
        internal static readonly string ErrorMessageCreateSiteContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageCreateSiteContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message for issue when assigning default site content type while sending document to OneDrive
        /// </summary>
        /// <value>Error message for issue when assigning default site content type while sending document to OneDrive</value>
        internal static readonly string ErrorMessageAssignDefaultContentType = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageAssignDefaultContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created
        /// </summary>
        /// <value>Error message when for issue when Content Types selected by the users are not present in the Content Type hub or site collection where the matter library is created</value>
        internal static readonly string ErrorMessageContentTypes = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageContentType", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code when there is an issue while assigning permissions to calendar list
        /// </summary>
        /// <value>Error code when there is an issue while assigning permissions to calendar list</value>
        internal static readonly string ErrorCodeCalendarCreation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeCalendarCreation", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code when there is an issue while assigning permissions to calendar list
        /// </summary>
        /// <value>Error code when there is an issue while assigning permissions to calendar list</value>
        internal static readonly string ErrorMessageTaskCreation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageTaskCreation", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message when there is an issue while assigning permissions to calendar list
        /// </summary>
        /// <value>Error message when there is an issue while assigning permissions to calendar list</value>
        internal static readonly string ErrorMessageCalendarCreation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageCalendarCreation", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code when there is an issue while creating calendar list
        /// </summary>
        /// <value>Error code when there is an issue while creating calendar list</value>
        internal static readonly string ErrorCodeAddCalendarList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeAddCalendarList", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error code when there is an issue while creating task list
        /// </summary>
        /// <value>Error code when there is an issue while creating task list</value>
        internal static readonly string ErrorCodeAddTaskList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorCodeAddTaskList", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message when there is an issue while creating calendar list
        /// </summary>
        /// <value>Error message when there is an issue while creating calendar list</value>
        internal static readonly string ErrorMessageAddCalendarList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageAddCalendarList", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the error message when there is an issue while creating task list
        /// </summary>
        /// <value>Error message when there is an issue while creating task list</value>
        internal static readonly string ErrorMessageAddTaskList = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ErrorMessageAddTaskList", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the body of the matter mail for conflict check
        /// </summary>
        internal static readonly string MatterMailBodyConflictCheck = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Conflict_Check", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the body of the matter mail for Team Members
        /// </summary>
        internal static readonly string MatterMailBodyTeamMembers = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Team_Members", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Gets the body of the matter mail for Matter Information
        /// </summary>
        internal static readonly string MatterMailBodyMatterInformation = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Mail_Body_Matter_Information", Enumerators.ResourceFileLocation.App_GlobalResources);
    }
}