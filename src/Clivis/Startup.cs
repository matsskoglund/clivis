using System.Collections.Generic;
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
            if (env.IsDevelopment()) { 
                       
            }
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOptions();
            services.AddCors();
            services.Configure<AppKeyConfig>(configs =>
              {                  
                  configs.UserName = Configuration["NETATMO_USERNAME"];
                  configs.Password = Configuration["NETATMO_PASSWORD"];
                  configs.NetatmoClientId = Configuration["NETATMO_CLIENTID"];
                  configs.NetatmoClientSecret = Configuration["NETATMO_CLIENTSECRET"];
                  configs.NibeClientId = Configuration["NIBE_CLIENTID"];
                  configs.NibeClientSecret = Configuration["NIBE_CLIENTSECRET"];
                  configs.NibeRedirectURI = Configuration["NIBE_REDIRECTURL"];
                  configs.NibeHost = Configuration["NIBE_HOST"];
                  configs.NetatmoHost = Configuration["NETATMO_HOST"];
                  configs.BuildVersion = Configuration["BUILD_VERSION"];
              });
           

            // Add framework services.
            services.AddMvc();

            ConcurrentDictionary<string, IClimateSource> sources = new ConcurrentDictionary<string, IClimateSource>();
            //NibeUnit nibe = new NibeUnit() { CodeFilePath = "data/code.txt"};
            
            sources["Nibe"] = new NibeUnit() { CodeFilePath = "data/code.txt" };
            sources["Netatmo"] = new NetatmoUnit();           
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
            app.UseCors(builder =>
                 builder.WithOrigins("*")
                 .AllowAnyHeader());

            app.UseMvc();
        }
    }

}
