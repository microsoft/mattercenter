using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.Legal.MatterCenter.Service;
using Newtonsoft.Json;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class TaxonomyUniTest
    {
        private readonly TestServer testServer;
        public TaxonomyUniTest()
        {
            testServer = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());
        }
        [Fact]
        public async void Get_Current_Site_Title()
        {
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.GetAsync("http://localhost:58775/api/v1/taxonomy/getcurrentsitetitle");
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }

        /// <summary>
        /// This integration test case is for getting the clients with correct inputs passed
        /// </summary>
        [Fact]
        public async void Get_Taxonomy_Hierarchy_For_Clients()
        {

            var termStoreViewModel = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-mcuisite",
                    TermSetName = "Clients",
                    CustomPropertyName = "ClientURL"
                }
            };

            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:58775/api/v1/taxonomy/gettaxonomy", termStoreViewModel);
                var result = response.Content.ReadAsJsonAsync<ClientTermSets>().Result;
                Assert.NotNull(result);
                Assert.NotEmpty(result.ClientTerms);             
            }
        }

        /// <summary>
        /// This integration test case is for getting the parctice groups with correct inputs passed
        /// </summary>
        [Fact]
        public async void Get_Taxonomy_Hierarchy_For_PracticeGroups()
        {

            var termStoreViewModel = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-mcuisite",
                    TermSetName = "Practice Groups",
                    CustomPropertyName = "FolderNames"
                }
            };

            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:58775/api/v1/taxonomy/gettaxonomy", termStoreViewModel);
                var result = response.Content.ReadAsJsonAsync<TermSets>().Result;
                Assert.NotNull(result);
                Assert.NotEmpty(result.PGTerms);
            }
        }


        /// <summary>
        /// This integration test case is for not getting the clients with incorrect inputs passed
        /// </summary>
        [Fact]
        public async void Get_Taxonomy_Hierarchy_For_Clients_NoResults()
        {

            var termStoreViewModel = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-mcuisite",
                    TermSetName = "NoClients",
                    CustomPropertyName = "ClientURL"
                }
            };

            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:58775/api/v1/taxonomy/gettaxonomy", termStoreViewModel);
                var result = response.Content.ReadAsJsonAsync<ErrorResponse>().Result;
                Assert.NotNull(result);
                Assert.Equal("404", result.ErrorCode);
            }
        }


        /// <summary>
        /// This integration test case is for not getting the parctice groups with in correct inputs passed
        /// </summary>
        [Fact]
        public async void Get_Taxonomy_Hierarchy_For_PracticeGroups_NoResults()
        {

            var termStoreViewModel = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-nosite",
                    TermSetName = "No Practice Groups",
                    CustomPropertyName = "FolderNames"
                }
            };

            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:58775/api/v1/taxonomy/gettaxonomy", termStoreViewModel);
                var result = response.Content.ReadAsJsonAsync<ErrorResponse>().Result;
                Assert.NotNull(result);
                Assert.Equal("404", result.ErrorCode);
            }
        }

        /// <summary>
        /// This integration test case is for not getting the clients with incorrect inputs passed
        /// </summary>
        [Fact]
        public async void Internal_Server_Error()
        {

            var termStoreViewModel = new TermStoreViewModel()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite1234"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "Site Collection - microsoft.sharepoint.com-teams-mcuisite",
                    TermSetName = "NoClients",
                    CustomPropertyName = "ClientURL"
                }
            };

            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:58775/api/v1/taxonomy/gettaxonomy", termStoreViewModel);
                var result = response.Content.ReadAsJsonAsync<ErrorResponse>().Result;       
                Assert.NotNull(result);
                Assert.Equal("500", result.ErrorCode);
            }
        }
    }
}
