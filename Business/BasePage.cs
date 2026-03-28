using Core.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Business
{
    public abstract class BasePage
    {
        protected readonly IWebDriver driver;
        protected readonly WebDriverWait wait;

        protected BasePage(IWebDriver driver, int? timeoutSeconds = null)
        {
            this.driver = driver;
            int timeout = timeoutSeconds ?? TestConfig.Instance.DefaultTimeoutSeconds;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

        protected IWebElement WaitForElement(By locator)
        {
            try
            {
                return wait.Until(d =>
                {
                    var el = d.FindElements(locator).FirstOrDefault();
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"Element not found, not visible or not enabled: {locator}");
            }
        }

        public abstract bool IsLoaded();
    }
}
