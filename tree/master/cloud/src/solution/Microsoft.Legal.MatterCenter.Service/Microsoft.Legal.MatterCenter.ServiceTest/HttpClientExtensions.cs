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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYzNDM4ODg0LCJuYmYiOjE0NjM0Mzg4ODQsImV4cCI6MTQ2MzQ0Mjc4NCwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC41OS4xMDgiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6IjgzNTcxOWIwLTJmN2EtNDA0ZC05MTU4LTlhMWRhNTM5N2IzYSIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.UbQ0PU2eHj47r_jHuLNr76-DU-Ul8-8pPZtJJ9CeH39IJDJpUfyJxg0OE_i9zk_ig1VufPRKCZTfN0V33pfmtv2X7TVwFcs8SnE0WHUnH0Rsmyii2DMDHKAWjdxY20SHEW7eVjMNE4hAVTTMnA9GNlSn9KUdrAnaGg0LmEaurm3yZIpxpiFIMCFXPrI6nnxTvXPwKhkc3tmKUravMEqDJwVkLlLbJDxAcNINMla88EtjZKQnQ09Qhx3xQE5-0fJHE2QkHlBPgvDE945MVr4jLCsALTPa7GUtjRY7xXG3qKF8YrvYmMjs0l_wV9ifsaOJvHzifNO0uHmGCBAX7ATH8Q");
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
