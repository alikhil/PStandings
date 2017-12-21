using System;
using Standings.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

using Standings.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Standings.Parser
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        public static ILoggerFactory LoggerFabric;

        public static IServiceProvider ServiceProvider;

        static void Main(string[] args)
        {
            var m = new MyModel();
            
            Console.WriteLine(m.Name);
            Console.WriteLine("Hello World!");

            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var parser = ServiceProvider.GetService<Parser>();
            parser.Start();
            LoggerFabric?.Dispose();
        }
        
         private static void ConfigureServices(IServiceCollection services)
        {

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddDbContext<PcmsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("StandingsConnectionString"))
                        .EnableSensitiveDataLogging());


            LoggerFabric = new LoggerFactory()
                .AddConsole((LogLevel)Enum.Parse(typeof(LogLevel), Configuration["Logging:LogLevel:Default"]));
                
            services.AddLogging();
            
            services.AddSingleton<IConfigurationRoot>(Configuration);

            services.AddTransient<Parser>();
        }
    }
}
