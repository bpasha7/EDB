using DBMS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Settings;
using NLog;
using System;
//using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace Program
{
    public static class Program
    {
        static public ServiceCollection _serviceCollection;
        static public ServiceProvider _serviceProvider;
        static public DatabaseManagmentSystem Domain
        {
            get
            {
#if DEBUG
                Build();
#endif
                return _serviceProvider.GetService<DatabaseManagmentSystem>();
            }
        }
        static void Main(string[] args)
        {

            Build();
            //Run
            _serviceProvider.GetService<DatabaseManagmentSystem>().Run();
        }
        //
        public static void Build()
        {
            _serviceCollection = new ServiceCollection();

            ConfigureServices(_serviceCollection);

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        static public void ConfigureServices(IServiceCollection serviceCollection)
        {

            //// add services
            //serviceCollection.AddTransient<ITestService, TestService>();

            var path = System.IO.Directory.GetCurrentDirectory();

#if DEBUG
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString() + "\\EDB";
#endif

            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.Configure<SystemSettings>(configuration);

            //var settings = new SystemSettings();
            //configuration.Bind(settings);

            // add app
            serviceCollection.AddTransient<DatabaseManagmentSystem>();

            serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            //serviceCollection.AddLogging((builder) => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
        }
    }
}
