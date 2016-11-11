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
        public string CodeFilePath { get; set; }

        private string _code = null;
        public string code {
            get
            {
                // If no code set, try to read one from file
                if(_code == null)
                {
                    // If file exist read it
                    if (File.Exists(CodeFilePath))
                    {
                        _code = File.ReadAllText(CodeFilePath);                      
                    }
                }
                // Null is returned if the file does not exist
                return _code;
            }
            set
            {
                if (value == null)
                    return;

                string code = value;
                FileInfo fi = new FileInfo(CodeFilePath);
                if (CodeFilePath == null)
                    throw new InvalidOperationException("The code file is nor set");
                File.WriteAllText(CodeFilePath, code);
            }         
        }

        public string redirect_uri { get; set; }
        private NibeAuth nibeAuth = new NibeAuth();

        public NibeUnit()
        {
            
        }

        public void init(AppKeyConfig config)
        {
            // Check to see if we have a code
            if (code == null)
                throw new Exception("Code is null");
                        
            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "authorization_code" ),
                    new KeyValuePair<string, string>("client_id", config.ClientId),
                    new KeyValuePair<string, string>("client_secret", config.ClientSecret),
                    new KeyValuePair<string, string>("code", this.code),
                    new KeyValuePair<string, string>( "redirect_uri", config.RedirectURI),
                    new KeyValuePair<string, string>( "scope", "READSYSTEM")
            };

            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            Console.WriteLine(pairs.ToString());
            HttpResponseMessage response = client.PostAsync("https://api.nibeuplink.com/oauth/token", outcontent).Result;
            if (!response.IsSuccessStatusCode)
            {
                int statusCode = (int)response.StatusCode;
                throw new Exception(statusCode + " " + response.ReasonPhrase);
            }
            

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
            if (!response.IsSuccessStatusCode)
            {
                int statusCode = (int)response.StatusCode;
                throw new Exception(statusCode + " " + response.ReasonPhrase);
            }

            string contentResult = response.Content.ReadAsStringAsync().Result;

            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(contentResult);
            nibeAuthJson = JsonConvert.SerializeObject(nibeAuth);
            File.WriteAllText("nibeauth.json", nibeAuthJson);
        }

       
        private NibeAuth getNibeAuthJson()
        {
            if (File.Exists("nibeauth.json"))
            {
                string nibeAuthJson = File.ReadAllText("nibeauth.json");
                nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(nibeAuthJson);
                return nibeAuth;
            }
            else
                return null;

            
        }

        ClimateItem reading = new ClimateItem();
        

        public ClimateItem CurrentReading(AppKeyConfig AppConfig)
        {
            HttpClient client = new HttpClient();
            if (getNibeAuthJson() == null)
                throw new Exception("Code not found");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nibeAuth.access_token);
            var response = client.GetAsync("https://api.nibeuplink.com/api/v1/systems/27401/parameters?parameterIds=outdoor_temperature&parameterIds=indoor_temperature").Result;
            if (!response.IsSuccessStatusCode)
            {
                // If it didn't work, try to refresh the token
                Refresh(AppConfig);

                int statusCode = (int)response.StatusCode;
                throw new Exception(statusCode + " " + response.ReasonPhrase);
            }
            string contentResult = response.Content.ReadAsStringAsync().Result;

            NibeTemp nibeOutdoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[0];
            NibeTemp nibeIndoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[1];

            reading.OutdoorValue = nibeOutdoorTemp.displayValue.Remove(nibeOutdoorTemp.displayValue.Length - 2);
            reading.IndoorValue = nibeIndoorTemp.displayValue.Remove(nibeIndoorTemp.displayValue.Length - 2);
            reading.TimeStamp = DateTime.Now;

            return reading;
        }
    }
}