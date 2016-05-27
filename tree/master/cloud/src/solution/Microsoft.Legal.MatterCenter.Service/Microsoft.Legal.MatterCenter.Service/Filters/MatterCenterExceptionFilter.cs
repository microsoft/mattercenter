using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Legal.MatterCenter.Models;
using System;
using System.Diagnostics;
using System.Net;

namespace Microsoft.Legal.MatterCenter.Service.Filters
{
    public class MatterCenterExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public MatterCenterExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this._logger = logger.CreateLogger("Matter Center Exception Filter");
        }

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
            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                DeclaredType = typeof(ErrorResponse)                
            };
        }
    }
}
