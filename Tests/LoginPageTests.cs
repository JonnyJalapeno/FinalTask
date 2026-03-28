using Business;
using Core;
using Core.Config;
using FluentAssertions;
using log4net;
using OpenQA.Selenium;
using Tests.Sources;

namespace Tests
{
    [TestFixtureSource(typeof(BrowserFixtureSource), nameof(BrowserFixtureSource.Browsers))]
    [Parallelizable(ParallelScope.Fixtures)]
    public class LoginPageTests
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginPageTests));
        private readonly string browser;
        private readonly IWebDriver driver;
        private LoginPage loginPage = null!;

        public LoginPageTests(string browser)
        {
            this.browser = browser;
            log.Info($"Initializing {browser} driver");
            driver = DriverFactory.GetFactory(browser, TestConfig.Instance.BrowserOptions).CreateDriver();
            log.Info($"{browser} driver initialized successfully");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            log.Info($"Quitting {browser} driver");
            driver.Quit();
            driver.Dispose();
            log.Info($"{browser} driver quit successfully");
        }

        [SetUp]
        public void SetUp()
        {
            log.Info($"Navigating to saucedemo.com on {browser}");
            driver.Navigate().GoToUrl(TestConfig.Instance.BaseUrl);
            loginPage = new LoginPage(driver);
            log.Info("LoginPage initialized");
        }

        [Test, TestCaseSource(typeof(CredentialsSource), nameof(CredentialsSource.InvalidCredentials))]
        public void UC1_LoginWithEmptyCredentials(string username, string password)
        {
            log.Info($"UC1 — LoginWithEmptyCredentials username: {username}, password: {password}");

            loginPage.EnterAllCredentials(username, password)
                .ClearAllCredentials()
                .ClickLogin();
            var errorMessage = loginPage.GetErrorMessage();

            log.Info($"UC1 — Error message received: {errorMessage}");
            errorMessage.Should().Contain("Username is required");
        }

        [Test, TestCaseSource(typeof(CredentialsSource), nameof(CredentialsSource.InvalidCredentials))]
        public void UC2_LoginWithUsernameOnly(string username, string password)
        {
            log.Info($"UC2 — LoginWithUsernameOnly username: {username}, password: {password}");

            loginPage.EnterAllCredentials(username, password)
                .ClearPassword()
                .ClickLogin();
            var errorMessage = loginPage.GetErrorMessage();

            log.Info($"UC2 — Error message received: {errorMessage}");
            errorMessage.Should().Contain("Password is required");
        }

        [Test, TestCaseSource(typeof(CredentialsSource), nameof(CredentialsSource.ValidCredentials))]
        public void UC3_LoginWithValidCredentials(string username, string password)
        {
            log.Info($"UC3 — LoginWithValidCredentials username: {username}, password: {password}");

            loginPage.EnterAllCredentials(username, password)
                .ClickLogin();

            InventoryPage inventoryPage = new InventoryPage(driver);
            inventoryPage.IsLoaded().Should().BeTrue();

            log.Info($"UC3 — Successfully logged in, current url: {driver.Url}");
        }
    }
}
