using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Clivis.Util;

namespace Clivis.Models.Nibe
{


    public class NibeUnit : IClimateSource
    {       
        private NibeAuth nibeAuth = new NibeAuth();
        public string encryptionKey { get; set; }
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
                        _code = Protector.DecryptString(File.ReadAllText(CodeFilePath), encryptionKey);
                        File.Delete(CodeFilePath);
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
                    if (Directory.Exists(fi.Directory.ToString()) == false)
                        Directory.CreateDirectory(fi.Directory.ToString());
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException("The code file is nor set");
                }
                File.WriteAllText(CodeFilePath, Protector.EncryptString(code, encryptionKey));
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
            var uri = new UriBuilder(config.NibeHost)
            {
                Path = "/oauth/token",
                Query = "parameterIds=outdoor_temperature&parameterIds=indoor_temperature"
            }.Uri;

            HttpResponseMessage response = client.PostAsync(uri, outcontent).Result;
            if (!response.IsSuccessStatusCode)
            {
                int statusCode = (int)response.StatusCode;
                throw new Exception(statusCode + " " + response.ReasonPhrase);
            }
            

            string contentResult = response.Content.ReadAsStringAsync().Result;

            nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(contentResult);

            string nibeAuthJson = JsonConvert.SerializeObject(nibeAuth);
            File.WriteAllText("data/nibeauth.json", Protector.EncryptString(nibeAuthJson, encryptionKey));
        }


   
        public NibeAuth Refresh(AppKeyConfig AppConfig)
        {
            string nibeAuthJson = Protector.DecryptString(File.ReadAllText("data/nibeauth.json"), encryptionKey);
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
            var uri = new UriBuilder(AppConfig.NibeHost)
            {
                Path = "/oauth/token",
                Query = "parameterIds=outdoor_temperature&parameterIds=indoor_temperature"
            }.Uri;

            var response = client.PostAsync(uri, outcontent).Result;

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
            File.WriteAllText("data/nibeauth.json", Protector.EncryptString(nibeAuthJson, encryptionKey));

            // Success, a new access and refresh token is in the file
            return nibeAuth;
        }

       
        private NibeAuth getNibeAuthJson()
        {
            if (File.Exists("data/nibeauth.json"))
            {
                string nibeAuthJson = Protector.DecryptString(File.ReadAllText("data/nibeauth.json"), encryptionKey);
                nibeAuth = JsonConvert.DeserializeObject<NibeAuth>(nibeAuthJson);
                return nibeAuth;
            }
            else
                return null;            
        }

        public ClimateItem GetReadingWithAccessCode(string accessCode, AppKeyConfig config)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);

            var uri = new UriBuilder(config.NibeHost)
            {
                Path = $"/api/v1/systems/27401/parameters",
                Query = "parameterIds=outdoor_temperature&parameterIds=indoor_temperature"
            }.Uri;

           
            var response =  client.GetAsync(uri).Result;
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
            item = GetReadingWithAccessCode(auth.access_token, AppConfig);

            // If we get null back, it didn't work and we try to refresh with the refresh token
            if (item == null)
            {

                // If it didn't work to get a new access token we bail out and return null
                auth = Refresh(AppConfig);


                if (auth == null)
                    return null;

                // It worked to get a new access token, try agin to get data using the new access token
                item = GetReadingWithAccessCode(auth.access_token, AppConfig);
            }

            // If get an item back we return it, it we get null back we can't still access data and return null        
            return item;

        }
    }
}