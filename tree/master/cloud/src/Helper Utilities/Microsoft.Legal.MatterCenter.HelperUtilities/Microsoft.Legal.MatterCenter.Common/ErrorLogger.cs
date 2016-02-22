// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Common
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="ErrorLogger.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file logs error information into error file.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Common
{
    #region using
    using System;
    using System.IO;
    #endregion

    /// <summary>
    /// A class for logging error information into error file
    /// </summary>
    public static class ErrorLogger
    {
        /// <summary>
        /// This function logs error message to specified text file
        /// </summary>
        /// <param name="filePath">Full path of error file</param>
        /// <param name="errorMessage">Error message to be logged</param>
        public static void LogErrorToTextFile(string filePath, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                DisplayErrorMessage(errorMessage);
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine(errorMessage + " occurred at " + DateTime.Now);
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// Function is used to display error message on console
        /// </summary>
        /// <param name="input">Message to display</param>		
        public static void DisplayErrorMessage(string input)
        {
            ConsoleColor currentColor = Console.ForegroundColor; // save current color of console
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(input);
            Console.ForegroundColor = currentColor;
        }
    }
}