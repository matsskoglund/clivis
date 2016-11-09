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

namespace Clivis.Controllers
{
    
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        public AppKeyConfig AppConfigs { get; }

        private IClimateSource nibe;

        // Repository for the Climate Items
        public IClimateRepository ClimateItems { get; set; }

        public ClimateController(IClimateRepository climateItems, IOptions<AppKeyConfig> configs, IClimateSource nibeUnit)
        {
            AppConfigs = configs.Value;            
            ClimateItems = climateItems;
            nibe = nibeUnit;
        }

        // /api/climate
        [HttpGet]
        public ClimateItem GetClimate([FromQuery] string code, [FromQuery] string state)
        {
            nibe.code = code;
            
            ClimateItem item = new ClimateItem() { IndoorValue = null, OutdoorValue = null, TimeStamp = DateTime.Now };


            return item;
        }
        

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
                    item = ClimateItems.Latest(configs);
                }

                return item;

            }     

        // POST api/climate
        [HttpPost]
        public IActionResult Create([FromBody]ClimateItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            ClimateItems.Add(item);
            return CreatedAtRoute("GetClimate", new { timeStamp = item.TimeStamp }, item);
        }
    }
}