// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-akdigh
// Created          : 03-31-2014
//
// ***********************************************************************
// <copyright file="ConstantStrings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines constants used under current project.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Web;
    #endregion

    /// <summary>
    ///  Contains all constants declaration for Matter Center.
    /// </summary>
    public static class ConstantStrings
    {
        /// <summary>
        /// Mail content type
        /// </summary>
        internal const string MailContentType = "text/plain; charset=us-ascii";

        /// <summary>
        /// The mail media sub type
        /// </summary>
        internal const string MailMediaSubType = "plain";

        /// <summary>
        /// The text media main type
        /// </summary>
        internal const string TextMediaMainType = "text";

        /// <summary>
        /// The HTML media main type
        /// </summary>
        internal const string HtmlMediaMainType = "html";

        /// <summary>
        /// The image media main type
        /// </summary>
        internal const string ImageMediaMainType = "image";

        /// <summary>
        /// The application media main type
        /// </summary>
        internal const string ApplicationMediaMainType = "application";

        /// <summary>
        /// The message media main type
        /// </summary>
        internal const string MessageMediaMainType = "message";

        /// <summary>
        /// The multipart media type
        /// </summary>
        internal const string MultipartMediaType = "MULTIPART/ALTERNATIVE";

        /// <summary>
        /// The mail media type
        /// </summary>
        public const string MailMediaType = "text/plain";

        /// <summary>
        /// Task list template type
        /// </summary>
        public const string TaskListTemplateType = "Tasks";

        /// <summary>
        /// Constant required for HTML file type constant
        /// </summary>
        public const string HtmlFileType = "HTML_x0020_File_x0020_Type";

        /// <summary>
        /// Constant required for impersonation claim type trust
        /// </summary>
        public const string TrustedForImpersonationClaimType = "trustedfordelegation";

        /// <summary>
        /// The mail attachment media main type
        /// </summary>
        public const string MailAttachmentMediaMainType = "message";

        /// <summary>
        /// The exchange service types
        /// </summary>
        public const string ExchangeServiceTypes = "http://schemas.microsoft.com/exchange/services/2006/types";

        /// <summary>
        /// Provision Matter AssignContentType validation value
        /// </summary>
        public const string ProvisionMatterAssignContentType = "5";

        /// <summary>
        /// The Document content type in user's OneDrive.
        /// </summary>
        public const string OneDriveDocumentContentType = "Document";

        /// <summary>
        /// The Folder content type in user's OneDrive.
        /// </summary>
        public const string OneDriveFolderContentType = "Folder";

        /// <summary>
        /// Represents the entity type user in people picker control
        /// </summary>
        public const string PeoplePickerEntityTypeUser = "User";

        /// <summary>
        /// The property associated with content type
        /// </summary>
        public const string OneDriveContentTypeProperty = "ContentTypeId";

        /// <summary>
        /// The mail message receiver header
        /// </summary>
        internal const string MailMessageReceiverHeader = "x-receiver";

        /// <summary>
        /// The mail message sender header
        /// </summary>
        internal const string MailMessageSenderHeader = "x-sender";

        /// <summary>
        /// Event type information
        /// </summary>
        internal const string EventInformation = "INFO";

        /// <summary>
        /// Event type error
        /// </summary>
        internal const string EventError = "ERROR";

        /// <summary>
        /// Event type warning
        /// </summary>
        internal const string EventWarning = "WARNING";

        /// <summary>
        /// Exception Message
        /// </summary>
        internal const string ExceptionMessage = "EXCEPTION MESSAGE:";

        /// <summary>
        /// Class Name
        /// </summary>
        internal const string ClassName = "CLASS NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        internal const string MethodName = "METHOD NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        internal const string LineNumber = "LINE NUMBER:";

        /// <summary>
        /// Default Event ID
        /// </summary>
        internal const int DefaultEventId = 1000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        internal const int LoggingFailedCode = 2000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        internal const string LoggingFailedMessage = "Failed to Logging";

        /// <summary>
        /// Constant required to get OriginalName site column
        /// </summary>
        private static readonly string searchEmailOriginalName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_OriginalName", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// The App web URL where send mail list is deployed
        /// </summary>
        public static readonly string ProvisionMatterAppURL = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Provision_Matter_App_URL", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// The send mail list name
        /// </summary>
        public static readonly string SendMailListName = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Send_Mail_List_Name", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// The log table name
        /// </summary>
        public static readonly string LogTableName = ConstantStrings.GetConfigurationFromResourceFile("Log", "UtilityLogTableName", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Calendar Id
        /// </summary>
        public const string CalendarId = "106";

        /// <summary>
        /// Calendar Name
        /// </summary>
        public const string CalendarName = "Calendar";

        /// <summary>
        /// Alternate name for Calendar list
        /// </summary>
        public const string CalendarNameAlternate = "Calendar List";

        /// <summary>
        /// Top Zone
        /// </summary>
        public const string TopZone = "TopLeftZone";

        /// <summary>
        /// Header Zone
        /// </summary>
        public const string HeaderZone = "HeaderZone";

        /// <summary>
        /// Footer Zone
        /// </summary>
        public const string FooterZone = "FooterZone";

        /// <summary>
        /// Middle left zone
        /// </summary>
        public const string MiddleLeftZone = "MiddleLeftZone";

        /// <summary>
        /// Right Zone
        /// </summary>
        public const string RightZone = "RightZone";

        /// <summary>
        /// Bottom Left Zone
        /// </summary>
        public const string BottomZone = "BottomLeftZone";

        /// <summary>
        /// Middle Center Zone
        /// </summary>
        public const string MiddleCenterZone = "MiddleMiddleZone";

        /// <summary>
        /// Master Page Gallery
        /// </summary>
        public const string MasterPageGallery = "Master Page Gallery";

        /// <summary>
        /// Default layout
        /// </summary>
        public const string DefaultLayout = "ReportCenterLayout";

        /// <summary>
        /// Opening curly brace
        /// </summary>
        public const string OpeningCurlyBrace = "{";

        /// <summary>
        /// Closing curly brace
        /// </summary>
        public const string ClosingCurlyBrace = "}";

        /// <summary>
        /// Style Id displaying Metadata Properties
        /// </summary>
        public const string StyleId = "MetaDataProperties";

        /// <summary>
        /// Hash Symbol
        /// </summary>
        public const string SymbolHash = "#";

        /// <summary>
        /// Page extension
        /// </summary>
        public const string AspxExtension = ".aspx";

        /// <summary>
        /// Resource Extension
        /// </summary>
        public const string ResourceFileExtension = ".resx";

        /// <summary>
        /// Site assets path
        /// </summary>
        public const string SiteAssets = "SiteAssets";

        /// <summary>
        /// The underscore constant
        /// </summary>
        public const string Underscore = "_";

        /// <summary>
        /// The string hyphen
        /// </summary>
        public const string Hyphen = "-";

        /// <summary>
        /// The true constant
        /// </summary>
        public const string TRUE = "true";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string FALSE = "false";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string YES = "Yes";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string NO = "No";

        /// <summary>
        /// The file leaf reference
        /// </summary>
        public const string FileLeafRef = "FileLeafRef";

        /// <summary>
        /// The file reference
        /// </summary>
        public const string FileRef = "FileRef";

        /// <summary>
        /// The constant for code string 
        /// </summary>
        public const string Code = "code:";

        /// <summary>
        /// The constant for value string 
        /// </summary>
        public const string Value = "value:";

        /// <summary>
        /// The service response constant
        /// </summary>
        public const string ServiceResponse = "{{ \"code\": \"{0}\", \"value\": \"{1}\" }}";

        /// <summary>
        /// The service response constant code part
        /// </summary>
        public const string SERVICE_RESPONSE_CODE_PART = "{{ \"code\": \"";

        /// <summary>
        /// The service response constant value part
        /// </summary>
        public const string SERVICE_RESPONSE_VALUE_PART = ", \"value\": \"";

        /// <summary>
        /// The service response constant for save search
        /// </summary>
        public const string ServiceResponseForSearch = "{{  \"value\": \"{0}\" }}";

        /// <summary>
        /// The service response HTML chunk for role information
        /// </summary>
        public const string RoleInfoHtmlChunk = "<div>{0}: {1}</div>";

        /// <summary>
        /// The backward slash constant
        /// </summary>
        public const string BackwardSlash = "\\";

        /// <summary>
        /// The forward slash constant
        /// </summary>
        public const string ForwardSlash = "/";

        /// <summary>
        /// The lists constant
        /// </summary>
        public const string Lists = "Lists";

        /// <summary>
        /// The pipe constant
        /// </summary>
        public const string Pipe = "|";

        /// <summary>
        /// The @ symbol constant
        /// </summary>
        public const string SymbolAt = "@";

        /// <summary>
        /// The (*) asterisk constant
        /// </summary>
        public const string Asterisk = "*";

        /// <summary>
        /// The comma  constant
        /// </summary>
        public const string Comma = ",";

        /// <summary>
        /// The semi colon constant
        /// </summary>
        public const string Semicolon = ";";

        /// <summary>
        /// The space constant
        /// </summary>
        public const string Space = " ";

        /// <summary>
        /// The Period constant
        /// </summary>
        public const string Period = ".";

        /// <summary>
        /// Horizontal tab constant
        /// </summary>
        public const string HorizontalTab = "\t";

        /// <summary>
        /// The Double quote constant
        /// </summary>
        public const string DoubleQuote = "\"";

        /// <summary>
        /// The opening bracket constant
        /// </summary>
        public const string OpeningBracket = "(";

        /// <summary>
        /// The closing bracket constant
        /// </summary>
        public const string ClosingBracket = ")";

        /// <summary>
        /// The opening angular bracket
        /// </summary>
        public const string OpeningAngularBracket = "<";

        /// <summary>
        /// The closing angular bracket
        /// </summary>
        public const string ClosingAngularBracket = ">";

        /// <summary>
        /// The dollar constant
        /// </summary>
        public const string DOLLAR = "$";

        /// <summary>
        /// The colon constant
        /// </summary>
        public const string COLON = ":";

        /// <summary>
        /// The operator ampersand constant
        /// </summary>
        public const string OperatorAmpersand = "&";

        /// <summary>
        /// The ?(question) constant
        /// </summary>
        public const string QUESTION = "?";

        /// <summary>
        /// The client identifier constant
        /// </summary>
        public const string ClientId = "client_id";

        /// <summary>
        /// The (=)operator equal constant
        /// </summary>
        public const string OperatorEqual = "=";

        /// <summary>
        /// The redirect URL constant
        /// </summary>
        public const string RedirectUrl = "redirect_uri";

        /// <summary>
        /// The standard token constant
        /// </summary>
        public const string StandardToken = "{StandardTokens}";

        /// <summary>
        /// The HTTP header constant
        /// </summary>
        public const string HTTP = "http";

        /// <summary>
        /// The HTTPS header constant
        /// </summary>
        public const string HTTPS = "https";

        /// <summary>
        /// The managing attorney value constant
        /// </summary>
        public const string ManagingAttorneyValue = "Responsible Attorney";

        /// <summary>
        /// The practice group value constant
        /// </summary>
        public const string PracticeGroupValue = "Practice Groups";

        /// <summary>
        /// The clients value constant
        /// </summary>
        public const string ClientsValue = "Clients";

        /// <summary>
        /// The AND Operator
        /// </summary>
        public const string OperatorAnd = "AND";

        /// <summary>
        /// The OR operator
        /// </summary>
        public const string OperatorOR = "OR";

        /// <summary>
        /// Constant required for range operator
        /// </summary>
        public const string OperatorRange = "range";

        /// <summary>
        /// Constant required for document library filter condition
        /// </summary>
        public const string DocumentLibraryFilterCondition = "contentclass:STS_List_DocumentLibrary";

        /// <summary>
        /// Constant required for document item filter condition
        /// </summary>
        public const string DocumentItemFilterCondition = "contentclass:STS_ListItem_DocumentLibrary";

        /// <summary>
        /// Constant required for no pinned matters message
        /// </summary>
        public const string NoPinnedMessage = "You do not have any pinned matters/documents";



        /// <summary>
        /// Constant required for extension for OneNote notebook
        /// </summary>
        public const string ExtensionOneNoteNotebook = ".Notebook";

        /// <summary>
        /// Constant required for the URL of the WOPIFrame that allows the user to open the OneNote for a matter
        /// </summary>
        public const string WOPIFrameURL = "_layouts/WopiFrame.aspx?sourcedoc=";

        /// <summary>
        /// Constant required for extension for OneNote table of content
        /// </summary>
        public const string ExtensionOneNoteTableOfContent = ".onetoc2";

        /// <summary>
        /// Constant required for authorization page URL
        /// </summary>
        public const string AuthorizationPage = "_layouts/15/OAuthAuthorize.aspx";

        /// <summary>
        /// Constant required for redirect page URL
        /// </summary>
        public const string RedirectPage = "_layouts/15/AppRedirect.aspx";

        /// <summary>
        /// Constant required for ACS principal name
        /// </summary>
        public const string AcsPrincipalName = "00000001-0000-0000-c000-000000000000";

        /// <summary>
        /// Constant required for ACS metadata end point relative URL
        /// </summary>
        public const string AcsMetadataEndPointRelativeURL = "metadata/json/1";

        /// <summary>
        /// Constant required for S2S protocol
        /// </summary>
        public const string S2SProtocol = "OAuth2";

        /// <summary>
        /// Constant required for delegation issuance
        /// </summary>
        public const string DelegationIssuance = "DelegationIssuance1.0";

        /// <summary>
        /// Constant required for document object
        /// </summary>
        public const string ConstantObjectForDocument = "oFindDocumentConstants";

        /// <summary>
        /// Constant required for find document
        /// </summary>
        public const string ConstantFileForDocument = "FindDocument";

        /// <summary>
        /// Constant required for file for Briefcase
        /// </summary>
        public const string ConstantFileForBriefcase = "LegalBriefcase";

        /// <summary>
        /// Constant required for object for Briefcase
        /// </summary>
        public const string ConstantObjectForBriefcase = "oLegalBriefcaseConstants";

        /// <summary>
        /// Constant required for object for matter
        /// </summary>
        public const string ConstantObjectFormatter = "oFindMatterConstants";

        /// <summary>
        /// The constant file for matter
        /// </summary>
        public const string ConstantFileFormatter = "FindMatter";

        /// <summary>
        /// The constant object for provision
        /// </summary>
        public const string ConstantObjectForProvision = "oMatterProvisionConstants";

        /// <summary>
        /// The constant file for provision
        /// </summary>
        public const string ConstantFileForProvision = "MatterProvision";

        /// <summary>
        /// The constant object for settings
        /// </summary>
        public const string ConstantObjectForSettings = "oSettingsConstants";

        /// <summary>
        /// The constant file for settings
        /// </summary>
        public const string ConstantFileForSettings = "Settings";

        /// <summary>
        /// The constant object for web dashboard
        /// </summary>
        public const string ConstantObjectForWebDashboard = "oWebDashboardConstants";

        /// <summary>
        /// The constant file for web dashboard
        /// </summary>
        public const string ConstantFileForWebDashboard = "WebDashboard";

        /// <summary>
        /// The mail address field regular expression
        /// </summary>
        public const string MaiAddressFieldregex = @"""[^""]*""";

        /// <summary>
        /// The hexadecimal decoder regular expression
        /// </summary>
        public const string HexDecoderRegex = "(\\=([0-9A-F][0-9A-F]))";

        /// <summary>
        /// The new line regular expression
        /// </summary>
        public const string NewLineRegex = @"^\s*$\n";

        /// <summary>
        /// The invalid character regular expression
        /// </summary>
        public const string InvalidCharRegex = @"[\*\?\|\\\t/:""'<>#{}%~&]";

        /// <summary>
        /// The Escape character pattern
        /// </summary>
        public const string EscapeCharacterPattern = @"[\a|\b|\f|\n|\r|\t|\v]";

        /// <summary>
        /// Constant required for email file extension
        /// </summary>
        public const string EmailFileExtension = ".eml";

        /// <summary>
        /// The mail file extension key
        /// </summary>
        public const string MailFileExtensionKey = "fileExtension";

        /// <summary>
        /// The mail sender key
        /// </summary>
        public const string MailSenderKey = "mailSender";

        /// <summary>
        /// The mail search email subject
        /// </summary>
        public const string MailSearchEmailSubject = "searchEmailSubject";

        /// <summary>
        /// The mail received date key
        /// </summary>
        public const string MailReceivedDateKey = "receivedDate";

        /// <summary>
        /// The mail search email from mailbox key
        /// </summary>
        public const string MailSearchEmailFromMailboxKey = "searchEmailFromMailbox";

        /// <summary>
        /// The mail receiver key
        /// </summary>
        public const string MailReceiverKey = "mailReceiver";

        /// <summary>
        /// The mail CC field identifier
        /// </summary>
        public const string MailCCAddressKey = "ccMailAddress";

        /// <summary>
        /// The mail categories identifier
        /// </summary>
        public const string MailCategoriesKey = "categories";

        /// <summary>
        /// The mail sensitivity identifier
        /// </summary>
        public const string MailSensitivityKey = "sensitivity";

        /// <summary>
        /// The mail sent date identifier
        /// </summary>
        public const string MailSentDateKey = "sentDate";

        /// <summary>
        /// Conversation Id identifier
        /// </summary>
        public const string MailConversationIdKey = "conversationId";

        /// <summary>
        /// Conversation Topic identifier
        /// </summary>
        public const string MailConversationTopicKey = "conversationTopic";

        /// <summary>
        /// Has Attachments identifier
        /// </summary>
        public const string MailHasAttachmentsKey = "hasAttachments";

        /// <summary>
        /// The mail attachment key
        /// </summary>
        public const string MailAttachmentKey = "attachment";

        /// <summary>
        /// The mail importance key
        /// </summary>
        public const string MailImportanceKey = "importance";

        /// <summary>
        /// The mail default importance
        /// </summary>
        public const string MailDefaultImportance = "normal";

        /// <summary>
        /// The original name of uploaded mail
        /// </summary>
        public const string MailOriginalName = "originalName";

        /// <summary>
        /// The upload succeeded
        /// </summary>
        public const string UploadSucceeded = "True";

        /// <summary>
        /// The upload failed
        /// </summary>
        public const string UploadFailed = "False";

        /// <summary>
        /// The SOAP envelop URI
        /// </summary>
        public const string SoapEnvelopURI = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// The exchange service message
        /// </summary>
        public const string ExchangeServiceMessage = "http://schemas.microsoft.com/exchange/services/2006/messages";

        /// <summary>
        /// The error code constant
        /// </summary>
        public const string ErrorCode = "{0}" + ConstantStrings.SymbolHash + ConstantStrings.Pipe + ConstantStrings.SymbolHash + "{1}";

        /// <summary>
        ///Provision Matter common validation value
        /// </summary>
        public const string ProvisionMatterCommonValidation = "0";

        /// <summary>
        /// Provision Matter CheckMatterExists validation value
        /// </summary>
        public const string ProvisionMatterCheckMatterExists = "1";

        /// <summary>
        /// Provision Matter CreateMatter validation value
        /// </summary>
        public const string ProvisionMatterCreateMatter = "2";

        /// <summary>
        /// Provision Matter AssignUserPermissions value
        /// </summary>
        public const string ProvisionMatterAssignUserPermissions = "3";

        /// <summary>
        /// Provision Matter MatterLandingPage validation value
        /// </summary>
        public const string ProvisionMatterMatterLandingPage = "4";

        /// <summary>
        /// Provision Matter UpdateMetadataForList validation value
        /// </summary>
        public const string ProvisionMatterUpdateMetadataForList = "6";

        /// <summary>
        /// Provision Matter ShareMatter validation value
        /// </summary>
        public const string ProvisionMatterShareMatter = "7";

        /// <summary>
        /// Edit Matter permission validation value
        /// </summary>
        public const string EditMatterPermission = "8";

        /// <summary>
        /// Base64 string format for the image
        /// </summary>
        internal static string base64ImageFormat = "data:image/png;base64, "; //ends with a space

        /// <summary>
        /// Placeholder for minimum value of date for date filtering in range operator
        /// </summary>
        public const string MinDate = "min";

        /// <summary>
        /// Placeholder for maximum value of date for date filtering in range operator
        /// </summary>
        public const string MaxDate = "max";

        /// <summary>
        /// Modified date column
        /// </summary>
        public const string MODIFIED_DATE_COLUMN = "Modified";

        /// <summary>
        /// Categories text
        /// </summary>
        internal static string categories = "category";

        /// <summary>
        /// The code part of the response
        /// </summary>
        public const string ResponseCode = "code";

        /// <summary>
        /// The value part of the response
        /// </summary>
        public const string ResponseValue = "value";

        /// <summary>
        /// The encoded double quotes
        /// </summary>
        public const string ENCODEDDOUBLEQUOTES = "&quot;";

        /// <summary>
        /// Mail content disposition header for web response
        /// </summary>
        public const string MailContentDispositionHeader = "\"Content-disposition\", \"attachment; filename=Error.txt\"";

        /// <summary>
        /// The string for matter
        /// </summary>
        public const string Matter = "Matter";

        /// <summary>
        /// Constant required to get message for file already exist with the same name
        /// </summary>
        private static string fileAlreadyExistMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "File_Already_Exist_Message", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required to get message for potential duplicate found
        /// </summary>
        private static string filePotentialDuplicateMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "File_Potential_Duplicate_Message", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required for query to get list of files present at the root of document library
        /// </summary>
        private static string folderStructureModified = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Folder_Structure_Modified", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required to get message for identical content
        /// </summary>
        private static string foundIdenticalContentMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Found_Identical_Content_Message", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required to get message for non-identical content
        /// </summary>
        private static string foundNonIdenticalContentMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Found_Non_Identical_Content_Message", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required to get message for content check fail
        /// </summary>
        private static string contentCheckFailed = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Content_Check_Failed", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Constant required to get tolerance for sent date
        /// </summary>
        private static string sentDateTolerance = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Sent_Date_Tolerance", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Redis cache connection string
        /// </summary>
        private static string redisCacheConnectionString = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Redis_Cache_Connection_String", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Error message for Token Request failed exception
        /// </summary>
        internal static string TokenRequestFailedErrorMessage = ConstantStrings.GetConfigurationFromResourceFile("Constants", "TokenRequestFailedErrorMessage", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Error code for Token Request failed exception
        /// </summary>
        internal static string TokenRequestFailedErrorCode = ConstantStrings.GetConfigurationFromResourceFile("Constants", "TokenRequestFailedErrorCode", Enumerators.ResourceFileLocation.bin);

        /// <summary>
        /// Host name key in Azure app settings
        /// </summary>
        public const string APP_SETTINGS_CACHE_HOSTNAME_KEY = "Cache_Host_Name";

        /// <summary>
        /// Primary key in Azure app settings
        /// </summary>
        public const string APP_SETTINGS_CACHE_PRIMARY_KEY = "Cache_Primary_Key";

        /// <summary>
        /// Links static string
        /// </summary>
        public const string LINKS_STATIC_STRING = "Links";

        /// <summary>
        /// Gets the Email CC site column.
        /// </summary>
        /// <value>The Email CC site column.</value>
        public static string SearchEmailCC
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_CC", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Categories site column.
        /// </summary>
        /// <value>The Email Categories site column.</value>
        public static string SearchEmailCategories
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_Categories", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Categories site column.
        /// </summary>
        /// <value>The Email Categories site column.</value>
        public static string SearchEmailSensitivity
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_Sensitivity", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Sent Date site column.
        /// </summary>
        /// <value>The Email Sent Date site column.</value>
        public static string SearchEmailSentDate
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_SentDate", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Conversation Id site column.
        /// </summary>
        /// <value>The Email Conversation Id site column.</value>
        public static string SearchEmailConversationId
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_ConversationId", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Conversation Topic site column.
        /// </summary>
        /// <value>The Email Conversation Topic site column.</value>
        public static string SearchEmailConversationTopic
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_ConversationTopic", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Has Attachments site column.
        /// </summary>
        /// <value>The Email Has Attachments site column.</value>
        public static string SearchEmailHasAttachments
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_HasAttachments", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email From site column.
        /// </summary>
        /// <value>The Email From site column.</value>
        public static string SearchEmailFrom
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_From", Enumerators.ResourceFileLocation.bin); ;
            }
        }

        /// <summary>
        /// Gets the Email From_Mailbox site column.
        /// </summary>
        /// <value>The Email From_Mailbox site column.</value>
        public static string SearchEmailFromMailbox
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_From_Mailbox", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Received Date site column.
        /// </summary>
        /// <value>The Email Received date site column.</value>
        public static string SearchEmailReceivedDate
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_ReceivedDate", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Subject site column.
        /// </summary>
        /// <value>The Email Subject site column.</value>
        public static string SearchEmailSubject
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_Subject", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Receiver site column.
        /// </summary>
        /// <value>The Email Receiver site column.</value>
        public static string SearchEmailTo
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_To", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Document Size site column.
        /// </summary>
        /// <value>The Document Size site column.</value>
        public static string SearchEmailFileSize
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_FileSize", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the OriginalName site column.
        /// </summary>
        /// <value>The original name site column.</value>
        public static string SearchEmailOriginalName
        {
            get
            {
                return searchEmailOriginalName;
            }
        }

        /// <summary>
        /// Gets the redis cache connection string
        /// </summary>
        /// <value>Redis cache connection string</value>
        public static string RedisCacheConnectionString
        {
            get
            {
                return redisCacheConnectionString;
            }
        }

        /// <summary>
        /// Gets the file already exist.
        /// </summary>
        /// <value>The file already exist.</value>
        public static string FileAlreadyExistMessage
        {
            get
            {
                return fileAlreadyExistMessage;
            }
        }

        /// <summary>
        /// Gets the flag if potential duplicate exist.
        /// </summary>
        /// <value>The potential duplicate exist.</value>
        public static string FilePotentialDuplicateMessage
        {
            get
            {
                return filePotentialDuplicateMessage;
            }
        }

        /// <summary>
        /// Gets the message for folder structure modified during upload
        /// </summary>
        /// <value>The get all files in folder query.</value>
        public static string FolderStructureModified
        {
            get
            {
                return folderStructureModified;
            }
        }

        /// <summary>
        /// Gets the get all files in folder query.
        /// </summary>
        /// <value>The get all files in folder query.</value>
        public static string GetAllFilesInFolderQuery
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Get_All_Files_In_Folder_Query", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the Email Attachments site column
        /// </summary>
        /// <value>The Email Attachments site column.</value>
        internal static string SearchEmailAttachments
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_Attachments", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the event viewer source.
        /// </summary>
        /// <value>The event viewer source.</value>
        internal static string EventViewerSource
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "EventViewer_Source", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the event viewer event identifier.
        /// </summary>
        /// <value>The event viewer event identifier.</value>
        internal static string EventViewerEventId
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "EventViewer_EventID", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the name of the event viewer log.
        /// </summary>
        /// <value>The name of the event viewer log.</value>
        internal static string EventViewerLogName
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "EventViewer_LogName", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the azure row key date format.
        /// </summary>
        /// <value>The azure row key date format.</value>
        internal static string AzureRowKeyDateFormat
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "Azure_RowKey_Date_Format", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the is deployed on azure.
        /// </summary>
        /// <value>The is deployed on azure.</value>
        internal static string IsDeployedOnAzure
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "IsDeployedOnAzure", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the cloud storage connection string.
        /// </summary>
        /// <value>The cloud storage connection string.</value>
        internal static string CloudStorageConnectionString
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "CloudStorageConnectionString", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets flag is logging on azure
        /// </summary>
        /// <value> Is Logging enabled On Azure</value>
        internal static string IsLoggingOnAzure
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Log", "IsLoggingOnAzure", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the incorrect parameter message
        /// </summary>
        /// <value>The incorrect parameter message.</value>
        public static string SpecialCharacterExpressionMatterDescription
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Special_Character_Expression_Matter_Description", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the incorrect parameter message
        /// </summary>
        /// <value>The incorrect parameter message.</value>
        public static string SpecialCharacterExpressionMatterId
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Special_Character_Expression_Matter_Id", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the incorrect parameter message
        /// </summary>
        /// <value>The incorrect parameter message.</value>
        public static string SpecialCharacterExpressionMatterTitle
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Special_Character_Expression_Matter_Title", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the error with empty file.
        /// </summary>
        /// <value>
        /// The error with empty file.
        /// </value>
        public static string ErrorEmptyFile
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Error_Empty_File", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the incorrect parameter message
        /// </summary>
        /// <value>The incorrect parameter message.</value>
        public static string SpecialCharacterExpressionContentType
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Special_Character_Expression_Content_Type", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the initialization vector for the encryption
        /// </summary>
        /// <value>The initialization vector for the encryption.</value>
        public static string EncryptionVector
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Encryption_Vector", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the email importance site column name
        /// </summary>
        public static string SearchEmailImportance
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Search_Email_Importance", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the email importance site column name
        /// </summary>
        public static string EditMatterAllowedPermissionLevel
        {
            get
            {
                return ConstantStrings.GetConfigurationFromResourceFile("Constants", "Edit_Matter_Allowed_Permission_Level", Enumerators.ResourceFileLocation.bin);
            }
        }

        /// <summary>
        /// Gets the message to be displayed for identical content
        /// </summary>
        public static string FoundIdenticalContent
        {
            get
            {
                return foundIdenticalContentMessage;
            }
        }

        /// <summary>
        /// Gets the message to be displayed for non-identical content
        /// </summary>
        public static string FoundNonIdenticalContent
        {
            get
            {
                return foundNonIdenticalContentMessage;
            }
        }

        /// <summary>
        /// Gets the message to be displayed when content check fails
        /// </summary>
        public static string ContentCheckFailed
        {
            get
            {
                return contentCheckFailed;
            }
        }

        /// <summary>
        /// Gets the tolerance for sent date
        /// </summary>
        public static string SentDateTolerance
        {
            get
            {
                return sentDateTolerance;
            }
        }



        /// <summary>
        /// Get the value corresponding to specified key from specified resource file
        /// </summary>
        /// <param name="fileName">Name of the resource file</param>
        /// <param name="keyName">Name of the key</param>
        /// <param name="resourceFileLocation">Resource file location</param>
        /// <returns>
        /// Value of the key
        /// </returns>
        public static string GetConfigurationFromResourceFile(string fileName, string keyName, Enumerators.ResourceFileLocation resourceFileLocation)
        {
            string resourceValue = string.Empty;
            ResXResourceReader resxReader = null;
            try
            {
                resxReader = new ResXResourceReader(HttpContext.Current.Server.MapPath(@"~/" + resourceFileLocation + ConstantStrings.ForwardSlash + fileName + ConstantStrings.ResourceFileExtension));
                ResourceSet resourceSet = new ResourceSet(resxReader);
                resourceValue = resourceSet.GetString(keyName);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            finally
            {
                resxReader.Close();
            }
            return resourceValue;
        }

        /// <summary>
        /// Represents existence of matter library, OneNote library, Calendar library, Task library and Matter landing page
        /// </summary>
        public enum MatterPrerequisiteCheck
        {
            /// <summary>
            /// Represents matter library, OneNote library, calendar library or task library existence
            /// </summary>
            LibraryExists,

            /// <summary>
            /// Represents matter landing page existence
            /// </summary>
            MatterLandingPageExists
        }

        /// <summary>
        /// The mail attributes
        /// </summary>
        internal static class MailAttributes
        {
            /// <summary>
            /// The BCC mail attribute
            /// </summary>
            internal const string BCC = "BCC";

            /// <summary>
            /// The cc mail attribute
            /// </summary>
            internal const string CC = "CC";

            /// <summary>
            /// The attachment mail attribute
            /// </summary>
            internal const string ATTACHMENT = "ATTACHMENT";

            /// <summary>
            /// The content description
            /// </summary>
            internal const string CONTENT_DESCRIPTION = "CONTENT-DESCRIPTION";

            /// <summary>
            /// The content disposition
            /// </summary>
            internal const string CONTENT_DISPOSITION = "CONTENT-DISPOSITION";

            /// <summary>
            /// The content identifier
            /// </summary>
            internal const string CONTENT_ID = "CONTENT-ID";

            /// <summary>
            /// The content transfer encoding
            /// </summary>
            internal const string CONTENT_TRANSFER_ENCODING = "CONTENT-TRANSFER-ENCODING";

            /// <summary>
            /// The content type mail attribute
            /// </summary>
            internal const string CONTENT_TYPE = "CONTENT-TYPE";

            /// <summary>
            /// The date mail attribute
            /// </summary>
            internal const string DATE = "DATE";

            /// <summary>
            /// From mail attribute
            /// </summary>
            internal const string FROM = "FROM";

            /// <summary>
            /// The sender mail attribute
            /// </summary>
            internal const string SENDER = "SENDER";

            /// <summary>
            /// The subject mail attribute
            /// </summary>
            internal const string SUBJECT = "SUBJECT";

            /// <summary>
            /// To mail attribute
            /// </summary>
            internal const string TO = "TO";

            /// <summary>
            /// The filename mail attribute
            /// </summary>
            internal const string FILENAME = "FILENAME";

            /// <summary>
            /// The size mail attribute
            /// </summary>
            internal const string SIZE = "SIZE";

            /// <summary>
            /// The creation date mail attribute
            /// </summary>
            internal const string CREATION_DATE = "CREATION-DATE";

            /// <summary>
            /// The modification date mail attribute
            /// </summary>
            internal const string MODIFICATION_DATE = "MODIFICATION-DATE";

            /// <summary>
            /// The read date mail attribute
            /// </summary>
            internal const string READ_DATE = "READ-DATE";

            /// <summary>
            /// The 7biT encoding
            /// </summary>
            internal const string BIT_7 = "7BIT";

            /// <summary>
            /// The 8biT encoding
            /// </summary>
            internal const string BIT_8 = "8BIT";

            /// <summary>
            /// The quoted printable
            /// </summary>
            internal const string QUOTED_PRINTABLE = "QUOTED-PRINTABLE";

            /// <summary>
            /// The base 64 encoding
            /// </summary>
            internal const string BASE64 = "BASE64";

            /// <summary>
            /// The importance mail attribute
            /// </summary>
            internal const string IMPORTANCE = "IMPORTANCE";

            /// <summary>
            /// The mail categories attribute
            /// </summary>
            internal const string CATEGORIES = "KEYWORDS";

            /// <summary>
            /// The mail received attribute
            /// </summary>
            internal const string RECEIVED = "RECEIVED";
        }
    }
}