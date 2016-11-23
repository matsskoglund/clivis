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
            if (code != null)
            {
                nibe.CodeFilePath = "code.txt";
                nibe.code = code;
            }          
            return new EmptyResult();
        }

 
        // /api/climate/Netatmo
        // /api/climate/Nibe
        [HttpGet("{source}")]
        public ClimateItem GetById(string source, string clientId, string clientSecret, string redirect_uri, string username, string password)            
        {
            AppKeyConfig configs = new AppKeyConfig() {ClientId = clientId, ClientSecret=clientSecret,UserName = username, Password = password, RedirectURI = redirect_uri };
                
            ClimateItem item = null;
            if (source.Equals("NibeLogin"))
                nibe.init(configs);
            if (source.Equals("Nibe") || source.Equals("NibeLogin"))
            {
                try
                {
                    item = nibe.CurrentReading(configs);
                }
                catch (Exception)
                {
                    return new ClimateItem() { IndoorValue = null, OutdoorValue = null, TimeStamp = DateTime.Now };
                }
            }
            if (source.Equals("Netatmo"))
            {                                    
                item = netatmo.CurrentReading(configs);
            }
            if(source.Equals("Reading"))
            {
                ClimateItem netatmoItem = netatmo.CurrentReading(configs);
                ClimateItem nibeItem = null;
                try
                {
                    nibeItem = nibe.CurrentReading(configs);
                }catch(Exception)
                {
                    nibeItem = null;
                }

                item = ClimateItem.ClimateMeanValues(netatmoItem, nibeItem);

            }

            return item;
        }


        [HttpGet("[controller]/[action]")] // Matches '/Climate/Latest'
        public IActionResult Latest()
        {
            return new EmptyResult();
        }


    }
}