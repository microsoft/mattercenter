using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.Legal.MatterCenter.Service;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                Url = "https://msmatter.sharepoint.com/sites/catalog"
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/getpinned", client);
                var result = response.Content.ReadAsJsonAsync<SearchResponseVM>().Result;
                Assert.NotNull(result);
                Assert.NotNull(result.MatterDataList);
                Assert.NotEmpty(result.MatterDataList);
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

        [Fact]
        public async void Get_Matter_Without_SearchTerm()
        {
            var searchRequest = new SearchRequestVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject = new SearchObject()
                {
                    PageNumber = 1,
                    ItemsPerPage=10,
                    SearchTerm="",
                    Filters = new FilterObject() { },
                    Sort = new SortObject()
                    {
                        ByProperty = "LastModifiedTime",
                        Direction = 1
                    }
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/get", searchRequest);
                var result = response.Content.ReadAsJsonAsync<SearchResponseVM>().Result;
                Assert.NotNull(result);
                Assert.NotEmpty(result.SearchResults);               

            }
        }

        [Fact]
        public async void Get_Matter_With_SearchTerm()
        {
            var searchRequest = new SearchRequestVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject = new SearchObject()
                {
                    PageNumber = 1,
                    ItemsPerPage = 10,
                    SearchTerm = "M",
                    Filters = new FilterObject() { },
                    Sort = new SortObject()
                    {
                        ByProperty = "LastModifiedTime",
                        Direction = 1
                    }
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/get", searchRequest);
                var result = response.Content.ReadAsJsonAsync<SearchResponseVM>().Result;
                Assert.NotNull(result);
                Assert.NotEmpty(result.SearchResults);

            }
        }


        [Fact]
        public async void Check_Matter_Exists()
        {
            var matterMetadataVM = new MatterMetdataVM()
            {
                Client = new Client()
                {                   
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter = new Matter()
                {
                    Name="New Matter",
                    MatterGuid= "e224f0ba891492dc05bf97d73f8b2934"
                },
                HasErrorOccurred = false
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/checkmatterexists", matterMetadataVM);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Get_Matter_Configurations()
        {
            var siteCollectionPath = "https://msmatter.sharepoint.com/sites/microsoft";
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/GetConfigurations", siteCollectionPath);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Check_Security_Group_Exists()
        {
            var blockedUserNames = new List<string>();
            blockedUserNames.Add("matteradmin@MSmatter.onmicrosoft.com");
            IList<IList<string>> assignUserNames = new List<IList<string>>(); 

            var userNames = new List<string>();
            userNames.Add("Venkat M");
            userNames.Add("");
            assignUserNames.Add(userNames);



            IList<IList<string>> assignUserEmails = new List<IList<string>>();
            var emails = new List<string>();
            emails.Add("venkatm@MSmatter.onmicrosoft.com");
            emails.Add("");
            assignUserEmails.Add(emails);

            var userIds = new List<string>();
            userIds.Add("txtAssign1");

            var matterInformationVM = new MatterInformationVM()
            {
                Client = new Client()
                {
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter = new Matter()
                {
                    Name = "New Matter",
                    AssignUserNames = assignUserNames,
                    AssignUserEmails= assignUserEmails,
                    Conflict = new Conflict()
                    {
                        Identified = "True"
                    },
                    BlockUserNames = blockedUserNames,
                },
                UserIds= userIds
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/checksecuritygroupexists", matterInformationVM);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }
    }
}
