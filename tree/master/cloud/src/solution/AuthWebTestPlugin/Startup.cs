using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#region Matter Namespaces

#endregion


namespace Matter.Legal.MatterCenter.PerfTestPlugins
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
        }

        public void ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            ConfigureMatterPackages(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var log = loggerFactory.CreateLogger<Startup>();
            try
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));                              
            }
            catch (System.Exception ex)
            {
               
            }
        }

        
        private void ConfigureMatterPackages(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

    }
}

