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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDYyNDkzNzAzLCJuYmYiOjE0NjI0OTM3MDMsImV4cCI6MTQ2MjQ5NzYwMywiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiOTguMjQ3LjIxLjE2MiIsIm5hbWUiOiJXaWxzb24gR2FqYXJsYSIsIm5vbmNlIjoiMjIxMThiZTktNDE3My00NTc2LWI4M2QtMWFhMjc2ODcxM2I1Iiwib2lkIjoiOTMzY2UxZmQtMjY5My00YWFlLWI3YmEtMmEwYjY4ZTQxMDI5Iiwic3ViIjoia3FneXRMQ1JRUlVjUU1vaFBYQkVCSkhPZ052azRFV0tBTTBhM1VZaEdwUSIsInRpZCI6IjNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInVuaXF1ZV9uYW1lIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidXBuIjoibWF0dGVyYWRtaW5ATVNtYXR0ZXIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.Gx7fveLksyGZ7-92fpTBtfmrJ_7DAohUgV5qFgs4W3j253bTJX6CGFFCHh34RT6G0UvVwmER4_b0yhGO_ProJjaRsz4VJVsfcMnQyN0osP94hYqV3aKvUaXs8_E4JCMYW9m3z8HpUR_Ib8AJJn_FpUjwLaSfGLAKqfkhw3RjUd00uVefMh3RzxqLKjzCbpdVwH2gj-P7xkb2oIWwnGYB_3llEbfYTfS0O2A4zT2K10sLnF9kTQdevlv00rpo7Nz14sMvAVsBMA_M9LWkLvvxEFvyOA2A_b1v7zcRY8MqC3tRzsi0NBfOy9u1o3zm2Q7ZCEjJvNxp_t-qdzYN6mCX6w");
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
