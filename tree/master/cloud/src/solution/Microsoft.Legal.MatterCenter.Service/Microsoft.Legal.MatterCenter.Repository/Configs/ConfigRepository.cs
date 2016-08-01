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
            IOptions<GeneralSettings> generalSettings,
            IOptions<LogTables> logTables)
        {
            this.generalSettings = generalSettings.Value;
            this.logTables = logTables.Value;
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
                    // Construct the queryConfigGroup operation for all  entities 
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
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
                CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions = new TableRequestOptions
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };
                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference("MatterCenterConfiguration");
          
                TableBatchOperation batchOperation = new TableBatchOperation();

                Dictionary<string, Dictionary<string, string>> allValues = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string,string> keyValues = new Dictionary<string, string>();

                Dictionary<string, EntityProperty> newProperties = new Dictionary<string, EntityProperty>();

                //add all the configsGroups and their key value pairs to a dictionary
                allValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(configs);

                var batch = new TableBatchOperation();
                TableQuery<DynamicTableEntity> queryFinal = new TableQuery<DynamicTableEntity>();
       
                foreach (KeyValuePair<string, Dictionary<string, string>> entry in allValues)
                {                  
                    foreach (KeyValuePair<string, string> keyValue in entry.Value)
                    {                 
                        TableQuery<DynamicTableEntity> entityQuery = new TableQuery<DynamicTableEntity>().Where(
                            TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("ConfigGroup", QueryComparisons.Equal.ToLower(), entry.Key),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("Key", QueryComparisons.Equal, keyValue.Key)));

                        var queryResult = table.ExecuteQuery(entityQuery);
                        if (queryResult.Count() > 0)
                        {
                            foreach (DynamicTableEntity entity in queryResult)                         
                            {
                                entity.Properties["ConfigGroup"].StringValue = entry.Key;
                                entity.Properties["Key"].StringValue = keyValue.Key;
                                entity.Properties["Value"].StringValue = keyValue.Value;
                                batchOperation.Merge(entity);    
                            }
                        }
                        else
                        {
                            DynamicTableEntity config = CreateEntity(entry, keyValue);
                            batchOperation.InsertOrReplace(config);
                        }                    
                       
                    }
                }

             table.ExecuteBatch(batchOperation);
             return true;
                         
            }
            catch (Exception ex)
            {
                throw;
            }
     

        }

        private DynamicTableEntity CreateEntity(KeyValuePair<string, Dictionary<string, string>> entry, KeyValuePair<string, string> keyValue)
        {
            DynamicTableEntity config = new DynamicTableEntity();
            config.PartitionKey = "MatterCenterConfig";
            config.RowKey = string.Format(CultureInfo.InvariantCulture, Guid.NewGuid().ToString());
            config.Properties.Add("ConfigGroup", new EntityProperty(entry.Key));
            config.Properties.Add("Key", new EntityProperty(keyValue.Key));
            config.Properties.Add("Value", new EntityProperty(keyValue.Value));

            return config;
        }

    
   }

   
}
