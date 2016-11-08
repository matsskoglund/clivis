using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Clivis.Models.Nibe
{


    public class NibeUnit : IClimateSource
    {
        public string clientId { get; set; }
        public string secret { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }

        private string systemId { get; set; }

        private string _code = null;
        public string code {
            get
            {
                // If no code set, try to read one from file
                if(_code == null)
                {
                    // If file exist read it
                    if (File.Exists("code.txt"))
                    {
                        _code = File.ReadAllText("code.txt");
                       
                    }
                }
                return _code;
            }
            set
            {
                string code = value;
                File.WriteAllText("code.txt", code);
            }
        }

        public string redirect_uri { get; set; }
        private NibeAuth nibeAuth = new NibeAuth();

        public void init(AppKeyConfig config)
        {
            login(config);
            //getNewReading(config);
            //Refresh(config);
        }
 
      

        private string inDoorTemperature;
        public string InDoorTemperature
        {
            get
            {
                return inDoorTemperature;

            }

        }

        private string outDoorTemperature;
        public string OutDoorTemperature
        {
            get
            {
                return outDoorTemperature;
                
            }
        }

        private void login(AppKeyConfig AppConfig)
        {                                    
            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "authorization_code" ),
                    new KeyValuePair<string, string>("client_id", AppConfig.ClientId),
                    new KeyValuePair<string, string>("client_secret", AppConfig.ClientSecret),
                    new KeyValuePair<string, string>("code", this.code),
                    new KeyValuePair<string, string>( "redirect_uri", AppConfig.RedirectURI),
                    new KeyValuePair<string, string>( "scope", "READSYSTEM")
            };

            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            Console.WriteLine(pairs.ToString());
            var response = client.PostAsync("https://api.nibeuplink.com/oauth/token", outcontent).Result;

            string contentResult = response.Content.ReadAsStringAsync().Result;

            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(contentResult);
            
            string nibeAuthJson = JsonConvert.SerializeObject(nibeAuth);
            File.WriteAllText("nibeauth.json", nibeAuthJson);
        }

        public void Refresh(AppKeyConfig AppConfig)
        {
            string nibeAuthJson = File.ReadAllText("nibeauth.json");
            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(nibeAuthJson);

            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "refresh_token" ),
                    new KeyValuePair<string, string>("client_id", AppConfig.ClientId),
                    new KeyValuePair<string, string>( "client_secret", AppConfig.ClientSecret),
                    new KeyValuePair<string, string>("refresh_token", nibeAuth.refresh_token)
            };
            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            Console.WriteLine(pairs.ToString());
            var response = client.PostAsync("https://api.nibeuplink.com/oauth/token", outcontent).Result;

            string contentResult = response.Content.ReadAsStringAsync().Result;

            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(contentResult);
            nibeAuthJson = JsonConvert.SerializeObject(nibeAuth);
            File.WriteAllText("nibeauth.json", nibeAuthJson);
        }

        public ClimateItem latestReading(AppKeyConfig AppConfigs)
        {
            return CurrentReading(AppConfigs);
        }

        ClimateItem reading = new ClimateItem();
        private void getNewReading(AppKeyConfig AppConfig)
        {
            var pairs = new List<KeyValuePair<string, string>>
                {
                     new KeyValuePair<string, string>("access_token", nibeAuth.access_token ),                     
                     new KeyValuePair<string, string>("client_id", AppConfig.ClientId),                     
                     new KeyValuePair<string, string>( "client_secret", AppConfig.ClientSecret),
                     new KeyValuePair<string, string>("code", this.code),                     
                     new KeyValuePair<string, string>( "redirect_uri", AppConfig.RedirectURI),
                     new KeyValuePair<string, string>( "scope", "READSYSTEM")
                };

            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            Console.WriteLine(pairs.ToString());

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nibeAuth.access_token);
            var response = client.GetAsync("https://api.nibeuplink.com/api/v1/systems/27401/parameters?parameterIds=outdoor_temperature&parameterIds=indoor_temperature").Result;

            string contentResult = response.Content.ReadAsStringAsync().Result;

            NibeTemp nibeOutdoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[0];
            NibeTemp nibeIndoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[1];
            outDoorTemperature = nibeOutdoorTemp.displayValue.Remove(nibeOutdoorTemp.displayValue.Length - 2);
            inDoorTemperature = nibeIndoorTemp.displayValue.Remove(nibeIndoorTemp.displayValue.Length - 2);

            
            reading.IndoorValue = inDoorTemperature;
            reading.OutdoorValue = outDoorTemperature;
            reading.TimeStamp = DateTime.Now;

        }


        public ClimateItem CurrentReading(AppKeyConfig AppConfig)
        {
            
            getNewReading(AppConfig);
            
            return reading;
        }
    }
}