using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using Clivis.Models.Netatmo;
using Clivis.Models.Nibe;
using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Clivis.Controllers
{
    
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        public AppKeyConfig AppConfigs { get; }

        public IClimateSource nibe { get; }
        public IClimateSource netatmo { get; }

        public ClimateController(IOptions<AppKeyConfig> configs, IDictionary<string, IClimateSource> climateSources)
        {

            AppConfigs = configs.Value;                      
            nibe = climateSources["Nibe"];
            netatmo = climateSources["Netatmo"];
        }

        // /api/climate
        [HttpGet]
        public IActionResult GetClimate([FromQuery] string code, [FromQuery] string state)
        {
            nibe.CodeFilePath = "code.txt";
            nibe.code = code;            
            return new EmptyResult();
        }

        // /api/climate/{source}/start
   /*     [HttpGet("{source}")]
        public IActionResult Index(string source)
        {
            return new EmptyResult();
        }
        */
        // /api/Netatmo
        // /api/Nibe
        [HttpGet("{source}")]
        public ClimateItem GetById(string source, string clientId, string clientSecret, string redirect_uri, string username, string password)            
        {
            AppKeyConfig configs = new AppKeyConfig() {ClientId = clientId, ClientSecret=clientSecret,UserName = username, Password = password, RedirectURI = redirect_uri };
                
            ClimateItem item = null;
            if (source.Equals("NibeLogin"))
                nibe.init(configs);
            if (source.Equals("Nibe") || source.Equals("NibeLogin"))
            {
                item = nibe.CurrentReading(configs);
            }
            if (source.Equals("Netatmo"))
            {                                    
                item = netatmo.CurrentReading(configs);
            }

            return item;
        }     
    }
}