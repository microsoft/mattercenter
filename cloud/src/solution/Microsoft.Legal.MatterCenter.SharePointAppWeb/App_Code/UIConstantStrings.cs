// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.SharePointAppWeb
// Author           : v-nikhid
// Created          : 06-16-2014
//
// ***********************************************************************
// <copyright file="UIConstantStrings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file holds all the constants used in the app web.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.SharePointAppWeb
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    #endregion

    /// <summary>
    /// Holds all the variables used in the app web.
    /// </summary>
    public static class UIConstantStrings
    {
        /// <summary>
        /// Query parameter app type
        /// </summary>
        internal const string QUERY_PARAMETER_APPTYPE = "appType";

        /// <summary>
        /// Name for refreshToken cookie
        /// </summary>
        internal const string refreshToken = "refreshToken";

        /// <summary>
        /// Name of SpAppToken
        /// </summary>
        internal const string SPAppToken = "SPAppToken";

        /// <summary>
        /// Name of Global constants file object
        /// </summary>
        internal const string GlobalConstants = "oGlobalConstants";

        /// <summary>
        /// IsEdit query string
        /// </summary>
        internal const string IsEdit = "IsEdit";

        /// <summary>
        /// matterName query string
        /// </summary>
        internal const string matterName = "matterName";

        /// <summary>
        /// clientUrl query string
        /// </summary>
        internal const string clientUrl = "clientUrl";

        /// <summary>
        /// clientDetails query string
        /// </summary>
        internal const string clientDetails = "clientDetails";

        /// <summary>
        /// The is deployed on azure
        /// </summary>
        private static string isDeployedOnAzure = ConstantStrings.GetConfigurationFromResourceFile("Constants", "IsDeployedOnAzure", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The log table name
        /// </summary>
        private static string logTableName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "UILogTableName", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The message with no inputs
        /// </summary>
        private static string messageNoInputs = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Message_No_Inputs", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The App redirect URL
        /// </summary>
        private static string appRedirectURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "App_Redirect_URL", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The client identifier
        /// </summary>
        private static string clientID = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ClientID", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The central repository
        /// </summary>
        private static string centralRepository = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Central_Repository_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The service path find matter for outlook
        /// </summary>
        private static string servicePathFindMatter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "ServicePath_FindMatter", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The error having invalid character
        /// </summary>
        private static string errorInvalidCharacter = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Error_Invalid_Character", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The temp email name
        /// </summary>
        private static string emailName = ConstantStrings.GetConfigurationFromResourceFile("WebDashboard", "Temp_Email_Name", Enumerators.ResourceFileLocation.App_LocalResources);

        /// <summary>
        /// The provision matter app name
        /// </summary>
        private static string provisionMatterName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "App_Name_Provision_Matters", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// 
        /// </summary>
        private static string timeStampFormat = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Time_Stamp_Format", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The tenant URL
        /// </summary>
        private static string siteURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Site_Url", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The sign out page URL
        /// </summary>
        private static string signOutURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Sign_Out_URL", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The user photo URL
        /// </summary>
        private static string userPhotoURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "User_Photo_Src", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The personal URL separator to extract my-site URL and account name
        /// </summary>
        private static string personalURLSeparator = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Perosnal_URL_Separator", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The access denied message for adding or editing team members with insufficient permissions
        /// </summary>
        private static string editMatterAccessDeniedMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Edit_Matter_Access_Denied_Message", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Query parameter value for Outlook app
        /// </summary>
        private static string is_Outlook = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Querystring_Outlook", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Query parameter value for Office app
        /// </summary>
        private static string is_Office = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Querystring_Office", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Query parameter value in case of Token Request failed exception
        /// </summary>
        internal static string TokenRequestFailedQueryString = ConstantStrings.GetConfigurationFromResourceFile("Constants", "TokenRequestFailedQueryString", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// Expiration period of refresh token cookie in hours
        /// </summary>
        internal static string RefreshTokenCookieExpiration = ConstantStrings.GetConfigurationFromResourceFile("Constants", "RefreshTokenCookieExpiration", Enumerators.ResourceFileLocation.App_GlobalResources);

        /// <summary>
        /// The query string for Edit Matter. The private variable is required as it has both getter and setter property.
        /// </summary>
        private static string editMatterQueryString = string.Empty;

        /// <summary>
        /// The query string for settings page.
        /// </summary>
        private static string settingsPageQueryString = string.Empty;

        /// <summary>
        /// The query string for Send To OneDrive page.
        /// </summary>
        private static string sendToOneDriveQueryString = string.Empty;

        /// <summary>
        /// The user photo small size code
        /// </summary>
        internal static string userPhotoSmall = "S";

        /// <summary>
        /// The user photo medium size code
        /// </summary>
        internal static string userPhotoMedium = "M";

        /// <summary>
        /// Title attribute used to set the tooltip
        /// </summary>
        internal static string titleAttribute = "title";

        /// <summary>
        /// User access for Provision Matter
        /// </summary>
        internal static bool provisionMatterAccess = false;

        /// <summary>
        /// Get the deployment status
        /// </summary>
        /// <value>
        /// The name of the Azure deployment flag.
        /// </value>
        internal static string IsDeployedOnAzure
        {
            get
            {
                return isDeployedOnAzure;
            }
        }

        /// <summary>
        /// Gets the name of the log table.
        /// </summary>
        /// <value>
        /// The name of the log table.
        /// </value>
        internal static string LogTableName
        {
            get
            {
                return logTableName;
            }
        }

        /// <summary>
        /// Gets the name of the temporary email.
        /// </summary>
        /// <value>
        /// The name of the temporary email.
        /// </value>
        internal static string EmailName
        {
            get
            {
                return emailName;
            }
        }

        /// <summary>
        /// Gets the message with no inputs.
        /// </summary>
        /// <value>
        /// The message no inputs.
        /// </value>
        internal static string MessageNoInputs
        {
            get
            {
                return messageNoInputs;
            }
        }

        /// <summary>
        /// Gets the application redirect URL.
        /// </summary>
        /// <value>
        /// The application redirect URL.
        /// </value>
        internal static string AppRedirectURL
        {
            get
            {
                return appRedirectURL;
            }
        }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        internal static string ClientID
        {
            get
            {
                return clientID;
            }
        }

        /// <summary>
        /// Gets the central repository.
        /// </summary>
        /// <value>
        /// The central repository.
        /// </value>
        internal static string CentralRepository
        {
            get
            {
                return centralRepository;
            }
        }

        /// <summary>
        /// Gets the service path find matter for outlook.
        /// </summary>
        /// <value>
        /// The service path find matter for outlook.
        /// </value>
        internal static string ServicePathFindMatter
        {
            get
            {
                return servicePathFindMatter;
            }
        }

        /// <summary>
        /// Gets the error with invalid character.
        /// </summary>
        /// <value>
        /// The error with invalid character.
        /// </value>
        internal static string ErrorInvalidCharacter
        {
            get
            {
                return errorInvalidCharacter;
            }
        }

        /// <summary>
        /// Get the query string parameter value for Outlook app
        /// </summary>        
        /// <value>
        /// The value of the Outlook app that has been used in query parameter
        /// </value>
        internal static string IS_OUTLOOK
        {
            get
            {
                return is_Outlook;
            }
        }

        /// <summary>
        /// Get the query string  parameter value for Office app
        /// </summary>                
        /// <value>
        /// The value for the Office app that has been used in query parameter
        /// </value>
        internal static string IS_OFFICE
        {
            get
            {
                return is_Office;
            }
        }

        /// <summary>
        /// Get the URL Referrer Cookie Name
        /// </summary>
        /// <value>
        /// The name of the URL Referrer Cookie.
        /// </value>
        internal static string URLReferrerCookieName
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "URLReferrerCookieName", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Get the Request Token Cookie Name
        /// </summary>
        /// <value>
        /// The name of the Request Token Cookie Name.
        /// </value>
        internal static string RequestTokenCookieName
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "RequestTokenCookieName", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Gets office.js file path
        /// </summary>
        public static string OfficeJSPath
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Office_JS_URL", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Get the Request Token Cookie Name
        /// </summary>
        /// <value>
        /// The name of the Request Token Cookie Name.
        /// </value>
        internal static string ProvisionMatterName
        {
            get
            {
                return provisionMatterName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static string TimeStampFormat
        {
            get
            {
                return timeStampFormat;
            }
        }

        /// <summary>
        /// Get the tenant site URL
        /// </summary>
        internal static string SiteURL
        {
            get
            {
                return siteURL;
            }
        }

        /// <summary>
        /// Get the sign out page URL
        /// </summary>
        internal static string SignOutURL
        {
            get
            {
                return signOutURL;
            }
        }

        /// <summary>
        /// Get the sign out page URL
        /// </summary>
        internal static string UserPhotoURL
        {
            get
            {
                return userPhotoURL;
            }
        }

        /// <summary>
        /// Get the sign out page URL
        /// </summary>
        internal static string PersonalURLSeparator
        {
            get
            {
                return personalURLSeparator;
            }
        }

        /// <summary>
        /// Get the access denied message for adding or editing team members with insufficient permissions
        /// </summary>
        internal static string EditMatterAccessDeniedMessage
        {
            get
            {
                return editMatterAccessDeniedMessage;
            }
        }

        /// <summary>
        /// The query string for Edit Matter
        /// </summary>
        internal static string EditMatterQueryString
        {
            get
            {
                return editMatterQueryString;
            }
            set
            {
                editMatterQueryString = value;
            }
        }

        /// <summary>
        /// The query string for settings page
        /// </summary>
        internal static string SettingsPageQueryString
        {
            get
            {
                return settingsPageQueryString;
            }
            set
            {
                settingsPageQueryString = value;
            }
        }

        /// <summary>
        /// The query string for Send To OneDrive page
        /// </summary>
        internal static string SendToOneDriveQueryString
        {
            get
            {
                return sendToOneDriveQueryString;
            }
            set
            {
                sendToOneDriveQueryString = value;
            }
        }

        /// <summary>
        /// Get access denied message for the settings page
        /// </summary>
        /// <value>
        /// The access denied message for the settings page
        /// </value>
        internal static string SettingsPageAccessDeniedMessage
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Settings_Page_Access_Denied_Message", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Get the Matter Center support URL 
        /// </summary>
        /// <value>
        /// The email of Matter Center support
        /// </value>
        internal static string MatterCenterSupportEmail
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Center_Support_Email", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }

        /// <summary>
        /// Boolean variable for user access for Provision Matter
        /// </summary>
        internal static bool ProvisionMatterAccess
        {
            get
            {
                return provisionMatterAccess;
            }
            set
            {
                provisionMatterAccess = value;
            }
        }
        /// <summary>
        /// Get tenant URL for the Matter Center app
        /// </summary>
        /// <value>
        /// The tenant URL for the Matter Center app
        /// </value>
        internal static string TenantUrl
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Tenant_Url", Enumerators.ResourceFileLocation.App_GlobalResources);
            }
        }
    }
}
