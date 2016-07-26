using Xunit;
using Microsoft.AspNetCore.TestHost;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Extensions.Options;

using Microsoft.Legal.MatterCenter.Utility;

using Moq;
using Microsoft.Legal.MatterCenter.Web.Common;
using Microsoft.Legal.MatterCenter.Web.Controllers;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class ConfigUnitTest
    {

     
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";

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

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptionsMonitor<ErrorSettings>>();
          
            var generalSettingsMoq = new Moq.Mock<IOptionsMonitor<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.CurrentValue.CloudStorageConnectionString).Returns("DefaultEndpointsProtocol=https;AccountName=mattercenterlogstoragev0;AccountKey=Y3s1Wz+u2JQ/wl5WSVB5f+31oXyBlcdFVLk99Pgo8y8/vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d/QePQ==");

            var environmentMoq = new Moq.Mock<IHostingEnvironment>(); 
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mcfork\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");

            var matterCenterServiceFunctionsMoq = new Moq.Mock<IMatterCenterServiceFunctions>();
               
            var logTablesMoq = new Moq.Mock<IOptionsMonitor<LogTables>>();

            ConfigRepository configRepository = new ConfigRepository(generalSettingsMoq.Object, logTablesMoq.Object);

            generalSettingsMoq.SetupGet(g => g.CurrentValue).Returns(genS);
            errorSettingsMoq.SetupAllProperties();

            String request = "";
                
            ConfigController controller = new ConfigController( errorSettingsMoq.Object, generalSettingsMoq.Object, matterCenterServiceFunctionsMoq.Object, configRepository, environmentMoq.Object);
     
            var result = controller.Get(request);

            Assert.True(result.Status > 0);
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
