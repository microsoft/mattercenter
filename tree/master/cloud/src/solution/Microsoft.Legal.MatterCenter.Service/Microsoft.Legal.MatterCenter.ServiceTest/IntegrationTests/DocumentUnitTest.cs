using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.Legal.MatterCenter.Service;
using Newtonsoft.Json;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class DocumentUnitTest
    {
        private readonly TestServer testServer;
        public DocumentUnitTest()
        {
            testServer = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());
        }
        
        
        /// <summary>
        /// This unit test will get all the user pinned documents. This is a positive test case
        /// </summary>
        [Fact]
        public async void Get_User_Pinned_Document()
        {
            var client = new Client()
            {
                Id = "123456",
                Name = "Microsoft",
                Url = "https://microsoft.sharepoint.com/teams/mcuisite"
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/document/getpinneddocuments", client);
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
        public async void NoPin_User_Document()
        {
            var client = new Client()
            {
                Url = "https://microsoft.sharepoint.com/teams/mcuisite"
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/document/unpinmatterdocument", client);
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
                    MatterName = "https://svalli.sharepoint.com/sites/mc/e0421c5e7fbf704023871b2acf64370m/Forms/AllItems.aspx"
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
        public async void Pin_User_Document()
        {
            var pinRequestVM = new PinRequestDocumentVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                DocumentData = new DocumentData()
                {
                    DocumentName= "Document", 
                    DocumentVersion= "2.0", 
                    DocumentClient= "Microsoft", 
                    DocumentClientId= "100002", 
                    DocumentClientUrl= "https://svalli.sharepoint.com/sites/mc", 
                    DocumentMatter= "For Matter Center", 
                    DocumentMatterId= "123456", 
                    DocumentOwner= "Lakshmanaswamy Premchand Peddakotla", 
                    DocumentUrl= "https://svalli.sharepoint.com/sites/mc/9c069bd7e681628e5107a87bfc49e648/emails/document2.docx", 
                    DocumentOWAUrl= "https://svalli.sharepoint.com/sites/mc/_layouts/WopiFrame.aspx?sourcedoc=https%3A%2F%2Fsvalli.sharepoint.com%2Fsites%2Fmc%2F9c069bd7e681628e5107a87bfc49e648%2FEmails%2FDocument.docx&amp;action=default&amp;DefaultItemOpen=1", 
                    DocumentExtension= "docx", 
                    DocumentCreatedDate= "2016-02-19T21:37:36Z", 
                    DocumentModifiedDate= "2/19/2016 9:37:53 PM", 
                    DocumentCheckoutUser= "NA", 
                    DocumentMatterUrl= "https://svalli.sharepoint.com/sites/mc/9c069bd7e681628e5107a87bfc49e648", 
                    DocumentParentUrl= "https://svalli.sharepoint.com/sites/mc/9c069bd7e681628e5107a87bfc49e648/Emails", 
                    DocumentID= "MICROSOFT-1625733529-9"
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/document/pindocument", pinRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }

        /// <summary>
        /// This unit test is for unpinng the user matter
        /// </summary>
        [Fact]
        public async void UnPin_User_Document()
        {
            var pinRequestVM = new PinRequestDocumentVM()
            {
                Client = new Client()
                {
                    Id = "123456",
                    Name = "Microsoft",
                    Url = "https://microsoft.sharepoint.com/teams/mcuisite"
                },
                DocumentData = new DocumentData()
                {
                    DocumentUrl = "https://svalli.sharepoint.com/sites/mc/e0421c5e7fbf704023871b2acf64370m/Forms/AllItems.aspx"
                }
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/document/unpindocument", pinRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Get_Document_Without_SearchTerm()
        {
            var searchRequest = new SearchRequestVM()
            {
                Client = new Client()
                {                    
                    Url = "https://msmatter.sharepoint.com/sites/catalog"
                },
                SearchObject = new SearchObject()
                {
                    PageNumber = 1,
                    ItemsPerPage = 10,
                    SearchTerm = "",
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
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/document/getdocuments", searchRequest);
                var result = response.Content.ReadAsJsonAsync<SearchResponseVM>().Result;
                Assert.NotNull(result);
                Assert.NotEmpty(result.DocumentDataList);

            }
        }
    }
}
