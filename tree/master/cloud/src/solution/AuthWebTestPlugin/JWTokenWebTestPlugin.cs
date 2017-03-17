using Microsoft.VisualStudio.TestTools.WebTesting;
using System.ComponentModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System.IO;

namespace Matter.Legal.MatterCenter.PerfTestPlugins
{

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

        [DisplayName("PerfAppId")]
        [Description("PerfAppId")]
        public string PerfAppId { get; set; }

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

            var projectName = "AuthWebTestPlugin";
            var solLength = "solution".Length;
            var path = Directory.GetCurrentDirectory();
            int index = path.IndexOf("solution");
            int last = index + solLength;
            string basePath = path.Remove(last);
            path = basePath + "\\" +  projectName;

            var builder = new ConfigurationBuilder()
                 .SetBasePath(path)
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
            PerfAppId = Configuration["General:PerfAppID"];

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(ClientId, AppKey);
          
            UserPasswordCredential user = new UserPasswordCredential(UserName, PassWord);

            var result = authContext.AcquireTokenAsync(ClientId, PerfAppId, user).Result;
            e.WebTest.Context[TokenValue] = "Bearer " + result.AccessToken;
        } 
    }
}
