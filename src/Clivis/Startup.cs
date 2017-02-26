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
                .AddJsonFile("data/secretsenc.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment()) { 
            //        builder.AddUserSecrets(); 
                                      
                }
               
                Configuration = builder.Build();
        
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOptions();
            string key = Configuration["Cliviskey"];
            services.Configure<AppKeyConfig>(configs =>
              {                  
                  configs.UserName = Configuration["NETATMO_USERNAME"];
                  configs.Password = Configuration["NETAMO_PASSWORD"];
                  //configs.NetatmoClientId = Protector.DecryptString(Configuration["NetatmoClientId"], key);
                  configs.NetatmoClientId = Configuration["NETATMO_CLIENTID"];
                  configs.NetatmoClientSecret = Configuration["NETATMO_CLIENTSECRET"];
                  //configs.NibeClientId = Protector.DecryptString(Configuration["NibeClientId"], key);
                  configs.NibeClientId = Configuration["NIBE_ID"];
                  //configs.NibeClientSecret = Protector.DecryptString(Configuration["NibeClientSecret"], key);
                  configs.NibeClientSecret = Configuration["NIBE_SECRET"];
                  //configs.NibeRedirectURI = Configuration["NibeRedirectURI"];
                  configs.NibeRedirectURI = Configuration["NIBE_REDIRECTURL"];
                  configs.NibeHost = Configuration["AppKeys:NibeHost"];
                  configs.NetatmoHost = Configuration["AppKeys:NetatmoHost"];
              });
           

            // Add framework services.
            services.AddMvc();

            ConcurrentDictionary<string, IClimateSource> sources = new ConcurrentDictionary<string, IClimateSource>();
            NibeUnit nu = new NibeUnit();
            nu.encryptionKey = "0123456789012345";
            sources["Nibe"] = nu;
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
            app.UseMvc();
        }
    }

}
