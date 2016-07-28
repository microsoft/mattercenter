


using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;

using System;

using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ConfigRepository : IConfigRepository
    {

        private GeneralSettings generalSettings;
        private LogTables logTables;
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
        /// <param name="configRequest"></param>
        /// <returns></returns>
        public async Task<List<DynamicTableEntity>> GetConfigurationsAsync(String filter)
        {
            return await Task.FromResult(this.GetConfigEntities(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configRequest"></param>
        /// <returns></returns>
        public async Task<bool> InsertUpdateConfigurationsAsync(String configs)
        {
            return await Task.FromResult(UpdateEntityProperty(configs));
        }

        /// <summary>
        /// Get all the current configs from the Config Storage table
        //
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        public List<DynamicTableEntity> GetConfigEntities(string filter)
        {
            var entities = new List<DynamicTableEntity>();
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions = new TableRequestOptions
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference("MatterCenterConfiguration");
                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();

                if (filter == "")
                {

                    // Construct the query operation for all  entities 
                    query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "MatterCenterConfig"));

                }
                else
                {
                    query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("ConfigGroup", QueryComparisons.Equal, filter));
                }

                var queryResult = table.ExecuteQuery(query);

                entities.AddRange(queryResult);

            }
            catch (Exception)
            {
                throw;
            }
            return entities;
        }


        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="partitionkey"></param>
        public bool UpdateEntityProperty(string configs)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                CloudTable table = storageAccount.CreateCloudTableClient().GetTableReference("MatterCenterConfiguration");

                Dictionary<string, Dictionary<string, string>> allValues = new Dictionary<string, Dictionary<string, string>>();

                //add all the configsGroups and their key value pairs to a dictionary
                allValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(configs);

                var batch = new TableBatchOperation();
                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
                TableQuery<DynamicTableEntity> query2 = new TableQuery<DynamicTableEntity>();
                String key;

                foreach (KeyValuePair<string, Dictionary<string, string>> entry in allValues)
                {
                    bool hasKey = entry.Value.TryGetValue("Key", out key);
                    query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("ConfigGroup", QueryComparisons.Equal, entry.Key));
                    query2 = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("Key", QueryComparisons.Equal, key));
                   // query2 = new TableQuery<DynamicTableEntity>().Where(TableQuery.CombineFilters(query, QueryComparisons.Equal, key));
                }


                //TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
                //TableResult retrievedResult = table.Execute(retrieveOperation);
                //TableEntity entity = (TableEntity)retrievedResult.Result;


                //TableOperation updateOperation = TableOperation.InsertOrReplace(entity);
                //table.Execute(updateOperation);


                //TableOperation mergeOperation = TableOperation.InsertOrMerge(entity);
                //table.Execute(mergeOperation);


            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

    }
}
