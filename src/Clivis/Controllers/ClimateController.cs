using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;

namespace Clivis.Controllers
{
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        // Repository for the Climate Items
        public IClimateRepository ClimateItems { get; set; }

        public ClimateController(IClimateRepository climateItems)
        {
            ClimateItems = climateItems;
        }
 

        [HttpGet]
        public IEnumerable<ClimateItem> GetAll()
        {
            //string accessToken = 

          
            ClimateItem item = new ClimateItem();
            item.Key = "tjena";
           item.SourceName = res;
            ClimateItems.Add(item);
            return ClimateItems.GetAll();


        }

        public static string ErrorOATHMessage = "";
        public static string AccessToken = "";
        public static string DeviceID = "";
        public static string ModuleID = "";
      



        private string AuthenticateOATH(string clientUsername, string clientPassword, string clientID, string clientSecret)
        {
            
            string ret = null;
            HttpClient client = new HttpClient();

            var pairs = new List<KeyValuePair<string, string>>
             {
                        new KeyValuePair<string, string>("grant_type", "password" ),
                        new KeyValuePair<string, string>("client_id", clientID),
                        new KeyValuePair<string, string>( "client_secret", clientSecret),
                        new KeyValuePair<string, string>("username", clientUsername),
                        new KeyValuePair<string, string>( "password", clientPassword),
                        new KeyValuePair<string, string>( "scope", "read_station")
            };

            var outcontent = new FormUrlEncodedContent(pairs);
            var resp =  client.PostAsync("https://api.netatmo.net/oauth2/token", outcontent).Result;
            string jsonData = "";
            byte[] data;
            data =  resp.Content.ReadAsByteArrayAsync().Result;                           
            jsonData = System.Text.Encoding.UTF8.GetString(data, 0, data.Length - 1);       
                      
            return jsonData;
                    
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