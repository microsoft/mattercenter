using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Legal.MatterCenter.Models;

namespace Microsoft.Legal.MatterCenter.Utility
{
    public class ServiceUtility
    {
        public static GeneralSettings GeneralSettings { get; set; }

        private static string RedisCacheHostName
        {
            get; set;
        }

        /// <summary>
        /// Will try to get data from azure redis cache for the given cache key
        /// </summary>
        /// <param name="key"></param>        
        /// <returns>cache data or empty string</returns>
        public static string GetDataFromAzureRedisCache(string key)
        {
            string cachedData = string.Empty;            
            try
            {
                if (!string.IsNullOrWhiteSpace(RedisCacheHostName) && !string.IsNullOrWhiteSpace(key))
                {
                    IDatabase cacheDatabase = Connection.GetDatabase();
                    cachedData = cacheDatabase.StringGet(key);
                    if (cachedData == null)
                    {
                        cachedData = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                cachedData = string.Empty;
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
            return cachedData;
        }

        /// <summary>
        /// Sets the data into Azure redis cache using the given key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="appSettings"></param>
        /// <param name="value">The value to be set into the Azure redis cache</param>
        public static void SetDataIntoAzureRedisCache<T>(string key, T value)
        {
            string cachedData = string.Empty;            
            try
            {
                if (!string.IsNullOrWhiteSpace(RedisCacheHostName) && !string.IsNullOrWhiteSpace(key))
                {
                    IDatabase cacheDatabase = Connection.GetDatabase();
                    var serializeData = JsonConvert.SerializeObject(value);                    
                    cacheDatabase.StringSet(key, JsonConvert.SerializeObject(serializeData), TimeSpan.FromDays(1));                   
                }
            }
            catch (Exception exception)
            {
                //Logger.LogError(exception, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ConstantStrings.LogTableName);
            }
        }

        /// <summary>
        /// Ensure that only one connection to azure redis cache has been enabled
        /// </summary>
        private static Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect($"{RedisCacheHostName}"));        

        /// <summary>
        /// 
        /// </summary>
        public static ConnectionMultiplexer Connection=> LazyConnection.Value;

        /// <summary>
        /// Remove Escape character
        /// </summary>
        /// <param name="message">The input message to remove escape character</param>
        /// <returns> Escape character removed message</returns>
        public static string RemoveEscapeCharacter(string message)=> Regex.Replace(message, ServiceConstants.ESCAPE_CHARACTER_PATTERN, string.Empty);

        /// <summary>
        /// Encodes the pinned user details
        /// </summary>
        /// <param name="value">Matter properties</param>
        /// <returns>Encoded String</returns>
        public static string EncodeValues(string value)=> !string.IsNullOrWhiteSpace(value) ? WebUtility.HtmlEncode(value.Trim()) : string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericResponseVM GenericResponse(string code, string value)
        {
            GenericResponseVM genericResponseVM = new GenericResponseVM();
            genericResponseVM.Code = code;
            genericResponseVM.Value = value;
            return genericResponseVM;
        }
    }
}
