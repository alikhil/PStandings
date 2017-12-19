using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Standings.Repositories;
using Standings.Data.Contexts;

namespace standings
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IContestRepository, ContestRepository>();
            services.AddTransient<ISubmissionRepository, SubmissionRepository>();
            services.AddDbContext<PcmsContext>(options => options
                            .UseSqlServer(Configuration.GetConnectionString("StandingsConnectionString"))
                            .EnableSensitiveDataLogging());
            services.AddResponseCaching();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseResponseCaching();

            // app.Run(async (context) =>
            // {
            //     context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            //     {
            //         Public = true,
            //         MaxAge = TimeSpan.FromSeconds(30)
            //     };
            //     context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };

            //     await context.Response.WriteAsync($"Hello World! {DateTime.UtcNow}");
            // });
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
