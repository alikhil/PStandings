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

            // Create service provider
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var parser = ServiceProvider.GetService<Parser>();
            parser.Start();
            LoggerFabric?.Dispose();
        }
        
         private static void ConfigureServices(IServiceCollection services)
        {

            // Build configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddDbContext<PcmsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("StandingsConnectionString"))
                        .EnableSensitiveDataLogging());


            // Add logging
            LoggerFabric = new LoggerFactory()
                .AddConsole((LogLevel)Enum.Parse(typeof(LogLevel), Configuration["Logging:LogLevel:Default"]));
                
            // serviceCollection.AddSingleton(new LoggerFactory()
                // .AddConsole((LogLevel)Enum.Parse(typeof(LogLevel), Configuration["Logging:LogLevel:Default"])));
            services.AddLogging();
            
            //EmailConnectionInfo emailConnection = new EmailConnectionInfo();
            //emailConnection.FromEmail = configuration["Email:FromEmail"].ToString();
            //emailConnection.ToEmail = configuration["Email:ToEmail"].ToString();
            //emailConnection.MailServer = configuration["Email:MailServer"].ToString();
            //emailConnection.NetworkCredentials = new NetworkCredential(configuration["Email:UserName"].ToString(), configuration["Email:Password"].ToString());
            //emailConnection.Port = int.Parse(configuration["Email:Port"]);

            // // Initialize serilog logger
            // Log.Logger = new LoggerConfiguration()
            //      //.WriteTo.MSSqlServer(configuration.GetConnectionString("LoggingSQLServer"), "logs")
            //      //.WriteTo.Email(emailConnection, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, mailSubject: "Azure Backup Error")
            //      .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
            //      .MinimumLevel.Debug()
            //      .Enrich.FromLogContext()
            //      .CreateLogger();

            // Add access to generic IConfigurationRoot
            services.AddSingleton<IConfigurationRoot>(Configuration);

            // Add services
            // serviceCollection.AddTransient<IBackupService, BackupService>();

            // Add app
            services.AddTransient<Parser>();
        }
    }
}
