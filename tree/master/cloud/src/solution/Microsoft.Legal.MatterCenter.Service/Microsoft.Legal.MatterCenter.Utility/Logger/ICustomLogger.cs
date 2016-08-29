

using Microsoft.Legal.MatterCenter.Models;
using System;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public interface ICustomLogger
    {
        ErrorResponse LogError(Exception exceptio, string className, string methodName, string logTableName);
        ErrorResponse GenerateErrorResponse(Exception ex);
    }
}
