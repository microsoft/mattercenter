using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class TaxonomyUniTest
    {
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";
        private AuthenticationContext authContext;
        
        public TaxonomyUniTest()
        {
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            ;
        }
        [Fact]
        public async void Get_Current_Site_Title()
        {
            
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.GetAsync("http://localhost:44324/api/v1/taxonomy/getcurrentsitetitle");
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }

        private async Task<string> GetAccessToken()
        {
            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential("844ffb77-5bfd-403e-9285-678e2eddc90c", "IAKt/4uoQFM0UJ1Ocj//WHOg1RzLspACzPAKkkPP0kw=");            
            //UserCredential userCredential = new UserCredential("v-lapedd@microsoft.com", "feb@2016"); //Before RTM
            UserCredential userCredential = new UserCredential("v-lapedd@microsoft.com");
            AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://microsoft.onmicrosoft.com/mcserviceadal",
                clientCred);
            return authResult.AccessToken;
            //AuthenticationContext authContext = new AuthenticationContext(clientCredentials.Authority);
            //ClientCredential clientCred = new ClientCredential(clientCredentials.ClientId, clientCredentials.ClientSecret);
            //AuthenticationResult authResult = await authContext.AcquireTokenAsync(clientCredentials.ClientResource, clientCred);
            //return authResult.AccessToken;
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
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "MatterCenterTerms",
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
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                TermStoreDetails = new TermStoreDetails()
                {
                    TermGroup = "MatterCenterTerms",
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
