// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="Home.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of home page </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System;
    using System.Configuration;
    using System.Threading;
    using TechTalk.SpecFlow;

    [Binding]
    public class Home
    {
        string URL = ConfigurationManager.AppSettings["Home"];
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();
        #region 01. Open the browser and load home page
        [When(@"user enters credentials on homepage")]
        public void WhenUserEntersCredentialsOnHomepage()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
        }

        [Then(@"home page should be loaded with element '(.*)'")]
        public void ThenTheHomePageShouldBeLoadedWithElement(string HomeContainer)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, HomeContainer, Selector.Id));
        }
        #endregion

        #region 02. Open the hamburger menu and verify all the elements
        [When(@"user clicks on hamburger menu on homepage")]
        public void WhenUserClicksOnHamburgerMenuOnHomepage()
        {
            webDriver.FindElement(By.ClassName("AppSwitcherContainer")).Click();
        }

        [Then(@"hamburger menu should display '(.*)','(.*)','(.*)' and '(.*)' menu")]
        public void ThenHamburgerMenuShouldDisplayAndMenu(string selectHome, string selectMatters, string selectDocument, string selectProvision)
        {
            Thread.Sleep(2000);
            string home = (string)scriptExecutor.ExecuteScript("var links = $('.AppMenuFlyoutPriLinks a')[0].text;return links"),
                   matters = (string)scriptExecutor.ExecuteScript("var links = $('.AppMenuFlyoutPriLinks a')[1].text;return links"),
                   documents = (string)scriptExecutor.ExecuteScript("var links = $('.AppMenuFlyoutPriLinks a')[2].text;return links"),
                   matterProvision = (string)scriptExecutor.ExecuteScript("var links = $('.AppMenuFlyoutPriLinks a')[3].text;return links");

            Assert.IsTrue(home.Equals(selectHome));
            Assert.IsTrue(matters.Equals(selectMatters));
            Assert.IsTrue(documents.Equals(selectDocument));
            Assert.IsTrue(matterProvision.Equals(selectProvision));
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.CloseSwitcher').click()");
        }
        #endregion

        #region 03. Verify the links on home page
        [When(@"user clicks on learn more and dismiss link")]
        public void WhenUserClicksOnLearnMoreAndDismissLink()
        {
            scriptExecutor.ExecuteScript("$('#HomeContainer > header > span > a')[1].click();");
        }

        [Then(@"it should dismiss the link")]
        public void ThenItShouldDismissTheLink()
        {
            string learnMoreLink = (string)scriptExecutor.ExecuteScript("var links = $('#HomeContainer > header > span > a').attr('href');return links");
            Assert.IsTrue(common.ElementPresent(webDriver, "RemoveWelcomeBar", Selector.Class));
            Assert.IsTrue(learnMoreLink.Equals("http://www.microsoft.com/mattercenter"));
        }
        #endregion

        #region 04. Verify all components of the page
        [When(@"user clicks on matters link")]
        public void WhenUserClicksOnMattersLink()
        {
            scriptExecutor.ExecuteScript("$('.MattersContainer a figure figcaption')[0].click();");
            Thread.Sleep(4000);
        }

        [Then(@"it should open the matter search page")]
        public void ThenItShouldOpenTheMatterSearchPage()
        {
            string PageURL = (string)scriptExecutor.ExecuteScript("var links = window.location.href;return links");
            Assert.IsTrue(PageURL.Contains("#/matters"));
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            string matters = (string)scriptExecutor.ExecuteScript("var links = $('.MattersContainer a figure figcaption')[0].innerText;return links");
            Assert.IsTrue(matters.Contains("Matters"));
        }

        [When(@"user clicks on documents link")]
        public void WhenUserClickOnDocumentsLink()
        {
            scriptExecutor.ExecuteScript("$('.DocumentsContainer figure figcaption')[0].click();");
            Thread.Sleep(4000);
        }

        [Then(@"it should open the document search page")]
        public void ThenItShouldOpenTheDocumentSearchPage()
        {
            string PageURL = scriptExecutor.ExecuteScript("var links = window.location.href; return links").ToString();
            Assert.IsTrue(PageURL.Contains("#/documents"));
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            string documents = (string)scriptExecutor.ExecuteScript("var links = $('.DocumentsContainer figure figcaption')[0].innerText;return links");
            Assert.IsTrue(documents.Contains("Documents"));
        }

        [When(@"user clicks on upload attachments link")]
        public void WhenUserClicksOnUploadAttachmentsLink()
        {
            scriptExecutor.ExecuteScript("$('.UploadAttachmentsLink').click();");
            Thread.Sleep(4000);
        }
        [Then(@"it should redirect to matter search page")]
        public void ThenItShouldRedirectToMatterSearchPage()
        {
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            string matterPage = (string)scriptExecutor.ExecuteScript("var links = $('.UploadAttachmentsLink').attr('href');return links");
            Assert.IsTrue(matterPage.Contains("#/matters"));
        }

        [When(@"user clicks on create a new matter")]
        public void WhenUserClicksOnCreateANewMatter()
        {
            scriptExecutor.ExecuteScript("$('.CreateMatterLink').click();");
            Thread.Sleep(4000);
        }

        [Then(@"it should open the matter provision page")]
        public void ThenItShouldOpenTheMatterProvisionPage()
        {
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            string matterProvision = (string)scriptExecutor.ExecuteScript("var links = $('.CreateMatterLink').attr('href');return links");
            Assert.IsTrue(matterProvision.Contains("#/createMatter"));
        }

        [When(@"user clicks on go to matter center home page")]
        public void WhenUserClicksOnGoToMatterCenterHomePage()
        {
            scriptExecutor.ExecuteScript("$('.MatterDashboard').click();");
            Thread.Sleep(4000);
        }

        [Then(@"it should open the matters page")]
        public void ThenItShouldOpenTheMattersPage()
        {
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            string matterDashboard = (string)scriptExecutor.ExecuteScript("var links = $('.MatterDashboardLink').attr('href');return links");
            Assert.IsTrue(matterDashboard.Contains("https://msmatter.sharepoint.com/SitePages/MatterCenterHome.aspx?section=1"));
        }
        #endregion

        #region 05. Verify the Matter Center support link
        [When(@"user click on Matter Center support link")]
        public void WhenUserClickOnMatterCenterSupportLink()
        {
            Thread.Sleep(100);
        }

        [Then(@"it should open draft mail with recipient '(.*)' and subject as '(.*)'")]
        public void ThenItShouldOpenDraftMailWithRecipientAndSubjectAs(string emailId, string emailSubject)
        {
            string supportLink = (string)scriptExecutor.ExecuteScript("var links = $('.emailLink')[1].href; return links");
            Assert.IsTrue(supportLink.Contains("mailto:" + emailId + "?subject=" + emailSubject));
        }
        #endregion

        #region 06. Verify the contextual help section
        [When(@"user clicks on contextual help icon\(\?\)")]
        public void WhenUserClicksOnContextualHelpIcon()
        {
            scriptExecutor.ExecuteScript("$('.ContextualHelpLogo').click();");
        }

        [Then(@"it should open the contextual help menu")]
        public void ThenItShouldOpenTheContextualHelpMenu()
        {
            int headerCount = 0;
            if (common.ElementPresent(webDriver, "ContextualHelpHeader", Selector.Class) && common.ElementPresent(webDriver, "contextualHelpSections", Selector.Class) && common.ElementPresent(webDriver, "ContextualHelpSupport", Selector.Class))
            {
                headerCount++;
            }
            Assert.IsTrue(headerCount.Equals(1));
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.closeContextualHelpFlyout').click();");
        }
        #endregion

        #region 07. Verify the user profile icon
        [When(@"user clicks on user profile icon")]
        public void WhenUserClicksOnUserProfileIcon()
        {
            scriptExecutor.ExecuteScript("$('.AppHeaderProfilePict').click();");
        }

        [Then(@"it should open user profile details")]
        public void ThenItShouldOpenUserProfileDetails()
        {
            int flyoutInfoCount = 0;
            if (common.ElementPresent(webDriver, "PersonalInfoFlyout", Selector.Class) && common.ElementPresent(webDriver, "PersonaContainer", Selector.Class) && common.ElementPresent(webDriver, "PersonaPictureContainer", Selector.Class)
                && common.ElementPresent(webDriver, "PersonaPictureDetails", Selector.Class) && common.ElementPresent(webDriver, "SignOutLink", Selector.Class))
            {
                flyoutInfoCount++;
            }
            Assert.IsTrue(flyoutInfoCount.Equals(1));
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.AppHeaderProfilePict').click();");
        }
        #endregion
    }
}
