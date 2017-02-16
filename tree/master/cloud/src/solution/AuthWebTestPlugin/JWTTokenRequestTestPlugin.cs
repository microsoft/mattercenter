//using Microsoft.VisualStudio.TestTools.WebTesting;
//using System;
//using System.IO;
//using System.ComponentModel;
//using System.Threading.Tasks;
//using Microsoft.Azure.KeyVault;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.Json;
//using System.Web.Hosting;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Http;
//using Microsoft.SharePoint.Client;
//using Microsoft.Extensions.Options;

//namespace JWTokenWebTestPlugin
//{
//    public class JWTokenRequestTestPlugin : WebTestRequestPlugin

//    {
//        [DisplayName("ClientId")]
//        [Description("ClientId")]
//        public string ClientId { get; set; }


//        [DisplayName("AppKey")]
//        [Description("AppKey")]
//        public string AppKey { get; set; }

//        [DisplayName("TokenValue")]
//        [Description("TokenValue")]
//        public string TokenValue { get; set; }


//        [DisplayName("Tenant")]
//        [Description("Tenant")]
//        public string Tenant { get; set; }


//        [DisplayName("AadInstance")]
//        [Description("AadInstance")]
//        public string AadInstance { get; set; }

//        [DisplayName("ToDoResourceId")]
//        [Description("ToDoResourceId")]
//        public string ToDoResourceId { get; set; }

//        [DisplayName("ContextParamaterName")]
//        [Description("ContextParamaterName")]
//        public string ContextParamaterName { get; private set; }

//        IConfigurationRoot configuration;
//        public static ClientAssertionCertificate AssertionCert { get; set; }

//        public IHostingEnvironment HostingEnvironment { get; }

//        public IConfigurationRoot Configuration { get; set; }

//        public KeyVaultHelper kv;


//        private IHttpContextAccessor httpContextAccessor;



//        public JWTokenRequestTestPlugin()
//        {

//            //  this.httpContextAccessor = httpContextAccessor;
//            //string projectName = env.ApplicationName;
//            //int projLength = projectName.Length;
//            //string path = env.ContentRootPath;
//            //int index = path.IndexOf(projectName);
//            //int last = index + projLength;
//            //string basePath = path.Remove(last);

//            var builder = new ConfigurationBuilder()
//                 .SetBasePath("C:\\Repos\\mattercenter\\tree\\master\\cloud\\src\\solution\\AuthWebTestPlugin")
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .AddEnvironmentVariables();


//            Configuration = builder.Build();



//            KeyVaultHelper kv = new KeyVaultHelper(Configuration);
//            KeyVaultHelper.GetCert(Configuration);
//            kv.GetKeyVaultSecretsCerticate();

//            ClientId = Configuration["General:KeyVaultClientID"];
//            AppKey = Configuration["General:appkey"];
//            Tenant = Configuration["General:Tenant"];
//            AadInstance = Configuration["General:AADInstance"];
//            ToDoResourceId = Configuration["General:SiteURL"];

//        }

//        private HttpContext Context => httpContextAccessor.HttpContext;

//        /// <summary>
//        /// This method will get the access token for the service and creats SharePoint ClientContext object and returns that object
//        /// </summary>
//        /// <param name="url">The SharePoint Url for which the client context needs to be creatwed</param>
//        /// <returns>ClientContext - Return SharePoint Client Context Object</returns>
//        public ClientContext GetClientContext(string url)
//        {
//            try
//            {
//                string accessToken = GetAccessToken().Result;
//                return GetClientContextWithAccessToken(Convert.ToString(url, System.Globalization.CultureInfo.InvariantCulture), accessToken);
//            }
//            catch (Exception ex)
//            {

//                throw;
//            }
//        }


//        /// <summary>
//        /// This method will get access for the web api from the Azure Active Directory. 
//        /// This method internally uses the Authorization token sent by the UI application
//        /// </summary>
//        /// <returns>Access Token for the web api service</returns>
//        private async Task<string> GetAccessToken()
//        {
//            try
//            {
//                ClientId = Configuration["ADAL:clientId"];
//                AppKey = Configuration["General:appkey"];
//                Tenant = Configuration["General:Tenant"];
//                AadInstance = Configuration["General:AADInstance"];
//                ToDoResourceId = Configuration["General:SiteURL"];

//                ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
//                string accessToken = TokenValue; //Context.Request.Headers["Authorization"].ToString().Split(' ')[1];
//                UserAssertion userAssertion = new UserAssertion(accessToken);
//                string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, AadInstance, Tenant);
//                //ToDo: Set the TokenCache to null. Need to implement custom token cache to support multiple users
//                //If we dont have the custom cache, there will be some performance overhead.
//                AuthenticationContext authContext = new AuthenticationContext(authority, null);
//                AuthenticationResult result = await authContext.AcquireTokenAsync(ToDoResourceId, clientCred, userAssertion);
//                return result.AccessToken;
//            }
//            catch (AggregateException ex)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {

//                throw;
//            }
//        }


//        // <summary>
//        /// Uses the specified access token to create a client context. For each and every request to SPO
//        /// an authorization header will be sent. With out authorization header, SPO will reject the request
//        /// </summary>
//        /// <param name="targetUrl">URL of the target SharePoint site</param>
//        /// <param name="accessToken">Access token to be used when calling the specified targetUrl</param>
//        /// <returns>A ClientContext ready to call targetUrl with the specified access token</returns>
//        private ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
//        {
//            try
//            {
//                ClientContext clientContext = new ClientContext(targetUrl);
//                clientContext.AuthenticationMode = ClientAuthenticationMode.Anonymous;
//                clientContext.FormDigestHandlingEnabled = false;
//                clientContext.ExecutingWebRequest +=
//                    delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
//                    {
//                        //For each SPO request, need to set bearer token to the Authorization request header
//                        webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
//                            "Bearer " + accessToken;
//                    };
//                return clientContext;
//            }
//            catch (Exception ex)
//            {

//                throw;
//            }
//        }

        




     
//        public override void PreRequest(object sender, PreRequestEventArgs e)
//        {

//            WebTestRequestHeaderCollection headers  = e.Request.Headers;
//            WebTestRequestHeader auth = e.Request.Headers[2];
            
            
//                TokenValue = auth.ToString();
            
              
//        }

     

//        public async Task<string> GetADToken(string clientid, string appkey, string aadinstance, string tenant, string ToDoResourceID)
//        {


//            AuthenticationResult result = null;
//            // string accessToken = Context.Request.Headers["Authorization"].ToString().Split(' ')[1];
//            // UserAssertion userAssertion = new UserAssertion(accessToken);
//            var authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, aadinstance, tenant);
//            AuthenticationContext authcontext = new AuthenticationContext(authority, new TokenCache());
//            ClientCredential credential = new ClientCredential(clientid, appkey);
//            result = await authcontext.AcquireTokenAsync(ToDoResourceID, credential);
//            return result.AccessToken;
//        }

//    }


//}
