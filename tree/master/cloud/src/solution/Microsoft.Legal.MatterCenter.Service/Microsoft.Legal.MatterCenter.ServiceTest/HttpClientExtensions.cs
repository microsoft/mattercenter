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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIxMWRkN2MyNi04YTBkLTRkNDEtOTIxMC05NDY4ODNmZmQ5ZDYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNDYxMjU3MjkxLCJuYmYiOjE0NjEyNTcyOTEsImV4cCI6MTQ2MTI2MTE5MSwiYWNyIjoiMSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4NDRmZmI3Ny01YmZkLTQwM2UtOTI4NS02NzhlMmVkZGM5MGMiLCJhcHBpZGFjciI6IjAiLCJkZXZpY2VpZCI6ImI5MDUxZjgzLTAzMDUtNDQ3NC1hMDhkLWQ4ZjJlYzYzMWU1YyIsImZhbWlseV9uYW1lIjoiUGVkZGFrb3RsYSIsImdpdmVuX25hbWUiOiJMYWtzaG1hbmFzd2FteSBQcmVtY2hhbmQiLCJpcGFkZHIiOiI5OC4yNDcuMjEuMTYyIiwibmFtZSI6Ikxha3NobWFuYXN3YW15IFByZW1jaGFuZCBQZWRkYWtvdGxhIChJbmZvc3lzIEx0ZCkiLCJvaWQiOiJjNTFjYTljNC1iZWQwLTRkM2MtOWE4Ny1hMmUyN2RiMThhYTkiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjEyNzUyMTE4NC0xNjA0MDEyOTIwLTE4ODc5Mjc1MjctMTM5MTAwMTEiLCJzY3AiOiJ1c2VyX2ltcGVyc29uYXRpb24iLCJzdWIiOiJFRGFKdktocFZ4M3pUUlljQXU4eVQ3Y24tQnNucGlCekFFUjZRRGVrNDdFIiwidGlkIjoiNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3IiwidW5pcXVlX25hbWUiOiJ2LWxhcGVkZEBtaWNyb3NvZnQuY29tIiwidXBuIjoidi1sYXBlZGRAbWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.IWY91lC2ByJmcEs7Bn0JETcuAH7LU2z8oYrdq1fe2up-_pjoAvdUNfL_p21gpmrSxsgdvJNMq-hHO7GlD4sn6v_UnTLQBbg1SAzL1NGBGat7xeOLOM83kGs2lF5bMclyDti6_zFTibuStMRC66A9oLfW5nzZ7zImJNwvMnmy7KPfcbWWbOUBb3zD3xFpwpbY9ghPFf4GtPt3w1miWyyddMyx4RXeeOUb97XWQZTL80xC45IHJtUSMf5iMg6vV7HflnRrQ0RyG3AsTSiJuBTC4C7TjWpyjlEQ962vtuob_Ak1DVlR3S-Hn559laB5-NuIpHJkWc97sTs7tUxkHIVyLw");
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
