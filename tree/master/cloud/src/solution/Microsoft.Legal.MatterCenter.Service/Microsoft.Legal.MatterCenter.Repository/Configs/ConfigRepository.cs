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
        public async Task<List<DynamicTableEntity>> GetConfigurationsAsync(DynamicTableEntity configRequest)
        {
            return await Task.FromResult(this.GetConfigEntities());
        }

        /// <summary>
        /// Update the status in Azure Table Storage for the corresponding Parition and Row Key
        /// for which the user has accepted the invitation
        /// </summary>
        /// <param name="externalSharingRequest"></param>
        public List<DynamicTableEntity> GetConfigEntities()
        {
           
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
                var entities = new List<DynamicTableEntity>();

                // Construct the query operation for all  entities 
                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "MatterCenterConfig"));
              
                var queryResult = table.ExecuteQuery(query);
                
                entities.AddRange(queryResult);
                return entities;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        }
}

