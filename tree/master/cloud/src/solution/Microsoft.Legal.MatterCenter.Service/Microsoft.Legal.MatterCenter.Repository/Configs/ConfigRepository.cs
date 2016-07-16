using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Globalization;
using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;


using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;

using System.Collections.ObjectModel;

using System.Net;

using System.Reflection;
using Microsoft.SharePoint.Client.WebParts;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ConfigRepository : IConfigRepository
    {

        private GeneralSettings generalSettings;
        private LogTables logTables;
        private IUsersDetails userDetails;
        private IConfigRepository config;


        public ConfigRepository(
            IOptionsMonitor<GeneralSettings> generalSettings,
            IOptionsMonitor<LogTables> logTables)
        {

            this.generalSettings = generalSettings.CurrentValue;
            this.logTables = logTables.CurrentValue;
        }

  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCollectionUrl"></param>
        /// <returns></returns>
        public async Task<ConfigEntities> GetConfigurationsAsync(ConfigEntities configRequest)
        {
            return await Task.FromResult(config.GetConfigEntities());
        }

        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        public ConfigEntities GetConfigEntities()
        {
            ConfigEntity[] configs = { };
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=mattercenterlogstoragev0;AccountKey=Y3s1Wz+u2JQ/wl5WSVB5f+31oXyBlcdFVLk99Pgo8y8/vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d/QePQ==");
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions = new TableRequestOptions
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference("MatterCenterConfiguration");
             
                TableContinuationToken token = new TableContinuationToken() { };

                //TableQuery<ConfigEntity> query = new TableQuery(from ent in table.CreateQuery<ConfigEntity>()
                //                                  select ent);

                var entities = new List<DynamicTableEntity>();
                do
                {
                    var queryResult = table.ExecuteQuerySegmented(new TableQuery(), token);
                    entities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);
            }
            catch (Exception)
            {
                throw;
            }
            return new ConfigEntities() { ConfigEntries = configs };
        }

        //    /// <summary>
        //    /// This method will store external requests information in Azure Table Storage
        //    /// </summary>
        //    /// <param name="externalSharingRequest"></param>
        //    /// <returns></returns>
        //    private void SaveExternalSharingRequest(MatterInformationVM matterInformation)
        //    {
        //        try
        //        {
        //            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
        //            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
        //            tableClient.DefaultRequestOptions = new TableRequestOptions
        //            {
        //                PayloadFormat = TablePayloadFormat.JsonNoMetadata
        //            };
        //            // Retrieve a reference to the table.
        //            CloudTable table = tableClient.GetTableReference(logTables.ExternalAccessRequests);
        //            // Create the table if it doesn't exist.
        //            table.CreateIfNotExists();
        //            //Insert the entity into Table Storage              
        //            matterInformation.PartitionKey = matterInformation.Matter.Name;
        //            matterInformation.RowKey = $"{Guid.NewGuid().ToString()}${matterInformation.Matter.Id}";
        //            matterInformation.Status = "Pending";
        //            string matterInformationObject = Newtonsoft.Json.JsonConvert.SerializeObject(matterInformation);
        //            matterInformation.SerializeMatter = matterInformationObject;
        //            TableOperation insertOperation = TableOperation.Insert(matterInformation);
        //            table.Execute(insertOperation);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }

        //    /// <summary>
        //    /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        //    /// for which the user has accepted the invitation
        //    /// </summary>
        //    /// <param name="externalSharingRequest"></param>
        //    private static void UpdateTableStorageEntity(MatterInformationVM matterInformation, TextWriter log, IConfigurationRoot configuration)
        //    {
        //        try
        //        {
        //            CloudStorageAccount cloudStorageAccount =
        //                CloudStorageAccount.Parse(configuration["Data:DefaultConnection:AzureStorageConnectionString"]);
        //            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
        //            // Create the CloudTable object that represents the "people" table.
        //            CloudTable table = tableClient.GetTableReference(configuration["Settings:MatterCenterConfiguration"]);
        //            // Create a retrieve operation that takes a entity.
        //            TableOperation retrieveOperation =
        //                TableOperation.Retrieve<MatterInformationVM>(matterInformation.PartitionKey, matterInformation.RowKey);
        //            // Execute the operation.
        //            TableResult retrievedResult = table.Execute(retrieveOperation);
        //            // Assign the result to a ExternalSharingRequest object.
        //            MatterInformationVM updateEntity = (MatterInformationVM)retrievedResult.Result;
        //            if (updateEntity != null)
        //            {
        //                updateEntity.Status = "Accepted";
        //                TableOperation updateOperation = TableOperation.Replace(updateEntity);
        //                table.Execute(updateOperation);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            log.WriteLine($"Exception occured in the method UpdateTableStorageEntity. {ex}");
        //        }
        //    }
        //}
    }
}

