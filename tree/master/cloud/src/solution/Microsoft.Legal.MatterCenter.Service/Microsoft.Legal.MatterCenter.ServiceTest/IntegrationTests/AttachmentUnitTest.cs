using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.ServiceTest.IntegrationTests
{
    public class AttachmentUnitTest
    {
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";

        public AttachmentUnitTest()
        {
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>()); //.UseStartup<Startup>());
        }

        [Fact]
        public async void Test_Doc_Attachment()
        {
            var attachments = new List<AttachmentDetails>();
            AttachmentDetails attachmentDetails = new AttachmentDetails() {
                attachmentType = "0",
                name = "program to modify MARCH2016.docx",
                originalName = "program to modify MARCH2016.docx",
                isInline = false,
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                id = "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAE+eM8AAABEgAQAGDuo5aZZdBAnm7TVKY1EoE=",
                size = 335497
            };

            
            attachments.Add(attachmentDetails);

            var foldePath = new List<string>();
            foldePath.Add("/sites/microsoft/1C0B1194EBF746DE829B8432A130EED3/Documents");

            var serviceRequestVM = new ServiceRequest()
            {
                AllowContentCheck = true,
                AttachmentToken = "eyJpc3MiOiIwMDAwMDAwMi0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDBAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXVkIjoiMDAwMDAwMDItMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwL291dGxvb2sub2ZmaWNlMzY1LmNvbUAzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJuYmYiOjE0NjM0MjMwODcsImV4cCI6MTQ2MzQyMzM4NywibmFtZWlkIjoiMTA3OGNjMjQtZTg5Yy00OGY4LWFmYTEtY2IzMzIxNjM2OTY2QDNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInZlciI6IkV4Y2hhbmdlLkNhbGxiYWNrLlYxIiwiYXBwY3R4c2VuZGVyIjoiaHR0cHM6Ly9tYXR0ZXJ3ZWJhcHAuYXp1cmV3ZWJzaXRlcy5uZXRAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXBwY3R4Ijoie1wib2lkXCI6XCI5MzNjZTFmZC0yNjkzLTRhYWUtYjdiYS0yYTBiNjhlNDEwMjlcIixcInB1aWRcIjpcIjEwMDNCRkZEOTc3QTM4REJcIixcInNtdHBcIjpcIm1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbVwiLFwidXBuXCI6XCJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb21cIixcInNjb3BlXCI6XCJQYXJlbnRJdGVtSWQ6QUFNa0FERm1NemhoTm1RMkxXRm1ZVFF0TkdFM1lTMWhPRGRpTFdRMk5HTXlOVGcwT0dZNVpnQkdBQUFBQUFBZlRVcEdlVzlZUTR1YzUzcVJjV3VvQndDdGc3ZFJGcGFZVHI3bm91RXBXK2M2QUFBQUFBRU1BQUN0ZzdkUkZwYVlUcjdub3VFcFcrYzZBQUFKZ1JvS0FBQT1cIn0ifQ.b1emwSawJwObcPJ5i8t4Y2GpYgmYyjxMtX-CkTmO72S6nhroYI1fqe_VlCjzkkGBvmRQ6BZslYaukem3mh_GPQAJAhOCcmFx5lVxJ3Ttivol-PHLMoQWUf9DloDo0_nIENLm9LFwqvYK2Yhp5zNTF9TMu7PDcuw2dKbTZqTQUm99y-ajqNx1tj1Zu23iXJj--DEjXdzSDtPzoAOhWyvq9c4WuvSbE07bXtXpIe0hf_A3MO7L4W2ERuJuiDmA_E1YanxzD9iSrN1vlSbLHdRI_hoqON0i3vUUjDYyeW5qNvpOLJfy48Uz5p1Tx_arIL5HXkHv6mI31jXwY8B3okzGJg",
                Attachments = attachments,
                EwsUrl = new System.Uri("https://outlook.office365.com/EWS/Exchange.asmx"),
                DocumentLibraryName = "Matter For Debugging Unit",
                FolderPath= foldePath,
                MailId= "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAE+eM8AAA=",
                PerformContentCheck=false,
                Overwrite=false,
                Subject= "Test email"
            };

            var matterClient = new Client()
            {
                Url = "https://msmatter.sharepoint.com/sites/microsoft"
            };


            var attachmentRequestVM = new AttachmentRequestVM()
            {
                ServiceRequest = serviceRequestVM,
                Client = matterClient
            };
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:44323/api/v1/document/uploadattachments", attachmentRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void Test_Mail_Attachment()
        {
            #region New Code Data
            //var attachments = new List<AttachmentDetails>();
            //AttachmentDetails attachmentDetails = new AttachmentDetails()
            //{
            //    attachmentType = "0",
            //    name= "program to modify MARCH2016.docx",
            //    originalName= "program to modify MARCH2016.docx",
            //    isInline= false,
            //    contentType= "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            //    id= "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAJgRoKAAABEgAQAJ0HuxfoKzZCt0IGCqWvGxE=",
            //    size= 335497
            //};


            //attachments.Add(attachmentDetails);

            //var foldePath = new List<string>();
            //foldePath.Add("/sites/microsoft/12e53e87cbc16d97763d4e87f1fbb8f9/Emails");

            //var serviceRequestVM = new ServiceRequest()
            //{
            //    AllowContentCheck = true,
            //    AttachmentToken = "",
            //    Attachments = attachments,
            //    EwsUrl = new System.Uri("https://outlook.office365.com/EWS/Exchange.asmx"),
            //    DocumentLibraryName = "testerV123",
            //    FolderPath = foldePath,
            //    MailId = "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAJgRoKAAA=",
            //    PerformContentCheck = false,
            //    Overwrite = false,
            //    Subject = "Test attachments.eml"
            //};

            //var matterClient = new Client()
            //{
            //    Url = "https://msmatter.sharepoint.com/sites/microsoft"
            //};


            //var attachmentRequestVM = new AttachmentRequestVM()
            //{
            //    ServiceRequest = serviceRequestVM,
            //    Client = matterClient
            //};

            #endregion

            #region Old Code Data
            var attachments = new List<AttachmentDetails>();
            AttachmentDetails attachmentDetails = new AttachmentDetails()
            {
                attachmentType = "0",
                name = "program to modify MARCH2016.docx",                
                isInline = false,
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                id = "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAJgRoKAAABEgAQAJ0HuxfoKzZCt0IGCqWvGxE=",
                size = 335497
            };


            attachments.Add(attachmentDetails);

            var foldePath = new List<string>();
            foldePath.Add("/sites/microsoft/12e53e87cbc16d97763d4e87f1fbb8f9/Emails");

            var serviceRequestVM = new ServiceRequest()
            {
                AllowContentCheck = true,
                AttachmentToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkkzTDVfM3pTRVZPT3RmQmZFTGpXRmMwaFNwWSJ9.eyJpc3MiOiIwMDAwMDAwMi0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDBAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXVkIjoiMDAwMDAwMDItMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwL291dGxvb2sub2ZmaWNlMzY1LmNvbUAzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJuYmYiOjE0NjM0NDEwMjIsImV4cCI6MTQ2MzQ0MTMyMiwibmFtZWlkIjoiMTA3OGNjMjQtZTg5Yy00OGY4LWFmYTEtY2IzMzIxNjM2OTY2QDNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInZlciI6IkV4Y2hhbmdlLkNhbGxiYWNrLlYxIiwiYXBwY3R4c2VuZGVyIjoiaHR0cHM6Ly9tYXR0ZXJ3ZWJhcHAuYXp1cmV3ZWJzaXRlcy5uZXRAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXBwY3R4Ijoie1wib2lkXCI6XCI5MzNjZTFmZC0yNjkzLTRhYWUtYjdiYS0yYTBiNjhlNDEwMjlcIixcInB1aWRcIjpcIjEwMDNCRkZEOTc3QTM4REJcIixcInNtdHBcIjpcIm1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbVwiLFwidXBuXCI6XCJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb21cIixcInNjb3BlXCI6XCJQYXJlbnRJdGVtSWQ6QUFNa0FERm1NemhoTm1RMkxXRm1ZVFF0TkdFM1lTMWhPRGRpTFdRMk5HTXlOVGcwT0dZNVpnQkdBQUFBQUFBZlRVcEdlVzlZUTR1YzUzcVJjV3VvQndDdGc3ZFJGcGFZVHI3bm91RXBXK2M2QUFBQUFBRU1BQUN0ZzdkUkZwYVlUcjdub3VFcFcrYzZBQUFKZ1JvS0FBQT1cIn0ifQ.JHVTqrNjDmwaJ2mZyiZ17GkAqILQywSH6Ac2wfH8DcEvIpwAZ_SztjlGlRQ9dysIxEqQxeO5Enb2Lin_jl1490EDGpZ4pN_go9ank5NcIZzxJP8WegjUzhtIcEilGIJuAwXTPXPoA0MUzhQN9JF9fUo5E63GncDzOKKrlvDT1dZmtrBqikvM1llmeOfNWLkGA_2qxjLzWam8gv9PZDTCAF_0TPoy8RaEJ5mwwHFos0KlA5R6__GW5fayS_tt8Y4Wd62Oo1JF-fePErrYa8g_n9fB3J_NsR1pDPtPVXvVYtK4A42iPvkvYwIZjcCDx5f-OPn_QXkY_wTaTpIUOlTPug",
                Attachments = attachments,
                EwsUrl = new System.Uri("https://outlook.office365.com/EWS/Exchange.asmx"),
                DocumentLibraryName = "testerV123",
                FolderPath = foldePath,
                MailId = "AAMkADFmMzhhNmQ2LWFmYTQtNGE3YS1hODdiLWQ2NGMyNTg0OGY5ZgBGAAAAAAAfTUpGeW9YQ4uc53qRcWuoBwCtg7dRFpaYTr7nouEpW+c6AAAAAAEMAACtg7dRFpaYTr7nouEpW+c6AAAJgRoKAAA=",
                PerformContentCheck = false,
                Overwrite = false,
                Subject = "Test attachments.eml"
            };

            var matterClient = new Client()
            {
                Url = "https://msmatter.sharepoint.com/sites/microsoft"
            };


            var attachmentRequestVM = new AttachmentRequestVM()
            {
                ServiceRequest = serviceRequestVM,
                Client = matterClient
            };

            #endregion
            using (var client = testServer.CreateClient().AcceptJson())
            {
                var response = await client.PostAsJsonAsync("http://localhost:44323/api/v1/document/uploadmail", attachmentRequestVM);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.NotNull(result);
            }
        }
    }
}
