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
                "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlliUkFRUlljRV9tb3RXVkpLSHJ3TEJiZF85cyIsImtpZCI6IlliUkFRUlljRV9tb3RXVkpLSHJ3TEJiZF85cyJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY3NzY3NDUxLCJuYmYiOjE0Njc3Njc0NTEsImV4cCI6MTQ2Nzc3MTM1MSwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiOTguMjQ3LjIxLjE2MiIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm5vbmNlIjoiMzk2NDUzYTUtZWQyNi00N2Q3LThiMGYtMGFmNTY5Njc4ZGM2Iiwib2lkIjoiOTMzY2UxZmQtMjY5My00YWFlLWI3YmEtMmEwYjY4ZTQxMDI5Iiwic3ViIjoia3FneXRMQ1JRUlVjUU1vaFBYQkVCSkhPZ052azRFV0tBTTBhM1VZaEdwUSIsInRpZCI6IjNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInVuaXF1ZV9uYW1lIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidXBuIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.d1BT4sBtxE1BOBtiI5zqH4_ZuVja5neCVrBXwxvERTQB4V022ns-o65r-d6NXyJGbhlwKszRHaz7fnIAKvRiEr4TcL9NoX9DoGHlFlBhBX0X28TqZNYt4DY11qmwRmf6bYL2VF3MLBUUxsU4RG-ovrXTURQNaOL7-lH59rWbrMd2mMCw3RQQg_60B2TJN1aFtCZp6g_jlhAk2s3te6b-xEFhBk_4evF8BjdAi84hPV-LG7q5VcoGEVGc6QjMRWkRyOcNEa3LJftRzyhS_LPPhq3lEtydeKY-MV6JnZ7WBr_97q5czQbwdAB9gICG9whdMTeEFBjNfQRZKgpXICiHZA");
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
