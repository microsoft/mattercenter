using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;

using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Extensions.Options;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Legal.MatterCenter.Web.Common;
using Microsoft.Legal.MatterCenter.Web.Controllers;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class ConfigUnitTest
    {


      

        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
        //public ILoggerFactory LoggerFactory { get; }
        private IConfigRepository configRepository; //{ get; set; }
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;
        private IHostingEnvironment hostingEnvironment;

        public ConfigUnitTest()
        {
        
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }



        [Fact]
        public void GetConfigController()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";

            var m = new Moq.Mock<IOptionsMonitor<ErrorSettings>>();
          
            var l = new Moq.Mock<IOptionsMonitor<GeneralSettings>>();
            l.SetupGet(p => p.CurrentValue.CloudStorageConnectionString).Returns("DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ");
            var h = new Moq.Mock<IHostingEnvironment>();
            h.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\MCFork\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");
            var ma = new Moq.Mock<IMatterCenterServiceFunctions>();
            var r = new Moq.Mock<IConfigRepository>();
            
            var lo = new Moq.Mock<IOptionsMonitor<LogTables>>();


            ConfigRepository configRepository = new ConfigRepository(l.Object, lo.Object);

            l.SetupGet(g => g.CurrentValue).Returns(genS);
            m.SetupAllProperties();

          
            DynamicTableEntity request = new DynamicTableEntity();
                
            ConfigController controller = new ConfigController( m.Object, l.Object, ma.Object, configRepository, h.Object);
     

            var result = controller.Get(request);

           // Assert.IsTrue(result.Count > 0);
        }


        /// This unit test will try to get all the users who can see a particular item
        /// </summary>
        [Fact]
        public  void GetConfigsFromTableRep()
        {
            

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString  =  "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";
            var m = new Moq.Mock<IOptionsMonitor<GeneralSettings>>();
            var l = new Moq.Mock<IOptionsMonitor<LogTables>>();
            
            m.SetupGet(g => g.CurrentValue).Returns(genS);
            m.SetupAllProperties();


            ConfigRepository configRepository = new ConfigRepository(m.Object, l.Object);
            var response = configRepository.GetConfigEntities();

        }


    }
}
