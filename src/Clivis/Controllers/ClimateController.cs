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
        public IEnumerable<ClimateItem> GetAll()
        {
            IClimateSource netatmo = new NetatmoUnit();
            netatmo.init(AppConfigs);
            string indoor = netatmo.inDoorTemperature;
            string outdoor = netatmo.outDoorTemperature;

            ClimateItem item = ClimateItems.Find("Netatmo");
            item.SourceName = indoor;            

            return ClimateItems.GetAll();
        }
        
        [HttpGet("{id}", Name = "GetClimate")]
        public IActionResult GetById(string id)
        {
            var item = ClimateItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
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
            return CreatedAtRoute("GetClimate", new { id = item.Key }, item);
        }
    }
}