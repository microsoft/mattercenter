using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.Legal.MatterCenter.Service;
using Newtonsoft.Json;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class MatterUnitTest
    {
        private readonly TestServer testServer;
        public MatterUnitTest()
        {
            testServer = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());
        }

        /// <summary>
        /// This unit test will get all the user pinned matters. This is a positive test case
        /// </summary>
        [Fact]
        public async void Get_User_Pinned_Matter()
        {
            var client = new Client()
            {
                Id = "123456",
                Name = "Microsoft",
                Url = "https://microsoft.sharepoint.com/teams/mcuisite"
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/getpinned", client);
                var result = response.Content.ReadAsJsonAsync<PinResponseVM>().Result;
                Assert.NotNull(result);
                Assert.NotNull(result.UserPinnedMattersList);
                Assert.NotEmpty(result.UserPinnedMattersList);
            }
        }


         /// <summary>
        /// This unit test is for negative test case and this test case wont get any results back
        /// This test case will get 404
        /// </summary>
        [Fact]
        public async void No_User_Pinned_Matter_Found()
        {
            var client = new Client()
            {                
                Url = "https://microsoft.sharepoint.com/teams/mcuisite"
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/getpinnedmatters", client);
                var result = response.Content.ReadAsJsonAsync<ErrorResponse>().Result;
                Assert.NotNull(result);
                Assert.Equal("404", result.ErrorCode);                
            }
        }

        /// <summary>
        /// This unit test is for unpinng the user matter
        /// </summary>
        [Fact]
        public async void UnPin_User_Matter()
        {
            var pinRequestVM = new PinRequestMatterVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                MatterData = new MatterData()
                {
                    MatterName= "https://svalli.sharepoint.com/sites/mc/e0421c5e7fbf704023871b2acf64370m/Forms/AllItems.aspx"                    
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/unpinmatter", pinRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);               
            }
        }

        /// <summary>
        /// This unit test is for creating a new pinned matter
        /// </summary>
        [Fact]
        public async void Pin_User_Matter()
        {
            var pinRequestVM = new PinRequestMatterVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                MatterData = new MatterData()
                {
                    MatterUrl = "https://svalli.sharepoint.com/sites/mc/e0421c5e7fbf704023871b2acf64370m/Forms/AllItems.aspx",
                    HideUpload = "false",
                    MatterAreaOfLaw = "Intellectual Property;",
                    MatterClient= "Microsoft",
                    MatterClientId= "100002",
                    MatterCreatedDate= "2016-02-18T11:19:05.000Z",
                    MatterDescription= "For Matter Center",
                    MatterGuid= "9c069bd7e681628e5107a87bfc49e648",
                    MatterID= "123456",
                    MatterModifiedDate= "2016-03-07T23:23:49Z",
                    MatterPracticeGroup= "Litigation;",
                    MatterName = "For Matter Center",
                    MatterResponsibleAttorney= "Matter Center",
                    MatterSubAreaOfLaw = "Trademark;",
                    MatterClientUrl= "https://svalli.sharepoint.com/sites/mc"
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/pinmatter", pinRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);               
            }
        }
    }
}
