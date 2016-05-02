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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYxOTY1MTU0LCJuYmYiOjE0NjE5NjUxNTQsImV4cCI6MTQ2MTk2OTA1NCwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTcyLjU2LjQxLjE0MiIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm5vbmNlIjoiYWZjZmI3MTktODRlYi00OTdlLThlNWItNzVjODQxMzYxMmY5Iiwib2lkIjoiOTMzY2UxZmQtMjY5My00YWFlLWI3YmEtMmEwYjY4ZTQxMDI5Iiwic3ViIjoia3FneXRMQ1JRUlVjUU1vaFBYQkVCSkhPZ052azRFV0tBTTBhM1VZaEdwUSIsInRpZCI6IjNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInVuaXF1ZV9uYW1lIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidXBuIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.bVIiUL3Jee1uvtcE-vR8XtDZWXcG5Axucy5XhpNDyiwnu86H2xaGSF1oRgII-5B-i_zwFDD2Din58jFjRBgftGcTCPj9iJ-Uy-ediIWjcYd57bzN5e5dkKwnLntzWq5RCvUSPIM-EBwfPuosElktpNC0XFlEkmDDo2wRNZJiaCGFzXypI0k58pgNBTrzlI3QqTAwYCoAsqNz4L6Fhtu-Y5bECIaxc9oO6-h-ynXnS8s_yfKj0SigOZD40O3sVLM0RpzVvm84-89PeJCPkW2pPoPEE08sAwaainBX6480aTLwdbFhtIhnu-UQN4vGbQS7oYvuKLPL7JJIcHpglicgUg");
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
