using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Core
{
    public class EdgeDriverFactory : IAbstractDriverFactory
    {
        private readonly EdgeOptions options;

        public EdgeDriverFactory(EdgeOptions? options = null)
        {
            this.options = options ?? new EdgeOptions();
        }

        public IWebDriver CreateDriver()
        {
            try
            {
                return new EdgeDriver(options);
            }
            catch (Exception ex)
            {
                throw new WebDriverException($"Failed to create EdgeDriver: {ex.Message}", ex);
            }
        }
    }
}
