using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class ExternalSharingUnitTest
    {
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";

        public ExternalSharingUnitTest()
        {
            testServer = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());
        }

        /// <summary>
        /// This unit test will try to get all the roles configured in matter center
        /// </summary>
        [Fact]
        public async void Send_ExternalSharing_Notification()
        {
            var externalSharingRequest = new ExternalSharingRequest()
            {
                Client = new Client()
                {
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                ClientName = "microsoft",
                MatterId = "351085190a4ce42e2871e748b4e5d8ce",
                Permission = "Full Control",
                Person = "premchand113@hotmail.com",
                Role = "Attorney Journal",
                Status = "Pending"
                //ExternalUserInfoList = new List<ExternalUserInfo>()
                //{
                //    new ExternalUserInfo()
                //    {
                //        Permission = "Full Control",
                //        Person = "premchand104@hotmail.com",
                //        Role = "Attorney Journal",
                //        Status = "Pending"
                //    }
                //    //,
                //    //new ExternalUserInfo()
                //    //{
                //    //    Permission = "Contribute",
                //    //    Person = "premchand_100@hotmail.com",
                //    //    Role = "Attorney Journal",
                //    //    Status = "Pending"
                //    //},
                //    //new ExternalUserInfo()
                //    //{
                //    //    Permission = "Read",
                //    //    Person = "premchand_101@hotmail.com",
                //    //    Role = "Attorney Journal",
                //    //    Status = "Pending"
                //    //},
                //}
            };
            
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:44323/api/v1/matter/sharematter", externalSharingRequest);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.Null(result);
            }
        }


        
    }
}
