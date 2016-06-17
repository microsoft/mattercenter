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
                "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY2MTg3MDIyLCJuYmYiOjE0NjYxODcwMjIsImV4cCI6MTQ2NjE5MDkyMiwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC4xMDEuMjMiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6IjBiN2ViYTkwLWNmZWMtNGUwNi1hY2Y0LTk1MWJiOTM2NTMwNSIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.dqwBSNVZRrOcenitgxsY9FRPYfiPzSa3M4nERqBHNzH19hBFKpA8Dh8RMz5Ey1C8ucWPzTrBRKuLUeTOPso_uHSWFBl4Ili3zgpb0hZmsfN1KGJJJkDoJ6druwa2rPo_vaHhZngS2KbmmPwvivcT1Z6m8KwET4iwkY11oUFH11ontb6OzNv3RkXs1y9pPRL9qtnx-EgYg4Tj6KPaPXhmNNL2uZMPaCovpNkW7ns_zMRq36YunaDY2C9XuZODEHzPaSKoUBMbozxYZWtnyxuJtmwXEex6Af-3oeKUigyH-hUiHYDdfVrwDwtziEmySw8ai1Llc5Tsj9_okDdcjAk3gw");
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
