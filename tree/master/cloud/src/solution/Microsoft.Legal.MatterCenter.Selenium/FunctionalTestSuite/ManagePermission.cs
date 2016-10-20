// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="ManagePermission.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform verification of manage permissions page </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using OpenQA.Selenium;
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using TechTalk.SpecFlow;
    using VisualStudio.TestTools.UnitTesting;

    [Binding]
    public class ManagePermission
    {
        string URL = HttpUtility.UrlDecode(ConfigurationManager.AppSettings["ManagePermissionPage"]);
        static IWebDriver webDriver = CommonHelperFunction.GetDriver();
        IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
        static int existingUsers = 0;
        CultureInfo culture = Thread.CurrentThread.CurrentCulture;
        CommonHelperFunction common = new CommonHelperFunction();

        #region 01. Open the browser and load manage permission page
        [When(@"user enters credentials on manage permissions page")]
        public void WhenUserEntersCredentialsOnManagePermissionsPage()
        {
            common.GetLogin(webDriver, URL);
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
        }

        [Then(@"manage permission page should be loaded with default permission")]
        public void ThenManagePermissionPageShouldBeLoadedWithDefaultPermission()
        {
            existingUsers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"),culture);
            Assert.IsTrue(0 < existingUsers);
        }
        #endregion

        #region 02. Verify addition of new Attorney
        [When(@"user adds new Attorney to the matter")]
        public void WhenUserAddsNewAttorneyToTheMatter()
        {
            scriptExecutor.ExecuteScript("$('#addMorePermissions').click()");
            webDriver.FindElement(By.Id("txtAssign" + (existingUsers + 1))).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
            scriptExecutor.ExecuteScript("$('.ui-menu-item')[0].click()");
            scriptExecutor.ExecuteScript("$('#ddlRoleAssign" + (existingUsers + 1) + "').val('Responsible Attorney')");
            scriptExecutor.ExecuteScript("$('#ddlPermAssign" + (existingUsers + 1) + "').val('Full Control')");
        }

        [Then(@"Attorney should be added in the matter")]
        public void ThenAttorneyShouldBeAddedInTheMatter()
        {
            int newUser = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"),culture);
            Assert.IsTrue(existingUsers + 1 == newUser);
        }
        #endregion

        #region 03. Verify newly added Attorney
        [When(@"user clicks on save button on manage permission page")]
        public void WhenUserClicksOnSaveButtonOnManagePermissionPage()
        {
            scriptExecutor.ExecuteScript("$('#btnSave').click()");
            Thread.Sleep(15000);
        }

        [Then(@"updated Attorney should be added in the matter")]
        public void ThenUpdatedAttorneyShouldBeAddedInTheMatter()
        {
            int newUser = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"), culture);
            Assert.IsTrue(existingUsers + 1 == newUser);
        }
        #endregion

        #region 04. Verify error on adding non-existing attorney

        [When(@"user adds non-existing Attorney to the matter")]
        public void WhenUserAddsNon_ExistingAttorneyToTheMatter()
        {
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
            existingUsers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"), culture);
            scriptExecutor.ExecuteScript("$('#addMorePermissions').click()");
            webDriver.FindElement(By.Id("txtAssign" + (existingUsers + 1))).SendKeys(ConfigurationManager.AppSettings["Gibberish"]);
            scriptExecutor.ExecuteScript("$('.ui-menu-item')[0].click()");
            scriptExecutor.ExecuteScript("$('#ddlRoleAssign" + (existingUsers + 1) + "').val('Responsible Attorney')");
            scriptExecutor.ExecuteScript("$('#ddlPermAssign" + (existingUsers + 1) + "').val('Full Control')");
            scriptExecutor.ExecuteScript("$('#btnSave').click()");
            Thread.Sleep(15000);
        }

        [Then(@"Attorney should not be added")]
        public void ThenAttorneyShouldNotBeAdded()
        {
            string incorrectInputs = (string)scriptExecutor.ExecuteScript("var error = $('.errorPopup .popUpFloatRight')[0].innerText; return error;");
            Assert.IsTrue(incorrectInputs.ToLower(CultureInfo.CurrentCulture).Contains("incorrect inputs"));
        }

        #endregion
    }
}
