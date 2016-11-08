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

        private NibeUnit nibe = new NibeUnit();

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

        [HttpPost("Nibe")]
        public ClimateItem GeNibeWithLogin([FromBody] AppKeyConfig configs)
        {
           // nibe.code = code;

            nibe.init(configs);

            ClimateItem latest = nibe.latestReading(configs);

            return latest;
        }

        private ClimateItem GetNetatmoValues()
        {
            return null;
        }

        [HttpGet("{source}", Name = "GetClimate")]
        public IActionResult GetById(string source)
        {
            
            if (source.Equals("NibeLogin"))
            {
//                NibeUnit nibe = new NibeUnit();
                nibe.init(AppConfigs);
                nibe.latestReading(AppConfigs);

            }
            if (source.Equals("NibeAuth"))
            {
  //              NibeUnit nibe = new NibeUnit();
                nibe.init(AppConfigs);
                nibe.latestReading(AppConfigs);

            }
            ClimateItem item = new ClimateItem() { TimeStamp = DateTime.Now, IndoorValue = "22.5", OutdoorValue = "6.4" };
                return new ObjectResult(item);
        }

   /*     [HttpGet("{code}", Name = "NibeLogin")]
       public IActionResult NibeLogin(string code, [FromQuery]string state)
        {
           
            
     //       NibeUnit nibe = new NibeUnit();
            nibe.code = code;

            nibe.init(AppConfigs);

            ClimateItem latest = nibe.latestReading(AppConfigs);
            
            return new ObjectResult(latest);
        }*/

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