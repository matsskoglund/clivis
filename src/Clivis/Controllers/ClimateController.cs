using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;
using Clivis.Models.Netatmo;
using Clivis.Models.Nibe;
using System;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Clivis.Controllers
{
public class LoggingEvents
{
    public const int GENERATE_ITEMS = 1000;
    public const int LIST_ITEMS = 1001;
    public const int GET_ITEM = 1002;
    public const int INSERT_ITEM = 1003;
    public const int UPDATE_ITEM = 1004;
    public const int DELETE_ITEM = 1005;

    public const int GET_ITEM_NOTFOUND = 4000;
    public const int UPDATE_ITEM_NOTFOUND = 4001;
}
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

        // /api/climate
        [HttpGet]
        public IActionResult GetClimate([FromQuery] string code, [FromQuery] string state)
        {
            _logger.LogInformation(LoggingEvents.GET_ITEM, "Getting item {code}",code);

            HostString host = new HostString(AppConfigs.NibeRedirectURI);
            if (Request != null)
                host = Request.Host;
            if (code != null)
            {
                nibe.CodeFilePath = "data/code.txt";
                
                nibe.code = code;
                nibe.init(AppConfigs);
            }
            return Redirect("http://" + host + "/api/climate/Nibe");
        }

        // /api/climate/Ping
        // /api/climate/Netatmo
        // /api/climate/Nibe
        [HttpGet("{source}")]
        public IActionResult GetById(string source)
        {
            _logger.LogInformation(LoggingEvents.GET_ITEM, "The source if {Source}", source);
            ClimateItem item = null;
            if (source.Equals("Nibe"))
            {
                // Read data from Nibe, if reading works we get data, if not we get null and try to do a login
                item = nibe.CurrentReading(AppConfigs);


                if (item == null)
                {
                    var uri = new UriBuilder(AppConfigs.NibeHost)
                    {
                        Path = $"/oauth/authorize",
                        Query = "response_type=code&client_id=" + AppConfigs.NibeClientId + "&scope=READSYSTEM&redirect_uri=" + AppConfigs.NibeRedirectURI + "&state=12345"
                    }.Uri;
                   
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
                    return Redirect(uri.AbsoluteUri);
                }
                item = ClimateItem.ClimateMeanValues(netatmoItem, nibeItem);
            }
            if (source.Equals("Ping"))
            {
                item = new ClimateItem() { IndoorValue = "20.5", OutdoorValue = "11.1", TimeStamp = DateTime.Now };
            }
            if (item == null)
                return new Microsoft.AspNetCore.Mvc.NoContentResult();
            else
                return Json(item);
        }
    }
}