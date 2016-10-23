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
           // item.SourceName = res;
            ClimateItems.Add(item);
            return ClimateItems.GetAll();


        }

        public static string ErrorOATHMessage = "";
        public static string AccessToken = "";
        public static string DeviceID = "";
        public static string ModuleID = "";
      



        private string AuthenticateOATH(string clientUsername, string clientPassword, string clientID, string clientSecret)
        {
            HttpResponseMessage response = null;
            string ret = null;
            try
            {
                ErrorOATHMessage = "";

                try
                {
                    HttpClient client = new HttpClient();

                    var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "password" ),
                        new KeyValuePair<string, string>("client_id", clientID),
                        new KeyValuePair<string, string>( "client_secret", clientSecret),
                        new KeyValuePair<string, string>("username", clientUsername),
                        new KeyValuePair<string, string>( "password", clientPassword),
                        new KeyValuePair<string, string>( "scope", "read_station read_thermostat write_thermostat")
                    };

                    var content = new FormUrlEncodedContent(pairs);

                    var resp = client.PostAsync("https://api.netatmo.net/oauth2/token", content).Result.Content;
              //      dynamic data = JsonConvert.DeserializeObject(resp.ToString());
                }
                catch (Exception e)
                {
                    ErrorOATHMessage = e.Message;
                }

                //string responseDecoded = resp.ToString();
                string expiresIn = "";
                string refreshToken = "";

                //dynamic data = JsonConvert.DeserializeObject(responseDecoded);
               // AccessToken = data.access_token;
               // expiresIn = data.expires_in;
               // refreshToken = data.refresh_token;

          /*      lvDebug.Items.Add("Access Token: " + AccessToken);
                lvDebug.Items.Add("Expires in: " + expiresIn);
                lvDebug.Items.Add("Refresh Token: " + refreshToken);
                tbAccessToken.Text = AccessToken;
                */

                if (string.IsNullOrEmpty(ErrorOATHMessage) == false)
                {
                   // lvDebug.Items.Add(ErrorOATHMessage);
                }
                return null;
            }
            catch (Exception e)
            {
               // lvDebug.Items.Add("Error during access token retrieval: " + e.Message);
            }
            return "fel";
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