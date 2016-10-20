// ****************************************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Selenium
// Author           : MAQ Software
// Created          : 11-09-2016
//
// ***********************************************************************
// <copyright file="Common.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used to perform common action using Selenium driver </summary>
// ****************************************************************************************

namespace Microsoft.Legal.MatterCenter.Selenium
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.IE;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Threading;

    public enum Selector { Id, Class, CssSelector, Xpath, LinkText };

    /// <summary>
    /// This class is used to perform common action using Selenium driver
    /// </summary>
    public class CommonHelperFunction
    {
        static IWebDriver webDriver = new InternetExplorerDriver();
        NameValueCollection cred = ConfigurationManager.GetSection("credSettings") as NameValueCollection;

        /// <summary>
        /// This method is used to perform authentication
        /// </summary>
        /// <param name="webDriver">Selenium driver object</param>
        public void Authenticate(IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver");
            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)webDriver;
            webDriver.FindElement(By.Id("cred_userid_inputtext")).Click();
            webDriver.FindElement(By.Id("cred_userid_inputtext")).Clear();
            scriptExecutor.ExecuteScript("document.getElementById('cred_userid_inputtext').value='"+ cred["UserName"]+"'");
            Thread.Sleep(5000);
            webDriver.FindElement(By.Id("cred_password_inputtext")).Click();
            webDriver.FindElement(By.Id("cred_password_inputtext")).Clear();
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("document.getElementById('cred_password_inputtext').setAttribute('value','"+ cred["Password"] + "')");
            Thread.Sleep(3000);
            scriptExecutor.ExecuteScript("document.getElementById('cred_keep_me_signed_in_checkbox').checked = true;");
            Thread.Sleep(2000);
            webDriver.FindElement(By.Id("cred_sign_in_button")).Click();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This method is used to check whether element is present or not
        /// </summary>
        /// <param name="webDriver">Selenium driver object</param>
        /// <param name="elementName">Element name</param>
        /// <param name="elementType">Element type</param>
        /// <returns>Boolean value based on elements present</returns>
        public bool ElementPresent(IWebDriver webDriver, string elementName, Selector elementType)
        {

            if (webDriver == null)
                throw new ArgumentNullException("webDriver");
            try
            {
                if (elementType == Selector.Id)
                {
                    webDriver.FindElement(By.Id(elementName));
                }
                else if (elementType == Selector.Class)
                {
                    webDriver.FindElement(By.ClassName(elementName));
                }
                else if (elementType == Selector.CssSelector)
                {
                    webDriver.FindElement(By.CssSelector(elementName));
                }
                else if (elementType == Selector.Xpath)
                {
                    webDriver.FindElement(By.XPath(elementName));
                }
                else if (elementType == Selector.LinkText)
                {
                    webDriver.FindElement(By.LinkText(elementName));
                }
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// This method is used to close the browser
        /// </summary>
        /// <param name="webDriver">Selenium driver object</param>
        public void CleanUp(IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver");
            webDriver.Quit();
        }

        /// <summary>
        /// This method is used to return Selenium driver object
        /// </summary>
        /// <returns>web driver object</returns>
        static public IWebDriver GetDriver()
        {
            return webDriver;
        }

        /// <summary>
        /// This method is used to perform SharePoint authentication 
        /// </summary>
        /// <param name="webDriver">Selenium driver object</param>
        /// <param name="URL">URL of page to load</param>
        public void GetLogin(IWebDriver webDriver, string URL)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver");

            webDriver.Navigate().GoToUrl(new Uri(URL));
            if (ElementPresent(webDriver, "use_another_account", Selector.Class))
            {
                webDriver.FindElement(By.ClassName("use_another_account")).Click();
                Authenticate(webDriver);
            }
            else if (ElementPresent(webDriver, "ms-spo-solutionItem", Selector.Class))
            {
                webDriver.FindElement(By.LinkText("Click here to sign in with a different account to this site.")).Click();
                Thread.Sleep(5000);
                webDriver.FindElement(By.ClassName("use_another_account")).Click();
                Authenticate(webDriver);
            }
            else if (ElementPresent(webDriver, "cred_userid_inputtext", 0))
            {
                Authenticate(webDriver);
            }
            Thread.Sleep(5000);
        }
    }
}
