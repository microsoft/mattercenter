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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYyMjMyMzIxLCJuYmYiOjE0NjIyMzIzMjEsImV4cCI6MTQ2MjIzNjIyMSwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC41OC44MSIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm5vbmNlIjoiZGE4M2Q3NjUtOTM3Yi00OTg5LTg4YTQtNzEwMzY0ODVmZjgzIiwib2lkIjoiOTMzY2UxZmQtMjY5My00YWFlLWI3YmEtMmEwYjY4ZTQxMDI5Iiwic3ViIjoia3FneXRMQ1JRUlVjUU1vaFBYQkVCSkhPZ052azRFV0tBTTBhM1VZaEdwUSIsInRpZCI6IjNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInVuaXF1ZV9uYW1lIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidXBuIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.Br6AtecBh2rtI6YG6x_jxI8SB_qpLTycQoZTlwvWqx7O5PWE8dbel6wfLFOavWDQF2IISaltkVF7qxQdeTfE2OexYd0Q-Ujni_c9Yj8yCkdLp4EijBK6nGWB-iimfKufSW3vX7o2vgbXnsoRg0-ubGvXdmjQ4M9a-7UD7SzU6oMmBQhxTMHVfiSdyVM6PJIMDGBE3eLhESzUDO4Wn4zSotkikxmLJveExx6CGiQpSmaqglmB8nVcIFf2HaAQD64djqdNVfb2_RM2CGBDufQ6TJvQxb0z13E8hRfwCTICQ0ydnFkwNMuMSXKYXun2E3IcvfTMlLjlfWflx47PUDflaQ");
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
