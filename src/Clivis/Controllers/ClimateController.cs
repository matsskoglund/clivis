using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using Clivis.Models.Netatmo;
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
        //private AppKeyConfig AppConfigs = new AppKeyConfig();

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

        private void UpdateNetatmo(AppKeyConfig configs)
        {
            IClimateSource netatmo = new NetatmoUnit();
            netatmo.init(AppConfigs);
            string indoor = netatmo.inDoorTemperature;
            string outdoor = netatmo.outDoorTemperature;
        }

        [HttpPost("Netatmo")]
        public ClimateItem GetClimateWithLogin([FromBody] AppKeyConfig configs)
        {
            ClimateItem item = new ClimateItem() { IndoorValue = null, OutdoorValue = null, TimeStamp = DateTime.Now };
            UpdateNetatmo(configs);

            return item;
        }

        [HttpGet("{source}", Name = "GetClimate")]
        public IActionResult GetById(string source)
        {
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