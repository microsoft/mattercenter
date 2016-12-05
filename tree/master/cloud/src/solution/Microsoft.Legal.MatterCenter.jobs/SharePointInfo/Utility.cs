using Microsoft.Legal.MatterCenter.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.Security;


namespace Microsoft.Legal.MatterCenter.Jobs
{
    public class Utility
    {
        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        public static void UpdateTableStorageEntity(MatterInformationVM matterInformation, TextWriter log, string connection, 
            string tableName, string status, string statusColumnName)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount =
                    CloudStorageAccount.Parse(connection);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference(tableName);
                // Create a retrieve operation that takes a entity.
                TableOperation retrieveOperation =
                    TableOperation.Retrieve<MatterInformationVM>(matterInformation.PartitionKey, matterInformation.RowKey);
                // Execute the operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);
                // Assign the result to a ExternalSharingRequest object.
                MatterInformationVM updateEntity = (MatterInformationVM)retrievedResult.Result;
                if (updateEntity != null)
                {
                    if(statusColumnName=="Status")
                    {
                        updateEntity.Status = status;
                    }                        
                    if (statusColumnName == "MatterUpdateStatus")
                    {
                        updateEntity.MatterUpdateStatus = status;
                    }                        
                    TableOperation updateOperation = TableOperation.Replace(updateEntity);
                    table.Execute(updateOperation);
                    log.WriteLine($"Updated the matter status to Accepted in Azure Table Storage");
                }
            }
            catch (Exception ex)
            {
                log.WriteLine($"Exception occured in the method UpdateTableStorageEntity. {ex}");
            }
        }


        /// <summary>
        /// This method will return the secure password for authentication to SharePoint Online
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        public static SecureString GetEncryptedPassword(string plainTextPassword)
        {
            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            foreach (char c in plainTextPassword)
            {
                securePassword.AppendChar(c);
            }
            //while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }
    }
}
