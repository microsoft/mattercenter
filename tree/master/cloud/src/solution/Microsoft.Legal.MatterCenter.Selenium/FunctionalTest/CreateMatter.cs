// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="CreateMatter.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of create matter page </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Threading;
    using TechTalk.SpecFlow;

    [Binding]
    public class CreateMatterSteps
    {
        string URL = ConfigurationManager.AppSettings["CreateMatter"];
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();

        #region 01. Open the browser and load 'create matter' page
        [When(@"user enters credentials on matter provision page")]
        public void WhenWeWillEnterAnd()
        {
            common.GetLogin(webDriver, URL);
        }

        [Then(@"matter provision page should be loaded with element '(.*)'")]
        public void ThenMatterProvisionPageShouldBeLoadedWithElement(string checkId)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, checkId, Selector.Id));
        }
        #endregion

        #region 02. Verify 'Open Matter' tab

        [When(@"user selects basic matter information")]
        public void WhenUserSelectsBasicMatterInformation()
        {
            Thread.Sleep(4000);
            webDriver.FindElement(By.XPath("//main/div/div/div")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select")).Click();
            Thread.Sleep(3000);
            new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByText(ConfigurationManager.AppSettings["DropDownKeyword"]);
            Thread.Sleep(2000);
            webDriver.FindElement(By.CssSelector("option[value=\"100002\"]")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("txtMatterName")).Click();
            webDriver.FindElement(By.Id("txtMatterName")).Clear();
            Random randomObj = new Random();
            string randomNumber = randomObj.Next(1, 99999).ToString(CultureInfo.CurrentCulture);
            webDriver.FindElement(By.Id("txtMatterName")).SendKeys(randomNumber);
            webDriver.FindElement(By.Id("txtMatterDesc")).Click();
            webDriver.FindElement(By.Id("txtMatterDesc")).Clear();
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("txtMatterDesc")).SendKeys(ConfigurationManager.AppSettings["MatterName"]);
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev').click();");
            Thread.Sleep(2000);
        }

        [Then(@"it should navigate to second step")]
        public void ThenItShouldNavigateToSecondStep()
        {
            string nextStep = (string)scriptExecutor.ExecuteScript("var step = $('.menuTextSelected')[0].innerText;return step");
            Assert.IsTrue(nextStep.Contains("Assign Permission"));
        }
        #endregion

        #region 03. Verify 'Assign Permission' tab

        [When(@"user selects permission for matter")]
        public void WhenUserSelectsPermissionFormatter()
        {
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//main/div/div/div[2]")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("txtConflictCheckBy")).Click();
            webDriver.FindElement(By.Id("txtConflictCheckBy")).Clear();
            webDriver.FindElement(By.Id("txtConflictCheckBy")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
            webDriver.FindElement(By.LinkText(ConfigurationManager.AppSettings["Attorney"])).Click();
            Thread.Sleep(3000);
            webDriver.FindElement(By.Id("txtUser1")).Click();
            webDriver.FindElement(By.Id("txtUser1")).Clear();
            webDriver.FindElement(By.Id("txtUser1")).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
            webDriver.FindElement(By.LinkText(ConfigurationManager.AppSettings["AttorneyMember"])).Click();
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.calendar').val('09/14/2016').trigger('change')");
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("roleUser1")).Click();
            new SelectElement(webDriver.FindElement(By.Id("roleUser1"))).SelectByText("Responsible Attorney");
            webDriver.FindElement(By.Id("chkConflictCheck")).Click();
            webDriver.FindElement(By.Id("txtBlockUser")).Click();
            webDriver.FindElement(By.Id("txtBlockUser")).Clear();
            webDriver.FindElement(By.Id("txtBlockUser")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
            webDriver.FindElement(By.LinkText(ConfigurationManager.AppSettings["Attorney"])).Click();
            Thread.Sleep(2000);
            new SelectElement(webDriver.FindElement(By.Id("permUser1"))).SelectByText("Full Control");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev').click();");
            Thread.Sleep(3000);
        }

        [Then(@"it should navigate to third step")]
        public void ThenItShouldNavigateToThirdStep()
        {
            string nextStep = (string)scriptExecutor.ExecuteScript("var step = $('.menuTextSelected')[0].innerText;return step");
            Assert.IsTrue(nextStep.Contains("Create and Notify"));
        }

        #endregion

        #region 04. Verify 'create and notify' tab

        [When(@"user checks all check boxes")]
        public void WhenUserCheckesAllCheckBoxes()
        {
            Thread.Sleep(4000);
            webDriver.FindElement(By.XPath("//main/div/div/div[3]")).Click();
            Thread.Sleep(2000);
            bool checkTrueOrFalse = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected2').prop('checked');return step;");
            if (checkTrueOrFalse == false)
            {
                scriptExecutor.ExecuteScript("$('#demo-checkbox-unselected2').click()");
            }
            checkTrueOrFalse = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected0').prop('checked');return step");
            if (checkTrueOrFalse == false)
            {
                scriptExecutor.ExecuteScript("$('#demo-checkbox-unselected0').click()");
            }
            checkTrueOrFalse = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected3').prop('checked');return step");
            if (checkTrueOrFalse == false)
            {
                scriptExecutor.ExecuteScript("$('#demo-checkbox-unselected3').click()");
            }
            checkTrueOrFalse = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected1').prop('checked');return step");
            if (checkTrueOrFalse == false)
            {
                scriptExecutor.ExecuteScript("$('#demo-checkbox-unselected1').click()");
            }
        }

        [Then(@"all check boxes should get checked")]
        public void ThenAllCheckBoxesShouldGetChecked()
        {
            bool checkIncludeEmailNotification = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected2').prop('checked');return step;");
            bool checkIncludeCalendar = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected0').prop('checked');return step;");
            bool checkIncludeTasks = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected3').prop('checked');return step;");
            bool checkIncludeRssFeed = (bool)scriptExecutor.ExecuteScript("var step = $('#demo-checkbox-unselected3').prop('checked');return step;");
            Assert.IsTrue(checkIncludeEmailNotification == true);
            Assert.IsTrue(checkIncludeCalendar == true);
            Assert.IsTrue(checkIncludeTasks == true);
            Assert.IsTrue(checkIncludeRssFeed == true);
        }

        [When(@"user clicks on create and notify")]
        public void WhenUserClicksOnCreateAndNotify()
        {
            scriptExecutor.ExecuteScript("$('#btnCreateMatter').click()");
            Thread.Sleep(120000);
        }

        [Then(@"a new matter should get created")]
        public void ThenNewMatterShouldGetCreated()
        {
            string checkHereLink = (string)scriptExecutor.ExecuteScript("var step = $('.notification a')[0].innerText;return step;");
            Assert.IsTrue(checkHereLink.Contains("here"));
        }

        #endregion
    }
}
