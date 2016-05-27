using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using System;
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
                    MatterClient = "Microsoft",
                    MatterClientId = "100002",
                    MatterCreatedDate = "2016-02-18T11:19:05.000Z",
                    MatterDescription = "For Matter Center",
                    MatterGuid = "9c069bd7e681628e5107a87bfc49e648",
                    MatterID = "123456",
                    MatterModifiedDate = "2016-03-07T23:23:49Z",
                    MatterPracticeGroup = "Litigation;",
                    MatterName = "For Matter Center",
                    MatterResponsibleAttorney = "Matter Center",
                    MatterSubAreaOfLaw = "Trademark;",
                    MatterClientUrl = "https://svalli.sharepoint.com/sites/mc"
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
                    Name = "New Matter",
                    MatterGuid = "e224f0ba891492dc05bf97d73f8b2934"
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
                    AssignUserEmails = assignUserEmails,
                    Conflict = new Conflict()
                    {
                        Identified = "True"
                    },
                    BlockUserNames = blockedUserNames,
                },
                UserIds = userIds
            };
            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/checksecuritygroupexists", matterInformationVM);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Create_Matter()
        {
            #region Create Matter Data
            string matterGuid = "1C0B1194EBF746DE829B8432A130EED3";

            var userIds = new List<string>();
            userIds.Add("txtAssign1");

            var blockUserNames = new List<string>();
            blockUserNames.Add("SaiG@MSmatter.onmicrosoft.com");

            var assignUserNames = new List<IList<string>>();
            var userNames = new List<string>();
            userNames.Add("Premchand peddakotla");
            userNames.Add("");
            assignUserNames.Add(userNames);


            var assignUserEmails = new List<IList<string>>();
            var userEmails = new List<string>();
            userEmails.Add("premp@MSmatter.onmicrosoft.com");
            userEmails.Add("");
            assignUserEmails.Add(userNames);

            var roles = new List<string>();
            roles.Add("Responsible Attorney");

            var folderNames = new List<string>();
            folderNames.Add("Emails");
            folderNames.Add("Documents");


            var matterMetaDataVM = new MatterMetdataVM()
            {
                Matter = new Matter()
                {
                    Name = "Matter For Debugging Unit",
                    Id = "Debug12341",
                    Description = "Matter for debugging Unit",
                    Conflict = new Conflict()
                    {
                        Identified = "True",
                        CheckBy = "matteradmin@MSmatter.onmicrosoft.com",
                        CheckOn = "05/03/2016",
                        SecureMatter = "True"
                    },
                    BlockUserNames = blockUserNames,
                    AssignUserNames = assignUserNames,
                    AssignUserEmails = assignUserEmails,
                    Roles = roles,
                    MatterGuid = matterGuid,
                    FolderNames = folderNames
                },
                Client = new Client()
                {
                    Id = "100001",
                    Name = "Microsoft",
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                MatterConfigurations = new MatterConfigurations()
                {
                    IsConflictCheck = true,
                    IsMatterDescriptionMandatory = true,
                    IsCalendarSelected = true,
                    IsTaskSelected = true
                },
                UserIds = userIds
            };
            #endregion

            #region Assign Content Type
            var contentTypes = new List<string>();
            contentTypes.Add("Copyright");
            contentTypes.Add("Patent");


            var assignContentTypeMetadata = new MatterMetadata()
            {
                Matter = new Matter()
                {
                    Name = "Matter For Debugging Unit",
                    Id = "Debug12341",
                    ContentTypes = contentTypes,
                    DefaultContentType = "Copyright",
                    MatterGuid =  matterGuid
                },
                Client = new Client()
                {
                    Url= "https://msmatter.sharepoint.com/sites/microsoft",
                    Name="Microsoft",
                    Id = "100001"
                },
                PracticeGroupTerm = new PracticeGroupTerm()
                {
                    TermName= "Litigation",
                    Id= "084887e6-3705-466c-823b-207563388464"
                },
                AreaTerm = new AreaTerm()
                {
                    TermName= "Intellectual Property",
                    Id= "162fb199-2f04-498d-a7ac-329a077bca9f"
                },
                SubareaTerm = new SubareaTerm()
                {
                    TermName = "Copyright",
                    Id = "15c5b16c-150b-4bf5-8470-59dfa951dcf8"
                }

            };

            #endregion

            #region Assign User Permission
            var permissions = new List<string>();
            permissions.Add("Full Control");


            var assignUserPermissionMetadataVM = new MatterMetdataVM()
            {
                Client = new Client()
                {
                    Url= "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter = new Matter()
                {
                    Name= "Matter For Debugging Unit",
                    Permissions= permissions,
                    AssignUserNames= assignUserNames,
                    AssignUserEmails= assignUserEmails,
                    MatterGuid = matterGuid
                },
                MatterConfigurations = new MatterConfigurations()
                {
                    IsCalendarSelected = true,
                    IsTaskSelected = true
                }
            };
            #endregion

            #region Create Matter Landing Page
            var createMatterLandingPage = new MatterMetdataVM()
            {
                Client = new Client()
                {
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                MatterConfigurations = new MatterConfigurations()
                {
                    IsConflictCheck = true,
                    IsMatterDescriptionMandatory = true,
                    IsCalendarSelected = true,
                    IsRSSSelected = true,
                    IsTaskSelected = true
                },
                Matter = new Matter()
                {
                    Name = "Matter For Debugging Unit",                    
                    Description = "Matter for debugging Unit",
                    AssignUserNames = assignUserNames,
                    AssignUserEmails = assignUserEmails,
                    BlockUserNames = blockUserNames,
                    Conflict = new Conflict()
                    {
                        Identified = "True",
                        CheckBy = "matteradmin@MSmatter.onmicrosoft.com",
                        CheckOn = "05/03/2016",
                        SecureMatter = "True"
                    },
                    Permissions = permissions,
                    MatterGuid = matterGuid                    
                }
            };
            #endregion

            #region Update Matter Metadata
            
            var ct = new List<string>();
            ct.Add("Copyright");
            ct.Add("");

            var uploadBlockedUsers = new List<string>();
            var docTemplateCount = new List<string>();
            docTemplateCount.Add("1");

            var matterMetadata = new MatterMetdataVM()
            {
                Client = new Client()
                {
                    Id = "100001",
                    Name = "Microsoft",
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter = new Matter()
                {
                    Name = "Matter For Debugging Unit",
                    Id = "Debug12341",
                    Description = "Matter for debugging Unit",
                    Conflict = new Conflict()
                    {
                        Identified = "True",
                        CheckBy = "matteradmin@MSmatter.onmicrosoft.com",
                        CheckOn = "05/03/2016",
                        SecureMatter = "True"
                    },
                    BlockUserNames = blockUserNames,
                    AssignUserNames = assignUserNames,
                    AssignUserEmails = assignUserEmails,
                    Roles = roles,
                    MatterGuid = matterGuid,
                    ContentTypes= ct,
                    DefaultContentType= "Copyright",
                    Permissions= permissions,
                    DocumentTemplateCount= docTemplateCount
                },
                MatterConfigurations = new MatterConfigurations()
                {
                    IsConflictCheck=true,
                    IsMatterDescriptionMandatory=true
                },
                MatterDetails = new MatterDetails()
                {
                    PracticeGroup = "Litigation;",
                    AreaOfLaw= "Intellectual Property;",
                    SubareaOfLaw= "Copyright;",
                    ResponsibleAttorney= "SaiKiran Gudala;",
                    ResponsibleAttorneyEmail= "SaiG@MSmatter.onmicrosoft.com;",
                    UploadBlockedUsers= uploadBlockedUsers,
                    TeamMembers= "SaiKiran Gudala;",
                    RoleInformation= "{\"Responsible Attorney\":\"Venkat M(venkatm@MSmatter.onmicrosoft.com)\"}"
                },
                MatterProvisionFlags = new MatterProvisionFlags()
                {
                    SendEmailFlag = true,
                    MatterLandingFlag = "true"
                }
            };
            #endregion

            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/create", matterMetaDataVM);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                if(result.IsError==false)
                {
                    //Call Assign Content Type API
                    response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/assigncontenttype", assignContentTypeMetadata);
                    result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                }

                if (result.IsError == false)
                {
                    //Call Assign Content Type API
                    response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/assignuserpermissions", assignUserPermissionMetadataVM);
                    result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                }

                if (result.IsError == false)
                {
                    //Call Assign Content Type API
                    response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/createlandingpage", createMatterLandingPage);
                    result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                }

                if (result.IsError == false)
                {
                    //Call Assign Content Type API
                    response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/UpdateMetadata", matterMetadata);
                    result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                }
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Delete_Matter()
        {
            var matterVM = new MatterVM()
            {
                Client = new Client()
                {                   
                    Url = "https://msmatter.sharepoint.com/sites/microsoft"
                },
                Matter = new Matter()
                {
                    Name = "Matter For Debugging Unit",
                    MatterGuid = "1C0B1194EBF746DE829B8432A130EED3"
                }
            };

            using (var testClient = testServer.CreateClient().AcceptJson())
            {
                var response = await testClient.PostAsJsonAsync("http://localhost:58775/api/v1/matter/deletematter", matterVM);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }
    }
}
