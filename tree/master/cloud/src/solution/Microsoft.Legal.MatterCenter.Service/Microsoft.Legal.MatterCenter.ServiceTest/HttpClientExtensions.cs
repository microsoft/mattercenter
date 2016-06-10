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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlliUkFRUlljRV9tb3RXVkpLSHJ3TEJiZF85cyIsImtpZCI6IlliUkFRUlljRV9tb3RXVkpLSHJ3TEJiZF85cyJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY1NTE3OTAzLCJuYmYiOjE0NjU1MTc5MDMsImV4cCI6MTQ2NTUyMTgwMywiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC41Ni4zOSIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm5vbmNlIjoiYWRlOWFmODktNTkwMi00MWEzLTkwNjctMzg3ZmY4MzFhNTYxIiwib2lkIjoiOTMzY2UxZmQtMjY5My00YWFlLWI3YmEtMmEwYjY4ZTQxMDI5Iiwic3ViIjoia3FneXRMQ1JRUlVjUU1vaFBYQkVCSkhPZ052azRFV0tBTTBhM1VZaEdwUSIsInRpZCI6IjNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInVuaXF1ZV9uYW1lIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidXBuIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.rMeq59V5hxw6xp9Q8GXptZaagtz5_vY-l7MnTNt8-NgC_ZU5trc1w1TYDUT5p4AbKvM3bYg_D5psea8bmKPaBM-OkTltwoSdi_7Dj81hh5n_zohpfKHEY6mhl6nMcG1nSNM_SHMK-_cgvKaBd3mPVtq_YJt-KdH-eBevewanznAQyFtskyOWMdPaOJfc2wFGuW52LjkbAysLFhLp3vtm5w1VWofHlR2ZunSYiXRKwQ7yGUnd0jS2aM98Pm9k1wMtPoIBN82Z_slzqSRbU2qwxatWz64sRHNI1GsJJmmyaELT_X8DSYZ6m2p0Wmp2MbrTMTqb9kCkKDrxqAR80oGvFQ");
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
