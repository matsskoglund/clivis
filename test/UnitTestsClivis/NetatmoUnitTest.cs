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
        // private string fileDir = "test/UnitTestsClivis/";

        private string netatmoData = "{\"body\":{\"modules\":[{\"_id\":\"02:00:00:05:51:92\",\"date_setup\":{\"sec\":1426265387,\"usec\":394000},\"main_device\":\"70:ee:50:04:79:2a\",\"module_name\":\"Outdoor module\",\"type\":\"NAModule1\",\"firmware\":44,\"last_message\":1481369097,\"last_seen\":1481369065,\"rf_status\":68,\"battery_vp\":5764,\"dashboard_data\":{\"time_utc\":1481369065,\"Temperature\":0.8,\"Humidity\":93,\"date_max_temp\":1481326466,\"date_min_temp\":1481353071,\"min_temp\":0.3,\"max_temp\":2.6},\"data_type\":[\"Temperature\",\"Humidity\"]},{\"_id\":\"05:00:00:00:6b:b4\",\"date_setup\":{\"sec\":1415294128,\"usec\":848000},\"main_device\":\"70:ee:50:04:79:2a\",\"module_name\":\"Regnmodul\",\"type\":\"NAModule3\",\"firmware\":8,\"last_message\":1481369097,\"last_seen\":1481369097,\"rf_status\":77,\"battery_vp\":5856,\"dashboard_data\":{\"time_utc\":1481369085,\"Rain\":0,\"sum_rain_24\":0,\"sum_rain_1\":0},\"data_type\":[\"Rain\"]},{\"_id\":\"02:00:00:21:0d:9e\",\"date_setup\":{\"sec\":1479736242,\"usec\":897000},\"main_device\":\"70:ee:50:21:11:1c\",\"module_name\":\"Outdoor\",\"type\":\"NAModule1\",\"firmware\":44,\"last_message\":1481368884,\"last_seen\":1481368865,\"rf_status\":77,\"battery_vp\":6110,\"dashboard_data\":{\"time_utc\":1481368865,\"Temperature\":0.7,\"Humidity\":92,\"date_max_temp\":1481324416,\"date_min_temp\":1481354920,\"min_temp\":0.5,\"max_temp\":2.8},\"data_type\":[\"Temperature\",\"Humidity\"]},{\"_id\":\"05:00:00:02:e9:3e\",\"date_setup\":{\"sec\":1479737473,\"usec\":780000},\"main_device\":\"70:ee:50:21:11:1c\",\"module_name\":\"Regnm\u00e4taren\",\"type\":\"NAModule3\",\"firmware\":8,\"last_message\":1481368885,\"last_seen\":1481368878,\"rf_status\":67,\"battery_vp\":6188,\"dashboard_data\":{\"time_utc\":1481368865,\"Rain\":0,\"sum_rain_24\":0.101,\"sum_rain_1\":0},\"data_type\":[\"Rain\"]}],\"devices\":[{\"_id\":\"70:ee:50:04:79:2a\",\"access_code\":\"FSMGhkZJkMSMq7AP0C\",\"alarm_config\":{\"default_alarm\":[{\"db_alarm_number\":0},{\"db_alarm_number\":1},{\"db_alarm_number\":2},{\"db_alarm_number\":6},{\"db_alarm_number\":4},{\"db_alarm_number\":5},{\"db_alarm_number\":7},{\"db_alarm_number\":22}],\"personnalized\":[{\"threshold\":10,\"data_type\":1,\"direction\":1,\"db_alarm_number\":9},{\"threshold\":25,\"data_type\":1,\"direction\":0,\"db_alarm_number\":8}]},\"cipher_id\":\"enc:16:5jkbDbwZi0lPx5wjTbOMbqSBRHiguL15jUhoxwI3sSN56X4U05KPTfxRQn\\/QGSrX\",\"co2_calibrating\":false,\"date_setup\":{\"sec\":1414329622,\"usec\":147000},\"firmware\":124,\"invitation_disable\":false,\"last_status_store\":1481369098,\"last_upgrade\":1440038669,\"meteo_alarms\":[{\"_id\":{\"$id\":\"559ce25f1c7759477a8b456b\"},\"area\":\"Stockholm\",\"begin\":1436336880,\"end\":1436423280,\"title\":\"__MA_ALARM_RAIN_TITLE\",\"type\":\"ALARM_RAIN\",\"text_field\":\"__MA_ALARM_RAIN_LEVEL_1\",\"level\":1,\"url\":\"http:\\/\\/www.meteoalarm.eu\\/handheld.php?level=2&area=SE021&day=0\",\"descr\":\"Under eftermiddagen passerar ett omr\u00e5de med lokalt kraftigt regn som p\u00e5 kort tid kan ge ca 35 mm.\",\"status\":\"new\",\"alarm_id\":196544,\"time_generated\":1436344927,\"origin\":\"meteoalarm\"}],\"module_name\":\"Indoor\",\"modules\":[\"02:00:00:05:51:92\",\"05:00:00:00:6b:b4\"],\"place\":{\"altitude\":42,\"city\":\"Huddinge\",\"country\":\"SE\",\"geoip_city\":\"Huddinge\",\"improveLocProposed\":true,\"location\":[17.976020288885,59.261200115053],\"timezone\":\"Europe\\/Stockholm\"},\"station_name\":\"Sn\u00e4ttringev\u00e4gen\",\"type\":\"NAMain\",\"wifi_status\":31,\"dashboard_data\":{\"AbsolutePressure\":998.8,\"time_utc\":1481369084,\"Noise\":54,\"Temperature\":22.1,\"Humidity\":35,\"Pressure\":1003.7,\"CO2\":690,\"date_max_temp\":1481324689,\"date_min_temp\":1481359433,\"min_temp\":21.8,\"max_temp\":22.6},\"data_type\":[\"Temperature\",\"CO2\",\"Humidity\",\"Noise\",\"Pressure\"]},{\"_id\":\"70:ee:50:21:11:1c\",\"cipher_id\":\"enc:16:zvpKru593VJIHaq9tD6r7wkwzp1wGdgRJOiTanl8tLCrkoSdG2zLGrbv3aSVVh\\/\\/\",\"co2_calibrating\":false,\"date_setup\":{\"sec\":1479736281,\"usec\":868000},\"firmware\":124,\"invitation_disable\":false,\"last_status_store\":1481368891,\"last_upgrade\":1479736258,\"module_name\":\"Indoor\",\"modules\":[\"02:00:00:21:0d:9e\",\"05:00:00:02:e9:3e\"],\"place\":{\"altitude\":16,\"city\":\"Eker\u00f6\",\"country\":\"SE\",\"improveLocProposed\":true,\"location\":[17.7922004,59.2883469],\"timezone\":\"Europe\\/Stockholm\"},\"station_name\":\"Tr\u00e4dg\u00e5rdsv\u00e4gen\",\"type\":\"NAMain\",\"wifi_status\":49,\"read_only\":true,\"dashboard_data\":{\"AbsolutePressure\":1006.8,\"time_utc\":1481368873,\"Noise\":40,\"Temperature\":19.1,\"Humidity\":39,\"Pressure\":1008.7,\"CO2\":559,\"date_max_temp\":1481324437,\"date_min_temp\":1481354662,\"min_temp\":18.2,\"max_temp\":20.8},\"data_type\":[\"Temperature\",\"CO2\",\"Humidity\",\"Noise\",\"Pressure\"]}]},\"status\":\"ok\",\"time_exec\":0.075790882110596,\"time_server\":1481369223}";
        NetatmoUnit netatmoUnit = new NetatmoUnit();
        public NetatmoUnitTests()
        {

        }

        [Fact]
        public void NetatmoUnitLoginSuccessFul()
        {
            string responseText = "{ \"access_token\":\"544cf4071c7759831d94cdf9|fcb30814afbffd0e39381e74fe38a59a\",\"refresh_token\":\"544cf4071c7759831d94cdf9|2b2c270c1208e8f67d3bd3891e395e1a\",\"scope\":[\"read_station\"],\"expires_in\":10800,\"expire_in\":10800}";
            string deviceResponse = netatmoData;
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

            apiNetatmoStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NetatmoHost = apiNetatmoStub.Address;

            netatmoUnit.init(configs);

        }

        [Fact]
        public void NetatmoUnitLoginFailDueToLogin()
        {
            //string deviceResponse = File.ReadAllText(fileDir + "netatmodeviceresponse.json");
            string deviceResponse = netatmoData;
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
            Assert.Equal("Could not login, response: ", ex.Message);
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
            //string deviceResponse = File.ReadAllText(fileDir + "netatmodeviceresponse.json");
            string deviceResponse = netatmoData;
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
        [Fact]
        public void NetatmoUnitCurrentReadingDataCouldNoBeRead()
        {
            string responseText = "{ \"access_token\":\"544cf4071c7759831d94cdf9|fcb30814afbffd0e39381e74fe38a59a\",\"refresh_token\":\"544cf4071c7759831d94cdf9|2b2c270c1208e8f67d3bd3891e395e1a\",\"scope\":[\"read_station\"],\"expires_in\":10800,\"expire_in\":10800}";
            //string deviceResponse = File.ReadAllText(fileDir + "netatmodeviceresponse.json");
            string deviceResponse = netatmoData;
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
            string responseReading = null;


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
            Assert.Null(actualReading);
        }

    }

}
