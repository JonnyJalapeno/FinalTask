using Business;
using Core;
using Core.Config;
using FluentAssertions;
using log4net;
using OpenQA.Selenium;
using Reqnroll;
using Tests.Helpers;

namespace Tests.Steps
{
    [Binding]
    public class LoginSteps
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginSteps));

        private readonly FeatureContext featureContext;
        private LoginPage? loginPage;

        public LoginSteps(FeatureContext featureContext)
        {
            this.featureContext = featureContext;
        }

        private LoginPage LoginPage
        {
            get
            {
                var driver = featureContext.Get<IWebDriver>("Driver");
                if (driver == null) throw new InvalidOperationException("Driver not initialized");
                return loginPage ??= new LoginPage(driver);
            }
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            var browser = BrowserResolver.Resolve(featureContext);

            if (browser == null)
            {
                log.Warn($"[BeforeFeature] Skipping '{featureContext.FeatureInfo.Title}' " +
                         $"— no tag matches configured browsers: " +
                         $"[{string.Join(", ", TestConfig.Instance.Browsers)}]");
                return;
            }

            var driver = DriverFactory
                .GetFactory(browser, TestConfig.Instance.BrowserOptions)
                .CreateDriver();

            featureContext.Set(driver, "Driver");
            log.Info($"[BeforeFeature] '{browser}' driver initialized.");
        }

        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            if (!featureContext.TryGetValue<IWebDriver>("Driver", out var driver))
                return;

            driver.Quit();
            driver.Dispose();
            log.Info($"[AfterFeature] driver quit.");
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            log.Info($"[BeforeScenario] Running scenario on: {BrowserResolver.Resolve(featureContext)}");
        }

        [Given("I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            var driver = featureContext.Get<IWebDriver>("Driver");
            driver.Navigate().GoToUrl(TestConfig.Instance.BaseUrl);
            log.Info("[Step] Given — navigated to login page");
        }

        [When("I enter {string} and {string}")]
        public void WhenIEnterCredentials(string username, string password)
        {
            LoginPage.EnterAllCredentials(username, password);
            log.Info($"[Step] When — entered username: {username}");
        }

        [When("I clear all credentials")]
        public void WhenIClearAllCredentials()
        {
            LoginPage.ClearAllCredentials();
            log.Info("[Step] When — cleared all credentials");
        }

        [When("I clear the password")]
        public void WhenIClearThePassword()
        {
            LoginPage.ClearPassword();
            log.Info("[Step] When — cleared password");
        }

        [When("I click login")]
        public void WhenIClickLogin()
        {
            LoginPage.ClickLogin();
            log.Info("[Step] When — clicked login");
        }

        [Then("I should see error containing {string}")]
        public void ThenIShouldSeeErrorContaining(string expectedMessage)
        {
            var error = LoginPage.GetErrorMessage();
            log.Info($"[Step] Then — error message: '{error}'");
            error.Should().Contain(expectedMessage);
        }

        [Then("I should be redirected to inventory page")]
        public void ThenIShouldBeRedirectedToInventoryPage()
        {
            var driver = featureContext.Get<IWebDriver>("Driver");
            var inventoryPage = new InventoryPage(driver);
            inventoryPage.IsLoaded().Should().BeTrue();
            log.Info($"[Step] Then — redirected to: {driver.Url}");
        }
    }
}