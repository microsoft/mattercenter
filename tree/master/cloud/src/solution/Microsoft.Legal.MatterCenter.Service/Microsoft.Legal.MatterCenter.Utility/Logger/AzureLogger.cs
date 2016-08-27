using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Globalization;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class AzureLogger
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
        internal static string LogInAzure(Exception exception, string className, string methodName, 
            string logTableName, int lineNumber, LogTables logTables, GeneralSettings generalSettings)
        {
            string connStr = generalSettings.CloudStorageConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(logTableName);
            table.CreateIfNotExists();
            AzureLogEntity tableEntityObj = new AzureLogEntity();
            tableEntityObj.PartitionKey = className;
            tableEntityObj.MethodName = methodName;
            string date = DateTime.Now.ToUniversalTime().ToString(logTables.AzureRowKeyDateFormat, CultureInfo.InvariantCulture);
            tableEntityObj.RowKey = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", date, Guid.NewGuid().ToString());
            tableEntityObj.Stacktrace = exception.StackTrace;
            tableEntityObj.LogMessage = exception.Message;
            tableEntityObj.ErrorCode = exception.HResult;
            tableEntityObj.LineNumber = lineNumber;
            TableOperation insertOp = TableOperation.Insert(tableEntityObj);
            table.Execute(insertOp);
            return date;
        }
    }

    public class AzureLogEntity : TableEntity
    {        

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
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get; set; }
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public string Stacktrace { get; set; }
    }
}
