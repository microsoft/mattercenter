// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProvisionWebDashboard
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains constants list to be used.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.ProvisionWebDashboard
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// Class contains constants list to be used.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Backslash symbol
        /// </summary>
        public const string Backslash = "\\";

        /// <summary>
        /// This flag is used for running the tool for provisioning matter center home page, required in capital.
        /// </summary>
        public const string MatterCenterHome = "MATTERCENTERHOME";

        /// <summary>
        /// This flag is used for running the tool for provisioning settings page, required in capital.
        /// </summary>
        public const string Settings = "SETTINGS";

        /// <summary>
        /// This flag is used for running the tool for provisioning document details page, required in capital.
        /// </summary>
        public const string DocumentDetails = "DOCUMENTDETAILS";

        /// <summary>
        /// Zone for web part
        /// </summary>
        public const string WebPartZone = "TopZone";

        /// <summary>
        /// Middle web part zone
        /// </summary>
        public const string MiddleWebPartZone = "MiddleLeftZone";

        /// <summary>
        /// Zone index to be used while creating matter page
        /// </summary>
        public const int ZoneIndex = 1;

        /// <summary>
        /// Query to get everything
        /// </summary>
        public const string QueryAll = "<View></View>";

        /// <summary>
        /// Query to get only files
        /// </summary>
        public const string QueryGetSpecificFiles = @"<View><Query><Where><Eq><FieldRef Name=FSObjType /><Value Type=Integer>0</Value></Eq></Where></Query></View>";

        /// <summary>
        /// Error message while creating web part
        /// </summary>
        public const string ErrorMessage = "Error occurred while creating web part";

        /// <summary>
        /// Delete page message
        /// </summary>
        public const string DeletePageMessage = "Deleting matter center pages from: ";

        /// <summary>
        /// Provisioning page message
        /// </summary>
        public const string ProvisioningPageMessage = "Provisioning matter center pages on: ";

        /// <summary>
        /// Provisioning success message
        /// </summary>
        public const string SuccessProvisionPageMessage = "Successfully provisioned the matter center page: ";

        /// <summary>
        /// Message for deleting list
        /// </summary>
        public const string DeleteListMessage = "Deleting configuration list";

        /// <summary>
        /// Message to be displayed after list is deleted
        /// </summary>
        public const string DeletedListMessage = "Deleted configuration list";

        /// <summary>
        /// Message for Invalid parameter
        /// </summary>
        public const string InvalidParameter = "Value should be either true or false";

        /// <summary>
        /// Message for Invalid filename
        /// </summary>
        public const string InvalidParameterFileName = "Value should be True Mattercenterhome/settings/DocumentDetails";

        /// <summary>
        /// Message for Create list
        /// </summary>    
        public const string CreateListMessage = "Creating configuration list";

        /// <summary>
        /// Configuration list already present message
        /// </summary>
        public const string ListAlreadyPresent = "Matter Configuration list is already present";

        /// <summary>
        /// Delete file message
        /// </summary>
        public const string DeleteFileMessage = "Successfully deleted file: ";

        /// <summary>
        /// Delete page success message
        /// </summary>
        public const string DeletePageSuccessMessage = "Deleting matter center pages from: ";

        /// <summary>
        /// Title column
        /// </summary>
        public const string ColumnTitle = "Title";

        /// <summary>
        /// Title column value
        /// </summary>
        public const string TitleColumnValue = "Matter Configurations";

        /// <summary>
        /// Configuration list string
        /// </summary>
        public const string ConfigurationList = "configuration list";

        /// <summary>
        /// Settings page string
        /// </summary>
        public const string SettingsPage = "settings page";

        /// <summary>
        /// Failure message for Provisioning page
        /// </summary>
        public const string ProvisioningPageExceptionMessage = "Exception while Provisioning Matter Center page: {0} \n Stack Trace: {1}";

        /// <summary>
        /// Failure message while creating configuration list
        /// </summary>
        public const string CreateConfigListExceptionMessage = "Exception while creating configurations list: {0}";

        /// <summary>
        /// Failure message while deleting configuration list
        /// </summary>
        public const string DeleteConfigListExceptionMessage = "Exception while deleting configurations list: {0}";

        /// <summary>
        /// Failure message while breaking permission
        /// </summary>
        public const string BreakingPermissionExceptionMessage = "Exception while breaking permission of the {0}: {1}";

        /// <summary>
        ///  Failure message while assigning permission
        /// </summary>
        public const string AssigningPermissionsExceptionMessage = "Exception while assigning permission of the {0}: {1}";

        /// <summary>
        /// Failure message while assigning permissions to users/groups
        /// </summary>
        public const string AssigningPermissionsUsersExceptionMessage = "Exception while assigning permission to user/group: {0} Exception: {1}";
    }
}
