﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Microsoft.Legal.MatterCenter.Selenium.SpecFlow.Specs
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class MatterDashboardPageFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "MatterDashboard.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Matter Dashboard Page", "", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Matter Dashboard Page")))
            {
                Microsoft.Legal.MatterCenter.Selenium.SpecFlow.Specs.MatterDashboardPageFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("01. Open the browser and load Matter Center home page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _01_OpenTheBrowserAndLoadMatterCenterHomePage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("01. Open the browser and load Matter Center home page", new string[] {
                        "E2E"});
#line 4
this.ScenarioSetup(scenarioInfo);
#line 5
 testRunner.When("user enters credentials on matter dashboard page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 6
 testRunner.Then("Matter Center home page should be loaded with element \'mcIcon\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("02. Verify the hamburger menu")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _02_VerifyTheHamburgerMenu()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("02. Verify the hamburger menu", new string[] {
                        "E2E"});
#line 9
this.ScenarioSetup(scenarioInfo);
#line 10
 testRunner.When("user clicks on hamburger menu on Matter Center homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 11
 testRunner.Then("hamburger menu should be loaded", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("05. Verify the matter fly out on Matter Center homepage")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _05_VerifyTheMatterFlyOutOnMatterCenterHomepage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("05. Verify the matter fly out on Matter Center homepage", new string[] {
                        "E2E"});
#line 14
this.ScenarioSetup(scenarioInfo);
#line 15
 testRunner.When("user clicks on matter fly out", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 16
 testRunner.Then("a matter fly out should be seen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("06. Verify the search feature on matter center homepage")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _06_VerifyTheSearchFeatureOnMatterCenterHomepage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("06. Verify the search feature on matter center homepage", new string[] {
                        "E2E"});
#line 19
this.ScenarioSetup(scenarioInfo);
#line 20
 testRunner.When("user types \'test\' in search box on Matter Center Homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 21
 testRunner.Then("all results having \'test\' keyword should be displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("04. Verify the upload button functionality")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _04_VerifyTheUploadButtonFunctionality()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("04. Verify the upload button functionality", new string[] {
                        "E2E"});
#line 24
this.ScenarioSetup(scenarioInfo);
#line 25
 testRunner.When("user clicks on upload button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 26
 testRunner.Then("an upload pop up should be seen", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("03. Verify the pin/unpin functionality")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _03_VerifyThePinUnpinFunctionality()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("03. Verify the pin/unpin functionality", new string[] {
                        "E2E"});
#line 29
this.ScenarioSetup(scenarioInfo);
#line 30
 testRunner.When("user clicks on pin or unpin", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
 testRunner.Then("matter should get pinned or unpinned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("09. Verify the advance filter functionality")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _09_VerifyTheAdvanceFilterFunctionality()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("09. Verify the advance filter functionality", new string[] {
                        "E2E"});
#line 34
this.ScenarioSetup(scenarioInfo);
#line 35
 testRunner.When("user clicks on advance filter", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 36
 testRunner.Then("filter results should be shown to user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("08. Verify the sort functionality in matter center home")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _08_VerifyTheSortFunctionalityInMatterCenterHome()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("08. Verify the sort functionality in matter center home", new string[] {
                        "E2E"});
#line 39
this.ScenarioSetup(scenarioInfo);
#line 40
 testRunner.When("user sorts data for All matters in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 41
 testRunner.Then("all records should be sorted in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 42
 testRunner.When("user sorts data for All matters in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.Then("all records should be sorted in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 45
 testRunner.When("user sorts data for Pinned matters in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 46
 testRunner.Then("all records should be sorted in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 47
 testRunner.When("user sorts data for Pinned matters in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 48
 testRunner.Then("all records should be sorted in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 50
 testRunner.When("user sorts data for My matters in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
 testRunner.Then("all records should be sorted in ascending order", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 52
 testRunner.When("user sorts data for My matters in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 53
 testRunner.Then("all records should be sorted in ascending order of created date", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("10. Verify the footer on matter center home")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _10_VerifyTheFooterOnMatterCenterHome()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("10. Verify the footer on matter center home", new string[] {
                        "E2E"});
#line 56
this.ScenarioSetup(scenarioInfo);
#line 57
 testRunner.When("user navigates to the footer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 58
 testRunner.Then("footer should have all the links", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("07. Verify the search feature using managed properties on matter center home page" +
            "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _07_VerifyTheSearchFeatureUsingManagedPropertiesOnMatterCenterHomePage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("07. Verify the search feature using managed properties on matter center home page" +
                    "", new string[] {
                        "E2E"});
#line 61
this.ScenarioSetup(scenarioInfo);
#line 62
 testRunner.When("user types \'MCMatterName:Test\' in search box on Matter Center Homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 63
 testRunner.Then("all results having \'Test\' keyword should be displayed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("12. Verify enterprise search feature on matter center home page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _12_VerifyEnterpriseSearchFeatureOnMatterCenterHomePage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("12. Verify enterprise search feature on matter center home page", new string[] {
                        "E2E"});
#line 66
this.ScenarioSetup(scenarioInfo);
#line 67
 testRunner.When("user types \'Test\' in enterprise search box on Matter Center Home page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 68
 testRunner.Then("user should redirect to enterprise page with search results for \'Test\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("11. Verify no results on invalid search")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Matter Dashboard Page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("E2E")]
        public virtual void _11_VerifyNoResultsOnInvalidSearch()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("11. Verify no results on invalid search", new string[] {
                        "E2E"});
#line 71
this.ScenarioSetup(scenarioInfo);
#line 72
 testRunner.When("user types gibberish in search box on Matter Center dashboard", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 73
 testRunner.Then("no results should be displayed on Matter Center dashboard", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
