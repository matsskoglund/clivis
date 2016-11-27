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
        private NibeAuth nibeAuth = new NibeAuth();

        public string clientId { get; set; }
        public string secret { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }

        private string systemId { get; set; }
        public string CodeFilePath { get; set; }
        public string redirect_uri { get; set; }

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
                try
                {
                    FileInfo fi = new FileInfo(CodeFilePath);
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException("The code file is nor set");
                }
                File.WriteAllText(CodeFilePath, code);
            }         
        }


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
                    new KeyValuePair<string, string>("client_id", config.NibeClientId),
                    new KeyValuePair<string, string>("client_secret", config.NibeClientSecret),
                    new KeyValuePair<string, string>("code", this.code),
                    new KeyValuePair<string, string>( "redirect_uri", config.NibeRedirectURI),
                    new KeyValuePair<string, string>( "scope", "READSYSTEM")
            };

            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);
            
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


   
        public NibeAuth Refresh(AppKeyConfig AppConfig)
        {
            string nibeAuthJson = File.ReadAllText("nibeauth.json");
            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(nibeAuthJson);

            //Login  
            var pairs = new List<KeyValuePair<string, string>>
            {
                    new KeyValuePair<string, string>("grant_type", "refresh_token" ),
                    new KeyValuePair<string, string>("client_id", AppConfig.NibeClientId),
                    new KeyValuePair<string, string>( "client_secret", AppConfig.NibeClientSecret),
                    new KeyValuePair<string, string>("refresh_token", nibeAuth.refresh_token)
            };
            HttpClient client = new HttpClient();
            var outcontent = new FormUrlEncodedContent(pairs);

            var response = client.PostAsync("https://api.nibeuplink.com/oauth/token", outcontent).Result;

            // If we could not refresh
            if (!response.IsSuccessStatusCode)
            {
                int statusCode = (int)response.StatusCode;
                return null;
            }

            // Replace the access and refresh token in the file with the new values
            string contentResult = response.Content.ReadAsStringAsync().Result;

            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(contentResult);
            nibeAuthJson = JsonConvert.SerializeObject(nibeAuth);
            File.WriteAllText("nibeauth.json", nibeAuthJson);

            // Success, a new access and refresh token is in the file
            return nibeAuth;
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

        public ClimateItem GetReadingWithAccessCode(string accessCode)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);
            var response = client.GetAsync("https://api.nibeuplink.com/api/v1/systems/27401/parameters?parameterIds=outdoor_temperature&parameterIds=indoor_temperature").Result;
            if (!response.IsSuccessStatusCode)
            {
                // If it didn't work, return null
                return null;
            }
            string contentResult = response.Content.ReadAsStringAsync().Result;

            NibeTemp nibeOutdoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[0];
            NibeTemp nibeIndoorTemp = JsonConvert.DeserializeObject<List<NibeTemp>>(contentResult)[1];

            reading.OutdoorValue = nibeOutdoorTemp.displayValue.Remove(nibeOutdoorTemp.displayValue.Length - 2);
            reading.IndoorValue = nibeIndoorTemp.displayValue.Remove(nibeIndoorTemp.displayValue.Length - 2);
            reading.TimeStamp = DateTime.Now;

            return reading;
        }

        ClimateItem reading = new ClimateItem();
  
        public ClimateItem CurrentReading(AppKeyConfig AppConfig)
        {
            ClimateItem item = null;

            // Get the access and refresh tokens if they exist, if they don't exist we cannot proceceed and return null 
            NibeAuth auth = getNibeAuthJson();

            if (auth == null)
                return null;

            // Access and refresh token exist,  try to access using the access token
            item = GetReadingWithAccessCode(auth.access_token);

            // If we get null back, it didn't work and we try to refresh with the refresh token
            if (item == null)

                // If it didn't work to get a new access token we bail out and return null
                auth = Refresh(AppConfig);
            if (auth == null)
                return null;

            // It worked to get a new access token, try agin to get data using the new access token
            item = GetReadingWithAccessCode(auth.access_token);

            // If get an item back we return it, it we get null back we can't still access data and return null        
            return item;

        }
    }
}