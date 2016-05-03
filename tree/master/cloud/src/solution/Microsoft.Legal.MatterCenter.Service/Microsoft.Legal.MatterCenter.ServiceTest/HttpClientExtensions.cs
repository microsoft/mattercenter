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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYyMzE1NTQwLCJuYmYiOjE0NjIzMTU1NDAsImV4cCI6MTQ2MjMxOTQ0MCwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC41OS4xMzQiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6IjlmYzEzZmNmLTRkZDktNDgyYS1iZjhiLWU2OTEwZDQ3ZmMxMCIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.SHkd20ptL3O0t5Nlfd3CZUEbnpjguT2ly7oZJkOudVOv4O8n-fNSjHeq6OMzaqr9282-YUals2dHKrY4B30EyPUAP99nbPuEYdADmR8Ag5Yo0uvzAt9L2dySotTmdqWynVX0S7Ft5mqQrKy5J-nW-0Uz-HYKduvhgzSqlTv0rnlvIoVJcYfzql6wEQCm-kM1hjAqHRJJzuFkzEoxa-Rixfx38ER8wc1NJrS9lSItgRIelY9rHA0dDbjx5Nsn5ZN-_o1s_Kv4gwgExcNE1KD4xOn5JGzw_0IjJDeeiQj17iNeHB4Up5-rzcwaZSGlXufIqkGi3e_6St9hW6sxbrXx5Q");
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
