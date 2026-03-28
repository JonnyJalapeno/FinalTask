using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;


namespace Core
{
    public static class DriverFactory
    {
        public static IAbstractDriverFactory GetFactory(string browser, string[] args)
            => browser.ToLowerInvariant() switch
            {
                "chrome" => new ChromeDriverFactory(CreateChromeOptions(args)),
                "edge" => new EdgeDriverFactory(CreateEdgeOptions(args)),
                _ => throw new ArgumentOutOfRangeException(nameof(browser), $"Unsupported browser: {browser}")
            };

        private static ChromeOptions CreateChromeOptions(string[] args)
        {
            var options = new ChromeOptions();
            foreach (var arg in args)
            {
                options.AddArgument(arg);
            }
                
            return options;
        }

        private static EdgeOptions CreateEdgeOptions(string[] args)
        {
            var options = new EdgeOptions();
            foreach (var arg in args)
            {
                options.AddArgument(arg);
            }
                
            return options;
        }
    }
}
