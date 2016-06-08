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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYxNjg5NTM4LCJuYmYiOjE0NjE2ODk1MzgsImV4cCI6MTQ2MTY5MzQzOCwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTMxLjEwNy4xNjAuMzYiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6ImEwMzZhMDZjLTM5ODEtNDQ1NC1hODk2LTZhMzQ2ZjgxNzYwOSIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.ciqpSsbxz_JSHzH57UEvWglc0PsKBSuOm2BuYPmMrlhM06lnxemd6Z-YvFtb9og1tm5SS0C7vHyG1OSxU4bkGViUw-sZDSxOXwGaIMwlXPz4S4c2uf1ky-Mjd4u4KvHculqxXZlocin3uGsTlJwN_lVxhJuuxFpzIfNL-N2vcEQXFVs5aWCs0gXLTQGEct-mWZEi6iqz3fcs3q3JZsIeX6uEKq9SEHVY9nWALBgeyTLlV8T6F_9PSxfgApRAru2pNlBeIsOWEWFVI_FtUkKYKL3BAQihZfLX8OqOVHQ4eSY28paZEC9bKMv13-_uMaLItNzlO5TOqvlWDdUIA_rAUA");
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
