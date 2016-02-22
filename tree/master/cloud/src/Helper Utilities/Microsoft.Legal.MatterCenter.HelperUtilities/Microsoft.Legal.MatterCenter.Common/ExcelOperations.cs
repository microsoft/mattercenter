// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Common
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="ExcelOperations.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file provides common methods for reading the Excel file.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Common
{
    #region using
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    #endregion

    /// <summary>
    /// A class provides Excel related operations to read Excel sheets
    /// </summary>
    public static class ExcelOperations
    {
        /// <summary>
        /// A variable containing file path to save error logs 
        /// </summary>
        private static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\Logs\\ErrorLog.txt";

        /// <summary>
        /// This function reads Excel file and returns dictionary
        /// </summary>
        /// <param name="filePath">Full path to Excel file</param>
        /// <param name="sheetName">Sheet name containing configurations</param>
        /// <returns>Dictionary of configurations</returns>
        public static Dictionary<string, string> ReadFromExcel(string filePath, string sheetName)
        {
            if (!(string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(sheetName)))
            {
                Dictionary<string, string> myList = new Dictionary<string, string>();
                try
                {
                    using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filePath, false))
                    {
                        WorkbookPart workbookPart = myDoc.WorkbookPart;
                        IEnumerable<Sheet> sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);
                        if (sheets.Count() == 0)
                        {
                            throw new ArgumentException("Message: Provided Sheet name is incorrect");
                        }
                        string relationshipId = sheets.First().Id.Value;
                        WorksheetPart worksheetPart = (WorksheetPart)myDoc.WorkbookPart.GetPartById(relationshipId);
                        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        foreach (Row row in sheetData.Elements<Row>())
                        {
                            List<string> rowValue = new List<string>();
                            rowValue = ReadRow(row, workbookPart).ToList();
                            if (null != rowValue && 0 < rowValue.Count()) // Check count to avoid adding of blank row entry which having meta data
                            {
                                myList.Add(rowValue[0].Trim(), rowValue[1].Trim());
                            }
                        }
                    }
                }
                catch (FileNotFoundException exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Invalid file path, file not found" + exception.Message);
                }
                catch (Exception exception)
                {
                    if (exception.Message.IndexOf("Invalid Hyperlink", 0, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        ErrorLogger.DisplayErrorMessage("Unable to read Excel. Please remove all hyperlinks from configuration Excel");
                    }
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception Details: " + exception.Message);
                }
                return myList;
            }
            else
            {
                throw new ArgumentException("Message: FilePath or Sheet name missing");
            }
        }

        /// <summary>
        /// Function is used to return all the values that are read from Sheet
        /// </summary>
        /// <param name="filePath">File path of the Excel</param>
        /// <param name="sheetName">Sheet to be read from Excel</param>
        /// <returns>Two-dimensional list with all values from Sheet</returns>
        public static Collection<Collection<string>> ReadSheet(string filePath, string sheetName)
        {
            Collection<Collection<string>> myList = new Collection<Collection<string>>();
            Collection<string> rowList = new Collection<string>();
            try
            {
                using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = myDoc.WorkbookPart;
                    IEnumerable<Sheet> sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);
                    if (sheets.Count() == 0)
                    {
                        ErrorLogger.DisplayErrorMessage("Provided sheet name is invalid!");
                        return myList;
                    }
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)myDoc.WorkbookPart.GetPartById(relationshipId);
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    foreach (Row row in sheetData.Elements<Row>())
                    {
                        rowList = ReadRow(row, workbookPart);
                        if (null != rowList && 0 < rowList.Count()) // Check count to avoid adding of blank row entry which having meta data
                        {
                            myList.Add(rowList);
                        }
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Invalid file path, file not found" + exception.Message);
            }
            catch (Exception exception)
            {
                if (exception.Message.IndexOf("Invalid Hyperlink", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    ErrorLogger.DisplayErrorMessage("Unable to read Excel. Please remove all hyperlinks from configuration Excel");
                }
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception Details: " + exception.Message);
            }
            return myList;
        }

        /// <summary>
        /// Function is used to read row values for the provided excel sheet
        /// </summary>
        /// <param name="row">Row from the excel sheet</param>
        /// <param name="workbookPart">WorkbookPart object</param>
        /// <returns>List of values in row</returns>
        private static Collection<string> ReadRow(Row row, WorkbookPart workbookPart)
        {
            string value = string.Empty;
            int rowSize = row.Elements().Count(), emptyValue = 0;
            Collection<string> rowValue = new Collection<string>();
            try
            {
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (null != cell)
                    {
                        CellValue cellValue = cell.Descendants<CellValue>().FirstOrDefault();
                        if (null != cellValue)
                        {
                            value = cellValue.Text;
                            if (null != cell.DataType)
                            {
                                switch (cell.DataType.Value)
                                {
                                    case CellValues.SharedString:
                                        var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                        if (null != stringTable)
                                        {
                                            value = stringTable.SharedStringTable.
                                              ElementAt(int.Parse(value, CultureInfo.InvariantCulture)).InnerText;
                                        }
                                        break;
                                    case CellValues.Boolean:
                                        switch (value)
                                        {
                                            case "0":
                                                value = "FALSE";
                                                break;
                                            default:
                                                value = "TRUE";
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            value = string.Empty;
                        }
                        value = value.Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            emptyValue++;
                        }
                        if (string.Equals("NA", value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = string.Empty;
                        }
                        rowValue.Add(value);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Exception occurred while reading row value: " + exception.Message);
            }
            if (emptyValue == rowSize)
            {
                return null;
            }
            return rowValue;
        }

        /// <summary>
        /// Function to validate user credentials
        /// </summary>
        /// <param name="userName">User name to validate</param>
        /// <param name="password">Password to validate</param>
        /// <returns>Flag if user credentials are validated</returns>
        public static bool IsNullOrEmptyCredential(string userName, string password)
        {
            return (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) ? true : false;
        }
    }

    /// <summary>
    /// A class which containing methods and members for handling errors
    /// </summary>
    public static class ErrorMessage
    {
        /// <summary>
        /// Types of notification messages
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Represents success message
            /// </summary>
            Success,

            /// <summary>
            /// Represents warning message
            /// </summary>
            Warning,

            /// <summary>
            /// Represents error message
            /// </summary>
            Error,

            /// <summary>
            /// Represents notification message
            /// </summary>
            Notification
        }

        /// <summary>
        /// Displays message on console
        /// </summary>
        /// <param name="message">string message to be displayed</param>
        /// <param name="messageType">Message type </param>
        public static void ShowMessage(string message, MessageType messageType)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            ConsoleColor textColor;
            switch (messageType)
            {
                case MessageType.Success:
                    textColor = ConsoleColor.Green;
                    break;
                case MessageType.Warning:
                    textColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Error:
                    textColor = ConsoleColor.Red;
                    break;
                case MessageType.Notification:
                default:
                    textColor = originalColor;
                    break;
            }

            Console.ForegroundColor = textColor;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }
}