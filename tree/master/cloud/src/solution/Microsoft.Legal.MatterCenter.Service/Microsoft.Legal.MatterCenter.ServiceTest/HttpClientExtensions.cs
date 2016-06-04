using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public static class HttpClientExtensions
    {
        public static HttpClient AcceptJson(this HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY0OTk1NTgyLCJuYmYiOjE0NjQ5OTU1ODIsImV4cCI6MTQ2NDk5OTQ4MiwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC45OS4xMTgiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6IjYxOTg0N2VjLTdkODMtNDM1Mi1iNzhhLTQ1MzFiMWE5MjQ3NyIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.TpqeLeoXjgACxlrps9lcbXx1h2Wh82_etIjBsXKvRJvRp1dd9z2Qb99JVmrIZl7bFXJ9z-djno0x4ESx-DXGvxYIH9s6OXRWR-M0jNYrQZvMxTE3w39HR1zfs35bVmR5AVVNWpiuEnN5sJlpxYl3oT4VYL0vD9jajViGeWDmnPKKkbXcxUzqhHgwAXkqZN9dlXM9khO1IGHobF9hTjfOOS1klrlnT2JQ__iAWWV7koFwBOqmj2Zk-RDuoLPvqVZrYwK_GH3T9uoiXCltuYOHyYsGuCahxAbymoW69mvE4b5IAphVSDKKYK5zfE-MKBs56VEs_g6sxyWdb0vfgT6Ozw");
            return client;
        }

        
    }

    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            return await content.ReadAsAsync<T>(GetJsonFormatters());
        }

        private static IEnumerable<JsonMediaTypeFormatter> GetJsonFormatters()
        {
            yield return new JsonMediaTypeFormatter();            
        }
    }
}
