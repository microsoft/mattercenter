// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-lapedd
// Created          : 04-07-2016
//
// ***********************************************************************
// <copyright file="ErrorResponse.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>

// ***********************************************************************
using System;

namespace Microsoft.Legal.MatterCenter.Models
{
    /// <summary>
    /// ErrorResponse object that will be sent to the client for all the errors that are generated in the service
    /// </summary>
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public bool IsTokenValid { get; set; }
        public string StackTrace { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int? LineNumber { get; set; }
        public string ErrorDate { get; set; }
        public Exception Exception { get; set; }
        public bool IsErrror { get; set; }
        public override string ToString()
        {
            return $"EXCEPTION MESSAGE:{Message} CLASS NAME: {ClassName} METHOD NAME: {MethodName} LINE NUMBER: {LineNumber}";
        }
    }
}
