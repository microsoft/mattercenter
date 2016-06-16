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
            client.DefaultRequestHeaders.Add("Authorization",
                "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY2MDM0MDAyLCJuYmYiOjE0NjYwMzQwMDIsImV4cCI6MTQ2NjAzNzkwMiwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC4xMDQuNDAiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6ImM5YzhkZjMzLTE4YjEtNDFlNC05NDkyLTBkMDJlYmIxODIwNCIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.U5JUrX1z2_OUE-xNL4M06CBDjtPbfuPVO6e8SMzdLAGHTsp8CaM0CDGwfzTgiZ4YU3smbCeTJRrf6DYYbWwvF7uYXmPTkJ_HYMCgRDO6XQOEPXvprOEDA2ZDXiF92yL4GEw3hUMFXq0DTuOfEx-EzpxCIM8q8tJmUP29cVtf9bLNZL7k5ruQbzZD7QcGFbd51bVdwPzIFENwty4gEZ3nzEq1HcRtXdS9NkQ3QaSDdI1Lkd8Y-6yHhx7ieL6cDIKcA8Dxl0dfWN8EQE7q_ag6s01mBxDJ20BmtEOSxMf2_j9fJr4qtr_kQ8D1oWUXSibyuvFRDprgu48VxdYGJZlNQg");
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
