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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJodHRwczovL21hdHRlcndlYmFwcC5henVyZXdlYnNpdGVzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY1NjQ1OTU4LCJuYmYiOjE0NjU2NDU5NTgsImV4cCI6MTQ2NTY0OTg1OCwiYWNyIjoiMSIsImFtciI6WyJwd2QiXSwiYXBwaWQiOiI3ODE0NmU3Zi0wNGE2LTRiNjctYjA3OS1jNDE5MDExMzQ1NGUiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiOTguMjQ3LjIxLjE2MiIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.E8DQX9t_EcB7SyB3uGW9JOY3J8ku6WwXjfMIokebKPubcOrk48i4A5KAl5NWilRc534etOuf8bpbzAoKWevcVkIv2sjAena_nj3iFRd827630E8V6DEQ8ad4Z0Pqvvrhlyn6MfGuXFOGbUiQrM4kaeVwJ0YxwCb5qX74PBsz7c_Fr-Q2mwliBx5dD13pBjPmJ2eFO7_FpbM4bQ-BfvFH_P3KcfQ9v4kfOY-E01onu3ItpHMBa7ljETEO7xyFAEqRarhYjtiJL0ltaCkLK_OOnDz6sMvAEs0NMi-H1_WRN--BnIBT0TKE2pGDiHr68B-c0tN4auPBTxzobWqmWkVa0g");
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
