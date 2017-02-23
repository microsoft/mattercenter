using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

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


    public class JWTokenWebTestPlugin : WebTestPlugin
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

        [DisplayName("UserName")]
        [Description("UserName")]
        public string UserName { get; set; }

        [DisplayName("PassWord")]
        [Description("PassWord")]
        public string PassWord { get; set; }



        [DisplayName("ContextParamaterName")]
        [Description("ContextParamaterName")]
        public string ContextParamaterName { get; private set; }

        IConfigurationRoot configuration;
        public static ClientAssertionCertificate AssertionCert { get; set; }

        public IHostingEnvironment HostingEnvironment { get; }

        public IConfigurationRoot Configuration { get; set; }

        public KeyVaultHelper kv;


      
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/msmatter.onmicrosoft.com";

        private IHttpContextAccessor httpContextAccessor;


        public JWTokenWebTestPlugin()
        {

            // this.httpContextAccessor = httpContextAccessor;
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
        }



        public override async void PreWebTest(object sender, PreWebTestEventArgs e)
        {

            ClientId = Configuration["ADAL:clientId"];
            AppKey = Configuration["General:appkey"];
            Tenant = Configuration["General:Tenant"];
            AadInstance = Configuration["General:AADInstance"];
            ToDoResourceId = Configuration["General:SiteURL"];
            UserName = Configuration["General:AdminUserName"];
            PassWord = Configuration["General:AdminPassword"];

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(ClientId, AppKey);
          
            UserPasswordCredential user = new UserPasswordCredential(UserName, PassWord);

            var result = authContext.AcquireTokenAsync(ClientId, "12e2877a-b640-4d09-8e03-8ff0db4bfcd2", user).Result;
            e.WebTest.Context[TokenValue] = "Bearer " + result.AccessToken;
        }

      
    }


}
