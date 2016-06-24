using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

#region Matter Center Namespaces
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;

#endregion

namespace Microsoft.Legal.MatterCenter.Service.Filters
{
    /// <summary>
    /// All unhandled exception in the matter center will be handled by this class. 
    /// This MatterCenterExceptionFilter will be added to StartUp.cs 
    /// </summary>
    public class MatterCenterExceptionFilter : IExceptionFilter
    {
        private string instrumentationKey;
        private readonly ILogger _logger;        
        public MatterCenterExceptionFilter(ILoggerFactory logger,string instrumentationKey)
        {
            this.instrumentationKey = instrumentationKey;            
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this._logger = logger.CreateLogger("Matter Center Exception Filter");
        }

        /// <summary>
        /// Implement OnException method of IExceptionFilter which will be invoked
        /// for all unhandled exceptions
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            this._logger.LogError("MatterCenterExceptionFilter", context.Exception);
            var stackTrace = new StackTrace(context.Exception, true);
            StackFrame stackFrameInstance = null;

            if(stackTrace.GetFrames().Length>0)
            {
                for(int i=0; i< stackTrace.GetFrames().Length; i++)
                {
                    if(stackTrace.GetFrames()[i].ToString().Contains("Microsoft.Legal.Matter"))
                    {
                        stackFrameInstance = stackTrace.GetFrames()[i];
                        break;
                    }
                }
            }
            //Create custom exception response that needs to be send to client
            var response = new ErrorResponse()
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.ToString(),
                Description = "Error occured in the system. Please contact the administrator",
                //Exception = context.Exception.ToString(),
                LineNumber = stackFrameInstance?.GetFileLineNumber(),
                MethodName = stackFrameInstance?.GetMethod().Name,
                ClassName = stackFrameInstance?.GetMethod().DeclaringType.Name,
                ErrorCode = ((int)HttpStatusCode.InternalServerError).ToString()
            };

            //Create properties that need to be added to application insights
            var properties = new Dictionary<string, string>();
            properties.Add("StackTrace", response.StackTrace);
            properties.Add("LineNumber", response.LineNumber.ToString());
            properties.Add("MethodName", response.MethodName.ToString());
            properties.Add("ClassName", response.ClassName.ToString());
            properties.Add("ErrorCode", response.ErrorCode.ToString());           

            //Create Telemetry object to add exception to the application insights
            var ai = new TelemetryClient();
            ai.InstrumentationKey = instrumentationKey;
            if(ai.IsEnabled())
            {
                //add exception to the Application Insights
                ai.TrackException(context.Exception, properties);
            }           
            
            //Send the exceptin object to the client
            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                DeclaredType = typeof(ErrorResponse)                
            };
        }
    }
}
