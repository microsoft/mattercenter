using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#region Matter Namespaces

#endregion


namespace JWTokenWebTestPlugin
{
    public class Startup
    {
        #region Properties
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IConfigurationRoot Configuration { get; set; }

        #endregion

        public Startup(IHostingEnvironment env, ILoggerFactory logger)
        {
            this.HostingEnvironment = env;
            this.LoggerFactory = logger;


            //string projectName = env.ApplicationName;
            //int projLength = projectName.Length;
            //string path = env.ContentRootPath;
            //int index = path.IndexOf(projectName);
            //int last = index + projLength;
            //string basePath = path.Remove(last);

            //var builder = new ConfigurationBuilder()
            //     .SetBasePath(basePath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables();

            //if (HostingEnvironment.IsDevelopment())
            //{
            //    // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
             
            //}
            //Configuration = builder.Build();


            //KeyVaultHelper kv = new KeyVaultHelper(Configuration);
            //KeyVaultHelper.GetCert(Configuration);
            //kv.GetKeyVaultSecretsCerticate();

         
            // JWTokenWebTestPlugin jWTokenWebTestPlugin = new JWTokenWebTestPlugin();

        }

        public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            //KeyVaultHelper keyVaultHelper = new KeyVaultHelper(Configuration);
            //KeyVaultHelper.GetCert(Configuration);
            //keyVaultHelper.GetKeyVaultSecretsCerticate();
            // services.AddSingleton(Configuration);
            // ConfigureSettings(services);

            ConfigureMatterPackages(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory
          //,IOptionsMonitor<GeneralSettings> generalSettings,
          //IOptionsMonitor<TaxonomySettings> taxonomySettings,
          //IOptionsMonitor<MatterSettings> matterSettings,
          //IOptionsMonitor<DocumentSettings> documentSettings,
          //IOptionsMonitor<SharedSettings> sharedSettings,
          //IOptionsMonitor<MailSettings> mailSettings,
          //IOptionsMonitor<ListNames> listNames,
          //IOptionsMonitor<LogTables> logTables,
          //IOptionsMonitor<SearchSettings> searchSettings,
          //IOptionsMonitor<CamlQueries> camlQueries,
          //IOptionsMonitor<ContentTypesConfig> contentTypesConfig,
          //IOptionsMonitor<MatterCenterApplicationInsights> matterCenterApplicationInsights
          )
        {
            //CreateConfig(env);

            var log = loggerFactory.CreateLogger<Startup>();
            try
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                

                
            }
            catch (System.Exception ex)
            {
               
            }
        }

        //private void ConfigureSettings(IServiceCollection services)
        //{
        //    services.Configure<GeneralSettings>(this.Configuration.GetSection("General"));
        //    services.Configure<UIConfigSettings>(this.Configuration.GetSection("UIConfig"));
        //    services.Configure<TaxonomySettings>(this.Configuration.GetSection("Taxonomy"));
        //    services.Configure<MatterSettings>(this.Configuration.GetSection("Matter"));
        //    services.Configure<DocumentSettings>(this.Configuration.GetSection("Document"));
        //    services.Configure<SharedSettings>(this.Configuration.GetSection("Shared"));
        //    services.Configure<MailSettings>(this.Configuration.GetSection("Mail"));
        //    services.Configure<ErrorSettings>(this.Configuration.GetSection("ErrorMessages"));
        //    services.Configure<ListNames>(this.Configuration.GetSection("ListNames"));
        //    services.Configure<LogTables>(this.Configuration.GetSection("LogTables"));
        //    services.Configure<SearchSettings>(this.Configuration.GetSection("Search"));
        //    services.Configure<CamlQueries>(this.Configuration.GetSection("CamlQueries"));
        //    services.Configure<ContentTypesConfig>(this.Configuration.GetSection("ContentTypes"));
        //    services.Configure<MatterCenterApplicationInsights>(this.Configuration.GetSection("ApplicationInsights"));

        //}

        private void ConfigureMatterPackages(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

    }
}

