using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.SharePoint.Client;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.TestHost;

using System.Net.Http;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel;

namespace JWTokenWebTestPlugin
{


    public class AntiForgeryHelper
    {
        public static string ExtractAntiForgeryToken(string htmlResponseText)
        {
            if (htmlResponseText == null) throw new ArgumentNullException("htmlResponseText");

            System.Text.RegularExpressions.Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

        public static async Task<string> ExtractAntiForgeryToken(HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
        }
    }


    public class JWTokenWebTestPlugin: WebTestPlugin 
    {
        [DisplayName("ClientId")]
        [Description("ClientId")]
        public string ClientId { get; set; }


        [DisplayName("AppKey")]
        [Description("AppKey")]
        public string AppKey { get; set; }

        [DisplayName("TokenValue")]
        [Description("TokenValue")]
        public string TokenValue { get; set; }


        [DisplayName("Tenant")]
        [Description("Tenant")]
        public string Tenant { get; set; }


        [DisplayName("AadInstance")]
        [Description("AadInstance")]
        public string AadInstance { get; set; }

        [DisplayName("ToDoResourceId")]
        [Description("ToDoResourceId")]
        public string ToDoResourceId { get; set; }

        [DisplayName("ContextParamaterName")]
        [Description("ContextParamaterName")]
        public string ContextParamaterName { get; private set; }

        IConfigurationRoot configuration;
        public static ClientAssertionCertificate AssertionCert { get; set; }

        public IHostingEnvironment HostingEnvironment { get; }

        public IConfigurationRoot Configuration { get; set; }

        public KeyVaultHelper kv;


        //private IHttpContextAccessor httpContextAccessor;
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
      //  private AuthenticationContext authContext;
       // private HttpContext Context => httpContextAccessor.HttpContext;
        private IHttpContextAccessor httpContextAccessor;


        public JWTokenWebTestPlugin ()
        {
           
            this.httpContextAccessor = httpContextAccessor;
            //string projectName = env.ApplicationName;
            //int projLength = projectName.Length;
            //string path = env.ContentRootPath;
            //int index = path.IndexOf(projectName);
            //int last = index + projLength;
            //string basePath = path.Remove(last);

            var builder = new ConfigurationBuilder()
                 .SetBasePath("C:\\Repos\\mattercenter\\tree\\master\\cloud\\src\\solution\\AuthWebTestPlugin")
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();


            Configuration = builder.Build();

      

            KeyVaultHelper kv = new KeyVaultHelper(Configuration);
            KeyVaultHelper.GetCert(Configuration);
            kv.GetKeyVaultSecretsCerticate();

            ClientId = Configuration["General:KeyVaultClientID"];
            AppKey = Configuration["General:appkey"];
            Tenant = Configuration["General:Tenant"];
            AadInstance = Configuration["General:AADInstance"];
            ToDoResourceId = Configuration["General:GraphUrl"];

           // testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
           // var contextAccessor = testServer.Host.Services.GetRequiredService<IHttpContextAccessor>().HttpContext;
    

          //  HttpRequest httpContextAccessor = contextAccessor.HttpContext.Request;



        }







        /// <summary>
        /// This method will get the access token for the service and creats SharePoint ClientContext object and returns that object
        /// </summary>
        /// <param name="url">The SharePoint Url for which the client context needs to be creatwed</param>
        /// <returns>ClientContext - Return SharePoint Client Context Object</returns>
        //public ClientContext GetClientContext(string url)
        //{
        //    try
        //    {
        //      //  string accessToken = GetAccessToken().Result;
        ////        return GetClientContextWithAccessToken(Convert.ToString(url, System.Globalization.CultureInfo.InvariantCulture), accessToken);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        /// <summary>
        /// This method will get access for the web api from the Azure Active Directory. 
        /// This method internally uses the Authorization token sent by the UI application
        /// </summary>
        /// <returns>Access Token for the web api service</returns>
        //public async Task<string> GetAccessToken()
        //{
        //    try
        //    {

        //        ///ClientId = "12e2877a-b640-4d09-8e03-8ff0db4bfcd2";
        //        //AppKey = "OWl16dmqpGu7t17o+EkuXwZYhD3WNSLKNchXRcKrZUQ=";
        //        ClientId = Configuration["ADAL:clientId"];
        //        AppKey = Configuration["General:appkey"];
        //        Tenant = Configuration["General:Tenant"];
        //        AadInstance = Configuration["General:AADInstance"];
        //        ToDoResourceId = Configuration["General:SiteURL"];
        //        //string resource = generalSettings.GraphUrl;

        //        ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
        //       // string accessToken = httpContextAccessor.Headers["Authorization"].ToString().Split(' ')[1];
        //      //  UserAssertion userAssertion = new UserAssertion(accessToken);
        //        string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, AadInstance, Tenant);
        //        //ToDo: Set the TokenCache to null. Need to implement custom token cache to support multiple users
        //        //If we dont have the custom cache, there will be some performance overhead.
        //        AuthenticationContext authContext = new AuthenticationContext(authority, null);
        //      //  AuthenticationResult result = await authContext.AcquireTokenAsync(ToDoResourceId, clientCred, userAssertion);
        //       // return result.AccessToken;
        //    }
        //    catch (AggregateException ex)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        // <summary>
        /// Uses the specified access token to create a client context. For each and every request to SPO
        /// an authorization header will be sent. With out authorization header, SPO will reject the request
        /// </summary>
        /// <param name="targetUrl">URL of the target SharePoint site</param>
        /// <param name="accessToken">Access token to be used when calling the specified targetUrl</param>
        /// <returns>A ClientContext ready to call targetUrl with the specified access token</returns>
        //  private ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        //{
        //    try
        //    {
        //        ClientContext clientContext = new ClientContext(targetUrl);
        //        clientContext.AuthenticationMode = ClientAuthenticationMode.Anonymous;
        //        clientContext.FormDigestHandlingEnabled = false;
        //        clientContext.ExecutingWebRequest +=
        //            delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
        //            {
        //                //For each SPO request, need to set bearer token to the Authorization request header
        //                webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
        //                    "Bearer " + accessToken;
        //            };
        //        return clientContext;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}



        private static async Task<UserAssertion> GetUserAssertion()
        {

            //TokenCache tokenCache = new TokenCache();
            //AuthenticationManager authManager = new AuthenticationManager);

            //using (ClientContext context = AuthenticationManager.GetAzureADNativeApplicationAuthenticatedContext(catalogURL, appID, redirectURL, tokenCache))
            //{
            //    Microsoft.SharePoint.Client.Web web = context.Web;
            //    context.Load(web, webObject => webObject.CurrentUser);
            //    context.ExecuteQuery();
            //    LoggedinUser = context.Web.CurrentUser.LoginName;
            //    accessToken = tokenCache.ReadItems().FirstOrDefault().AccessToken;
            //}


       


            string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            int posx = currentUser.LastIndexOf("\\");
            if ((posx > 0))
            {
                currentUser = currentUser.Substring((posx + 1));
                currentUser.ToUpper();
            }

            //var prop = Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", "yourusername", "yourpwd"))));
            }

            var bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;
            string userAccessToken = bootstrapContext.Token;

            UserAssertion userAssertion = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer", "matteradmin");
            return userAssertion;
        }







        public override async void PreWebTest(object sender, PreWebTestEventArgs e)
        {


            ClientId = Configuration["ADAL:clientId"];
            //ClientId = "12e2877a-b640-4d09-8e03-8ff0db4bfcd2";
            //  AppKey = "OWl16dmqpGu7t17o+EkuXwZYhD3WNSLKNchXRcKrZUQ=";
            AppKey = Configuration["General:appkey"];
            Tenant = Configuration["General:Tenant"];
            AadInstance = Configuration["General:AADInstance"];
            ToDoResourceId = Configuration["General:SiteURL"];

            //if (string.IsNullOrEmpty(this.TokenValue))
            //    throw new ArgumentNullException();

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(ClientId, AppKey);

            // UserAssertion userAssertion = GetUserAssertion();

            //var result = await authContext.AcquireTokenAsync(AADbaseUrl, clientCredential, userAssertion);

            //       var result = authContext.AcquireTokenAsync(ToDoResourceId, clientCredential, userAssertion).GetAwaiter().GetResult();   //false - third party, true - first party
            //task.Wait();
            //var result = task.Result;

            //var result = authContext.AcquireToken("https://graph.windows.net", clientCredential);
            // result.AccessToken







            //TokenValue = GetToken().GetAwaiter().GetResult();
            //  e.WebTest.Context[TokenValue] = result.AccessToken;

            // "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyIsImtpZCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDg3MTc5Nzk1LCJuYmYiOjE0ODcxNzk3OTUsImV4cCI6MTQ4NzE4MzY5NSwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTMxLjEwNy4xNzQuMTgzIiwibmFtZSI6IldpbHNvbiBHYWphcmxhIiwibm9uY2UiOiIzMDJkNzFhOC02ODQ1LTRlNGEtYWJlNS00MTg3ZGNlOGM5NGEiLCJvaWQiOiI5MzNjZTFmZC0yNjkzLTRhYWUtYjdiYS0yYTBiNjhlNDEwMjkiLCJwbGF0ZiI6IjMiLCJzdWIiOiJrcWd5dExDUlFSVWNRTW9oUFhCRUJKSE9nTnZrNEVXS0FNMGEzVVloR3BRIiwidGlkIjoiM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwidW5pcXVlX25hbWUiOiJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb20iLCJ2ZXIiOiIxLjAifQ.FNUbU4MOgi2-NheyGoXowH3JP8fzU-intm4nn3g5fyr_XYOYHQbJM3QBT2JkOdKveC4JTFpyhmxN-e_44zP22XzdZDCBjYEF2eua75AT5CSOEQ4r4oiV0xE3wjsBiCO2TNHd_m2KTOm3-QfGR-6htBmkcvTjbMojmB_wC7n9JSNmRjWJwWEjbOLmI21HdBxNncK-AhFr-T2JfgkazfiuxLNIa9ujJV_oTzDIuwB3_6KuWH4YORl4YXoRjEDXDhlEY3jvOInRBAEwNpBysJD7CeUakyMwvYWfD_A97vO8Qnj4A1HigjcO3F6xYNFjqMk_L3RJEnw6H0rIGWDhSe2pHg";

            //GetADToken(ClientId, AppKey, AadInstance, Tenant, ToDoResourceId).GetAwaiter().GetResult();

            //e.WebTest.Context[TokenValue] = GetAccessToken();


            //  GetADToken(ClientId, AppKey, AadInstance, Tenant, ToDoResourceId).GetAwaiter().GetResult(); 

            //e.WebTest.Context[TokenValue] = GetClientContext(ToDoResourceId);

            // var header = new WebTestRequestHeader("Authorization", bearerToken);

            //GetADToken(ClientId, AppKey, AadInstance, Tenant, ToDoResourceId).GetAwaiter().GetResult(); 


            e.WebTest.Context[TokenValue] = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyIsImtpZCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyJ9.eyJhdWQiOiJiOTRmMDdkZi1jODI1LTQzMWYtYjljNS1iOTQ5OWU4ZTlhYzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQvIiwiaWF0IjoxNDg3MjkzNDUyLCJuYmYiOjE0ODcyOTM0NTIsImV4cCI6MTQ4NzI5NzM1MiwiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IkdhamFybGEiLCJnaXZlbl9uYW1lIjoiV2lsc29uIiwiaXBhZGRyIjoiMTMxLjEwNy4xNDcuODMiLCJuYW1lIjoiV2lsc29uIEdhamFybGEiLCJub25jZSI6ImRhNmQ2MTZiLTk0ZmItNDI3NS05NzFiLTM5OTUxMTc5OGZhNCIsIm9pZCI6IjkzM2NlMWZkLTI2OTMtNGFhZS1iN2JhLTJhMGI2OGU0MTAyOSIsInBsYXRmIjoiMyIsInN1YiI6ImtxZ3l0TENSUVJVY1FNb2hQWEJFQkpIT2dOdms0RVdLQU0wYTNVWWhHcFEiLCJ0aWQiOiIzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJ1bmlxdWVfbmFtZSI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Im1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbSIsInZlciI6IjEuMCJ9.FnnrNFD3wDykhyiK27s7ivHzMZhaavaWzw4klMVwaEz_kKidokqmAlZbYKXVh1f_M_f312hogPGJwbdLEm9ft64fMsCreLiSyutdeXX_1sWOiPD5P2rQKSb8lPwPE-jBLF9uXkDjkDdKkZDJtbq51w3EU04FqibqixSyPSBb5W55NByznAjyzzPUN_eT7iUs5g9ULIDUoXN1mIiBw3-icL5dcssWM4WY_X4bGZxU--jqu5914pdhJTdIifLhO6esqvl314IvVqpG8BnLOdULhtkrR4_1x-v-5qhQ5WEkC1qmyjH0aoHljGvxU2SBq6wCSVPuddNw9dIFSUX6DfLS9w";
        }

        //public override void PreRequest(object sender, PreRequestEventArgs e)
        //{

        //    //WebTestRequestHeaderCollection headers = e.Request.Headers;
        //    //WebTestRequestHeader auth = e.Request.Headers[2];


        //    //TokenValue = auth.ToString();
        //    // e.WebTest.Context[TokenValue] = GetAccessToken();

        //    e.WebTest.Context[TokenValue] = GetADToken(ClientId, AppKey, AadInstance, Tenant, ToDoResourceId).GetAwaiter().GetResult();
        //}

        //public override void PostRequest(object sender, PostRequestEventArgs e)
        //{

        //    WebHeaderCollection headers = e.Response.Headers;
        //    WebTestRequestHeaderCollection headersRequest = e.Request.Headers;

        //    if (e.Request.Headers.Contains("#id_token"))
        //        {


        //    }
        //    string re = e.Response.BodyString;
        //    string t;
        //    if (re.Contains("#id_token"))
        //    {
        //        t = re;
        //    }



        //    string[] head = headers.AllKeys;
        //    int index = 0;
        //    bool foundToken = false;
        //  foreach(string header in head)
        //    {

        //        if (header.Contains("Location"))
        //        {
        //            foundToken = true;
        //            break;
        //        }
        //        index++;
        //    }

        //    if (foundToken)
        //    {
        //        WebTestRequestHeader auth = e.Request.Headers[index];


        //        TokenValue = auth.ToString();
        //        e.WebTest.Context[TokenValue] = GetAccessToken();
        //    }

        //}




        //public override void PreRequest(object sender, PreRequestEventArgs e)
        //{
        //    var token = GetADToken(ClientId, AppKey, AadInstance, Tenant, ToDoResourceId).GetAwaiter().GetResult();
        //    var header = new WebTestRequestHeader("Authorization", token);
        //    e.Request.Headers.Add(header);
        //}


        public async Task<String> GetToken()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string token =  "";
            using (var client = testServer.CreateClient().AcceptJson())
            {
                response = await client.PostAsJsonAsync("https://matterwebapp.azurewebsites.net/api/v1/matter/cancreate", client);
                var result = response.Content.ReadAsStringAsync().Result;
                var tokens =  response.RequestMessage.Headers.GetValues("Authorization");

                foreach(string tok in tokens)
                {
                    token = tok;
                }
            }

            // response.EnsureSuccessStatusCode();

            // string antiForgeryToken = await AntiForgeryHelper.ExtractAntiForgeryToken(response);
            //return antiForgeryToken;
            return token.Remove(0, 7);
        }

        public async Task<string> GetADToken(string clientid, string appkey, string aadinstance,  string tenant, string ToDoResourceID)
            {

        



           AuthenticationResult result = null;
           // string accessToken = Context.Request.Headers["Authorization"].ToString().Split(' ')[1];
           // UserAssertion userAssertion = new UserAssertion(accessToken);
            var authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, aadinstance, tenant);
           // var Audience = "12e2877a-b640-4d09-8e03-8ff0db4bfcd2";
            AuthenticationContext authcontext = new AuthenticationContext(authority, new TokenCache());
            ClientCredential credential = new ClientCredential(clientid, appkey);
            result = await authcontext.AcquireTokenAsync(ToDoResourceID, credential);
            return result.AccessToken;
        }

    }


}
