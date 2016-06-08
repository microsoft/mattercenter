// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-rijadh
// Created          : 12-01-2014
//
// ***********************************************************************
// <copyright file="MatterCenterException.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// This file is used for handling custom exceptions occurred in Matter Center.
// Cases where custom exception can occur in Matter Center:
// Case 1: Error occurred while assigning Content Type to matter
// Case 2: Error occurred while creating matter landing page
// Case 3: Error occurred while creating calendar list
// Case 4: Error occurred while assigning permission to calendar list
// Case 5: Error occurred if matter landing page already exists
// Case 6: Error occurred while creating task list
// Case 7: Error occurred while creating site column in case of OneDrive
// Case 8: Error occurred while creating site content type in case of OneDrive
// Case 9: Error occurred while assigning default content type to matter
// Case 10: Error occurred if OneDrive of user is not configured
// Case 11: Error occurred in case of Token Request failure exception
// </summary>
// ***********************************************************************
#region using
using System;
using System.Globalization;
using System.Runtime.Serialization;
#endregion
namespace Microsoft.Legal.MatterCenter.Utility
{
    /// <summary>
    /// Provides methods for handling custom exception
    /// </summary>
    [Serializable]
    public class MatterCenterException : Exception
    {
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public MatterCenterException()
            : base()
        { }

        /// <summary>
        /// Parameterized constructor for error message
        /// </summary>
        /// <param name="customErrorMessage">Custom error message</param>
        public MatterCenterException(string customErrorMessage)
            : base(customErrorMessage)
        { }

        /// <summary>
        /// Parameterized constructor for error code and message
        /// </summary>
        /// <param name="customErrorCode">Custom error code</param>
        /// <param name="customErrorMessage">Custom error message</param>
        public MatterCenterException(string customErrorCode, string customErrorMessage)
            : base(customErrorMessage)
        { HResult = Convert.ToInt32(customErrorCode, CultureInfo.InvariantCulture); }

        /// <summary>
        /// Parameterized constructor for error message and Exception object
        /// </summary>
        /// <param name="customErrorMessage">Custom error message</param>
        /// <param name="exception">Exception Object</param>
        public MatterCenterException(string customErrorMessage, Exception exception)
            : base(customErrorMessage, exception)
        { }

        /// <summary>
        /// Parameterized constructor for SerializationInfo and Streaming Context object
        /// </summary>
        /// <param name="serializationInfo">SerializationInfo object</param>
        /// <param name="streamingContext">Streaming Context object</param>
        protected MatterCenterException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }
    }
}
