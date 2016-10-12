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
        CommonHelperFunction common = new CommonHelperFunction();
        static int existingUsers = 0;

        #region 01. Open the browser and load manage permission page
        [When(@"user enters credentials on manage permissions page")]
        public void WhenUserWillGiveAnd()
        {
            webDriver.Navigate().GoToUrl(new Uri(URL));
            Thread.Sleep(4000);
        }

        [Then(@"manage permission page should be loaded with default permission")]
        public void ThenMnanagePermissionPageShouldBeLoadedWithDefaultPermission()
        {
            existingUsers = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"));
            Assert.IsTrue(0 < existingUsers);
        }
        #endregion

        #region 02. User will add Attorney to the Matter
        [When(@"user adds new Attorney to the matter")]
        public void WhenUserAddsNewAttroneyToTheMatter()
        {
            scriptExecutor.ExecuteScript("$('#addMorePermissions').click()");
            webDriver.FindElement(By.Id("txtAssign" + (existingUsers + 1))).SendKeys(ConfigurationManager.AppSettings["AttorneyName"]);
            webDriver.FindElement(By.LinkText(ConfigurationManager.AppSettings["AttorneyMember"])).Click();
            scriptExecutor.ExecuteScript("$('#ddlRoleAssign" + (existingUsers + 1) + "').val('Responsible Attorney')");
            scriptExecutor.ExecuteScript("$('#ddlPermAssign" + (existingUsers + 1) + "').val('Full Control')");
        }

        [Then(@"Attorney should be added in the matter")]
        public void ThenAttorneyShouldBeAddedInTheMatter()
        {
            int newUser = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"));
            Assert.IsTrue(existingUsers + 1 == newUser);
        }
        #endregion

        #region 03. User will save updated Attorney to the matter
        [When(@"user clicks on save button on manage permission page")]
        public void WhenUserWillClickOnSaveButtonOnManagePermissionPage()
        {
            scriptExecutor.ExecuteScript("$('#btnSave').click()");
            Thread.Sleep(15000);
        }

        [Then(@"updated Attorney should be added in the matter")]
        public void ThenUpdatedAttorneyShouldBeAddedInTheMatter()
        {
            int newUser = Convert.ToInt32(scriptExecutor.ExecuteScript("var length =$('.assignNewPermission').length;return length;"));
            Assert.IsTrue(existingUsers + 1 == newUser);
        }
        #endregion
    }
}
