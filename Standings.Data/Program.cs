using System;
using Standings.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

using Standings.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Standings.Data
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        public static ILoggerFactory LoggerFabric;

        static void Main(string[] args)
        {

            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();


            // var context = serviceProvider.GetService<PcmsContext>();

            // var contests = context.Contests.Where(c => c.Generation == "GR1");
            // var smth = contests.Select(c => c.Submissions
            //     .GroupBy(s => s.SubmitterId)
            //     .Select(g => new {
            //             StudentId = g.First().SubmitterId,
            //             Solved = g.Count(s => s.IsAccepted)
            //         }));
            // smth.First().First().
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
                options.UseSqlServer(Configuration.GetConnectionString("StandingsConnectionString")));

        }
    }
}
