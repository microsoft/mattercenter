using Xunit;
using Microsoft.AspNetCore.TestHost;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Extensions.Options;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Web;

using Moq;
using Microsoft.Legal.MatterCenter.Web.Controllers;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using Microsoft.Legal.MatterCenter.Web.Common;

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
        public void GetAllUIConfigs()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();
          
            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns("DefaultEndpointsProtocol=https;AccountName=mattercenterlogstoragev0;AccountKey=Y3s1Wz+u2JQ/wl5WSVB5f+31oXyBlcdFVLk99Pgo8y8/vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d/QePQ==");

            var environmentMoq = new Moq.Mock<IHostingEnvironment>();
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mcfork\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");

            var matterCenterServiceFunctionsMoq = new Moq.Mock<IMatterCenterServiceFunctions>();
               
            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("ConfigGroup");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");


            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();

            ConfigRepository configRepository = new ConfigRepository(generalSettingsMoq.Object, uiConfigsMoq.Object);

            generalSettingsMoq.SetupGet(g => g.Value).Returns(genS);
            errorSettingsMoq.SetupAllProperties();


            ConfigController controller = new ConfigController(errorSettingsMoq.Object, generalSettingsMoq.Object, uiConfigsMoq.Object, logTableMoq.Object, matterCenterServiceFunctionsMoq.Object, configRepository, environmentMoq.Object);

            var result = controller.Get("");

            Assert.True(result.Status > 0);
        }

        [Fact]
        public void GetSubsetUIConfigs()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns("DefaultEndpointsProtocol=https;AccountName=mattercenterlogstoragev0;AccountKey=Y3s1Wz+u2JQ/wl5WSVB5f+31oXyBlcdFVLk99Pgo8y8/vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d/QePQ==");

            var environmentMoq = new Moq.Mock<IHostingEnvironment>();
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mcfork\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");

            var matterCenterServiceFunctionsMoq = new Moq.Mock<IMatterCenterServiceFunctions>();

            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("ConfigGroup");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");

            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();

            ConfigRepository configRepository = new ConfigRepository(generalSettingsMoq.Object, uiConfigsMoq.Object);

            generalSettingsMoq.SetupGet(g => g.Value).Returns(genS);
            errorSettingsMoq.SetupAllProperties();

            ConfigController controller = new ConfigController(errorSettingsMoq.Object, generalSettingsMoq.Object, uiConfigsMoq.Object, logTableMoq.Object, matterCenterServiceFunctionsMoq.Object, configRepository, environmentMoq.Object);

            var result = controller.Get("Home");

            Assert.True(result.Status > 0);
        }

        [Fact]
        public void InsertUpdateUIConfigs()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns("DefaultEndpointsProtocol=https;AccountName=mattercenterlogstoragev0;AccountKey=Y3s1Wz+u2JQ/wl5WSVB5f+31oXyBlcdFVLk99Pgo8y8/vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d/QePQ==");

            var environmentMoq = new Moq.Mock<IHostingEnvironment>();
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mcfork\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");

            var matterCenterServiceFunctionsMoq = new Moq.Mock<IMatterCenterServiceFunctions>();

            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("ConfigGroup");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");

            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();

            ConfigRepository configRepository = new ConfigRepository(generalSettingsMoq.Object, uiConfigsMoq.Object);

            generalSettingsMoq.SetupGet(g => g.Value).Returns(genS);
            errorSettingsMoq.SetupAllProperties();

            ConfigController controller = new ConfigController(errorSettingsMoq.Object, generalSettingsMoq.Object, uiConfigsMoq.Object, logTableMoq.Object, matterCenterServiceFunctionsMoq.Object, configRepository, environmentMoq.Object);


            var configsStr = (@"{""Home"": {
    ""ContextualHelpHeader"": ""Matter Center For Outlook"",
    ""ContextualHelpBottomText"": ""Questions? Contact "",
    ""HelpRequesURL"": ""https://msmatter.sharepoint.com/sites/catalog"",
    ""MatterCenterSupportLinkText"": ""Matter Center Aisling Support"",
    ""MatterCenterSupportLink"": ""mailto:support@supportsite.com"",
    ""Change"": ""mailto:aisling@supportsite.com"",
  },
  ""MatterUsers"": {
    ""SearchUsersURL"": ""https://msmatter.sharepoint.com/sites/client"",
    ""StampedPropertiesURL"": ""https://msmatter.sharepoint.com/sites/microsoft"",
    ""Add"": ""test"",
  }}");



            var result = controller.InsertUpdate(configsStr);

            Assert.True(result.Status > 0);
        }




        /// This unit test will try to get all the users who can see a particular item
        /// </summary>
        [Fact]
        public void GetConfigsFromTableRep()
        {


            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString  =  "DefaultEndpointsProtocol = https; AccountName = mattercenterlogstoragev0; AccountKey = Y3s1Wz + u2JQ / wl5WSVB5f + 31oXyBlcdFVLk99Pgo8y8 / vxSO7P8wOjbbWdcS7mAZLkqv8njHROc1bQj8d / QePQ == ";
            var m = new Moq.Mock<IOptions<GeneralSettings>>();
            var l = new Moq.Mock<IOptions<UIConfigSettings>>();
            
            m.SetupGet(g => g.Value).Returns(genS);
            m.SetupAllProperties();


            ConfigRepository configRepository = new ConfigRepository(m.Object, l.Object);
            var response = configRepository.GetConfigEntities("");

        }


    }
}

