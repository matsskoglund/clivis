using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Clivis.Models;
using Clivis.Models.Nibe;
using Clivis.Models.Netatmo;
using System.Collections.Concurrent;

namespace Clivis
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                try
                {
                 builder.AddUserSecrets();                    
                }
                catch (System.Exception)
                {
                    // This need to be fixed
                    
                }
               
                Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppKeyConfig>(configs =>
              {
               //   configs.UserName = Configuration["NetatmoUserName"];
               //   configs.Password = Configuration["NetatmoPassword"];
               //   configs.ClientId = Configuration["NetatmoClientId"];
               //   configs.ClientSecret = Configuration["NetatmoClientSecret"];

              });

            // Add framework services.
            services.AddMvc();


            ConcurrentDictionary<string, IClimateSource> sources = new ConcurrentDictionary<string, IClimateSource>();
            sources["Nibe"] = new NibeUnit();
            sources["Netatmo"] = new NetatmoUnit();

            //services.AddSingleton<IDictionary<string, IClimateSource>, ConcurrentDictionary<string, IClimateSource>> ();  
            services.AddSingleton<IDictionary<string, IClimateSource>> (sources);                        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }

}
