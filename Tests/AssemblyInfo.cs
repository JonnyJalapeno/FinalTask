using log4net.Config;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]