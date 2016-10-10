using Microsoft.Extensions.Options;
using Microsoft.Legal.MatterCenter.Models;
using Microsoft.Legal.MatterCenter.Utility;

using System;

using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.SharePoint.Client;
using System.Security;
using System.IO;
namespace Microsoft.Legal.MatterCenter.Repository
{
    public class ConfigRepository : IConfigRepository
    {

        private GeneralSettings generalSettings;
        private UIConfigSettings configSettings;
        private IConfigRepository config;

        /// <summary>
        /// ConfigRepository with all the required dependency injections inserted
        /// </summary>
        /// <param name="generalSettings"></param>
        /// <param name="configSettings"></param>
        public ConfigRepository(
            IOptions<GeneralSettings> generalSettings,
              IOptions<UIConfigSettings> configSettings)
        {
            this.generalSettings = generalSettings.Value;
            this.configSettings = configSettings.Value;
            
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

        /// <summary>
        /// This method will upload uiconfigforspo.js into sharepoint catalog site collection
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="clientUrl"></param>
        public void UploadConfigFileToSPO(string filePath, string clientUrl)
        {
            using (ClientContext clientContext = new ClientContext(generalSettings.CentralRepositoryUrl))
            {
                //Connec to sharepoint using admin username and password
                SecureString password = GetEncryptedPassword(generalSettings.AdminPassword);
                clientContext.Credentials = new SharePointOnlineCredentials(generalSettings.AdminUserName, password);
                var web = clientContext.Web;
                var newFile = new FileCreationInformation
                {
                    Content = System.IO.File.ReadAllBytes(filePath),
                    Overwrite = true,
                    //The root at which the uiconfigforspo.js needs to be uploaded
                    Url = Path.Combine("SiteAssets/Matter Center Assets/Common Assets/Scripts/", Path.GetFileName(filePath))

                };
                //The document library names under which the uiconfigforspo.js has to be uploaded
                var docs = web.Lists.GetByTitle("Site Assets");
                Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(newFile);
                clientContext.Load(uploadFile);
                clientContext.ExecuteQuery();
                //After uploading the file to sharepoint site collection, delete the file from the app root
                if(!filePath.EndsWith("config.js"))
                {
                    System.IO.File.Delete(filePath);
                }                
            }
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

                CloudTable table = GetTable();
                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();

                if (filter == "")
                {
                    // Construct the queryConfigGroup operation for all  entities 
                    query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, configSettings.Partitionkey));
                }
                else
                {
                    query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition(configSettings.ConfigGroup, QueryComparisons.Equal, filter));
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
        /// Update  or Insert Configuration values 
        /// </summary>
        /// <param name="configs"></param>
        public bool UpdateEntityProperty(string configs)
        {
            try
            {
                // Retrieve a reference to the table.
                CloudTable table = GetTable();
          
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
                                TableQuery.GenerateFilterCondition(configSettings.ConfigGroup, QueryComparisons.Equal.ToLower(), entry.Key),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition(configSettings.Key, QueryComparisons.Equal, keyValue.Key)));

                       IEnumerable<DynamicTableEntity> queryResult = table.ExecuteQuery(entityQuery);
                       
                        if (queryResult.GetEnumerator().MoveNext())
                        {
                            foreach (DynamicTableEntity entity in queryResult)                         
                            {
                                entity.Properties[configSettings.ConfigGroup].StringValue = entry.Key;
                                entity.Properties[configSettings.Key].StringValue = keyValue.Key;
                                entity.Properties[configSettings.Value].StringValue = keyValue.Value;
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
            config.PartitionKey = configSettings.Partitionkey;
            config.RowKey = string.Format(CultureInfo.InvariantCulture, Guid.NewGuid().ToString());
            config.Properties.Add(configSettings.ConfigGroup, new EntityProperty(entry.Key));
            config.Properties.Add(configSettings.Key, new EntityProperty(keyValue.Key));
            config.Properties.Add(configSettings.Value, new EntityProperty(keyValue.Value));

            return config;
        }

        private CloudTable GetTable()
        {

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(generalSettings.CloudStorageConnectionString);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            tableClient.DefaultRequestOptions = new TableRequestOptions
            {
                PayloadFormat = TablePayloadFormat.JsonNoMetadata
            };
            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(configSettings.MatterCenterConfiguration);

            return table;
        }

    }

   
}
