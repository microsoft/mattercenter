using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;

using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Legal.MatterCenter.ServiceTest
{
    public class ExternalSharingUnitTest
    {
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";

        public ExternalSharingUnitTest()
        {
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        /// <summary>
        /// This unit test will try to get all the roles configured in matter center
        /// </summary>
        [Fact]
        public async void Send_ExternalSharing_Notification()
        {
            

            var assignUserEmails = new List<IList<string>>();
            var userEmails = new List<string>();
            userEmails.Add("matteradmin@MSmatter.onmicrosoft.com");
            assignUserEmails.Add(userEmails);

            userEmails = new List<string>();
            userEmails.Add("premchand250@hotmail.com"); 
            assignUserEmails.Add(userEmails);

            var assignUserNames = new List<IList<string>>();
            var userNames = new List<string>();
            userNames.Add("Wilson Gajarla");
            userNames.Add("");
            assignUserNames.Add(userNames);

            userNames = new List<string>();
            userNames.Add("premchand250@hotmail.com");
            userNames.Add("");
            assignUserNames.Add(userNames);

            var permissions = new List<string>();
            permissions.Add("Full Control");
            permissions.Add("Full Control");

            var roles = new List<string>();
            roles.Add("Responsible Attorney");
            roles.Add("Legal Admin");

            var uploadBlockedUsers = new List<string>();
            uploadBlockedUsers.Add("premp@MSmatter.onmicrosoft.com");
            var matterMetaInformation = new MatterInformationVM()
            {
                Client = new Client
                {
                    Url = "https://msmatter.sharepoint.com/sites/microsoft",
                    Id = "100001",
                    Name = "Microsoft"
                },
                Matter = new Matter
                {
                    Id= "351085190a4ce42e2871e748b4e5d8ce",
                    Name = "vTest4",
                    BlockUserNames = new List<string>()
                    {
                        "SaiG@MSmatter.onmicrosoft.com"
                    },
                    AssignUserNames = assignUserNames,
                    AssignUserEmails = assignUserEmails,
                    Permissions = permissions,
                    Roles = roles,
                    Conflict = new Conflict()
                    {
                        Identified = "True"
                    }
                },
                MatterDetails = new MatterDetails
                {
                    ResponsibleAttorney = "Wilson Gajarla;",
                    ResponsibleAttorneyEmail = "Wilson Gajarla;",
                    UploadBlockedUsers = uploadBlockedUsers,
                    TeamMembers = "Wilson Gajarla;premchand250@hotmail.com",
                    RoleInformation = "{\"Responsible Attorney\":\"Wilson Gajarla\",\"Legal Admin\":\"premchand250@hotmail.com\"}"
                },
                EditMode = true
            };
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:44323/api/v1/matter/sharematter", matterMetaInformation);
                var result = response.Content.ReadAsJsonAsync<GenericResponseVM>().Result;
                Assert.NotNull(result);
            }
        }       
    }
}
