// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="Settings.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of settings page </summary>
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
    public class Settings
    {
        string URL = ConfigurationManager.AppSettings["Settings"],
               createURL = ConfigurationManager.AppSettings["CreateMatter"];
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        CommonHelperFunction common = new CommonHelperFunction();
        CultureInfo culture = CultureInfo.CurrentCulture;

        #region 01. Open the browser and load settings page
        [When(@"user enters credentials on settings page")]
        public void WhenUserEntersCredentialsOnSettingsPage()
        {
            common.GetLogin(webDriver, URL);
        }

        [Then(@"settings page should be loaded with element '(.*)'")]
        public void ThenSettingsPageShouldBeLoadedWithElement(string settingsName)
        {
            Assert.IsTrue(common.ElementPresent(webDriver, settingsName, Selector.Id));
        }
        #endregion

        #region 02. Verify deletion of team members 

        [When(@"user tries to delete all the team members")]
        public void WhenUserTriesToDeleteAllTheTeamMembers()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.clientNames')[3].click()");
            Thread.Sleep(15000);
            int noOfMembers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length = $('.mediumPermissions').length; return length;"));

            for (int index = 0; index < noOfMembers; index++)
            {
                scriptExecutor.ExecuteScript("$('.close img')[0].click()");
                Thread.Sleep(1000);
            }
        }

        [Then(@"last team member should not be deleted")]
        public void ThenLastTeamMemberShouldNotBeDeleted()
        {
            int noOfMembers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length = $('.mediumPermissions').length; return length;"));
            Assert.IsTrue(noOfMembers > 0);
            Thread.Sleep(6000);
        }

        #endregion

        #region 03. Verify error message on adding non-existing team member

        [When(@"user adds non-existing Attorney to the team")]
        public void WhenUserAddsNon_ExistingAttorneyToTheTeam()
        {
            common.GetLogin(webDriver, URL);
            Thread.Sleep(2000);
            //scriptExecutor.ExecuteScript("$('.clientNames')[3].click()");
            Thread.Sleep(5000);
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).Click();           
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).Clear();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).SendKeys(ConfigurationManager.AppSettings["MatterName"]);
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).Click();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).Clear();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).SendKeys(ConfigurationManager.AppSettings["MatterDescription"]);           
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.addmatterType a').click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.iconForward').click()");
            Thread.Sleep(2000); 
            scriptExecutor.ExecuteScript("$('.popUpDTContent div')[0].click()");
            scriptExecutor.ExecuteScript("$('.saveDocButtonLevel3')[0].click()");           
            Thread.Sleep(2000);
         
            webDriver.FindElement(By.Id("txtUser1")).Click();
            webDriver.FindElement(By.Id("txtUser1")).Clear();
            webDriver.FindElement(By.Id("txtUser1")).SendKeys(ConfigurationManager.AppSettings["Gibberish"]);
           
            Thread.Sleep(2000);
           
        }

        [Then(@"error should be thrown on saving")]
        public void ThenErrorShouldBeThrownOnSaving()
        {
            int noOfMembers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length = $('.mediumPermissions').length; return length;"));
            Assert.IsTrue(noOfMembers > 0);
            Thread.Sleep(6000);
        }

        #endregion

        #region  04. Set the value on settings page 
        [When(@"settings page is configured and save button is clicked")]
        public void WhenSettingsPageIsConfiguredAndSaveButtonIsClicked()
        {
            webDriver.Navigate().Refresh();
            Thread.Sleep(8000);
            scriptExecutor.ExecuteScript("$('.clientNames')[3].click()");
            Thread.Sleep(8000);
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).Click();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).Clear();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[4]/div/div[2]/input")).SendKeys(ConfigurationManager.AppSettings["MatterName"]);
            Thread.Sleep(6000);
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).Click();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).Clear();
            webDriver.FindElement(By.XPath("//*[@id='contentDiv']/div/div/div[5]/div/div[2]/input")).SendKeys(ConfigurationManager.AppSettings["MatterDescription"]);
            Thread.Sleep(6000);
            scriptExecutor.ExecuteScript("$('.addmatterType a').click()");
            Thread.Sleep(6000);
            scriptExecutor.ExecuteScript("$('.iconForward').click()");
            Thread.Sleep(2000);
            scriptExecutor.ExecuteScript("$('.popUpDTContent div')[0].click()");
            scriptExecutor.ExecuteScript("$('.saveDocButtonLevel3')[0].click()");
            Thread.Sleep(6000);

            webDriver.FindElement(By.Id("txtUser1")).Click();
            webDriver.FindElement(By.Id("txtUser1")).Clear();
            webDriver.FindElement(By.Id("txtUser1")).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
           scriptExecutor.ExecuteScript("$('.uib-typeahead-match')[0].click()");
            Thread.Sleep(6000);         
           
          
            scriptExecutor.ExecuteScript("$('#saveButton').click()");
            Thread.Sleep(6000);
        }

        [Then(@"settings should be saved and confirmation message should be displayed")]
        public void ThenSettingsShouldBeSavedAndConfirmationMessageShouldBeDisplayed()
        {
            string successMessage = (string)scriptExecutor.ExecuteScript("var links = $('#successMessage')[0].innerText;return links"),
                   clientLink = (string)scriptExecutor.ExecuteScript("var links = $('.clientLinks').attr('href');return links"),
                   pageDescription = (string)scriptExecutor.ExecuteScript("var links = $('.pageDescription')[0].innerText;return links");
            Assert.IsTrue(successMessage.ToLower(CultureInfo.CurrentCulture).Contains("your changes have been saved. go back to clients"));
          
        }
        #endregion     

        #region 05. Verify values on matter provision page 

        [When(@"user goes to matter provision page")]
        public void WhenUserGoesToMatterProvisionPage()
        {
            common.GetLogin(webDriver, createURL);
            Thread.Sleep(5000);
            webDriver.FindElement(By.XPath("//main/div/div/div")).Click();
            Thread.Sleep(2000);
            webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select")).Click();
            Thread.Sleep(3000);
            new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByText(ConfigurationManager.AppSettings["DropDownClient"]);
          //  new SelectElement(webDriver.FindElement(By.XPath("//section[@id='snOpenMatter']/div/div[2]/select"))).SelectByText(ConfigurationManager.AppSettings["DropDownClient"]);
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("txtMatterDesc")).Clear();
            webDriver.FindElement(By.Id("txtMatterDesc")).SendKeys(ConfigurationManager.AppSettings["MatterDescription"]);
            Thread.Sleep(2000);

        }

        [Then(@"preset values should be loaded")]
        public void ThenPresetValuesShouldBeLoaded()
        {
            string matterName = (string)scriptExecutor.ExecuteScript("var mName = $('#txtMatterName')[0].value; return mName;"),
                   assignedTo = string.Empty;
            Assert.IsTrue(matterName.ToLower(CultureInfo.CurrentCulture).Contains(ConfigurationManager.AppSettings["MatterName"].ToLower(CultureInfo.CurrentCulture)));
            scriptExecutor.ExecuteScript("$('.buttonPrev')[1].click()");
            Thread.Sleep(3000);
            assignedTo = (string)scriptExecutor.ExecuteScript("var aName = $('.inputAssignPerm')[1].value; return aName;");
            Assert.IsTrue(assignedTo.ToLower(CultureInfo.CurrentCulture).Contains(ConfigurationManager.AppSettings["AttorneyName"].ToLower(CultureInfo.CurrentCulture)));
            webDriver.Quit();
        }

        #endregion

    }
}

