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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYxOTcxMDc2LCJuYmYiOjE0NjE5NzEwNzYsImV4cCI6MTQ2MTk3NDk3NiwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC4xMDIuMzUiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6ImYyOGI2MDM1LWUxN2ItNGU5Ny04OGU1LTk2Yjk5NTNkYmUyMyIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.qYd5xDTkYdTjo6KPAWwbKH3Icj62OCU2_LuJ2JPfIQZV2GkUp3OwRXejPogPsC8Vi-AkorXv71J2K4cLovWNJDmim7BwsMBnxQNBjmInUJEykHgNiYHr0lBdgeSI5n02Ar06Pji0YUZ7oMcNIFB6GOisL9b4Co7HZ2CWVttp3JGEpwQAP3aJIm1iWMstuL937-nPAIpIaVw33S-GL8dS3buT8n7Mfy0FOORAF5fM4GgAl5odJnG_Cl7tRIsuj3uyKjslwEwydC3p0thM0Z16JHI8zJcWr96zX3vKpxmkC2IDawyJdpHAZJffNrFRpj9WIet2Mjwhc54uqySo1JDOAg");
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
