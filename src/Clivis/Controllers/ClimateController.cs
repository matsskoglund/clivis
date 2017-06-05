using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Clivis.Controllers
{
    
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {

        public AppKeyConfig AppConfigs { get; }
        private readonly ILogger _logger;
        public IClimateSource nibe { get; }
        public IClimateSource netatmo { get; }

        public ClimateController(IOptions<AppKeyConfig> configs, IDictionary<string, IClimateSource> climateSources, ILogger<ClimateController> logger)
        {

            AppConfigs = configs.Value;
            nibe = climateSources["Nibe"];
            netatmo = climateSources["Netatmo"];
            _logger = logger;
        }

        /// Entrypoint where a code is returned from Nibe api after logging in
        // /api/climate
        [HttpGet]
        public IActionResult GetClimate([FromQuery] string code, [FromQuery] string state)
        {
            _logger.LogInformation("Getting item {code}", code);
            HostString host = new HostString(AppConfigs.NibeRedirectURI);
            if (Request != null)
                host = Request.Host;
         
            _logger.LogInformation("Host is {host}", host.Host + ":" + host.Port);
            if (code == null)
            {
                string redirectUrl = "http://" + host.Host + "/api/climate/Reading";
                _logger.LogInformation("redirecting to {redirect}", redirectUrl);
                return Redirect(redirectUrl);
            }
            
            if ((code != null) && (state.Equals("12345")))
            {
                nibe.code = code;
                nibe.init(AppConfigs);
            }
            
             string redirectString = "http://" + host.Host + "/api/climate/Nibe";
            _logger.LogInformation("redirecting to {redirect}", redirectString);
            return Redirect(redirectString);
        }

        // /api/climate/Ping
        // /api/climate/Netatmo
        // /api/climate/Nibe
        // /api/climate/Version
        [HttpGet("{source}")]
        public IActionResult GetBySource (string source)
        {
            _logger.LogInformation("The source is {Source}", source);

            ClimateItem item = null;
            if (source.Equals("Nibe"))
            {
                // Read data from Nibe, if reading works we get data, if not we get null and try to do a login
                item = nibe.CurrentReading(AppConfigs);
                
                if (item == null)
                {
                    _logger.LogInformation("Logging in to Nibe");
                    var uri = new UriBuilder(AppConfigs.NibeHost)
                    {
                        Path = $"/oauth/authorize",
                        Query = "response_type=code&client_id=" + AppConfigs.NibeClientId + "&scope=READSYSTEM&redirect_uri=" + AppConfigs.NibeRedirectURI + "&state=12345"
                    }.Uri;
                    _logger.LogInformation("Connecting to {uri}", uri);

                    return Redirect(uri.AbsoluteUri);
                }
            }

            if (source.Equals("Netatmo"))
            {
                item = netatmo.CurrentReading(AppConfigs);
            }
            if (source.Equals("Reading"))
            {
                
                ClimateItem netatmoItem = netatmo.CurrentReading(AppConfigs);
               
                ClimateItem nibeItem = null;
                nibeItem = nibe.CurrentReading(AppConfigs);
                if (nibeItem == null)
                {
                    var uri = new UriBuilder(AppConfigs.NibeHost)
                    {
                        Path = $"/oauth/authorize",
                        Query = "response_type=code&client_id=" + AppConfigs.NibeClientId + "&scope=READSYSTEM&redirect_uri=" + AppConfigs.NibeRedirectURI + "&state=12345"
                    }.Uri;

                    _logger.LogInformation("redirecting to {redirect}", uri.AbsoluteUri);
                    return Redirect(uri.AbsoluteUri);
                }
                item = ClimateItem.ClimateMeanValues(netatmoItem, nibeItem);
            }
            if (source.Equals("Ping"))
            {
                item = new ClimateItem() { IndoorValue = "20.5", OutdoorValue = "11.1", TimeStamp = DateTime.Now };
                _logger.LogInformation("Ping returning {item}", item.ToString());
            }
            if (source.Equals("Version"))
            {
                item = new ClimateItem() { IndoorValue = AppConfigs.BuildVersion, OutdoorValue = AppConfigs.BuildVersion, TimeStamp = DateTime.Now };
                _logger.LogInformation("Version returning {item}", AppConfigs.BuildVersion);
            }

            if (item == null)
                return new Microsoft.AspNetCore.Mvc.NoContentResult();
            else
                return Json(item);
        }
    }
}