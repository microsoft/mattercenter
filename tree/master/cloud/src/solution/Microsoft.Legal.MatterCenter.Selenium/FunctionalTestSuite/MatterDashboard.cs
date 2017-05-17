// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="MatterDashboard.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of matter dashboard page </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using Microsoft.Legal.MatterCenter.Selenium;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Threading;
    using TechTalk.SpecFlow;

    [Binding]
    public class MatterDashboard
    {
        string URL = ConfigurationManager.AppSettings["MatterDashboard"], initialState = String.Empty;
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();

        #region 01. Open the browser and load Matter Center home page
        [When(@"user enters credentials on matter dashboard page")]
        public void WhenUserEntersCredentialsOnMatterDashboardPage()
        {
            common.GetLogin(webDriver, URL);
        }

        [Then(@"Matter Center home page should be loaded with element '(.*)'")]
        public void ThenMatterCenterHomePageShouldBeLoadedWithElement(string checkId)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, checkId, Selector.Id));
        }
        #endregion

        #region 02. Verify the hamburger menu
        [When(@"user clicks on hamburger menu on Matter Center homepage")]
        public void WhenUserClicksOnHamburgerMenuOnMatterCenterHomepage()
        {
            scriptExecutor.ExecuteScript("$('#openHamburger').click();");
            Thread.Sleep(4000);
        }

        [Then(@"hamburger menu should be loaded")]
        public void ThenHamburgerMenuShouldBeLoaded()
        {
            string Matters = (string)scriptExecutor.ExecuteScript("var links= $('#searchMatterTab')[0].innerText;return links"),
                   Documents = (string)scriptExecutor.ExecuteScript("var links = $('#searchDocumentTab')[0].innerText;return links"),
                   Settings = (string)scriptExecutor.ExecuteScript("var links = $('#settingsLink')[0].innerText;return links");
            Assert.IsTrue(Matters.Contains("Matters"));
            Assert.IsTrue(Documents.Contains("Documents"));
            Assert.IsTrue(Settings.Contains("Settings"));
            scriptExecutor.ExecuteScript("$('#closeHamburger').click();");

        }
        #endregion

        #region 05. Verify the matter fly out
        [When(@"user clicks on matter fly out")]
        public void WhenUserClicksOnMatterFlyOut()
        {
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.col-xs-11 a')[0].click();");
            Thread.Sleep(2000);
        }

        [Then(@"a matter fly out should be seen")]
        public void ThenAMatterFlyOutShouldBeSeen()
        {
            string matterHeaderName = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content')[0].innerText ;return links"),
                   clientName = (string)scriptExecutor.ExecuteScript("var links = $('.FlyoutContentHeading')[0].innerText ;return links"),
                   clientMatterId = (string)scriptExecutor.ExecuteScript("var links = $('.FlyoutContentHeading')[3].innerText ;return links"),
                   subAreaOfLaw = (string)scriptExecutor.ExecuteScript("var links =  $('.FlyoutContentHeading')[4].innerText ;return links"),
                   responsibleAttorney = (string)scriptExecutor.ExecuteScript("var links =  $('.FlyoutContentHeading')[5].innerText ;return links"),
                   viewMatter = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content')[7].innerText ;return links"),
                   uploadToMatter = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content')[8].innerText;return links"),
                   flyoutClientName = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content .ms-font-m')[1].innerText;return links"),
                   flyoutClientMatterId = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content .ms-font-m')[3].innerText;return links"),
                   flyoutSubAreaOfLaw = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content .ms-font-m')[5].innerText;return links"),
                   flyoutResonsibleAttorney = (string)scriptExecutor.ExecuteScript("var links = $('.ms-Callout-content .ms-font-m')[7].innerText;return links");
            if (flyoutClientName != null && flyoutClientMatterId != null && flyoutSubAreaOfLaw != null && flyoutResonsibleAttorney != null && matterHeaderName != null)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(clientName.Contains("Client"));
            Assert.IsTrue(clientMatterId.Contains("Client.MatterID"));
            Assert.IsTrue(subAreaOfLaw.Contains("Sub Area of Law"));
            Assert.IsTrue(responsibleAttorney.Contains("Responsible Attorney"));
            Assert.IsTrue(viewMatter.Contains("View matter details"));
            Assert.IsTrue(uploadToMatter.Contains("Upload to a matter"));
        }
        #endregion

        #region  06. Verify the search feature
        [When(@"user types '(.*)' in search box on Matter Center Homepage")]
        public void WhenUserTypesInSearchBoxOnMatterCenterHomepage(string searchBoxValue)
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(5000);
            scriptExecutor.ExecuteScript("$('.col-xs-12 .form-control')[0].value='" + searchBoxValue + "'");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('#basic-addon1').click()");
            Thread.Sleep(5000);
        }

        [Then(@"all results having '(.*)' keyword should be displayed")]
        public void ThenAllResultsHavingKeywordShouldBeDisplayed(string searchBoxValue)
        {
            long linkLength = (long)scriptExecutor.ExecuteScript("var links = $('.col-xs-11').length;return links;");
            int linkCounter, tempCounter = 0;
            for (linkCounter = 0; linkCounter < linkLength; linkCounter++)
            {
                Thread.Sleep(1000);
                string test = (string)scriptExecutor.ExecuteScript("var links =$('.col-xs-11')[" + linkCounter + "].innerText;return links;");
                if (!String.IsNullOrWhiteSpace(searchBoxValue) && test.ToLower(CultureInfo.CurrentCulture).Contains(searchBoxValue.ToLower(CultureInfo.CurrentCulture)))
                    tempCounter++;
            }
            if (tempCounter > 0)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
        #endregion

        #region 11. Verify gibberish search

        [When(@"user types gibberish in search box on Matter Center dashboard")]
        public void WhenUserTypesGibberishInSearchBoxOnMatterCenterDashboard()
        {
            string searchBoxValue = ConfigurationManager.AppSettings["Gibberish"];
            common.GetLogin(webDriver, URL);
            Thread.Sleep(6000);           
           // webDriver.FindElement(By.CssSelector(".col-xs-12 .form-control")).Clear();
           //  Thread.Sleep(1000);
           webDriver.FindElement(By.CssSelector(".col-xs-12 .form-control")).SendKeys(searchBoxValue);
            Thread.Sleep(5000);
            scriptExecutor.ExecuteScript("$('#basic-addon1').click()");
            Thread.Sleep(5000);
        }

        [Then(@"no results should be displayed on Matter Center dashboard")]
        public void ThenNoResultsShouldBeDisplayedOnMatterCenterDashboard()
        {
            string searchResCount = (string)scriptExecutor.ExecuteScript("var text = $('.nav-tabs .active a')[0].innerText; return text;");
            Assert.IsTrue(searchResCount.ToLower(CultureInfo.CurrentCulture).Contains("0"));
        }

        #endregion

        #region 04. Verify the upload button functionality
        [When(@"user clicks on upload button")]
        public void WhenUserClicksOnUploadButton()
        {
            scriptExecutor.ExecuteScript("$('.ui-grid-cell-contents img')[1].click()");
            Thread.Sleep(5000);
        }

        [Then(@"an upload pop up should be seen")]
        public void ThenAnUploadPopUpShouldBeSeen()
        {
            string checkpopupheader = (string)scriptExecutor.ExecuteScript("var links = $('.attachmentHeader ')[0].innerText;return links");
            Assert.IsTrue(checkpopupheader.Contains("Drag and drop items to folders on the right"));
            scriptExecutor.ExecuteScript(" $('.modalClose img')[0].click();");
            Thread.Sleep(5000);
        }
        #endregion

        #region  03. Verify the pin/unpin functionality
        [When(@"user clicks on pin or unpin")]
        public void WhenUserClicksOnPinOrUnpin()
        {
            initialState = (string)scriptExecutor.ExecuteScript("var links = $('.ui-grid-cell-contents img')[0].src;return links");
            scriptExecutor.ExecuteScript("$('.ui-grid-cell-contents img')[0].click()");
            Thread.Sleep(5000);
        }

        [Then(@"matter should get pinned or unpinned")]
        public void ThenMatterShouldGetPinnedOrUnpinned()
        {
            string finalState = (string)scriptExecutor.ExecuteScript("var links = $('.ui-grid-cell-contents img')[0].src;return links");
            if ((initialState.ToLower(CultureInfo.CurrentCulture).Contains("pin") && finalState.ToLower(CultureInfo.CurrentCulture).Contains("unpin")) || (initialState.ToLower(CultureInfo.CurrentCulture).Contains("unpin") && finalState.ToLower(CultureInfo.CurrentCulture).Contains("pin")))
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
        #endregion

        #region 07. Verify the advance filter functionality
        [When(@"user clicks on advance filter")]
        public void WhenUserClicksOnAdvanceFilter()
        {
            scriptExecutor.ExecuteScript("$('.col-xs-3 img')[0].click();");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.input-group-addon')[1].click();");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-md-2 a')[1].click();");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.refinerWrapper input')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript(" $('.filterOkButton .dashboardSearch')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.input-group-addon')[2].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-md-2 a')[1].click();");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.refinerWrapper input')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.filterOkButton .dashboardSearch')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.input-group-addon')[3].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-md-2 a')[1].click();");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.refinerWrapper input')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.filterOkButton .dashboardSearch')[0].click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('#filterAdvancedSearch').click()");
            Thread.Sleep(5000);
        }

        [Then(@"filter results should be shown to user")]
        public void ThenFilterResultsShouldBeShownToUser()
        {
            long length = (long)scriptExecutor.ExecuteScript("var links = $('.col-xs-11').length;return links");
            // ***This calculation will return the number of elements present in matter grid***
            long finalValue = (8 + (length - 1) * 6); 
            int counter = 0;
            for (int documentCounter = 8; documentCounter <= finalValue; documentCounter = documentCounter + 6)
            {
                string checkClient = (string)scriptExecutor.ExecuteScript("var links = $('.ui-grid-cell-contents')[" + documentCounter + "].innerText;return links");
                if (checkClient.Contains("Amazon"))
                    counter++;
            }
            Assert.IsTrue(counter >=5);
        }
        #endregion

        #region 08. Verify the sort functionality
        [When(@"user sorts data for All matters in ascending order")]
        public void WhenUserSortsDataForAllMattersInAscendingOrder()
        {
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[1].click()");
            Thread.Sleep(4000);
        }

        [Then(@"all records should be sorted in ascending order")]
        public void ThenAllRecordsShouldBeSortedInAscendingOrder()
        {
            int totalDocument = 0, documentCount = 0;
            char[] delimiters = new char[] { '\r', '\n' };

            long length = (long)scriptExecutor.ExecuteScript("var links = $('.col-xs-11').length;return links");
            string sortedDocument = "[", duplicateDocuments = null;
            string[] documentList = new string[length];

            for (int documentCounter = 0; documentCounter < length; documentCounter++)
            {
                string datachunk = (string)scriptExecutor.ExecuteScript("var links = $('.col-xs-11')[" + documentCounter + "].innerText;return links");
                string[] rows = datachunk.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (!(rows[0].Equals(duplicateDocuments)))
                {

                    if (rows[0] != null)
                        documentList[documentCounter] = rows[0];
                    duplicateDocuments = rows[0];
                }
            }
            var tempDocumentList = new List<string>();
            foreach (var document in documentList)
            {
                if (!string.IsNullOrWhiteSpace(document))
                {
                    tempDocumentList.Add(document);
                    sortedDocument += "'" + document + "',";
                }
            }
            sortedDocument.TrimEnd(',');
            sortedDocument += "]";
            var sortedDocumentList = scriptExecutor.ExecuteScript("var oDocumentList = " + sortedDocument + ".sort();return oDocumentList");
            foreach (string element in (IEnumerable)sortedDocumentList)
            {
                if (string.Equals(element.Trim(), tempDocumentList[documentCount].Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    totalDocument++;
                }
                documentCount++;
            }
            Thread.Sleep(2000);
            Assert.IsTrue(totalDocument >= 0);
        }

        [When(@"user sorts data for All matters in ascending order of created date")]
        public void WhenUserSortsDataForAllMattersInAscendingOrderOfCreatedDate()
        {
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[3].click()");
            Thread.Sleep(4000);
        }

        [Then(@"all records should be sorted in ascending order of created date")]
        public void ThenAllRecordsShouldBeSortedInAscendingOrderOfCreatedDate()
        {
            int totalDocument = 0, documentCount = 0;
            char[] delimiters = new char[] { '\r', '\n' };

            long length = (long)scriptExecutor.ExecuteScript("var links = $('.col-xs-11').length;return links");
            string sortedDocument = "[", duplicateDocuments = null;
            string[] documentList = new string[length];

            for (int documentCounter = 0; documentCounter < length; documentCounter++)
            {
                string datachunk = (string)scriptExecutor.ExecuteScript("var links = $('.col-xs-11 a').eq(" + documentCounter + ").attr('details');links=JSON.parse(links);return links.matterCreatedDate");
                string[] rows = datachunk.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (!(rows[0].Equals(duplicateDocuments)))
                {

                    if (rows[0] != null)
                        documentList[documentCounter] = rows[0];
                    duplicateDocuments = rows[0];
                }
            }
            var tempDocumentList = new List<string>();
            foreach (var document in documentList)
            {
                if (!string.IsNullOrWhiteSpace(document))
                {
                    tempDocumentList.Add(document);
                    sortedDocument += "'" + document + "',";
                }
            }
            sortedDocument.TrimEnd(',');
            sortedDocument += "]";
            var sortedDocumentList = scriptExecutor.ExecuteScript("var oDocumentList = " + sortedDocument + ".sort();return oDocumentList");
            foreach (string element in (IEnumerable)sortedDocumentList)
            {
                if (string.Equals(element.Trim(), tempDocumentList[documentCount].Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    totalDocument++;
                }
                documentCount++;
            }

            Thread.Sleep(2000);
            Assert.IsTrue(totalDocument >= 0);
        }

        [When(@"user sorts data for Pinned matters in ascending order")]
        public void WhenUserSortsDataForPinnedMattersInAscendingOrder()
        {
            scriptExecutor.ExecuteScript("$('.nav-tabs a')[3].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[1].click()");
            Thread.Sleep(4000);
        }

        [When(@"user sorts data for Pinned matters in ascending order of created date")]
        public void WhenUserSortsDataForPinnedMattersInAscendingOrderOfCreatedDate()
        {
            scriptExecutor.ExecuteScript("$('.nav-tabs a')[3].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[3].click()");
            Thread.Sleep(4000);
        }

        [When(@"user sorts data for My matters in ascending order")]
        public void WhenUserSortsDataForMyMattersInAscendingOrder()
        {
            scriptExecutor.ExecuteScript("$('.nav-tabs a')[1].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[1].click()");
            Thread.Sleep(4000);
        }

        [When(@"user sorts data for My matters in ascending order of created date")]
        public void WhenUserSortsDataForMyMattersInAscendingOrderOfCreatedDate()
        {
            scriptExecutor.ExecuteScript("$('.nav-tabs a')[1].click()");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-sm-offset-6 img')[0].click()");
            Thread.Sleep(4000);
            scriptExecutor.ExecuteScript("$('.col-xs-offset-7 li')[3].click()");
            Thread.Sleep(4000);
        }

        #endregion

        #region  09. Verify the footer
        [When(@"user navigates to the footer")]
        public void WhenUserNavigatesToTheFooter()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
        }

        [Then(@"footer should have all the links")]
        public void ThenFooterShouldHaveAllTheLinks()
        {
            string checkFeedbackAndSupport = (string)scriptExecutor.ExecuteScript("var links = $('.rightFooterElement a')[0].href;return links"),
                   checkPrivacyAndCookies = (string)scriptExecutor.ExecuteScript("var links = $('.rightFooterElement a')[1].href;return links"),
                   checkTermsOfUse = (string)scriptExecutor.ExecuteScript("var links = $('.rightFooterElement a')[2].href;return links"),
                   checkMicrosoft = (string)scriptExecutor.ExecuteScript("var links = $('.rightFooterElement')[3].innerText;return links"),
                   checkMicrosoftLogo = (string)scriptExecutor.ExecuteScript("var links = $('#footer div a')[0].href;return links");

            Assert.IsTrue(checkMicrosoftLogo.Contains(ConfigurationManager.AppSettings["MicrosoftLogo"]));
            Assert.IsTrue(checkFeedbackAndSupport.Contains(ConfigurationManager.AppSettings["FeedbackAndSupport"]));
            Assert.IsTrue(checkPrivacyAndCookies.Contains(ConfigurationManager.AppSettings["PrivacyAndCookies"]));
            Assert.IsTrue(checkTermsOfUse.Contains(ConfigurationManager.AppSettings["TermsOfUse"]));
            Assert.IsTrue(checkMicrosoft.Contains("2016 Microsoft"));
        }

        #endregion

        #region 10. Verify enterprise search
        [When(@"user types '(.*)' in enterprise search box on Matter Center Home page")]
        public void WhenUserTypesInEnterpriseSearchBoxOnMatterCenterHomepage(string searchBoxValue)
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
            webDriver.FindElement(By.Id("searchText")).SendKeys(searchBoxValue);
            scriptExecutor.ExecuteScript("$('.searchIcon').click()");
            Thread.Sleep(10000);
        }

        [Then(@"user should redirect to enterprise page with search results for '(.*)'")]
        public void ThenUserShouldRedirectToEnterprisePageWithSearchResults(string searchBox)
        {
            string url = webDriver.Url;
            Console.Write(url);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(searchBox) && url.ToLower(CultureInfo.CurrentCulture).Contains(searchBox.ToLower(CultureInfo.CurrentCulture)));
        }

        #endregion
    }
}
