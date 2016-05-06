using Xunit;
using Microsoft.AspNet.TestHost;
using Microsoft.Legal.MatterCenter.Models;
using System.Net.Http;

using System.Collections.Generic;

namespace Microsoft.Legal.MatterCenter.ServiceTest.IntegrationTests
{
    public class AttachmentUnitTest
    {
        private readonly TestServer testServer;
        private const string authority = "https://login.windows.net/microsoft.onmicrosoft.com";

        public AttachmentUnitTest()
        {
            testServer = new TestServer(TestServer.CreateBuilder().UseStartup<Startup>());
        }

        [Fact]
        public async void Test_Attachment()
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
                AttachmentToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkkzTDVfM3pTRVZPT3RmQmZFTGpXRmMwaFNwWSJ9.eyJpc3MiOiIwMDAwMDAwMi0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDBAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXVkIjoiMDAwMDAwMDItMDAwMC0wZmYxLWNlMDAtMDAwMDAwMDAwMDAwL291dGxvb2sub2ZmaWNlMzY1LmNvbUAzYzQ1NjJjYS0zOWE0LTRkOGItOTFmZi02ZDNlZWZhYjVjMWQiLCJuYmYiOjE0NjI0OTU1MjgsImV4cCI6MTQ2MjQ5NTgyOCwibmFtZWlkIjoiNjdlZWZlMzctNWFkNi00MTAxLTgxNTItZTc5ZGU3OWRkOTc3QDNjNDU2MmNhLTM5YTQtNGQ4Yi05MWZmLTZkM2VlZmFiNWMxZCIsInZlciI6IkV4Y2hhbmdlLkNhbGxiYWNrLlYxIiwiYXBwY3R4c2VuZGVyIjoiaHR0cHM6Ly9tYXR0ZXJ1aXYwLmF6dXJld2Vic2l0ZXMubmV0L3BhZ2VzL0hvbWUuYXNweD9hcHBUeXBlPU91dGxvb2tAM2M0NTYyY2EtMzlhNC00ZDhiLTkxZmYtNmQzZWVmYWI1YzFkIiwiYXBwY3R4Ijoie1wib2lkXCI6XCI5MzNjZTFmZC0yNjkzLTRhYWUtYjdiYS0yYTBiNjhlNDEwMjlcIixcInB1aWRcIjpcIjEwMDNCRkZEOTc3QTM4REJcIixcInNtdHBcIjpcIm1hdHRlcmFkbWluQE1TbWF0dGVyLm9ubWljcm9zb2Z0LmNvbVwiLFwidXBuXCI6XCJtYXR0ZXJhZG1pbkBNU21hdHRlci5vbm1pY3Jvc29mdC5jb21cIixcInNjb3BlXCI6XCJQYXJlbnRJdGVtSWQ6QUFNa0FERm1NemhoTm1RMkxXRm1ZVFF0TkdFM1lTMWhPRGRpTFdRMk5HTXlOVGcwT0dZNVpnQkdBQUFBQUFBZlRVcEdlVzlZUTR1YzUzcVJjV3VvQndDdGc3ZFJGcGFZVHI3bm91RXBXK2M2QUFBQUFBRU1BQUN0ZzdkUkZwYVlUcjdub3VFcFcrYzZBQUFFK2VNOEFBQT1cIn0ifQ.NJdGoSOkjVrdRnCgMYIMgsU78puNglRJEvkTu94trk3s2HMaH5crCMXIATBrxHTg4tBQM-Dd_a7Xo5xPS8TyhL3fDO-mZDnZyUj9XtUp_Dlva6hjuqCcfWzOM0RntUiPAl8KyHmk5LtbiPGXamaAEOH27vCJYwXL973tTPmuXFDIGG-gkocS8wK8NMLIdPqhlEXT9JXpSg55kY1FR6_HgvRQCCw6LRtoEjIHr3nHr8QDVIF6q7Aql4VRrIvZgJqX1A5xZBi5r2BrWRHFF5QWzi_OTS2QwPGKcvUjNP22YJ4WXE5_YWr3xEeURvIDclEePXgfbiEbp9brken6fQVvLw",
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
    }
}
