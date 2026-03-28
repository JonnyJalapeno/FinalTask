using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Config
{
    public sealed class TestConfig
    {
        private static readonly Lazy<TestConfig> lazy =
            new(() => LoadConfig());

        public static TestConfig Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public string BaseUrl { get; }
        public string[] Browsers { get; }
        public string[] BrowserOptions { get; }
        public int DefaultTimeoutSeconds { get; }

        [JsonConstructor]
        private TestConfig(
            string BaseUrl,
            string[] Browsers,
            string[] BrowserOptions,
            int DefaultTimeoutSeconds)
        {
            this.BaseUrl = BaseUrl ?? string.Empty;
            this.Browsers = Browsers ?? Array.Empty<string>();
            this.BrowserOptions = BrowserOptions ?? Array.Empty<string>();
            this.DefaultTimeoutSeconds = DefaultTimeoutSeconds;
        }

        private static TestConfig LoadConfig()
        {
            var fileName = Environment.GetEnvironmentVariable("TEST_CONFIG_FILE")
                           ?? "testconfig.json";

            var path = Path.Combine(AppContext.BaseDirectory, fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Config file not found at: {path}");
            }

            var json = File.ReadAllText(path);

            var config = JsonSerializer.Deserialize<TestConfig>(json);

            if (config == null)
            {
                throw new InvalidOperationException("Failed to deserialize testconfig.json");
            }

            return config;
        }
    }
}