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
using Microsoft.Legal.MatterCenter.Models;
using System.Globalization;
using Microsoft.SharePoint.Client;
using Microsoft.Legal.MatterCenter.Service;

namespace Microsoft.Legal.MatterCenter.UnitTest
{
    public class MatterControllerUnitTests : IClassFixture<CompositionRootFixture>
    {


        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
        private readonly CompositionRootFixture _fixture;

        private IMatterRepository matterRepository; //{ get; set; }
        private IMatterCenterServiceFunctions matterCenterServiceFunctions;


        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ILoggerFactory LoggerFactory { get; }


        public MatterControllerUnitTests(CompositionRootFixture fixture)
        {

            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _fixture = fixture;

        }

        [Fact]
        public void GetPinnnedMattersTest()
        {
            //Arrange
            GeneralSettings genS = new GeneralSettings();
            genS.CloudStorageConnectionString = _fixture.Configuratuion.GetSection("General").GetSection("CloudStorageConnectionString").Value.ToString();

            //Need to Mock the injected services and setup any properties on these that the test requires
            var errorSettingsMoq = new Moq.Mock<IOptions<ErrorSettings>>();
            errorSettingsMoq.SetupAllProperties();

            var matterSettingsMoq = new Moq.Mock<IOptions<MatterSettings>>();
            matterSettingsMoq.SetupAllProperties();

            var customLoggerMoq = new Moq.Mock<CustomLogger>();
            customLoggerMoq.SetupAllProperties();

            var validationFunctionsMoq = new Moq.Mock<ValidationFunctions>();
            validationFunctionsMoq.SetupAllProperties();

            var editFunctionsMoq = new Moq.Mock<EditFunctions>();
            editFunctionsMoq.SetupAllProperties();

            var matterProvisionMoq = new  Moq.Mock<MatterProvision>();
            matterProvisionMoq.SetupAllProperties();

            var uiConfigsMoq = new Moq.Mock<IOptions<UIConfigSettings>>();
            uiConfigsMoq.SetupGet(t => t.Value.MatterCenterConfiguration).Returns("MatterCenterConfiguration");
            uiConfigsMoq.SetupGet(p => p.Value.Partitionkey).Returns("MatterCenterConfig");
            uiConfigsMoq.SetupGet(c => c.Value.ConfigGroup).Returns("Home");
            uiConfigsMoq.SetupGet(k => k.Value.Key).Returns("Key");
            uiConfigsMoq.SetupGet(v => v.Value.Value).Returns("Value");

            var logTableMoq = new Moq.Mock<IOptions<LogTables>>();

            var clientContext = new Moq.Mock<ClientContext>();
            var searchRequestVM = new Moq.Mock<SearchRequestVM>();
            var spoAuthorization = new Moq.Mock<ISPOAuthorization>();
            searchRequestVM.SetupAllProperties();
            spoAuthorization.Setup(s => s.GetClientContext(searchRequestVM.Object.ToString()));
            //  clientContext.Setup( s => s.Url).Returns(new clien)
            clientContext.SetupAllProperties();



            clientContext.Setup(c => c.Url).Returns(clientContext.Object.Url);

              //Setup Mock Repository data
              var TestPinResponseVM = new SearchResponseVM();

            IList<MatterData> matterDataList = new List<MatterData>();
            var matterdata1 = new MatterData();
            matterdata1.MatterClientUrl = "test";
            matterDataList.Add(new MatterData());

            TestPinResponseVM.MatterDataList = matterDataList;
            //mockList.Add(new DynamicTableEntity("test2", "test2"));

            var mockRepository = new Moq.Mock<IMatterRepository>();
            mockRepository.Setup(x => x.GetPinnedRecordsAsync(searchRequestVM.Object, clientContext.Object)).Returns
                (Task.FromResult(TestPinResponseVM));


            // Need matterCenterServiceFunctions object to return the data from the API
            var matterCenterServiceFunctions = new MatterCenterServiceFunctions();

            MatterController controller = new MatterController(errorSettingsMoq.Object, matterSettingsMoq.Object, matterCenterServiceFunctions, mockRepository.Object, customLoggerMoq.Object,logTableMoq.Object, validationFunctionsMoq.Object, editFunctionsMoq.Object, matterProvisionMoq.Object, spoAuthorization.Object);

            //Setup data
            var searchRequestVMTest = new SearchRequestVM();
            searchRequestVMTest.Client = new Client();
            searchRequestVMTest.Client.Url = "Microsoft";
            searchRequestVMTest.SearchObject = new SearchObject();
            searchRequestVMTest.SearchObject.SearchTerm = "Test";


           //Call API
           var resultTask = controller.GetPin(searchRequestVMTest);
            resultTask.Wait();

            //Get result and convert to ObjectResult to extract the data
            var result = resultTask.Result;
            var config = (ObjectResult)result;

            //Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Value);
            Assert.Equal(config.Value, TestPinResponseVM);
        }

    }
}