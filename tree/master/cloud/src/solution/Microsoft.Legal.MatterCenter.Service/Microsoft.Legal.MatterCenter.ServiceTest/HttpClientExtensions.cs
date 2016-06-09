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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY1NDMwMjExLCJuYmYiOjE0NjU0MzAyMTEsImV4cCI6MTQ2NTQzNDExMSwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC4xMDMuMTIyIiwibmFtZSI6IldpbHNvbiBHYWphcmxhIiwibm9uY2UiOiI2Nzc2MDBmOS04NGUxLTRlNmMtYjBmNy04NDc5ODY0ZDhkOTIiLCJvaWQiOiI5MzNjZTFmZC0yNjkzLTRhYWUtYjdiYS0yYTBiNjhlNDEwMjkiLCJzdWIiOiJrcWd5dExDUlFSVWNRTW9oUFhCRUJKSE9nTnZrNEVXS0FNMGEzVVloR3BRIiwidGlkIjoiM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwidW5pcXVlX25hbWUiOiJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb20iLCJ2ZXIiOiIxLjAifQ.T-r5-g2o7iKMEhvhRwr-Eg2Aq9g0ZFVCfTnimZL8E5wzc0pPIkpi6gxhftqEv4W76HwlHH-jg9Qq8xVulnV5XqIx7i8Jx7neViqvBEQdQamdt7bHgMSPNIgImtrY-CoD11YEjKZ8hoIT8mATiMhZCIZXM6drAKSHxlQIu8Rj1PfuTExd3LaxEV5ObK1umfTQU25iFFllAke7lYjOEffD6t2wzVMepowkHpl2Td7cbnp2WaM1IAZcdJiZjZn482kZeAFpyMsRzWdZ7wLd8FU5RqyUCkrF0PkoJzudm8QzPVmkS5V6G3ezVEWVIch7mErH3qkBEBYvKaNhpTe2ruZsJA");
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
