// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="DocumentLanding.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of document landing page </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using OpenQA.Selenium;
    using System;
    using System.Configuration;
    using System.Threading;
    using System.Web;
    using TechTalk.SpecFlow;
    using VisualStudio.TestTools.UnitTesting;

    [Binding]
    public class DocumentLanding
    {
        static string link = HttpUtility.UrlDecode(ConfigurationManager.AppSettings["DocumentLandingPage"]);
        Uri URL = new Uri(link);
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();
        bool pinned = false;

        #region 01. Open the browser and load document landing page
        [When(@"user enters credentials on document landing page")]
        public void WhenUserEntersCredentialsOnDocumentLandingPage()
        {
            webDriver.Navigate().GoToUrl(URL);
        }

        [Then(@"document landing page should be loaded with element '(.*)'")]
        public void ThenDocumentLandingPageShouldBeLoadedWithElement(string checkId)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, checkId, Selector.Id));
        }
        #endregion

        #region 02. Verify action links
        string initialState,checkIn, checkOut;
        [When(@"user loads document landing page")]
        public void WhenUserLoadsDocumentLandingPage()
        {
            Thread.Sleep(5000);
            initialState = (string)scriptExecutor.ExecuteScript("var links = $('#pinUnpinDocument span').eq(0).attr('class');return links");
            checkIn = (string)scriptExecutor.ExecuteScript("var links = $('#checkInDocument').attr('class');return links");
            checkOut = (string)scriptExecutor.ExecuteScript("var links = $('#checkOutDocument').attr('class');return links");
            Thread.Sleep(5000);
        }

        [Then(@"document action links should be present")]
        public void ThenDocumentActionLinksShouldBePresent()
        {
            string checkDownload = (string)scriptExecutor.ExecuteScript("var links = $('#spanDownload')[0].innerText;return links"),
                   checkShare = (string)scriptExecutor.ExecuteScript("var links = $('#spanShare')[0].innerText;return links"),
                   checkSend = (string)scriptExecutor.ExecuteScript("var links = $('#spanSendLink')[0].innerText;return links"),
                   finalState = (string)scriptExecutor.ExecuteScript("var links = $('#pinUnpinDocument span').eq(2).attr('class');return links");
            Assert.IsTrue(checkDownload.Contains("download"));
            Assert.IsTrue(checkShare.Contains("share"));
            Assert.IsTrue(checkSend.Contains("send link"));

            if ((string.IsNullOrWhiteSpace(checkIn) && checkOut.Contains("hide")) || (checkIn.Contains("hide") && string.IsNullOrWhiteSpace(checkOut)))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
            if ((string.IsNullOrWhiteSpace(initialState) && finalState.Contains("hide")) || (initialState.Contains("hide") && string.IsNullOrWhiteSpace(finalState)))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
        #endregion

        #region 03. Verify file properties
        [When(@"user expands file properties section")]
        public void WhenUserExpandsFilePropertiesSection()
        {
            scriptExecutor.ExecuteScript("$('#fileProperties img')[0].click()");
            Thread.Sleep(1000);
            scriptExecutor.ExecuteScript("$('#fileProperties img')[0].click()");
            Thread.Sleep(1000);

        }

        [Then(@"all file properties should be present")]
        public void ThenAllFilePropertiesShouldBePresent()
        {
            string checkFileName = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[2].innerText;return links"),
                   checkClient = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[4].innerText;return links"),
                   checkMatter = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[6].innerText;return links"),
                   checkMatterId = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[8].innerText;return links"),
                   checkDocumnentId = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[10].innerText;return links"),
                   checkCheckedOutTo = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[16].innerText;return links"),
                   checkAuthor = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[22].innerText;return links"),
                   checkDateCreated = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[24].innerText;return links"),
                   checkPracticeGroup = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[26].innerText;return links"),
                   checkAreaOfLaw = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[28].innerText;return links"),
                   checkSubAreaofLaw = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[30].innerText;return links"),
                   checkFileType = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[32].innerText;return links"),
                   checkFileSize = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[34].innerText;return links"),
                   checkFileAccess = (string)scriptExecutor.ExecuteScript("var links = $('#accessValue')[0].href;return links"),
                   checkViewMore = (string)scriptExecutor.ExecuteScript("var links = $('#viewMoreFileProperties')[0].innerText;return links"),
                   checkFileNameHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[1].innerText;return links"),
                   checkClientHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[3].innerText;return links"),
                   checkMatterHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[5].innerText;return links"),
                   checkClientMatterIdHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[7].innerText;return links"),
                   checkDocumentIdHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[9].innerText;return links"),
                   checkCheckedOutToHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[11].innerText;return links"),
                   checkAuthorHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[17].innerText;return links"),
                   checkDateCreatedHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[23].innerText;return links"),
                   checkPracticeGroupHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[25].innerText;return links"),
                   checkAreaOfLawHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[27].innerText;return links"),
                   checkSubAreaOfLawHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[29].innerText;return links"),
                   checkFileTypeHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[31].innerText;return links"),
                   checkFileSizeHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[33].innerText;return links"),
                   checkFileAccessHeader = (string)scriptExecutor.ExecuteScript("var links = $('#metadataProperties li span')[35].innerText;return links");


            if ((!string.IsNullOrWhiteSpace(checkFileName) && !string.IsNullOrWhiteSpace(checkClient) && !string.IsNullOrWhiteSpace(checkMatter) && !string.IsNullOrWhiteSpace(checkMatterId) && !string.IsNullOrWhiteSpace(checkDocumnentId) && !string.IsNullOrWhiteSpace(checkCheckedOutTo) && !string.IsNullOrWhiteSpace(checkAuthor) && !string.IsNullOrWhiteSpace(checkDateCreated) && !string.IsNullOrWhiteSpace(checkPracticeGroup) && !string.IsNullOrWhiteSpace(checkAreaOfLaw) && !string.IsNullOrWhiteSpace(checkSubAreaofLaw) && !string.IsNullOrWhiteSpace(checkFileType) && !string.IsNullOrWhiteSpace(checkFileSize) && !string.IsNullOrWhiteSpace(checkFileAccess)))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(checkViewMore.Contains("View More"));
            Assert.IsTrue(checkClientHeader.Contains("Client:"));
            Assert.IsTrue(checkFileNameHeader.Contains("File Name:"));
            Assert.IsTrue(checkMatterHeader.Contains("Matter:"));
            Assert.IsTrue(checkClientMatterIdHeader.Contains("Client.MatterID:"));
            Assert.IsTrue(checkDocumentIdHeader.Contains("Document ID:"));
            Assert.IsTrue(checkCheckedOutToHeader.Contains("Checked out to:"));
            Assert.IsTrue(checkAuthorHeader.Contains("Author:"));
            Assert.IsTrue(checkDateCreatedHeader.Contains("Date created:"));
            Assert.IsTrue(checkPracticeGroupHeader.Contains("Practice group:"));
            Assert.IsTrue(checkAreaOfLawHeader.Contains("Area of law:"));
            Assert.IsTrue(checkSubAreaOfLawHeader.Contains("Sub area of law:"));
            Assert.IsTrue(checkFileTypeHeader.Contains("File type:"));
            Assert.IsTrue(checkFileSizeHeader.Contains("File size:"));
            Assert.IsTrue(checkFileAccessHeader.Contains("File access:"));

        }
        #endregion

        #region 04. Verify version details

        [When(@"user expands version section")]
        public void WhenUserExpandsVersionSection()
        {
            scriptExecutor.ExecuteScript("$('.viewMoreContainer img')[0].click()");
            Thread.Sleep(1000);
            scriptExecutor.ExecuteScript("$('.viewMoreContainer img')[0].click()");
            Thread.Sleep(1000);
        }

        [Then(@"all versions of the document should be seen")]
        public void ThenAllVersionsOfTheDocumentShouldBeSeen()
        {
            string checkVersion = (string)scriptExecutor.ExecuteScript("var links = $('#versionHeader span')[0].innerText;return links"),
                   checkModified = (string)scriptExecutor.ExecuteScript("var links = $('#versionHeader span')[1].innerText;return links"),
                   checkModifiedBy = (string)scriptExecutor.ExecuteScript("var links = $('#versionHeader span')[2].innerText;return links"),
                   checkViewMore = (string)scriptExecutor.ExecuteScript("var links = $('#viewMoreVersions')[0].innerText;return links"),
                   checkVersionNumber = (string)scriptExecutor.ExecuteScript("var links = $('.versionNumber ')[0].innerText;return links");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(checkVersionNumber));
            Assert.IsTrue(checkVersion.Contains("Version"));
            Assert.IsTrue(checkModified.Contains("Modified"));
            Assert.IsTrue(checkModifiedBy.Contains("Modified By"));
            Assert.IsTrue(checkViewMore.Contains("View More"));
        }
        #endregion

        #region 05. Verify the footer links
        [When(@"user clicks on footer on document landing page")]
        public void WhenUserClicksOnFooterOnDocumentLandingPage()
        {
            webDriver.Navigate().GoToUrl(URL);
            Thread.Sleep(5000);
        }

        [Then(@"all links should be present on footer on document landing page")]
        public void ThenAllLinksShouldBePresentOnFooterOnDocumentLandingPage()
        {
            string checkFeedbackAndSupport = (string)scriptExecutor.ExecuteScript("var links =$('#feedbackSupport')[0].innerText;return links"),
                   checkPrivacyAndCookies = (string)scriptExecutor.ExecuteScript("var links =$('#privacyLink')[0].href;return links"),
                   checkTermsOfUse = (string)scriptExecutor.ExecuteScript("var links =$('#termsOfUse')[0].href;return links"),
                   checkMicrosoft = (string)scriptExecutor.ExecuteScript("var links =$('.footerLink span')[0].innerText;return links"),
                   checkMicrosoftLogo = (string)scriptExecutor.ExecuteScript("var links =$('.footerLogo a img')[0].title;return links");

            Thread.Sleep(1000);

            Assert.IsTrue(checkMicrosoftLogo.Contains("Microsoft"));
            Assert.IsTrue(checkFeedbackAndSupport.Contains("Feedback & Support"));
            Assert.IsTrue(checkPrivacyAndCookies.Contains("privacy"));
            Assert.IsTrue(checkTermsOfUse.Contains("intellectualproperty"));
            Assert.IsTrue(checkMicrosoft.Contains("2016 Microsoft"));
        }

        #endregion

        #region 06. Verify pin/unpin functionality

        [When(@"user clicks on pin/unpin button")]
        public void WhenUserClicksOnPinUnpinButton()
        {
            Thread.Sleep(2000);
            if((Boolean)(scriptExecutor.ExecuteScript("var bool = $('#unpinDocument').hasClass('hide'); return bool;")))
            {
                scriptExecutor.ExecuteScript("$('#spanPin').click();");
                pinned = true;
            }
            else
            {
                scriptExecutor.ExecuteScript("$('#spanUnpin').click();");
                pinned = false;
            }
            Thread.Sleep(2000);
        }

        [Then(@"document should get pinned/unpinned")]
        public void ThenDocumentShouldGetPinnedUnpinned()
        {
            if ((Boolean)(scriptExecutor.ExecuteScript("var bool = $('#unpinDocument').hasClass('hide'); return bool;")) && !pinned)
            {
                Assert.IsTrue(true);
            }
            else if (!(Boolean)(scriptExecutor.ExecuteScript("var bool = $('#unpinDocument').hasClass('hide'); return bool;")) && pinned)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsFalse(true);
            }
        }

        #endregion


    }
}
