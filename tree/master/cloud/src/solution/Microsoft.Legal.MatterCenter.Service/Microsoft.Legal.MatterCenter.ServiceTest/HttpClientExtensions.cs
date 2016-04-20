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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIxMWRkN2MyNi04YTBkLTRkNDEtOTIxMC05NDY4ODNmZmQ5ZDYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNDYxMTc4NDY5LCJuYmYiOjE0NjExNzg0NjksImV4cCI6MTQ2MTE4MjM2OSwiYWNyIjoiMSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI4NDRmZmI3Ny01YmZkLTQwM2UtOTI4NS02NzhlMmVkZGM5MGMiLCJhcHBpZGFjciI6IjAiLCJkZXZpY2VpZCI6ImI5MDUxZjgzLTAzMDUtNDQ3NC1hMDhkLWQ4ZjJlYzYzMWU1YyIsImZhbWlseV9uYW1lIjoiUGVkZGFrb3RsYSIsImdpdmVuX25hbWUiOiJMYWtzaG1hbmFzd2FteSBQcmVtY2hhbmQiLCJpbl9jb3JwIjoidHJ1ZSIsImlwYWRkciI6Ijk4LjI0Ny4yMS4xNjIiLCJuYW1lIjoiTGFrc2htYW5hc3dhbXkgUHJlbWNoYW5kIFBlZGRha290bGEgKEluZm9zeXMgTHRkKSIsIm9pZCI6ImM1MWNhOWM0LWJlZDAtNGQzYy05YTg3LWEyZTI3ZGIxOGFhOSIsIm9ucHJlbV9zaWQiOiJTLTEtNS0yMS0yMTI3NTIxMTg0LTE2MDQwMTI5MjAtMTg4NzkyNzUyNy0xMzkxMDAxMSIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6IkVEYUp2S2hwVngzelRSWWNBdTh5VDdjbi1Cc25waUJ6QUVSNlFEZWs0N0UiLCJ0aWQiOiI3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJ1bmlxdWVfbmFtZSI6InYtbGFwZWRkQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJ2LWxhcGVkZEBtaWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.DEdzAvcHvxZwZJClpvK_s-DSvSSUlWHqkKbKH7e3f0E1wxqsVoXzI6AEpgLMqaLlbyajx87m2rCs6A_nat5g45w8Q2I9PmBgjoeccW8GZ6z81InvS3It1RNEqSo4bqoVYECkwjVDw77sDEPzrB0S6yPf9whuORD2JVYUHqp9EF1AYqbfSqgG6QMetBd9j-Eg5XA5feqFRWKoqbRQ4s3IYtu2sI_TiX8RGLM_b_WN1yH6RjPPjtz2sBxZm354Sfk8mqxOeas6qRmJiq0XuulsTFEsAiGPxdcLH2SBKwlzDv9GeJQq2ni4dmOXC2_tO_iOeUNBJo6DJ5jO49NzE-O6TQ");
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
