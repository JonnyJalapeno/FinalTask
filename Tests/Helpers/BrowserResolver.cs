using Core.Config;
using Reqnroll;

namespace Tests.Helpers
{
    public static class BrowserResolver
    {
        public static string? Resolve(FeatureContext featureContext)
        {
            var configured = TestConfig.Instance.Browsers
                .Select(b => b.ToLowerInvariant())
                .ToArray();

            return featureContext.FeatureInfo.Tags
                .Select(t => t.ToLowerInvariant())
                .FirstOrDefault(t => configured.Contains(t));
        }
    }
}