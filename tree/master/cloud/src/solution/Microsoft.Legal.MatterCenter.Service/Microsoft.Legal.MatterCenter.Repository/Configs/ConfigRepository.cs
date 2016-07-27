using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;

using System;

using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;


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

    }
}

