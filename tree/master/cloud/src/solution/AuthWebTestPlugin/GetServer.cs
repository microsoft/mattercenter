using Microsoft.VisualStudio.TestTools.WebTesting;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Matter.Legal.MatterCenter.PerfTestPlugins
{
    public class GetServer : WebTestPlugin
    {
        [DisplayName("WebServer")]
        [Description("WebServer")]
        public string WebServer { get; set; }

        public IConfigurationRoot Configuration { get; set; }

        public GetServer()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath("C:\\Repos\\mattercenter\\tree\\master\\cloud\\src\\solution\\AuthWebTestPlugin")
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
           
            Configuration = builder.Build();
        }

        public override async void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            e.WebTest.Context[WebServer] = Configuration["General:WebServerURL"];
        }

    }
}
