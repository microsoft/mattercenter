using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.SwaggerGen;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNet.Http;

#region Matter Namespaces
using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Legal.MatterCenter.Service;
using Microsoft.Legal.MatterCenter.Service.Filters;
using Microsoft.AspNet.Authentication.JwtBearer;
using System.Globalization;
#endregion

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class Startup
    {

        #region Properties
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IConfigurationRoot Configuration { get; set; }
        public IApplicationEnvironment ApplicationEnvironment { get; }
        #endregion

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv, ILoggerFactory logger)
        {
            this.HostingEnvironment = env;
            this.ApplicationEnvironment = appEnv;
            this.LoggerFactory = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            if (HostingEnvironment.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
            ConfigureSettings(services);
            services.AddCors();
            services.AddLogging();
            ConfigureMvc(services, LoggerFactory);
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            ConfigureMatterPackages(services);
            ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            var log = loggerFactory.CreateLogger<Startup>();
            try
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
                app.UseIISPlatformHandler();

                app.UseApplicationInsightsRequestTelemetry();
                if (env.IsDevelopment())
                {
                    app.UseBrowserLink();
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }

                app.UseApplicationInsightsExceptionTelemetry();
                app.UseDefaultFiles();
                app.UseStaticFiles();
                CheckAuthorization(app);
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                app.UseMvc();
                app.UseSwaggerGen();
                app.UseSwaggerUi();
            }
            catch (Exception ex)
            {
                app.Run(
                        async context => {
                            log.LogError($"{ex.Message}");
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync(ex.Message).ConfigureAwait(false);
                            await context.Response.WriteAsync(ex.StackTrace).ConfigureAwait(false);
                        });

            }
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        #region Private Methods

        #region Swagger
        private string pathToDoc = "Microsoft.Legal.MatterCenter.Service.xml";

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureSwaggerDocument(options => {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Matter Center API Version V1",
                    Description = "This matter center api is for V1 release"
                });
                options.IgnoreObsoleteActions = true;
                options.OperationFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlActionComments(pathToDoc));

            });

            services.ConfigureSwaggerSchema(options =>
            {
                options.DescribeAllEnumsAsStrings = true;
                options.IgnoreObsoleteProperties = true;
                options.ModelFilter(new Swashbuckle.SwaggerGen.XmlComments.ApplyXmlTypeComments(pathToDoc));

            });
        }


        #endregion

        private void ConfigureMvc(IServiceCollection services, ILoggerFactory logger)
        {
            var builder = services.AddMvc().AddDataAnnotationsLocalization();
            builder.AddJsonOptions(o => {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
                o.SerializerSettings.Formatting = Formatting.Indented;
            });

            //builder.AddMvcOptions(o => { o.Filters.Add(new MatterCenterFilter(logger)); });
            builder.AddMvcOptions(o => { o.Filters.Add(new MatterCenterExceptionFilter(logger)); });
        }


        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<GeneralSettings>(this.Configuration.GetSection("General"));
            services.Configure<TaxonomySettings>(this.Configuration.GetSection("Taxonomy"));
            services.Configure<MatterSettings>(this.Configuration.GetSection("Matter"));
            services.Configure<DocumentSettings>(this.Configuration.GetSection("Document"));
            services.Configure<SharedSettings>(this.Configuration.GetSection("Shared"));
            services.Configure<MailSettings>(this.Configuration.GetSection("Mail"));
            services.Configure<ErrorSettings>(this.Configuration.GetSection("ErrorMessages"));
            services.Configure<ListNames>(this.Configuration.GetSection("ListNames"));
            services.Configure<LogTables>(this.Configuration.GetSection("LogTables"));
            services.Configure<SearchSettings>(this.Configuration.GetSection("Search"));
            services.Configure<CamlQueries>(this.Configuration.GetSection("CamlQueries"));
        }

        private void ConfigureMatterPackages(IServiceCollection services)
        {
            services.AddSingleton<ISPOAuthorization, SPOAuthorization>();
            services.AddSingleton<ITaxonomyRepository, TaxonomyRepository>();
            services.AddScoped<IMatterCenterServiceFunctions, MatterCenterServiceFunctions>();
            services.AddSingleton<ITaxonomy, Taxonomy>();
            services.AddSingleton<ISite, Site>();
            services.AddSingleton<IMatterRepository, MatterRepository>();
            services.AddSingleton<IUsersDetails, UsersDetails>();
            services.AddSingleton<ICustomLogger, CustomLogger>();
            services.AddSingleton<IDocumentRepository, DocumentRepository>();
            services.AddSingleton<ISearch, Search>();
            services.AddSingleton<ISPList, SPList>();
            services.AddSingleton<ISPPage, SPPage>();
            services.AddSingleton<ISharedRepository, SharedRepository>();
            services.AddSingleton<IValidationFunctions, ValidationFunctions>();
            services.AddSingleton<IEditFunctions, EditFunctions>();
            services.AddSingleton<IMatterProvision, MatterProvision>();
        }

        private void CheckAuthorization(IApplicationBuilder app)
        {
            app.UseJwtBearerAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.Authority = String.Format(CultureInfo.InvariantCulture,
                    this.Configuration.GetSection("General").GetSection("AADInstance").Value.ToString(),
                    this.Configuration.GetSection("General").GetSection("Tenant").Value.ToString());
                options.Audience = this.Configuration.GetSection("General").GetSection("ClientId").Value.ToString();
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context => {
                        return Task.FromResult(0);
                    },
                    OnValidatedToken = context => {
                        return Task.FromResult(0);
                    }
                };
            });
        }
        #endregion
    }
}
