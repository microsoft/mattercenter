// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-akvira
// Created          : 04-01-2014
//
// ***********************************************************************
// <copyright file="Logger.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines logging errors in event viewer or Azure table storage.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    #endregion

    /// <summary>
    /// Provides functionality to log error message in Event Viewer or Azure table storage.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets  the line number where exception has occurred.
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="className">Class Name where exception occur</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logTableName">Name of the log table.</param>
        /// <returns>Error logged in event viewer</returns>
        public static string LogError(Exception exception, string className, string methodName, string logTableName)
        {
            string result = string.Empty;
            try
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                int lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                string MCException = MatterCenterExceptions(exception, className, methodName, logTableName, lineNumber);
                result = MCException;
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                int lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                string MCException = MatterCenterExceptions(ex, className, methodName, logTableName, lineNumber);
                result = MCException;
            }
            return result;
        }
        /// <summary>
        /// Logs error message in Azure table storage or Event Viewer.
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="className">Class Name where exception occur</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logTableName">Name of the log table.</param>
        /// <param name="lineNumber">Line Number of the log table.</param>
        /// <returns>Error logged in event viewer</returns>
        public static string MatterCenterExceptions(Exception exception, string className, string methodName, string logTableName, int lineNumber)
        {
            string errorDate = DateTime.Now.ToString(ConstantStrings.AzureRowKeyDateFormat, CultureInfo.InvariantCulture);
            string errorCode = string.Empty;
            string result = string.Empty;
            try
            {
                if (null != exception)
                {
                    if (Convert.ToBoolean(ConstantStrings.IsLoggingOnAzure, CultureInfo.InvariantCulture))
                    {
                        //// Log to Azure table storage
                        errorDate = AzureLogger.LogInAzure(exception, className, methodName, logTableName, lineNumber);
                    }
                    else
                    {
                        string logMessage = string.Concat(ConstantStrings.ExceptionMessage + exception.Message + " " + ConstantStrings.ClassName + className + " " + ConstantStrings.MethodName + methodName + " " + ConstantStrings.LineNumber + lineNumber);
                        //// Log to event viewer
                        EventViewerLogger.LogInEventViewer(logMessage, ConstantStrings.EventError);
                    }
                    errorCode = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ErrorCode, exception.HResult, errorDate);
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, errorCode, ServiceUtility.RemoveEscapeCharacter(exception.Message));
                }
                else
                {
                    errorCode = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ErrorCode, ConstantStrings.LoggingFailedCode, errorDate);
                    result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, errorCode, ConstantStrings.LoggingFailedMessage);
                }
            }
            catch (Exception)
            {
                errorCode = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ErrorCode, ConstantStrings.LoggingFailedCode, errorDate);
                result = string.Format(CultureInfo.InvariantCulture, ConstantStrings.ServiceResponse, errorCode, ConstantStrings.LoggingFailedMessage);
            }
            return result;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Not all classes are static. Hence, we cannot have the same type for all the classes even after making them partial.")]

    /// <summary>
    /// Provides methods to log exceptions in Event Viewer.
    /// </summary>
    public static class EventViewerLogger
    {
        /// <summary>
        ///  Logs exception in Event Viewer.
        /// </summary>
        /// <param name="eventLog"> Event description </param>
        /// <param name="type"> Type of log </param>
        internal static void LogInEventViewer(string eventLog, string type)
        {
            string sourceName = ConstantStrings.EventViewerSource;
            int eventID;
            if (!int.TryParse(ConstantStrings.EventViewerEventId, NumberStyles.Any, CultureInfo.InvariantCulture, out eventID))
            {
                eventID = ConstantStrings.DefaultEventId;
            }

            // Create new event source if not exists
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, ConstantStrings.EventViewerLogName);
            }

            switch (type)
            {
                case ConstantStrings.EventWarning:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Warning, eventID);
                    break;
                case ConstantStrings.EventError:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Error, eventID);
                    break;
                default:
                    EventLog.WriteEntry(sourceName, eventLog, EventLogEntryType.Information, eventID);
                    break;
            }
        }
    }

    /// <summary>
    /// Provides methods to log exceptions in Azure table storage.
    /// </summary>
    public static class AzureLogger
    {
        /// <summary>
        /// Logs message to Azure table storage.
        /// </summary>
        /// <param name="exception">Exception Object</param>
        /// <param name="className">Class Name where exception occur</param>
        /// <param name="methodName">Method Name where exception occur</param>
        /// <param name="logTableName">Name of the log table.</param>
        /// <param name="lineNumber">Line Number of the log table.</param>
        /// <returns>Returns date of logging</returns>
        internal static string LogInAzure(Exception exception, string className, string methodName, string logTableName, int lineNumber)
        {
            string connStr = ConstantStrings.CloudStorageConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(logTableName);
            table.CreateIfNotExists();
            AzureLogEntity tableEntityObj = new AzureLogEntity();
            tableEntityObj.PartitionKey = className;
            tableEntityObj.MethodName = methodName;
            string date = DateTime.Now.ToUniversalTime().ToString(tableEntityObj.DateFormat, CultureInfo.InvariantCulture);
            tableEntityObj.RowKey = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", date, Guid.NewGuid().ToString());
            tableEntityObj.LogMessage = exception.Message;
            tableEntityObj.ErrorCode = exception.HResult;
            tableEntityObj.LineNumber = lineNumber;
            TableOperation insertOp = TableOperation.Insert(tableEntityObj);
            table.Execute(insertOp);
            return date;
        }
    }

    /// <summary>
    /// Declares entities within Azure log.
    /// </summary>
    [CLSCompliant(false)]
    public class AzureLogEntity : TableEntity
    {
        /// <summary>
        /// Azure row key date format
        /// </summary>
        private string dateFormat = ConstantStrings.AzureRowKeyDateFormat;

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        /// <value>
        /// The log message.
        /// </value>
        public string LogMessage { get; set; }
    
        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        /// <value>The date format.</value>
        internal string DateFormat
        {
            get { return this.dateFormat; }
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get; set; }
    }
}