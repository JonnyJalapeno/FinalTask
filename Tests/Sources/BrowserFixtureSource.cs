using Core.Config;


namespace Tests.Sources
{
    public static class BrowserFixtureSource
    {
        public static IEnumerable<string> Browsers =>
            TestConfig.Instance.Browsers;
    }

}
