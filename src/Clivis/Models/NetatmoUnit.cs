using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Clivis.Models.Netatmo
{


    public class NetatmoUnit : IClimateSource
    {
        public string code { get; set; }
        public string clientId { get; set; }
        public string secret { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }

        private string deviceId { get; set; }

        private string moduleId { get; set; }
        public string CodeFilePath { get; set;  }


        private NetatmoAuth netatmoAuth = new NetatmoAuth();
       
        public void init(AppKeyConfig config)
        {
            login(config);
            setDeviceAndModuleID();
        }

        public string outDoorTemperature {
            get
            {            
                    HttpClient client = new HttpClient();
                    string url = "http://api.netatmo.net/api/getmeasure?access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&module_id=" + moduleId + "&type=Temperature&limit=1&date_end=last&scale=30min";

                    var resp = client.GetAsync(url).Result;
                    var response = resp.Content.ReadAsStringAsync().Result;

                    dynamic data = JsonConvert.DeserializeObject(response);

                    string temperature = data.body[0].value[0][0]; // temperature

                    return temperature;

            }
        }

        public string inDoorTemperature
        {
            get
            {
              
                        HttpClient client = new HttpClient();
                        string url = "http://api.netatmo.net/api/getmeasure?access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&type=Temperature&limit=1&date_end=last&scale=30min";                     

                        var resp = client.GetAsync(url).Result;
                        string response = resp.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(response);
                        
                        string temperature = data.body[0].value[0][0]; // temperature

                        return temperature;
              
               
            }
        }

        private void login(AppKeyConfig AppConfigs)
        {                                    
            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "password" ),
                    new KeyValuePair<string, string>("client_id", AppConfigs.ClientId),
                    new KeyValuePair<string, string>( "client_secret", AppConfigs.ClientSecret),
                    new KeyValuePair<string, string>("username", AppConfigs.UserName),
                    new KeyValuePair<string, string>( "password", AppConfigs.Password),
                    new KeyValuePair<string, string>( "scope", "read_station")
            };
            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            var response = client.PostAsync("https://api.netatmo.net/oauth2/token", outcontent).Result;
            
            string contentResult = response.Content.ReadAsStringAsync().Result;

            netatmoAuth = JsonConvert.DeserializeObject<NetatmoAuth>(contentResult);
          
        }

        private void setDeviceAndModuleID()
        {
            string response = "";

                HttpClient client = new HttpClient();
                string url = "http://api.netatmo.net/api/devicelist?access_token=" + netatmoAuth.access_token;
                var resp = client.GetAsync(url).Result;
                response = resp.Content.ReadAsStringAsync().Result;
                
          

            dynamic data = JsonConvert.DeserializeObject(response);
            deviceId = data.body.devices[0]._id;
            moduleId = data.body.modules[0]._id;
        }

        public ClimateItem CurrentReading(AppKeyConfig AppConfigs)
        {
            login(AppConfigs);
            setDeviceAndModuleID();
            ClimateItem reading = new ClimateItem();
            reading.IndoorValue = this.inDoorTemperature;
            reading.OutdoorValue = this.outDoorTemperature;
            reading.TimeStamp = DateTime.Now;
            return reading;
        }
    }
}