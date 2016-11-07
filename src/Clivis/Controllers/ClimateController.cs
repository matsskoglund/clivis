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
            var queryString = Request.QueryString;
            if(source.Equals("Nibe"))
            {
                var pairs = new List<KeyValuePair<string, string>>
                {
                        new KeyValuePair<string, string>("grant_type", "password" ),
                        new KeyValuePair<string, string>("client_id", AppConfigs.NetatmoClientId),
                        new KeyValuePair<string, string>( "client_secret", AppConfigs.NetatmoClientSecret),
                        new KeyValuePair<string, string>("username", AppConfigs.NetatmoUserName),
                        new KeyValuePair<string, string>( "password", AppConfigs.NetatmoPassword),
                        new KeyValuePair<string, string>( "scope", "read_station")
                };
                HttpClient client = new HttpClient();
                var outcontent = new FormUrlEncodedContent(pairs);
                var response = client.PostAsync("https://api.netatmo.net/oauth2/token", outcontent).Result;

                string contentResult = response.Content.ReadAsStringAsync().Result;                
                NetatmoAuth netatmoAuth = JsonConvert.DeserializeObject<NetatmoAuth>(contentResult);
            }
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