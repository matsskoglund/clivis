using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clivis.Models.Netatmo;
using System.IO;
using Clivis.Models;
using Stubbery;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;

namespace ClivisTests
{
    public class NetatmoUnitTests
    {
        NetatmoUnit netatmoUnit = new NetatmoUnit();
        public NetatmoUnitTests()
        {

        }

        [Fact]
        public void NetatmoUnitLoginSuccessFul()
        {
            string responseText = "{ \"access_token\":\"544cf4071c7759831d94cdf9|fcb30814afbffd0e39381e74fe38a59a\",\"refresh_token\":\"544cf4071c7759831d94cdf9|2b2c270c1208e8f67d3bd3891e395e1a\",\"scope\":[\"read_station\"],\"expires_in\":10800,\"expire_in\":10800}";
            string deviceResponse = File.ReadAllText("netatmodeviceresponse.json");
            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Post(
                "/oauth2/token",
                (request, args) =>
                {
                    return responseText;
                }
                );
            apiNetatmoStub.Get(
                "/api/devicelist",
                (request,response) =>
                {
                    return deviceResponse;
                }
                );

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            netatmoUnit.init(configs);

        }

        [Fact]
        public void NetatmoUnitLoginFailDueToLogin()
        {
            string deviceResponse = File.ReadAllText("netatmodeviceresponse.json");
            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Get(
                "/api/devicelist",
                (request, response) =>
                {
                    return deviceResponse;
                }
                );

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;
            
            Exception ex = Assert.Throws<Exception>(() => netatmoUnit.init(configs));
            Assert.Equal("Could not login", ex.Message);
        }

        [Fact]
        public void NetatmoUnitLoginFailDueToSetDevice()
        {
            string responseText = "{ \"access_token\":\"544cf4071c7759831d94cdf9|fcb30814afbffd0e39381e74fe38a59a\",\"refresh_token\":\"544cf4071c7759831d94cdf9|2b2c270c1208e8f67d3bd3891e395e1a\",\"scope\":[\"read_station\"],\"expires_in\":10800,\"expire_in\":10800}";
            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Post(
                "/oauth2/token",
                (request, args) =>
                {
                    return responseText;
                }
                );
           
            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            Exception ex = Assert.Throws<Exception>(() => netatmoUnit.init(configs));
            Assert.Equal("Could not set device and module", ex.Message);
        }

        [Fact]
        public void NetatmoUnitIndoortemperature()
        {
            string responseText = "{\"body\":[{\"beg_time\":1481372100,\"value\":[[22.1]]}],\"status\":\"ok\",\"time_exec\":0.027220010757446,\"time_server\":1481371840}";


            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Get(
                "/api/getmeasure",
                (request, args) =>
                {
                    return responseText;
                }
                );

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            string actualTemp = netatmoUnit.inDoorTemperature(configs);
            Assert.Equal("22.1", actualTemp);
        }

        [Fact]
        public void NetatmoUnitOutdoortemperature()
        {
            string responseText = "{\"body\":[{\"beg_time\":1481372100,\"value\":[[0.7]]}],\"status\":\"ok\",\"time_exec\":0.034345865249634,\"time_server\":1481371970}";


            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Get(
                "/api/getmeasure",
                (request, args) =>
                {
                    return responseText;
                }
                );

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            string actualTemp = netatmoUnit.outDoorTemperature(configs);
            Assert.Equal("0.7", actualTemp);
        }

        [Fact]
        public void NetatmoUnitCurrentReading()
        {
            string responseText = "{ \"access_token\":\"544cf4071c7759831d94cdf9|fcb30814afbffd0e39381e74fe38a59a\",\"refresh_token\":\"544cf4071c7759831d94cdf9|2b2c270c1208e8f67d3bd3891e395e1a\",\"scope\":[\"read_station\"],\"expires_in\":10800,\"expire_in\":10800}";
            string deviceResponse = File.ReadAllText("netatmodeviceresponse.json");
            ApiStub apiNetatmoStub = new ApiStub();
            apiNetatmoStub.Post(
                "/oauth2/token",
                (request, args) =>
                {
                    return responseText;
                }
                );
            apiNetatmoStub.Get(
                "/api/devicelist",
                (request, response) =>
                {
                    return deviceResponse;
                }
                );
            string responseReading = "{\"body\":[{\"beg_time\":1481372100,\"value\":[[0.7]]}],\"status\":\"ok\",\"time_exec\":0.034345865249634,\"time_server\":1481371970}";

            
            apiNetatmoStub.Get(
                "/api/getmeasure",
                (request, args) =>
                {
                    return responseReading;
                }
                );

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            ClimateItem actualReading = netatmoUnit.CurrentReading(configs);
            Assert.NotNull(actualReading);
            Assert.Equal("0.7", actualReading.IndoorValue);
            Assert.Equal("0.7", actualReading.OutdoorValue);
            Assert.NotNull(actualReading.TimeStamp);
            
            Assert.True(DateTime.Now >= actualReading.TimeStamp);
        }
    }
}
