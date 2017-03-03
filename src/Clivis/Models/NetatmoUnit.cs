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
        public string CodeFilePath { get; set; }

        private NetatmoAuth netatmoAuth = new NetatmoAuth();

        public void init(AppKeyConfig config)
        {
            login(config);
            setDeviceAndModuleID(config);
        }

        private void login(AppKeyConfig configs)
        {
           
            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "password" ),
                    new KeyValuePair<string, string>("client_id", configs.NetatmoClientId),
                    new KeyValuePair<string, string>( "client_secret", configs.NetatmoClientSecret),
                    new KeyValuePair<string, string>("username", configs.UserName),
                    new KeyValuePair<string, string>( "password", configs.Password),
                    new KeyValuePair<string, string>( "scope", "read_station")
            };
            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);

            var uri = new UriBuilder(configs.NetatmoHost)
            {
                Path = "/oauth2/token"
            }.Uri;


            var response = client.PostAsync(uri, outcontent).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception("Could not login with ID:" + configs.NetatmoClientId + " Secret: " + configs.NetatmoClientSecret + " UserName: " + configs.UserName + " Password: " + configs.Password + " Response: " + response.Content.ReadAsStringAsync().Result);

            string contentResult = response.Content.ReadAsStringAsync().Result;
            netatmoAuth = JsonConvert.DeserializeObject<NetatmoAuth>(contentResult);
        }

        private void setDeviceAndModuleID(AppKeyConfig config)
        {
            string response = "";

            HttpClient client = new HttpClient();
            var uri = new UriBuilder(config.NetatmoHost)
            {
                Path = "/api/devicelist",
                Query = "access_token=" + netatmoAuth.access_token
            }.Uri;


            var resp = client.GetAsync(uri).Result;
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Could not set device and module");

            response = resp.Content.ReadAsStringAsync().Result;


            dynamic data = JsonConvert.DeserializeObject(response);

            deviceId = data.body.devices[0]._id;
            moduleId = data.body.modules[0]._id;
        }


        public string outDoorTemperature(AppKeyConfig configs)
        {
            HttpClient client = new HttpClient();
            var uri = new UriBuilder(configs.NetatmoHost)
            {
                Path = "/api/getmeasure",
                Query = "access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&module_id=" + moduleId + "&type=Temperature&limit=1&date_end=last&scale=max"
            }.Uri;

            var resp = client.GetAsync(uri).Result;
            var response = resp.Content.ReadAsStringAsync().Result;

            dynamic data = JsonConvert.DeserializeObject(response);

            string temperature = data.body[0].value[0][0]; // temperature

            return temperature;
        }

        public string inDoorTemperature(AppKeyConfig configs)
        {

            HttpClient client = new HttpClient();
            var uri = new UriBuilder(configs.NetatmoHost)
            {
                Path = "/api/getmeasure",
                Query = "access_token=" + netatmoAuth.access_token + "&device_id=" + deviceId + "&type=Temperature&limit=1&date_end=last&scale=max"
            }.Uri;

            var resp = client.GetAsync(uri).Result;
            string response = resp.Content.ReadAsStringAsync().Result;
            dynamic data = JsonConvert.DeserializeObject(response);

            string temperature = data.body[0].value[0][0]; // temperature

            return temperature;
        }
        public ClimateItem CurrentReading(AppKeyConfig AppConfigs)
        {
            login(AppConfigs);
            setDeviceAndModuleID(AppConfigs);
            ClimateItem reading = new ClimateItem();
            try
            {
                reading.IndoorValue = this.inDoorTemperature(AppConfigs);
                reading.OutdoorValue = this.outDoorTemperature(AppConfigs);
                reading.TimeStamp = DateTime.Now;
                return reading;
            }

            // Somehow we could not get the data.
            catch
            {
                return null;
            }
        }
    }
}