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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJkYTRkMTk0NS0xMWRhLTRjMTEtOTI4Mi1iOWYzYzI3YjYxYWUiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNDYwNTkwMjUwLCJuYmYiOjE0NjA1OTAyNTAsImV4cCI6MTQ2MDU5NDE1MCwiYWNyIjoiMSIsImFtciI6WyJ3aWEiXSwiYXBwaWQiOiJjODhhZWE1Yy01ZjNhLTRkODQtODIzNC03NDA0N2ZkYTFlZWQiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IlBlZGRha290bGEiLCJnaXZlbl9uYW1lIjoiTGFrc2htYW5hc3dhbXkgUHJlbWNoYW5kIiwiaW5fY29ycCI6InRydWUiLCJpcGFkZHIiOiIxMzEuMTA3LjE0Ny41NiIsIm5hbWUiOiJMYWtzaG1hbmFzd2FteSBQcmVtY2hhbmQgUGVkZGFrb3RsYSAoSW5mb3N5cyBMdGQpIiwib2lkIjoiYzUxY2E5YzQtYmVkMC00ZDNjLTlhODctYTJlMjdkYjE4YWE5Iiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTIxMjc1MjExODQtMTYwNDAxMjkyMC0xODg3OTI3NTI3LTEzOTEwMDExIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoicVJFVERXN1hNQndBcUxNRkdCZWg0dllNM3N0ZlNsd0dYb19scEp0OVROVSIsInRpZCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsInVuaXF1ZV9uYW1lIjoidi1sYXBlZGRAbWljcm9zb2Z0LmNvbSIsInVwbiI6InYtbGFwZWRkQG1pY3Jvc29mdC5jb20iLCJ2ZXIiOiIxLjAifQ.HN4_XORn5z8aq8bUGq--EphQZkl6S4pvXXq5eeRvsmIae6jgPO1idHpoTSqpdZoF68GaC3e-FQpxOpJgJ-qPm99HrmEApZ1p72zqqkic-1ZiaOY_KrNyyAM5OYV1XnTkzN8vOzwM79o4iFStq0OLqlDA3ighGxkEGXYet9M5sybhPtvsH_sgOczNN1ZFWq1_18EuQyP9t0SkgtLlMFqcHbNygPIWVt6VaC0yxSo5ecrj2EKuC_T7r4ti5epmGbS7RftfSLAa9DQiQDRiTbVThjWiIcxmJ_66ynMrrf7ddHqgzV0Pz2z9mFbE9W9JoJN6kqBavx89TU1Y4Z2Im32Z5Q");
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
