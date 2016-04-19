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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiIxMWRkN2MyNi04YTBkLTRkNDEtOTIxMC05NDY4ODNmZmQ5ZDYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNDYxMDg4MjY5LCJuYmYiOjE0NjEwODgyNjksImV4cCI6MTQ2MTA5MjE2OSwiYXBwaWQiOiI4NDRmZmI3Ny01YmZkLTQwM2UtOTI4NS02NzhlMmVkZGM5MGMiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwib2lkIjoiMzFkOGQzODUtZWUzOS00ZTVkLWFkYjMtZGVmM2RkOGJhZTYwIiwic3ViIjoiMzFkOGQzODUtZWUzOS00ZTVkLWFkYjMtZGVmM2RkOGJhZTYwIiwidGlkIjoiNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3IiwidmVyIjoiMS4wIn0.VW_2wWMfEFvB1HjfkA1CN27nOKrqX_LCWs8aGUgkQkyjF_zrArDukx3RiF6ZLt0dv_jRkxmiMCwbLhzIYeJ1ueXJ-GVfn4rGgnrYRR8OnCPx03_RKqK0DlFyRUaIaTHrgdX27x9D6y_xOwOkMKds6weUYYyQAI5mc1m_uL6427hG7hkDYDmAM8770pDkbS1CWtYtLa_J1u5H_c9LgCn3vTspRSQqnM46n3QfK_swZCj8WKcb1Dh2PtDBTmYaPuKsr3X0GoX9hQtDEO7CE5QrxFgjA8tLfobURsl9DzAsGWxuy328MM-EmtePgg07z2O9jKOuCfJu78efLfUbaGyKmQ");
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
