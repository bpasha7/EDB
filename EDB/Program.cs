using DBMS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Settings;
using System;

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
        }
    }

    //public class ProgramTest
    //{
    //    public ServiceCollection _serviceCollection;
    //    public ServiceProvider _serviceProvider;
    //    public DatabaseManagmentSystem Domain
    //    {
    //        get
    //        {
    //            return _serviceProvider.GetService<DatabaseManagmentSystem>();
    //        }
    //    }
    //    public ProgramTest()
    //    {

    //    }
    //    //
    //    private void Build()
    //    {
    //        _serviceCollection = new ServiceCollection();

    //        ConfigureServices(_serviceCollection);

    //        _serviceProvider = _serviceCollection.BuildServiceProvider();
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="serviceCollection"></param>
    //    private void ConfigureServices(IServiceCollection serviceCollection)
    //    {

    //        //// add services
    //        //serviceCollection.AddTransient<ITestService, TestService>();

    //        var builder = new ConfigurationBuilder()
    //            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    //            .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
    //            .AddEnvironmentVariables();

    //        IConfigurationRoot configuration = builder.Build();

    //        serviceCollection.AddOptions();
    //        serviceCollection.Configure<SystemSettings>(configuration);

    //        //var settings = new SystemSettings();
    //        //configuration.Bind(settings);

    //        // add app
    //        serviceCollection.AddTransient<DatabaseManagmentSystem>();
    //    }
    //}

}
