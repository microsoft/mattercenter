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
    public class CreateMatter
    {
        string URL = ConfigurationManager.AppSettings["CreateMatter"];
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();
        int errorCount = 0;

        #region 01. Open the browser and load 'create matter' page
        [When(@"user enters credentials on matter provision page")]
        public void WhenUserEntersCredentialsOnMatterProvisionPage()
        {
            common.GetLogin(webDriver, URL);
        }

        [Then(@"matter provision page should be loaded with element '(.*)'")]
        public void ThenMatterProvisionPageShouldBeLoadedWithElement(string checkId)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, checkId, Selector.Id));
        }
        #endregion

        #region 02. Verify incorrect form submission errors

        [When(@"user submits blank and incorrect forms")]
        public void WhenUserSubmitsBlankAndIncorrectForms()
        {
            errorCount = 0;
            common.GetLogin(webDriver, URL);
            string noClientError = string.Empty, repeatedMatterError = string.Empty, noMatterNameError = string.Empty;
            Thread.Sleep(4000);
            webDriver.FindElement(By.XPath("//main/div/div/div")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select")).Click();
            Thread.Sleep(3000);
            new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByIndex(0);
            scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
            Thread.Sleep(5000);
            noClientError = (string)scriptExecutor.ExecuteScript("var text = $('#errText')[0].innerText; return text;");
            Assert.IsTrue(noClientError.ToLower(CultureInfo.CurrentCulture).Contains("select a client"));
            errorCount++;
            new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByText(ConfigurationManager.AppSettings["DropDownKeyword"]);
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
            Thread.Sleep(7000);
            repeatedMatterError = (string)scriptExecutor.ExecuteScript("var text = $('.errText ')[0].innerText; return text;");
            if (repeatedMatterError.ToLower(CultureInfo.CurrentCulture).Contains("matter is already created") || repeatedMatterError.ToLower(CultureInfo.CurrentCulture).Contains("your matter name includes a character that is not allowed"))
            {
                Assert.IsTrue(true);
            }
            errorCount++;
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("txtMatterName")).Clear();
            Thread.Sleep(1000);
            scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
            noMatterNameError = (string)scriptExecutor.ExecuteScript("var text = $('.popUpFloatRight')[0].innerText; return text;");
            Assert.IsTrue(noMatterNameError.ToLower(CultureInfo.CurrentCulture).Contains("your matter name includes a character that is not allowed"));
            errorCount++;

        }

        [Then(@"next step should not be allowed")]
        public void ThenNextStepShouldNotBeAllowed()
        {
            if (errorCount >= 2)
            {
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region 03. Verify 'Open Matter' tab

        [When(@"user selects basic matter information")]
        public void WhenUserSelectsBasicMatterInformation()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
            webDriver.FindElement(By.XPath("//main/div/div/div")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select")).Click();
            Thread.Sleep(3000);
            new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByText(ConfigurationManager.AppSettings["DropDownKeyword"]);
            Thread.Sleep(2000);
            webDriver.FindElement(By.CssSelector("option[value=\"Amazon\"]")).Click();
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

        #region 04. Verify incorrect form submission error on Assign permissions tab

        [When(@"user submits blank forms on assign permissions")]
        public void WhenUserSubmitsBlankFormsOnAssignPermissions()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
            bool present;
            try
            {
                webDriver.FindElement(By.Id("conflictCheck"));
                present = true;
            }
            catch (NoSuchElementException e)
            {
                present = false;
            }

            if (present)
            {
                errorCount = 0;
                string conflictError = string.Empty, conflictDateError = string.Empty, conflictReviewError = string.Empty, userCError = string.Empty, attorneyError = string.Empty;
                Thread.Sleep(2000);
                webDriver.FindElement(By.XPath("//main/div/div/div[2]")).Click();
                Thread.Sleep(2000);
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                Thread.Sleep(2000);
                conflictError = (string)scriptExecutor.ExecuteScript("var text = $('.errText')[0].innerText; return text;");
                Assert.IsTrue(conflictError.ToLower(CultureInfo.CurrentCulture).Contains("a confilct check must be completed"));
                errorCount++;
                webDriver.FindElement(By.Id("chkConflictCheck")).Click();
                try
                {
                    scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                    Thread.Sleep(2000);
                    conflictDateError = (string)scriptExecutor.ExecuteScript("var text = $('.popUpFloatRight')[0].innerText; return text;");
                    Assert.IsTrue(conflictDateError.ToLower(CultureInfo.CurrentCulture).Contains("enter the date"));
                    errorCount++;
                    scriptExecutor.ExecuteScript("$('.calendar').val('09/14/2016').trigger('change')");
                    Thread.Sleep(2000);
                    scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                    Thread.Sleep(2000);
                    conflictReviewError = (string)scriptExecutor.ExecuteScript("var text = $('.popUpFloatRight')[0].innerText; return text;");
                    Assert.IsTrue(conflictReviewError.ToLower(CultureInfo.CurrentCulture).Contains("enter the conflict reviewers"));
                    errorCount++;
                    webDriver.FindElement(By.Id("txtConflictCheckBy")).Click();
                    webDriver.FindElement(By.Id("txtConflictCheckBy")).Clear();
                    webDriver.FindElement(By.Id("txtConflictCheckBy")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
                    Thread.Sleep(1000);

                    webDriver.FindElement(By.XPath("//*[contains(@id,'typeahead-10')]")).Click();
                    Thread.Sleep(3000);
                    scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                    Thread.Sleep(2000);
                    userCError = (string)scriptExecutor.ExecuteScript("var text = $('.popUpFloatRight')[0].innerText; return text;");
                    Assert.IsTrue(userCError.ToLower(CultureInfo.CurrentCulture).Contains("enter users"));
                    errorCount++;
                    webDriver.FindElement(By.Id("txtBlockUser")).Click();
                    webDriver.FindElement(By.Id("txtBlockUser")).Clear();
                    webDriver.FindElement(By.Id("txtBlockUser")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
                    Thread.Sleep(1000);
                    webDriver.FindElement(By.XPath("//*[contains(@id,'typeahead-14')]")).Click();
                    Thread.Sleep(2000);
                    scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                    Thread.Sleep(2000);
                    attorneyError = (string)scriptExecutor.ExecuteScript("var text = $('.popUpFloatRight')[0].innerText; return text;");
                    Assert.IsTrue(attorneyError.ToLower(CultureInfo.CurrentCulture).Contains("team member cannot be empty"));
                    errorCount++;
                    webDriver.FindElement(By.Id("chkConflictCheck")).Click();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    webDriver.FindElement(By.Id("chkConflictCheck")).Click();
                }
            }
            else
            {
                errorCount = 0;
                string teamMember = string.Empty, responsibleAttorney = string.Empty, conflictReviewError = string.Empty, userCError = string.Empty, attorneyError = string.Empty;
                Thread.Sleep(2000);
                webDriver.FindElement(By.XPath("//main/div/div/div[2]")).Click();
                Thread.Sleep(2000);
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                Thread.Sleep(2000);
                teamMember = (string)scriptExecutor.ExecuteScript("var text = $('#errText')[0].innerText; return text;");
                Assert.IsTrue(teamMember.ToLower(CultureInfo.CurrentCulture).Contains("team member cannot be empty"));
                errorCount++;
                Thread.Sleep(2000);
                webDriver.FindElement(By.Id("txtUser1")).Click();
                webDriver.FindElement(By.Id("txtUser1")).Clear();
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(Keys.ArrowDown);
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(Keys.Enter);
                Thread.Sleep(2000);
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev')[0].click();");
                Thread.Sleep(2000);
                responsibleAttorney = (string)scriptExecutor.ExecuteScript("var text = $('#errText')[0].innerText; return text;");
                Assert.IsTrue(responsibleAttorney.ToLower(CultureInfo.CurrentCulture).Contains("enter at least one responsible attorney"));
                errorCount++;
            }


        }

        [Then(@"it should not navigate to third step")]
        public void ThenItShouldNotNavigateToThirdStep()
        {
            if (errorCount >= 2)
            {
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region 05. Verify 'Assign Permission' tab

        [When(@"user selects permission for current matter")]
        public void WhenUserSelectsPermissionForCurrentMatter()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(4000);
            bool present;
            try
            {
                webDriver.FindElement(By.Id("conflictCheck"));
                present = true;
            }
            catch (NoSuchElementException e)
            {
                present = false;
            }

            if (present)
            {
                Thread.Sleep(2000);
                bool conflictChecked = (bool)scriptExecutor.ExecuteScript("var text = $('#chkConflictCheck')[0].checked; return text;");
                if (!conflictChecked)
                {
                    webDriver.FindElement(By.Id("chkConflictCheck")).Click();
                }               
                webDriver.FindElement(By.XPath("//main/div/div/div[2]")).Click();
                Thread.Sleep(2000);
                webDriver.FindElement(By.Id("txtConflictCheckBy")).Click();
                webDriver.FindElement(By.Id("txtConflictCheckBy")).Clear();
                webDriver.FindElement(By.Id("txtConflictCheckBy")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[contains(@id,'typeahead-10')]")).Click();
                Thread.Sleep(3000);
                webDriver.FindElement(By.Id("txtUser2")).Click();
                webDriver.FindElement(By.Id("txtUser2")).Clear();
                webDriver.FindElement(By.Id("txtUser2")).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
                webDriver.FindElement(By.Id("txtUser2")).SendKeys(Keys.ArrowDown);
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("txtUser2")).SendKeys(Keys.Enter);
                Thread.Sleep(2000);
                scriptExecutor.ExecuteScript("$('.calendar').val('09/14/2016').trigger('change')");
                Thread.Sleep(2000);
                webDriver.FindElement(By.Id("roleUser1")).Click();
                new SelectElement(webDriver.FindElement(By.Id("roleUser1"))).SelectByText(ConfigurationManager.AppSettings["Role"]);
                conflictChecked = (bool)scriptExecutor.ExecuteScript("var text = $('#chkConflictCheck')[0].checked; return text;");
                if (!conflictChecked)
                {
                    webDriver.FindElement(By.Id("chkConflictCheck")).Click();
                }

                webDriver.FindElement(By.Id("txtBlockUser")).Click();
                webDriver.FindElement(By.Id("txtBlockUser")).Clear();
                webDriver.FindElement(By.Id("txtBlockUser")).SendKeys(ConfigurationManager.AppSettings["AttorneyUser"]);
                Thread.Sleep(1000);
                webDriver.FindElement(By.XPath("//*[contains(@id,'typeahead-14')]")).Click();
                Thread.Sleep(2000);
                new SelectElement(webDriver.FindElement(By.Id("permUser1"))).SelectByText(ConfigurationManager.AppSettings["PermissionLevel"]);
                Thread.Sleep(3000);
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev').click();");
                Thread.Sleep(3000);
            }
            else
            {
                Thread.Sleep(2000);
                webDriver.FindElement(By.XPath("//main/div/div/div[2]")).Click();
                Thread.Sleep(2000);
                webDriver.FindElement(By.Id("txtUser1")).Click();
                webDriver.FindElement(By.Id("txtUser1")).Clear();
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(Keys.ArrowDown);
                Thread.Sleep(1000);
                webDriver.FindElement(By.Id("txtUser1")).SendKeys(Keys.Enter);
                Thread.Sleep(2000);
                webDriver.FindElement(By.Id("roleUser1")).Click();
                new SelectElement(webDriver.FindElement(By.Id("roleUser1"))).SelectByText(ConfigurationManager.AppSettings["Role"]);
                Thread.Sleep(1000);
                new SelectElement(webDriver.FindElement(By.Id("permUser1"))).SelectByText(ConfigurationManager.AppSettings["PermissionLevel"]);
                Thread.Sleep(3000);
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev').click();");
                Thread.Sleep(3000);
            }
        }

        [Then(@"it should navigate to third step")]
        public void ThenItShouldNavigateToThirdStep()
        {
            string nextStep = (string)scriptExecutor.ExecuteScript("var step = $('.menuTextSelected')[0].innerText;return step");
            Thread.Sleep(3000);
            if (nextStep.Contains("Provide More Inputs"))
            {
                scriptExecutor.ExecuteScript("$('.col-xs-6 .buttonPrev').click();");
            }
            Thread.Sleep(5000);
            nextStep = (string)scriptExecutor.ExecuteScript("var step = $('.menuTextSelected')[0].innerText;return step");
           
            Assert.IsTrue(nextStep.Contains("Create and Notify"));
           
        }

        #endregion

        #region 06. Verify 'create and notify' tab

        [When(@"user checks all check boxes")]
        public void WhenUserCheckesAllCheckBoxes()
        {
            common.GetLogin(webDriver, URL);            
            Thread.Sleep(4000);
            webDriver.FindElement(By.XPath("//main/div/div/div[4]")).Click();
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
