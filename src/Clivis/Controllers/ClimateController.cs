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

namespace Clivis.Controllers
{

    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        public AppKeyConfig AppConfigs { get; }

        public IClimateSource nibe { get; }
        public IClimateSource netatmo { get; }

        public ClimateController(IOptions<AppKeyConfig> configs, IDictionary<string, IClimateSource> climateSources)
        {

            AppConfigs = configs.Value;
            nibe = climateSources["Nibe"];
            netatmo = climateSources["Netatmo"];
        }

        // /api/climate
        [HttpGet]
        public IActionResult GetClimate([FromQuery] string code, [FromQuery] string state)
        {

            HostString host = new HostString("localhost", 5050);
            if (Request != null)
                host = Request.Host;
            if (code != null)
            {
                nibe.CodeFilePath = "code.txt";
                nibe.code = code;
                nibe.init(AppConfigs);
            }
            return Redirect("http://" + host + "/api/climate/Nibe");
        }


        // /api/climate/Netatmo
        // /api/climate/Nibe
        [HttpGet("{source}")]
        public IActionResult GetById(string source)
        {
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
            if (item == null)
                return new Microsoft.AspNetCore.Mvc.NoContentResult();
            else
                return Json(item);
        }
    }
}