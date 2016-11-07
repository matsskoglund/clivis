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
        

        // Repository for the Climate Items
        public IClimateRepository ClimateItems { get; set; }

          public ClimateController(IClimateRepository climateItems, IOptions<AppKeyConfig> configs)
        {
            AppConfigs = configs.Value;            
            ClimateItems = climateItems;
        }
        

        [HttpGet]
        public ClimateItem GetClimate()
        {

            ClimateItem item = new ClimateItem() { IndoorValue = null, OutdoorValue = null, TimeStamp = DateTime.Now };


            return item;
        }
        

        [HttpPost("Netatmo")]
        public ClimateItem GetClimateWithLogin([FromBody] AppKeyConfig configs)
        {
            ClimateItem item = ClimateItems.Latest(configs);
            return item;
        }

        private ClimateItem GetNetatmoValues()
        {
            return null;
        }

        [HttpGet("{source}", Name = "GetClimate")]
        public IActionResult GetById(string source)
        {
            NibeUnit nibe = new NibeUnit();
            nibe.init(AppConfigs);
            ClimateItem item = new ClimateItem() { TimeStamp = DateTime.Now, IndoorValue = "22.5", OutdoorValue = "6.4" };
            return new ObjectResult(item);
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