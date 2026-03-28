using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Core
{
    public class ChromeDriverFactory : IAbstractDriverFactory
    {
        private readonly ChromeOptions options;

        public ChromeDriverFactory(ChromeOptions? options = null)
        {
            this.options = options ?? new ChromeOptions();
        }

        public IWebDriver CreateDriver()
        {
            try
            {
                return new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                throw new WebDriverException($"Failed to create ChromeDriver: {ex.Message}", ex);
            }
        }
    }
}
