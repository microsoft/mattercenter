// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="ServiceConstants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************

namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// All the constants that are used in matter center will be defined here
    /// </summary>
    public class ServiceConstants
    {

        public const string SUCCESS = "200";
        /// <summary>
        /// The underscore constant
        /// </summary>
        public const string UNDER_SCORE = "_";

        /// <summary>
        /// The invalid char regex
        /// </summary>
        public const string INVALID_CHARREGEX = @"[\*\?\|\\\t/:""'<>#{}%~&]";

        /// <summary>
        /// The start end regex
        /// </summary>
        public const string START_END_REGEX = @"^[\. ]|[\. ]$";

        /// <summary>
        /// The invalid rule regex
        /// </summary>
        public const string INVALID_RULE_REGEX = @"\.{2,}";

        /// <summary>
        /// The extra space regex
        /// </summary>
        public const string EXTRA_SPACE_REGEX = " {2,}";

        /// <summary>
        /// The mail address field regular expression
        /// </summary>
        public const string MAIL_ADDRESS_FIELD_REGEX = @"""[^""]*""";

        /// <summary>
        /// The invalid file name regex
        /// </summary>
        public const string INVALID_FILENAME_REGEX = "_fajlovi|.files|-Dateien|_fichiers|_bestanden|_file|_archivos|-filer|_tiedostot|_pliki|_soubory|_elemei|_ficheiros|_arquivos|_dosyalar|_datoteke|_fitxers|_failid|_fails|_bylos|_fajlovi|_fitxategiak$";



        #region Azure Cache keys
        /// <summary>
        /// Clients cache key
        /// </summary>
        public const string CACHE_CLIENTS = "Clients";

        /// <summary>
        /// Matter type cache key
        /// </summary>
        public const string CACHE_MATTER_TYPE = "MatterType";

        /// <summary>
        /// Roles cache key
        /// </summary>
        public const string CACHE_ROLES = "Roles";

        /// <summary>
        /// The encoded double quotes
        /// </summary>
        public const string ENCODED_DOUBLE_QUOTES = "&quot;";

        /// <summary>
        /// The Double quote constant
        /// </summary>
        public const string DOUBLE_QUOTE = "\"";

        /// <summary>
        /// The (*) asterisk constant
        /// </summary>
        public const string ASTERISK = "*";

        /// <summary>
        /// The comma  constant
        /// </summary>
        public const string COMMA = ",";

        /// <summary>
        /// The semi colon constant
        /// </summary>
        public const string SEMICOLON = ";";

        /// <summary>
        /// The space constant
        /// </summary>
        public const string SPACE = " ";

        /// <summary>
        /// The Period constant
        /// </summary>
        public const string PERIOD = ".";

        /// <summary>
        /// Horizontal tab constant
        /// </summary>
        public const string HORIZONTAL_TAB = "\t";       

        /// <summary>
        /// The opening bracket constant
        /// </summary>
        public const string OPENING_BRACKET = "(";

        /// <summary>
        /// The backward slash constant
        /// </summary>
        public const string BACKWARD_SLASH = "\\";

        /// <summary>
        /// The forward slash constant
        /// </summary>
        public const string FORWARD_SLASH = "/";

        /// <summary>
        /// The lists constant
        /// </summary>
        public const string LISTS = "Lists";

        /// <summary>
        /// The pipe constant
        /// </summary>
        public const string PIPE = "|";

        /// <summary>
        /// Hash Symbol
        /// </summary>
        public const string SYMBOL_HASH = "#";

        /// <summary>
        /// The @ symbol constant
        /// </summary>
        public const string SYMBOL_AT = "@";

        /// <summary>
        /// The closing bracket constant
        /// </summary>
        public const string CLOSING_BRACKET = ")";

        /// <summary>
        /// The opening angular bracket
        /// </summary>
        public const string OPENING_ANGULAR_BRACKET = "<";

        /// <summary>
        /// The closing angular bracket
        /// </summary>
        public const string CLOSING_ANGULAR_BRACKET = ">";

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
        public const string OPERATOR_AMPERSAND = "&";

        /// <summary>
        /// The ?(question) constant
        /// </summary>
        public const string QUESTION = "?";

        /// <summary>
        /// The OR operator
        /// </summary>
        public const string OPERATOR_OR = "OR";

        /// <summary>
        /// The AND operator
        /// </summary>
        public const string OPERATOR_AND = "AND";

        /// <summary>
        /// Constant required for range operator
        /// </summary>
        public const string OPERATOR_RANGE = "range";


        /// <summary>
        /// Placeholder for minimum value of date for date filtering in range operator
        /// </summary>
        public const string MIN_DATE = "min";

        /// <summary>
        /// Placeholder for maximum value of date for date filtering in range operator
        /// </summary>
        public const string MAX_DATE = "max";

        /// <summary>
        /// Constant required for document library filter condition
        /// </summary>
        public const string DOCUMENT_LIBRARY_FILTER_CONDITION = "contentclass:STS_List_DocumentLibrary";

        /// <summary>
        /// Constant required for document item filter condition
        /// </summary>
        public const string DOCUMENT_ITEM_FILTER_CONDITION = "contentclass:STS_ListItem_DocumentLibrary";

        /// <summary>
        /// Open square brace
        /// </summary>
        public const string OPEN_SQUARE_BRACE = "[";

        /// <summary>
        /// The (=)operator equal constant
        /// </summary>
        public const string OPERATOR_EQUAL = "=";

        /// <summary>
        /// Close square brace
        /// </summary>
        public const string CLOSE_SQUARE_BRACE = "]";

        /// <summary>
        /// Name of the path field in results returned from SharePoint search
        /// </summary>
        public const string PATH_FIELD_NAME = "Path";

        /// <summary>
        /// Event type information
        /// </summary>
        public const string EVENT_INFORMATION = "INFO";

        /// <summary>
        /// Event type error
        /// </summaryClassName
        public const string EVENT_ERROR = "ERROR";

        /// <summary>
        /// Event type warning
        /// </summary>
        public const string EVENT_WARNING = "WARNING";

        /// <summary>
        /// Exception Message
        /// </summary>
        public const string EXCEPTION_MESSAGE = "EXCEPTION MESSAGE:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string CLASS_NAME = "CLASS NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string METHOD_NAME = "METHOD NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string LINE_NUMBER = "LINE NUMBER:";

        /// <summary>
        /// Default Event ID
        /// </summary>
        public const int DEFAULT_EVENT_ID = 1000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        public const int LOGGING_FAILED_CODE = 2000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        public const string LOGGING_FAILED_MESSAGE = "Failed to Logging";

        /// <summary>
        /// The error code constant
        /// </summary>
        public const string ERROR_CODE = "{0}" + SYMBOL_HASH + PIPE + SYMBOL_HASH + "{1}";

        /// <summary>
        /// The Escape character pattern
        /// </summary>
        public const string ESCAPE_CHARACTER_PATTERN = @"[\a|\b|\f|\n|\r|\t|\v]";

        /// <summary>
        /// Constant required for no pinned matters message
        /// </summary>
        public const string NO_PINNED_MESSAGE = "You do not have any pinned matters/documents";

        /// <summary>
        /// The column name for document GUID
        /// </summary>
        public const string DOCUMENT_GUID_COLUMN_NAME = "UniqueId";

        /// <summary>
        /// Links static string
        /// </summary>
        public const string LINKS_STATIC_STRING = "Links";

        /// <summary>
        /// Represents the entity type user in people picker control
        /// </summary>
        public const string PeoplePickerEntityTypeUser = "User";

        /// <summary>
        /// Provision Matter CreateMatter validation value
        /// </summary>
        public const string ProvisionMatterCreateMatter = "2";

        /// <summary>
        ///Provision Matter common validation value
        /// </summary>
        public const string ProvisionMatterCommonValidation = "0";

        /// <summary>
        /// Provision Matter CheckMatterExists validation value
        /// </summary>
        public const string ProvisionMatterCheckMatterExists = "1";

        

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
        /// Provision Matter AssignContentType validation value
        /// </summary>
        public const string ProvisionMatterAssignContentType = "5";

        

        /// <summary>
        /// The false constant
        /// </summary>
        public const string FALSE = "false";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string TRUE = "true";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string YES = "Yes";

        /// <summary>
        /// The false constant
        /// </summary>
        public const string NO = "No";

        /// <summary>
        /// Page extension
        /// </summary>
        public const string ASPX_EXTENSION = ".aspx";
        /// <summary>
        /// The service response HTML chunk for role information
        /// </summary>
        public const string RoleInfoHtmlChunk = "<div>{0}: {1}</div>";

        /// <summary>
        /// The HTTPS header constant
        /// </summary>
        public const string HTTPS = "https";

        /// <summary>
        /// The HTTP header constant
        /// </summary>
        public const string HTTP = "http";

        /// <summary>
        /// The string for matter
        /// </summary>
        public const string MATTER = "Matter";

        /// <summary>
        /// Modified date column
        /// </summary>
        public const string MODIFIED_DATE_COLUMN = "Modified";


        /// <summary>
        /// String to be used for creating default value for metadata. This string is in following format: WSSID;#VAL|GUID
        /// </summary>
        public const string MetadataDefaultValue = "{0};#{1}|{2}";
        #endregion

        #region Mail

        /// <summary>
        /// The SOAP envelop URI
        /// </summary>
        public const string SOAP_ENVELOP_URI = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// The exchange service message
        /// </summary>
        public const string EXCHANGE_SERVICE_MESSAGE = "http://schemas.microsoft.com/exchange/services/2006/messages";

        /// <summary>
        /// Constant required for email file extension
        /// </summary>
        public const string EMAIL_FILE_EXTENSION = ".eml";

        /// <summary>
        /// The mail file extension key
        /// </summary>
        public const string MAIL_FILE_EXTENSION_KEY = "fileExtension";

        /// <summary>
        /// The mail sender key
        /// </summary>
        public const string MAIL_SENDER_KEY = "mailSender";

        /// <summary>
        /// The mail search email subject
        /// </summary>
        public const string MAIL_SEARCH_EMAIL_SUBJECT = "searchEmailSubject";

        /// <summary>
        /// The mail received date key
        /// </summary>
        public const string MAIL_RECEIVED_DATEKEY = "receivedDate";

        /// <summary>
        /// The mail search email from mailbox key
        /// </summary>
        public const string MAIL_SEARCH_EMAIL_FROM_MAIL_BOX_KEY = "searchEmailFromMailbox";

        /// <summary>
        /// The mail receiver key
        /// </summary>
        public const string MAIL_RECEIVER_KEY = "mailReceiver";

        /// <summary>
        /// The mail CC field identifier
        /// </summary>
        public const string MAIL_CC_ADDRESS_KEY = "ccMailAddress";
        
        /// <summary>
        /// The mail categories identifier
        /// </summary>
        public const string MAIL_CATEGORIES_KEY = "categories";

        /// <summary>
        /// The mail sensitivity identifier
        /// </summary>
        public const string MAIL_SENSITIVITY_KEY = "sensitivity";

        /// <summary>
        /// The mail sent date identifier
        /// </summary>
        public const string MAIL_SENT_DATE_KEY = "sentDate";

        /// <summary>
        /// Conversation Id identifier
        /// </summary>
        public const string MAIL_CONVERSATIONID_KEY = "conversationId";

        /// <summary>
        /// Conversation Topic identifier
        /// </summary>
        public const string MAIL_CONVERSATION_TOPIC_KEY = "conversationTopic";

        /// <summary>
        /// Has Attachments identifier
        /// </summary>
        public const string MAIL_HAS_ATTACHMENTS_KEY = "hasAttachments";

        /// <summary>
        /// The mail attachment key
        /// </summary>
        public const string MAIL_ATTACHMENT_KEY = "attachment";

        /// <summary>
        /// The mail importance key
        /// </summary>
        public const string MAIL_IMPORTANCE_KEY = "importance";

        /// <summary>
        /// The mail default importance
        /// </summary>
        public const string MAIL_DEFAULT_IMPORTANCE = "normal";

        /// <summary>
        /// The original name of uploaded mail
        /// </summary>
        public const string MAIL_ORIGINAL_NAME = "originalName";

        /// <summary>
        /// Categories text
        /// </summary>
        public const string CATEGORIES = "category";

       

        /// <summary>
        /// The upload succeeded
        /// </summary>
        public const bool UPLOAD_SUCCEEDED = true;

        /// <summary>
        /// The upload failed
        /// </summary>
        public const bool UPLOAD_FAILED = false;
        /// <summary>
        /// Mail content type
        /// </summary>
        public const string MAIL_CONTENT_TYPE = "text/plain; charset=us-ascii";

        /// <summary>
        /// The mail media sub type
        /// </summary>
        public const string MAIL_MEDIA_SUBTYPE = "plain";

        /// <summary>
        /// The text media main type
        /// </summary>
        public const string TEXT_MEDIA_MAIN_TYPE = "text";

        /// <summary>
        /// The HTML media main type
        /// </summary>
        public const string HtmlMediaMainType = "html";

        /// <summary>
        /// The image media main type
        /// </summary>
        public const string IMAGE_MEDIA_MAIN_TYPE = "image";

        /// <summary>
        /// The application media main type
        /// </summary>
        public const string APPLICATION_MEDIA_MAIN_TYPE = "application";

        /// <summary>
        /// The message media main type
        /// </summary>
        public const string MESSAGE_MEDIA_MAIN_TYPE = "message";

        /// <summary>
        /// The multipart media type
        /// </summary>
        public const string MULTI_PART_MEDIA_TYPE = "MULTIPART/ALTERNATIVE";

        /// <summary>
        /// The string hyphen
        /// </summary>
        public const string HYPHEN = "-";

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
        public const string TRUSTED_FOR_IMPERSONATION_CLAIMTYPE = "trustedfordelegation";

        /// <summary>
        /// The mail attachment media main type
        /// </summary>
        public const string MAIL_ATTACHMENT_MEDIA_MAINT_YPE = "message";

        /// <summary>
        /// The mail search email from mailbox key
        /// </summary>
        public const string MAIL_SEARCH_EMAIL_FROM_MAILBOX_KEY = "searchEmailFromMailbox";

        /// <summary>
        /// The exchange service types
        /// </summary>
        public const string EXCHANGE_SERVICE_TYPES = "http://schemas.microsoft.com/exchange/services/2006/types";

        /// <summary>
        /// The hexadecimal decoder regular expression
        /// </summary>
        public const string HEX_DECODER_REGEX = "(\\=([0-9A-F][0-9A-F]))";


        /// <summary>
        /// The Document content type in user's OneDrive.
        /// </summary>
        public const string ONEDRIVE_DOCUMENT_CONTENTTYPE = "Document";

        /// <summary>
        /// The Folder content type in user's OneDrive.
        /// </summary>
        public const string ONEDRIVE_FOLDER_CONTENTTYPE = "Folder";

       
        /// <summary>
        /// The property associated with content type
        /// </summary>
        public const string OneDrive_ContentType_Property = "ContentTypeId";

        /// <summary>
        /// The mail message receiver header
        /// </summary>
        public const string Mail_Message_Receiver_Header = "x-receiver";

        /// <summary>
        /// The mail message sender header
        /// </summary>
        public const string Mail_Message_Sender_Header = "x-sender";

        /// <summary>
        /// Event type information
        /// </summary>
        public const string Event_Information = "INFO";

        /// <summary>
        /// Event type error
        /// </summary>
        public const string Event_Error = "ERROR";

        /// <summary>
        /// Event type warning
        /// </summary>
        public const string EventWarning = "WARNING";

        /// <summary>
        /// Exception Message
        /// </summary>
        public const string ExceptionMessage = "EXCEPTION MESSAGE:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string ClassName = "CLASS NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string MethodName = "METHOD NAME:";

        /// <summary>
        /// Class Name
        /// </summary>
        public const string LineNumber = "LINE NUMBER:";

        /// <summary>
        /// Default Event ID
        /// </summary>
        public const int DefaultEventId = 1000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        public const int LoggingFailedCode = 2000;

        /// <summary>
        /// Default Failed to logging code
        /// </summary>
        public const string LoggingFailedMessage = "Failed to Logging";


        /// <summary>
        /// The mail SOAP request
        /// </summary>
        public const string MAIL_SOAP_REQUEST =
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                    xmlns:m=""http://schemas.microsoft.com/exchange/services/2006/messages""
                    xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"" 
                    xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Header>
                    <t:RequestServerVersion Version=""Exchange2013""/>
                    </soap:Header>
                    <soap:Body>
                    <m:GetItem>
                    <m:ItemShape>
                        <t:BaseShape>Default</t:BaseShape>
                        <t:IncludeMimeContent>true</t:IncludeMimeContent>
                        <t:AdditionalProperties>
		                    <t:FieldURI FieldURI=""item:ConversationId""/>		          
			                <t:FieldURI FieldURI=""message:ConversationTopic""/>			  
        		        </t:AdditionalProperties>
                    </m:ItemShape>
                    <m:ItemIds>
                    <t:ItemId Id=""{0}""/>
                    </m:ItemIds>
                    </m:GetItem>
                    </soap:Body>
                    </soap:Envelope>";

        /// <summary>
        /// The attachment SOAP request
        /// </summary>
        public const string ATTACHMENT_SOAP_REQUEST =
                     @"<?xml version=""1.0"" encoding=""utf-8""?>
                     <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                     xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                     xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
                     xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"">
                     <soap:Header>
                     <t:RequestServerVersion Version=""Exchange2013"" />
                     </soap:Header>
                     <soap:Body>
                     <GetAttachment xmlns=""http://schemas.microsoft.com/exchange/services/2006/messages""
                      xmlns:t=""http://schemas.microsoft.com/exchange/services/2006/types"">
                     <AttachmentShape>
                     <t:IncludeMimeContent>true</t:IncludeMimeContent>
                        <t:AdditionalProperties>
		                    <t:FieldURI FieldURI=""item:ConversationId""/>		                    
			                <t:FieldURI FieldURI=""message:ConversationTopic""/>			                
        		        </t:AdditionalProperties>
                     </AttachmentShape>
                     <AttachmentIds>
                     <t:AttachmentId Id=""{0}""/>
                     </AttachmentIds>
                     </GetAttachment>
                     </soap:Body>
                    </soap:Envelope>";


        #endregion
        

        /// <summary>
        /// The Attachments tag in Exchange response
        /// </summary>
        public const string ATTACHMENTS_TAG = "Attachments";

        /// <summary>
        /// The FileAttachment tag in Exchange response
        /// </summary>
        public const string FILE_ATTACHMENT_TAG = "FileAttachment";

        /// <summary>
        /// The ItemAttachment tag in Exchange response
        /// </summary>
        public const string ITEM_ATTACHMENT_TAG = "ItemAttachment";

        /// <summary>
        /// The Name tag in Exchange response
        /// </summary>
        public const string FILE_NAME_TAG = "Name";

        /// <summary>
        /// The AttachmentID tag in Exchange response
        /// </summary>
        public const string ATTACHMENT_ID_TAG = "AttachmentId";

        /// <summary>
        /// The Id attribute in Exchange response
        /// </summary>
        public const string ID_ATTRIBUTE = "Id";

        /// <summary>
        /// The name of the HTTP header for web request
        /// </summary>
        public const string REQUEST_HEADER_NAME = "Authorization";

        /// <summary>
        /// The value of the HTTP header for web request
        /// </summary>
        public const string REQUEST_HEADER_VALUE = "Bearer {0}";

        /// <summary>
        /// The Method for web request
        /// </summary>
        public const string REQUEST_METHOD = "POST";

        /// <summary>
        /// The value of the Content-type HTTP header for web request
        /// </summary>
        public const string REQUEST_CONTENT_TYPE = "text/xml; charset=utf-8";
        /// <summary>
        /// The mail attributes
        /// </summary>
        public static class MailAttributes
        {
            /// <summary>
            /// The BCC mail attribute
            /// </summary>
            public const string BCC = "BCC";

            /// <summary>
            /// The cc mail attribute
            /// </summary>
            public const string CC = "CC";

            /// <summary>
            /// The attachment mail attribute
            /// </summary>
            public const string ATTACHMENT = "ATTACHMENT";

            /// <summary>
            /// The content description
            /// </summary>
            public const string CONTENT_DESCRIPTION = "CONTENT-DESCRIPTION";

            /// <summary>
            /// The content disposition
            /// </summary>
            public const string CONTENT_DISPOSITION = "CONTENT-DISPOSITION";

            /// <summary>
            /// The content identifier
            /// </summary>
            public const string CONTENT_ID = "CONTENT-ID";

            /// <summary>
            /// The content transfer encoding
            /// </summary>
            public const string CONTENT_TRANSFER_ENCODING = "CONTENT-TRANSFER-ENCODING";

            /// <summary>
            /// The content type mail attribute
            /// </summary>
            public const string CONTENT_TYPE = "CONTENT-TYPE";

            /// <summary>
            /// The date mail attribute
            /// </summary>
            public const string DATE = "DATE";

            /// <summary>
            /// From mail attribute
            /// </summary>
            public const string FROM = "FROM";

            /// <summary>
            /// The sender mail attribute
            /// </summary>
            public const string SENDER = "SENDER";

            /// <summary>
            /// The subject mail attribute
            /// </summary>
            public const string SUBJECT = "SUBJECT";

            /// <summary>
            /// To mail attribute
            /// </summary>
            public const string TO = "TO";

            /// <summary>
            /// The filename mail attribute
            /// </summary>
            public const string FILENAME = "FILENAME";

            /// <summary>
            /// The size mail attribute
            /// </summary>
            public const string SIZE = "SIZE";

            /// <summary>
            /// The creation date mail attribute
            /// </summary>
            public const string CREATION_DATE = "CREATION-DATE";

            /// <summary>
            /// The modification date mail attribute
            /// </summary>
            public const string MODIFICATION_DATE = "MODIFICATION-DATE";

            /// <summary>
            /// The read date mail attribute
            /// </summary>
            public const string READ_DATE = "READ-DATE";

            /// <summary>
            /// The 7biT encoding
            /// </summary>
            public const string BIT_7 = "7BIT";

            /// <summary>
            /// The 8biT encoding
            /// </summary>
            public const string BIT_8 = "8BIT";

            /// <summary>
            /// The quoted printable
            /// </summary>
            public const string QUOTED_PRINTABLE = "QUOTED-PRINTABLE";

            /// <summary>
            /// The base 64 encoding
            /// </summary>
            public const string BASE64 = "BASE64";

            /// <summary>
            /// The importance mail attribute
            /// </summary>
            public const string IMPORTANCE = "IMPORTANCE";

            /// <summary>
            /// The mail categories attribute
            /// </summary>
            public const string CATEGORIES = "KEYWORDS";

            /// <summary>
            /// The mail received attribute
            /// </summary>
            public const string RECEIVED = "RECEIVED";

            
        }
    }
}
