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

        #endregion
    }
}
