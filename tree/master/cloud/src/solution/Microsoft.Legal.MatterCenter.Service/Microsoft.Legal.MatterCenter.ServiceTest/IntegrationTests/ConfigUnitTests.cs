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




namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class ConfigUnitTest
    {


      

        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
        //public ILoggerFactory LoggerFactory { get; }
        private IConfigRepository configRepository; //{ get; set; }
        //private GeneralSettings generalSettings { get; set; }
        //private LogTables logTables { get; set; }
        //private IOptionsMonitor<GeneralSettings> gensettings { get; set; }
        //private IOptionsMonitor<LogTables> logTabs { get; set; }


        public ConfigUnitTest()
        {
        
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
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

            ConfigEntities configs = new ConfigEntities()
            {
                ConfigEntries = new List<ConfigEntity>()

            };

            ConfigRepository configRepository = new ConfigRepository(m.Object, l.Object);
            var response = configRepository.GetConfigEntities();


        }


    }
}
