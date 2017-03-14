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

            var projectName = "AuthWebTestPlugin";
            var solLength = "solution".Length;
            var path = System.IO.Directory.GetCurrentDirectory();
            int index = path.IndexOf("solution");
            int last = index + solLength;
            string basePath = path.Remove(last);
            path = basePath + "\\" + projectName;

            var builder = new ConfigurationBuilder()
                 .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 
            Configuration = builder.Build();
        }

        public override async void PreWebTest(object sender, PreWebTestEventArgs e)
        {
            e.WebTest.Context[WebServer] = Configuration["General:WebServerURL"];
        }

    }
}
