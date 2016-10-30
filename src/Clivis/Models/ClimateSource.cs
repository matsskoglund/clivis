using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Clivis.Models.Netatmo
{


    public class NetatmoUnit : IClimateSource
    {
        public string clientID { get; set; }
        public string secret { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }

        private string deviceId { get; set; }

        private string moduleId { get; set; }
        

        private NetatmoAuth netatmoAuth = new NetatmoAuth();
       
        public void init(AppKeyConfig config)
        {
            login(config);
            setDeviceAndModuleID();
        }

        public string outDoorTemperature {
            get
            {            
                try
                {
                    HttpClient client = new HttpClient();
                    string url = "http://api.netatmo.net/api/getmeasure?access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&module_id=" + moduleId + "&type=Temperature&limit=1&date_end=last&scale=30min";

                    var resp = client.GetAsync(url).Result;
                    var response = resp.Content.ReadAsStringAsync().Result;

                    dynamic data = JsonConvert.DeserializeObject(response);

                    string temperature = data.body[0].value[0][0]; // temperature

                    return temperature;

                }
                catch (Exception e)
                {

                }            

            return null;
            }
        }

        public string inDoorTemperature
        {
            get
            {
               try
                    {
                        HttpClient client = new HttpClient();
                        string url = "http://api.netatmo.net/api/getmeasure?access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&type=Temperature&limit=1&date_end=last&scale=30min";                     

                        var resp = client.GetAsync(url).Result;
                        string response = resp.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(response);
                        
                        string temperature = data.body[0].value[0][0]; // temperature

                        return temperature;
                }
                catch (Exception e)
                {

                }

                return null;
            }
        }

        private void login(AppKeyConfig AppConfigs)
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
            Console.WriteLine("Trying to login with");
            Console.WriteLine("Id"+AppConfigs.NetatmoClientId);
            Console.WriteLine(AppConfigs.NetatmoClientSecret);
            Console.WriteLine(AppConfigs.NetatmoUserName);
            Console.WriteLine("pw" + AppConfigs.NetatmoPassword);
            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            var response = client.PostAsync("https://api.netatmo.net/oauth2/token", outcontent).Result;
            Console.Write(response.ToString());
            string contentResult = response.Content.ReadAsStringAsync().Result;

            netatmoAuth = JsonConvert.DeserializeObject<NetatmoAuth>(contentResult);
          
        }

        private void setDeviceAndModuleID()
        {
            string response = "";

            try
            {
                HttpClient client = new HttpClient();
                string url = "http://api.netatmo.net/api/devicelist?access_token=" + netatmoAuth.access_token;
                var resp = client.GetAsync(url).Result;
                response = resp.Content.ReadAsStringAsync().Result;
                Console.Write(response.ToString());
            }
            catch (Exception e)
            {
                Console.Write(netatmoAuth.access_token);
            }


            dynamic data = JsonConvert.DeserializeObject(response);
            deviceId = data.body.devices[0]._id;
            moduleId = data.body.modules[0]._id;
/*            var altitude = data.body.devices[0].place.altitude;
            var country = data.body.devices[0].place.country;
            var latitude = data.body.devices[0].place.location[0];
            var longitude = data.body.devices[0].place.location[1];
            var timezone = data.body.devices[0].place.timezone;            */
        }
    }
}