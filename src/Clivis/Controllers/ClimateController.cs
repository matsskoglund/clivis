using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Clivis.Controllers
{

    public class NetatmoAuth{
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public List<string> scope { get; set; }
        public int expires_in { get; set; }
        public int expire_in { get; set; }
    }

    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        //public AppKeyConfig AppConfigs { get; }
        private AppKeyConfig AppConfigs = new AppKeyConfig();

        // Repository for the Climate Items
        public IClimateRepository ClimateItems { get; set; }

        public ClimateController(IClimateRepository climateItems)
        {
            ClimateItems = climateItems;
        }


        private string getDeviceID(string accessToken)
        {
            string response = "";

           try
           {
                    HttpClient client = new HttpClient();
                    string url = "http://api.netatmo.net/api/devicelist?access_token=" + accessToken;
                    var resp = client.GetAsync(url).Result;
                    response = resp.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
            }


                dynamic data = JsonConvert.DeserializeObject(response);
                var deviceID = data.body.devices[0]._id;
                var moduleID = data.body.modules[0]._id;
                var altitude = data.body.devices[0].place.altitude;
                var country = data.body.devices[0].place.country;
                var latitude = data.body.devices[0].place.location[0];
                var longitude = data.body.devices[0].place.location[1];
                var timezone = data.body.devices[0].place.timezone;

                return (string)deviceID;
                //ModuleID = (string)moduleID;
            
            return null;
        }
        private string getModuleId(string accessToken)
        {
            string response = "";

            try
            {
                HttpClient client = new HttpClient();
                string url = "http://api.netatmo.net/api/devicelist?access_token=" + accessToken;
                var resp = client.GetAsync(url).Result;
                response = resp.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
            }


            dynamic data = JsonConvert.DeserializeObject(response);
            var deviceID = data.body.devices[0]._id;
            var moduleID = data.body.modules[0]._id;
            var altitude = data.body.devices[0].place.altitude;
            var country = data.body.devices[0].place.country;
            var latitude = data.body.devices[0].place.location[0];
            var longitude = data.body.devices[0].place.location[1];
            var timezone = data.body.devices[0].place.timezone;

            return (string)moduleID;
            
        }

        private string getMeasurements(string accessToken, string deviceID, string moduleId, string measurementType)
        {
            try
            {
                string response = "";
                

                              

                try
                {
                    HttpClient client = new HttpClient();
                    string url = "";
                    if (measurementType == "Outdoor")
                    {
                        url = "http://api.netatmo.net/api/getmeasure?access_token=" + accessToken + "&device_id=" + deviceID + "&module_id=" + moduleId + "&type=Temperature&limit=1&date_end=last&scale=30min";
                    }
                    else if (measurementType == "Indoor")
                    {
                        url = "http://api.netatmo.net/api/getmeasure?access_token=" + accessToken + "&device_id=" + deviceID + "&type=Temperature&limit=1&date_end=last&scale=30min";
                    }
                    
                    
                    var resp = client.GetAsync(url).Result;
                    response = resp.Content.ReadAsStringAsync().Result;

                    
                }
                catch (Exception e)
                {
                    
                }

                
                dynamic data = JsonConvert.DeserializeObject(response);

                string temperature = data.body[0].value[0][0]; // temperature

                return temperature;
                }
            catch (Exception e)
            {

            }
            return null;
        }



        [HttpGet]
        public IEnumerable<ClimateItem> GetAll()
        {
            //string accessToken = 
            /*string userName = this.AppConfigs.NetatmoUserName;
            string pass = this.AppConfigs.NetatmoPassword;
            string clientId = this.AppConfigs.NetatmoClientId;
            string clientSecret = this.AppConfigs.NetatmoClientSecret;
            */
            //string ret = AuthenticateOATH(userName, pass, clientId, clientSecret);
            IClimateSource netatmo = new Netatmo();
            netatmo.init();

            string ret = "";
            NetatmoAuth auth = null;
            ClimateItem item = new ClimateItem();
            try
            {
                //Login  
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
            var resp =  client.PostAsync("https://api.netatmo.net/oauth2/token", outcontent).Result;

            string d =  resp.Content.ReadAsStringAsync().Result;                           
            
            NetatmoAuth dataD = JsonConvert.DeserializeObject<NetatmoAuth>(d);
            item.Key = dataD.access_token;

                // Get DeviceList
                /*string url = "http://api.netatmo.net/api/devicelist?access_token=" + item.Key;

                HttpClient client2 = new HttpClient();
                var resp2 =  client2.GetAsync(url).Result;
                string d2 =  resp2.Content.ReadAsStringAsync().Result; 
                ret = d2;                */
                string device = getDeviceID(dataD.access_token);
                

                // Get module
                string module = getModuleId(dataD.access_token);
                

                // Get outdoor temp
                string temperature = getMeasurements(dataD.access_token, device, module, "Outdoor");
                item.SourceName = temperature;
            }
            catch (System.Exception)
            {
                
                item.Key = "Fel";
                
            }
                         
            //item.SourceName = ret;
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