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

using System;
using Microsoft.Legal.MatterCenter.Models;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Options;
using System.Net;

namespace Microsoft.Legal.MatterCenter.Utility
{

    public class CustomLogger :ICustomLogger
    {
        private LogTables logTables;
        private GeneralSettings generalSettings;
        ErrorResponse errorResponse;
        public CustomLogger(IOptions<LogTables> logTables, IOptions<GeneralSettings> generalSettings)
        {
            this.logTables = logTables.Value;
            this.generalSettings = generalSettings.Value;
            
        }


        /// <summary>
        /// This method will generate error response that will be sent to the client
        /// </summary>
        /// <param name="ex">Exception object that occured </param>
        /// <returns></returns>
        public ErrorResponse GenerateErrorResponse(Exception ex)
        {
            var stackTrace = new StackTrace(ex, true);
            StackFrame stackFrameInstance = null;

            if (stackTrace.GetFrames().Length > 0)
            {
                for (int i = 0; i < stackTrace.GetFrames().Length; i++)
                {
                    if (stackTrace.GetFrames()[i].ToString().Contains("Microsoft.Legal.Matter"))
                    {
                        stackFrameInstance = stackTrace.GetFrames()[i];
                        break;
                    }
                }
            }
            //Create custom exception response that needs to be send to client
            var response = new ErrorResponse()
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace.ToString(),
                Description = "Error occured in the system. Please contact the administrator",
                Exception = ex,
                LineNumber = stackFrameInstance?.GetFileLineNumber(),
                MethodName = stackFrameInstance?.GetMethod().Name,
                ClassName = stackFrameInstance?.GetMethod().DeclaringType.Name,
                ErrorCode = ((int)HttpStatusCode.InternalServerError).ToString(),
                IsErrror = true
            };
            return response;
        }


        /// <summary>
        /// Gets  the line number where exception has occurred.
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="className">Class Name where exception occur</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="logTableName">Name of the log table.</param>
        /// <returns>ErrorResponse</returns>
        public ErrorResponse LogError(Exception exception, string className, string methodName, string logTableName)
        {
            
            try
            {
                StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                int lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                errorResponse = MatterCenterExceptions(exception, className, methodName, logTableName, lineNumber);
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                int lineNumber = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                errorResponse = MatterCenterExceptions(ex, className, methodName, logTableName, lineNumber);
                
            }
            return errorResponse;
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
        public ErrorResponse MatterCenterExceptions(Exception exception, string className, string methodName, string logTableName, int lineNumber)
        {
            string errorDate = DateTime.Now.ToString(logTables.AzureRowKeyDateFormat, CultureInfo.InvariantCulture);
            string errorCode = string.Empty;
            string result = string.Empty;
            try
            {
                if (null != exception)
                {
                    if (Convert.ToBoolean(logTables.IsLoggingOnAzure, CultureInfo.InvariantCulture))
                    {
                        //// Log to Azure table storage
                        errorDate = AzureLogger.LogInAzure(exception, className, methodName, logTableName, lineNumber, logTables, generalSettings);
                    }
                    else
                    {                        
                        errorResponse = new ErrorResponse()
                        {
                            Message = exception.Message,
                            ClassName = className,
                            MethodName = methodName,
                            LineNumber = lineNumber

                        };
                        //// Log to event viewer
                        EventViewerLogger.LogInEventViewer(errorResponse.ToString(), ServiceConstants.EVENT_ERROR, logTables);
                    }                    
                    errorResponse = new ErrorResponse()
                    {
                        ErrorCode = exception.HResult.ToString(),
                        ErrorDate = errorDate,
                        Message = ServiceUtility.RemoveEscapeCharacter(exception.Message),
                       
                    };
                }
                else
                {                    
                    errorResponse = new ErrorResponse()
                    {
                        ErrorCode = ServiceConstants.LOGGING_FAILED_CODE.ToString(),
                        ErrorDate = errorDate,
                        Message = ServiceUtility.RemoveEscapeCharacter(exception.Message),
                        Description = ServiceConstants.LOGGING_FAILED_MESSAGE
                    };
                }
            }
            catch (Exception)
            {                
                errorResponse = new ErrorResponse()
                {
                    ErrorCode = ServiceConstants.LOGGING_FAILED_CODE.ToString(),
                    ErrorDate = errorDate,
                    Message = ServiceUtility.RemoveEscapeCharacter(exception.Message),
                    Description = ServiceConstants.LOGGING_FAILED_MESSAGE
                };
            }
            return errorResponse;
        }
    }
}
