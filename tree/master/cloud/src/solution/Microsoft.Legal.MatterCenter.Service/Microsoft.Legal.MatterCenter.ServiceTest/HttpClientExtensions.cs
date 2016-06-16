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
                "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDY2MDk1OTQwLCJuYmYiOjE0NjYwOTU5NDAsImV4cCI6MTQ2NjA5OTg0MCwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTY3LjIyMC4xMDQuNDAiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6IjNjZmYxMmJmLTkxOWEtNGMyOS1iNWNhLTE4ODRjYjU2Y2ZkZSIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.Fwu1qCsUo5a3Xu17zfk-XLK0qqblD515PnIDFQUJ1CFt8ulkFhERqKPwMuSm43YbjM3R0J9Jvcd6_TGyPtY8MtfCWB-NBnm0tyG6DKUK9L6Rs1iNajz3d1Qggh3OaBp3w4rYDkVeYMHxI-jxlZV2KQ49P-8_VlYtW-PzVtMr6UUccDUS0VGtGZ3Wgt9Vzlvx5SAJLITq1QQzSoYf2eV-raLjMSHoTc5G48_EnKXJ-FPjWSsCL99dKyXeMb6OPP_1i8hl0-__SdeOGMWtE-e0D-cz_ZNbdE-fTRaZvhoY43Bo8ctSILy2D_HZrRxn7MNhqp9Fy-LeHBS0zWZylZfo-w");
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
