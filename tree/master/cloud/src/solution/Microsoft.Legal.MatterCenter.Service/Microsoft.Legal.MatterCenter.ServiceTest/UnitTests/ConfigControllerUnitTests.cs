using Xunit;
using Microsoft.AspNetCore.TestHost;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Legal.MatterCenter.Repository;
using Microsoft.Extensions.Options;

using Microsoft.Legal.MatterCenter.Utility;
using Microsoft.Legal.MatterCenter.Web.Controllers;
using Microsoft.Legal.MatterCenter.Web.Common;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Legal.MatterCenter.ServiceTest;

namespace Microsoft.Legal.MatterCenter.UnitTest
{
    public class ConfigUnitTests : IClassFixture<CompositionRootFixture>
    {


        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
        private readonly CompositionRootFixture _fixture;

        private IConfigRepository configRepository; //{ get; set; }
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;


        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ILoggerFactory LoggerFactory { get; }


        public ConfigUnitTests(CompositionRootFixture fixture)
        {

            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _fixture = fixture;

        }


        [Fact]
        public void GetAllUIConfigsController()
        {
            //Arrange
            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();
            errorSettingsMoq.SetupAllProperties();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns(genS.CloudStorageConnectionString);
            generalSettingsMoq.SetupGet(g => g.Value).Returns(genS);

            var environmentMoq = new Moq.Mock<IHostingEnvironment>();
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mattercenter\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");
       
            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("ConfigGroup");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");

            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();     
           
            //Setup Mock Repository data
            List<DynamicTableEntity> mockList = new List<DynamicTableEntity>();
            mockList.Add(new DynamicTableEntity("test", "test"));
            mockList.Add(new DynamicTableEntity("test2", "test2"));

            var mockRepository = new Moq.Mock<IConfigRepository>();
            mockRepository.Setup(x => x.GetConfigurationsAsync("")).Returns
                (Task.FromResult(mockList));


            // Need matterCenterServiceFunctions object to return the data from the API
            var matterCenterServiceFunctions = new MatterCenterServiceFunctions();

            ConfigController controller = new ConfigController(errorSettingsMoq.Object, generalSettingsMoq.Object, uiConfigsMoq.Object, logTableMoq.Object, matterCenterServiceFunctions, mockRepository.Object, environmentMoq.Object);


            //Call API
            var resultTask = controller.Get("");
            resultTask.Wait();
      
            //Get result and convert to ObjectResult to extract the data
            var result = resultTask.Result;
            var config = (ObjectResult)result;
       
            //Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Value);
            Assert.Equal(config.Value, mockList);
        }


        [Fact]
        public void GetAllUIConfigsForSPO()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns(genS.CloudStorageConnectionString);
            generalSettingsMoq.SetupGet(p => p.Value.AdminUserName).Returns(_fixture.Configuratuion.GetSection("General").GetSection("AdminUserName").Value.ToString());
            generalSettingsMoq.SetupGet(p => p.Value.AdminPassword).Returns(_fixture.Configuratuion.GetSection("General").GetSection("AdminPassword").Value.ToString());
            generalSettingsMoq.SetupGet(p => p.Value.CentralRepositoryUrl).Returns("https://msmatter.sharepoint.com/sites/catalog");

            var environmentMoq = new Moq.Mock<IHostingEnvironment>();
            environmentMoq.SetupGet(p => p.WebRootPath).Returns(@"C:\Repos\mattercenter\tree\master\cloud\\src\solution\Microsoft.Legal.MatterCenter.Web\wwwroot");

           // var matterCenterServiceFunctionsMoq = new Moq.Mock<IMatterCenterServiceFunctions>();

            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("ConfigGroup");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");


            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();

            List<DynamicTableEntity> mockList = new List<DynamicTableEntity>();
            mockList.Add(new DynamicTableEntity("test", "test"));
            mockList.Add(new DynamicTableEntity("test2", "test2"));

            var mockRepository = new Moq.Mock<IConfigRepository>();
            mockRepository.Setup(x => x.GetConfigurationsAsync("")).Returns
                (Task.FromResult(mockList));

            generalSettingsMoq.SetupGet(g => g.Value).Returns(genS);
            errorSettingsMoq.SetupAllProperties();

            var matterCenterServiceFunctions = new MatterCenterServiceFunctions();

            ConfigController controller = new ConfigController(errorSettingsMoq.Object, generalSettingsMoq.Object, uiConfigsMoq.Object,
                logTableMoq.Object, matterCenterServiceFunctions, configRepository, environmentMoq.Object);

            

            //Call API
            var resultTask = controller.GetConfigsForSPO("");
            resultTask.Wait();

            //Get result and convert to ObjectResult to extract the data
            var result = resultTask.Result;
            var config = (ObjectResult)result;

            //Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Value);
            Assert.Equal(config.Value, mockList);
        }

        [Fact]
        public void GetSubsetUIConfigs()
        {

            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns(genS.CloudStorageConnectionString);

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

            ConfigRepository configRepository = new ConfigRepository(null, generalSettingsMoq.Object, uiConfigsMoq.Object);

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
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();

            var generalSettingsMoq = new Moq.Mock<IOptions<GeneralSettings>>();
            generalSettingsMoq.SetupGet(p => p.Value.CloudStorageConnectionString).Returns(genS.CloudStorageConnectionString);

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

            ConfigRepository configRepository = new ConfigRepository(null, generalSettingsMoq.Object, uiConfigsMoq.Object);

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
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();
            var m = new Moq.Mock<IOptions<GeneralSettings>>();
            var l = new Moq.Mock<IOptions<UIConfigSettings>>();

            m.SetupGet(g => g.Value).Returns(genS);
            m.SetupAllProperties();


            ConfigRepository configRepository = new ConfigRepository(null, m.Object, l.Object);
            var response = configRepository.GetConfigEntities("");

        }


    }
}

