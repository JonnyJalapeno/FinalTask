using Core.Extensions;
using OpenQA.Selenium;

namespace Business
{
    public class LoginPage : BasePage
    {
        private readonly By userName = By.CssSelector("#user-name");
        private readonly By passWord = By.CssSelector("#password");
        private readonly By loginButton = By.CssSelector("#login-button");
        private readonly By errorMessageBox = By.CssSelector(".error-message-container");
        private readonly string urlPart = "saucedemo.com";

        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        public override bool IsLoaded()
        {
            try
            {
                wait.Until(d => d.Url.Contains(urlPart));
                WaitForElement(userName);
                WaitForElement(passWord);
                WaitForElement(loginButton);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public LoginPage EnterUsername(string username)
        {
            WaitForElement(userName).SendKeys(username);
            return this;
        }

        public LoginPage ClearUsername()
        {
            WaitForElement(userName).ClearElement();
            return this;
        }

        public LoginPage EnterPassword(string password)
        {
            WaitForElement(passWord).SendKeys(password);
            return this;
        }

        public LoginPage ClearPassword()
        {
            WaitForElement(passWord).ClearElement();
            return this;
        }

        public LoginPage EnterAllCredentials(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            return this;
        }

        public LoginPage ClearAllCredentials()
        {
            ClearUsername();
            ClearPassword();
            return this;
        }

        public void ClickLogin()
        {
            WaitForElement(loginButton).Click();
        }

        public string GetErrorMessage()
        {
            return WaitForElement(errorMessageBox).Text;
        }
    }
}
