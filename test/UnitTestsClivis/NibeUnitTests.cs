using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clivis.Models.Nibe;
using System.IO;
using Clivis.Models;
using Stubbery;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;

namespace ClivisTests
{
    public class NibeUnitTests
    {
        private NibeUnit nibeUnit;
        private string codeFilePath = "code.txt";

        public NibeUnitTests()
        {
            nibeUnit = new NibeUnit() { clientId = "12345", code = null, passWord = "qwert", redirect_uri = "http://somuri", secret = "mysecret", userName = "myusername@user.se" };
            nibeUnit.CodeFilePath = codeFilePath;
        }
        [Fact]
        public void NibeUnit_code_setter_CodeIsWrittenToFile()
            {
            if (File.Exists(codeFilePath))
                File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted");
            
            nibeUnit.code = "newcode";
            Assert.True(File.Exists(codeFilePath));
            string fileContent = File.ReadAllText(codeFilePath);

            // Assert the file contents
            Assert.Equal("newcode", fileContent);

            // Assert that the variable is set in unit
            Assert.Equal("newcode", nibeUnit.code);

            // Cleanup
            File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted " + new FileInfo(codeFilePath).FullName);

        }
        [Fact]
        public void NibeUnit_code_setter_CodeIsNullAndNotWrittenToFile()
        {
            if (File.Exists(codeFilePath))
                File.Delete(codeFilePath);

            Assert.False(File.Exists(codeFilePath), "The code file could not be deleted");

            // Set code to null
            nibeUnit.code = null;

            // No file should have been created
            Assert.False(File.Exists(codeFilePath));
            
            // Assert that the variable is null
            Assert.Equal(null, nibeUnit.code);
        }

        [Fact]
        public void NibeUnit_code_setter_code_Is_Not_Null_And_CodeFilePath_is_null_throws_exception()
        {
            string saveCodePath = nibeUnit.CodeFilePath;

            nibeUnit.CodeFilePath = null;

            // Throws Exception
            Exception ex = Assert.Throws<InvalidOperationException>(() => nibeUnit.code = "newcode");

            Assert.Equal("The code file is nor set", ex.Message);
            // Restore
            nibeUnit.CodeFilePath = saveCodePath;
        }


        [Fact]
        public void NibeUnit_code_getter_CodeIsNull_Returned()
        {
            NibeUnit unit = new NibeUnit();

        // Assert that the variable is null
        Assert.Equal(null, unit.code);
        }

        [Fact]
        public void NibeUnit_code_getter_CodeIsNotNullAndFileExist_Code_From_File_Returned()
        {
            // Create a file with unique content
            DateTime codestamp = DateTime.Now;
            File.WriteAllText(codeFilePath, codestamp.ToString());

            // Read code
            string readCode = nibeUnit.code;

            
            // Assert that the code is correct
            Assert.Equal(codestamp.ToString(), nibeUnit.code);
        }


        [Fact]
        public void NibeUnit_Call_Init_When_code_Is_NULL_Throws_Exception()
        {
            Assert.Throws<Exception>(() => nibeUnit.init(null));           
        }

        [Fact]
        public void NibeUnit_Call_Init_When_code_Is_NotNull()
        {
            string fileContentExp = "{\"access_token\":\"AAEAAI3RKfqMqpUT6gPct8J4ysi2MvAITg9iJ - fydXOLDEP1DZ_9DbX3hh2Nw3DOydyc_IvBzNJW - k9uon0I4cxNuhhJJ5Z5OQEAUuB6er10aWLS7QnHAxvyAJFgsRUA9BBvmmA21MdBexw4JaKHaIswuxQwsOe3tBefPg8S_eEm44noPJIj_Zge4tgTZyTU1pIj5NksAm0T6i - tnf - wdrPg6AfWZHn1mNBBQnEosPNnq8OatCKK3CUunbCyNspB2v3p175WWSad2stb8Bo1nfvP8ZWebKIQFSYXNYLhYDd3T6EmxqVdIIj_Z6IFeqM4pTRndaIXrPdBCGRvNYCeA5puW2SkAQAAAAEAAB_Tqc5_JXDTzgaTS0amjWGCVvLo5ZV_5lIUMOxBfc7YlrLw0y2qhvUz3GwaMRx5WQdGdHhMkZpxzQPjiN - Zm2KGeyrTwFHj_fXFfxML3Gd_mQF5jrKmRcWwBXqTUDPwdOmPqXR94b6P4PqPzIXuoKvz_MRlrNfA1XmMCKagB8QsAdDmThu7QIR2gV5ENmJUcRHRY09XAAii4kYh6tyhvs8Zec7wgPRZ1Sq6aSOPYwdI3Ux3CRXUPgWxkGBbvCKPIu3keHJoZI - k1U81ha1AD5qj6RqMuM3m72VNXYuzSya62GfNP57BPupfgO_Igv9yWqf7jxPqy_XuAEYF0cNn5hAHDz3Kp9sRieCWW7fiiZwNghVp1 - jo_mdgyd1dPwqsh6UjOJqSuqWSEGWptpxZJ2bnB1akCmqpfKvgJgiV8Ilr68Tjw2uiMPGOlZF3b5T_uRszpTNFIfz4QpWdbbaHabeiBfit4oI3AqsCLEL3MU0W8Sk1QbxikEgON6v - 2lmkJ2t_iUGa3RXh3124QltUujywVVfeEJJupJjs1vRHZmD8\",\"token_type\":\"bearer\",\"expires_in\":1800,\"refresh_token\":\"8_Ln!IAAAAGQC4_hJShmgj10Be6CXXj6SJEJCobqeQMmvBdFp - flssQAAAAGiydTTibPGsB_03OYi - ASktAwRonG9sj0vHJpewUGGmGDxawAXVE4G5mpLHNcpezmDEFg2o3sXIRrdOlymY47itMwqTJyGCSxcoUw3OOVFiMA29VZWSTLjB_hCqMUhTTgxDAo1ykF - kjG - Q84X9xpdx1VEbyBK7LCMYR2h0fcrl0 - qV7MRAhJvKcy7YJ62CXfKm5Nq1PWJ4qTONFYRtL1Z5X8rJ_jLzLYFy3I4EykDqw\",\"scope\":\"READSYSTEM\"}";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Post(
                "/oauth/token",
                (request, args) =>
                {
                    return fileContentExp;
                }
                );
            apiNibeStub.Start();
            
            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            File.WriteAllText(codeFilePath, DateTime.Now.ToString());

            nibeUnit.init(configs);
            string fileContentAct = File.ReadAllText("nibeauth.json");

            Assert.Equal(fileContentExp, fileContentAct);
        }

        [Fact]
        public void NibeUnit_Call_Init_When_code_Is_NotNull_ButBadResponseFromApi()
        {

            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Request(HttpMethod.Post).
                IfRoute("/oauth/token").
                Response((request, args) =>
                {
                    return "{ Error, Not Authorized}";
                }
                ).StatusCode(StatusCodes.Status401Unauthorized);

            apiNibeStub.Start();
            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;
            Assert.Throws<Exception>(() => nibeUnit.init(configs));             
        }

        [Fact]
        public void NibeUnit_Refresh_Ok()
        {
            string fileContentExp = "{\"access_token\":\"AAEAAI3RKfqMqpUT6gPct8J4ysi2MvAITg9iJ - fydXOLDEP1DZ_9DbX3hh2Nw3DOydyc_IvBzNJW - k9uon0I4cxNuhhJJ5Z5OQEAUuB6er10aWLS7QnHAxvyAJFgsRUA9BBvmmA21MdBexw4JaKHaIswuxQwsOe3tBefPg8S_eEm44noPJIj_Zge4tgTZyTU1pIj5NksAm0T6i - tnf - wdrPg6AfWZHn1mNBBQnEosPNnq8OatCKK3CUunbCyNspB2v3p175WWSad2stb8Bo1nfvP8ZWebKIQFSYXNYLhYDd3T6EmxqVdIIj_Z6IFeqM4pTRndaIXrPdBCGRvNYCeA5puW2SkAQAAAAEAAB_Tqc5_JXDTzgaTS0amjWGCVvLo5ZV_5lIUMOxBfc7YlrLw0y2qhvUz3GwaMRx5WQdGdHhMkZpxzQPjiN - Zm2KGeyrTwFHj_fXFfxML3Gd_mQF5jrKmRcWwBXqTUDPwdOmPqXR94b6P4PqPzIXuoKvz_MRlrNfA1XmMCKagB8QsAdDmThu7QIR2gV5ENmJUcRHRY09XAAii4kYh6tyhvs8Zec7wgPRZ1Sq6aSOPYwdI3Ux3CRXUPgWxkGBbvCKPIu3keHJoZI - k1U81ha1AD5qj6RqMuM3m72VNXYuzSya62GfNP57BPupfgO_Igv9yWqf7jxPqy_XuAEYF0cNn5hAHDz3Kp9sRieCWW7fiiZwNghVp1 - jo_mdgyd1dPwqsh6UjOJqSuqWSEGWptpxZJ2bnB1akCmqpfKvgJgiV8Ilr68Tjw2uiMPGOlZF3b5T_uRszpTNFIfz4QpWdbbaHabeiBfit4oI3AqsCLEL3MU0W8Sk1QbxikEgON6v - 2lmkJ2t_iUGa3RXh3124QltUujywVVfeEJJupJjs1vRHZmD8\",\"token_type\":\"bearer\",\"expires_in\":1800,\"refresh_token\":\"8_Ln!IAAAAGQC4_hJShmgj10Be6CXXj6SJEJCobqeQMmvBdFp - flssQAAAAGiydTTibPGsB_03OYi - ASktAwRonG9sj0vHJpewUGGmGDxawAXVE4G5mpLHNcpezmDEFg2o3sXIRrdOlymY47itMwqTJyGCSxcoUw3OOVFiMA29VZWSTLjB_hCqMUhTTgxDAo1ykF - kjG - Q84X9xpdx1VEbyBK7LCMYR2h0fcrl0 - qV7MRAhJvKcy7YJ62CXfKm5Nq1PWJ4qTONFYRtL1Z5X8rJ_jLzLYFy3I4EykDqw\",\"scope\":\"READSYSTEM\"}";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Post(
                "/oauth/token",
                (request, args) =>
                {
                    return fileContentExp;
                }
                );
            apiNibeStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            File.WriteAllText(codeFilePath, DateTime.Now.ToString());

            NibeAuth res = nibeUnit.Refresh(configs);
            string resString = JsonConvert.SerializeObject(res);
            string fileContentAct = File.ReadAllText("nibeauth.json");
            NibeAuth resFile = JsonConvert.DeserializeObject<NibeAuth>(fileContentExp);
         //   Assert.Equal(fileContentExp, fileContentAct);
            Assert.Equal(fileContentExp, resString);
        }

        [Fact]
        public void NibeUnit_Refresh_NotOk()
        {
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Request(HttpMethod.Post).
                IfRoute("/oauth/token").
                Response((request, args) =>
                {
                    return "{ Error, Not Authorized}";
                }
                ).StatusCode(StatusCodes.Status401Unauthorized);

            apiNibeStub.Start();
            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;
            NibeAuth res = nibeUnit.Refresh(configs);
            Assert.Null(res);
        }

        [Fact]
        public void NibeUnit_GetReadingWithAccessCode_Is_Successful()
        {

            string reading = "[{\"parameterId\":40004,\"name\":\"outdoor_temperature\",\"title\":\"outdoor temp.\",\"designation\":\"BT1\",\"unit\":\"°C\",\"displayValue\":\"2.7°C\",\"rawValue\":27},{\"parameterId\":40033,\"name\":\"indoor_temperature\",\"title\":\"room temperature\",\"designation\":\"BT50\",\"unit\":\"°C\",\"displayValue\":\"22.7°C\",\"rawValue\":227}]";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Get(
                "/api/v1/systems/27401/parameters",
                (request, args) =>
                {
                    return reading;
                }
                );
            apiNibeStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            ClimateItem item = nibeUnit.GetReadingWithAccessCode("12345", configs);
            Assert.NotNull(item);
            Assert.Equal(item.IndoorValue, "22.7");
            Assert.Equal(item.OutdoorValue, "2.7");            
        }

        [Fact]
        public void NibeUnit_GetReadingWithAccessCode_Is_NotSuccessful()
        {
            //string reading = "[{\"parameterId\":40004,\"name\":\"outdoor_temperature\",\"title\":\"outdoor temp.\",\"designation\":\"BT1\",\"unit\":\"°C\",\"displayValue\":\"2.7°C\",\"rawValue\":27},{\"parameterId\":40033,\"name\":\"indoor_temperature\",\"title\":\"room temperature\",\"designation\":\"BT50\",\"unit\":\"°C\",\"displayValue\":\"22.7°C\",\"rawValue\":227}]";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Request(HttpMethod.Post).
                IfRoute("/oauth/token").
                Response((request, args) =>
                {
                    return "{ Error, Not Authorized}";
                }
                ).StatusCode(StatusCodes.Status401Unauthorized);
            apiNibeStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            ClimateItem item = nibeUnit.GetReadingWithAccessCode("12345", configs);
            Assert.Null(item);
        }


        [Fact]
        public void NibeUnit_CurrentReading_Is_SuccessfulWithAccessCode()
        {

            string reading = "[{\"parameterId\":40004,\"name\":\"outdoor_temperature\",\"title\":\"outdoor temp.\",\"designation\":\"BT1\",\"unit\":\"°C\",\"displayValue\":\"2.7°C\",\"rawValue\":27},{\"parameterId\":40033,\"name\":\"indoor_temperature\",\"title\":\"room temperature\",\"designation\":\"BT50\",\"unit\":\"°C\",\"displayValue\":\"22.7°C\",\"rawValue\":227}]";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Get(
                "/api/v1/systems/27401/parameters",
                (request, args) =>
                {
                    return reading;
                }
                );
            apiNibeStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            ClimateItem item = nibeUnit.CurrentReading(configs);
            Assert.NotNull(item);
            Assert.Equal(item.IndoorValue, "22.7");
            Assert.Equal(item.OutdoorValue, "2.7");
        }

        [Fact]
        public void NibeUnit_CurrentReading_Is_NotSuccessfulDueToMissingAuthFile()
        {
            string nibeauth = null;
            bool fileExist = false;
            if (File.Exists("nibeauth.json")) {
                nibeauth = File.ReadAllText("nibeauth.json");
                fileExist = true;
            }

            File.Delete("nibeauth.json");

            Assert.False(File.Exists("nibeauth.json"));

            
            AppKeyConfig configs = new AppKeyConfig();
            
            ClimateItem item = nibeUnit.CurrentReading(configs);
            Assert.Null(item);
            if (fileExist)
                File.WriteAllText("nibeauth.json", nibeauth);
        }

        [Fact]
        public void NibeUnit_CurrentReading_Is_SuccessfulWithRefresh()
        {
            Assert.True(File.Exists("nibeauth.json"));
            string reading = "[{\"parameterId\":40004,\"name\":\"outdoor_temperature\",\"title\":\"outdoor temp.\",\"designation\":\"BT1\",\"unit\":\"°C\",\"displayValue\":\"2.7°C\",\"rawValue\":27},{\"parameterId\":40033,\"name\":\"indoor_temperature\",\"title\":\"room temperature\",\"designation\":\"BT50\",\"unit\":\"°C\",\"displayValue\":\"22.7°C\",\"rawValue\":227}]";
            string fileContentExp = "{\"access_token\":\"AAEAAI3RKfqMqpUT6gPct8J4ysi2MvAITg9iJ - fydXOLDEP1DZ_9DbX3hh2Nw3DOydyc_IvBzNJW - k9uon0I4cxNuhhJJ5Z5OQEAUuB6er10aWLS7QnHAxvyAJFgsRUA9BBvmmA21MdBexw4JaKHaIswuxQwsOe3tBefPg8S_eEm44noPJIj_Zge4tgTZyTU1pIj5NksAm0T6i - tnf - wdrPg6AfWZHn1mNBBQnEosPNnq8OatCKK3CUunbCyNspB2v3p175WWSad2stb8Bo1nfvP8ZWebKIQFSYXNYLhYDd3T6EmxqVdIIj_Z6IFeqM4pTRndaIXrPdBCGRvNYCeA5puW2SkAQAAAAEAAB_Tqc5_JXDTzgaTS0amjWGCVvLo5ZV_5lIUMOxBfc7YlrLw0y2qhvUz3GwaMRx5WQdGdHhMkZpxzQPjiN - Zm2KGeyrTwFHj_fXFfxML3Gd_mQF5jrKmRcWwBXqTUDPwdOmPqXR94b6P4PqPzIXuoKvz_MRlrNfA1XmMCKagB8QsAdDmThu7QIR2gV5ENmJUcRHRY09XAAii4kYh6tyhvs8Zec7wgPRZ1Sq6aSOPYwdI3Ux3CRXUPgWxkGBbvCKPIu3keHJoZI - k1U81ha1AD5qj6RqMuM3m72VNXYuzSya62GfNP57BPupfgO_Igv9yWqf7jxPqy_XuAEYF0cNn5hAHDz3Kp9sRieCWW7fiiZwNghVp1 - jo_mdgyd1dPwqsh6UjOJqSuqWSEGWptpxZJ2bnB1akCmqpfKvgJgiV8Ilr68Tjw2uiMPGOlZF3b5T_uRszpTNFIfz4QpWdbbaHabeiBfit4oI3AqsCLEL3MU0W8Sk1QbxikEgON6v - 2lmkJ2t_iUGa3RXh3124QltUujywVVfeEJJupJjs1vRHZmD8\",\"token_type\":\"bearer\",\"expires_in\":1800,\"refresh_token\":\"8_Ln!IAAAAGQC4_hJShmgj10Be6CXXj6SJEJCobqeQMmvBdFp - flssQAAAAGiydTTibPGsB_03OYi - ASktAwRonG9sj0vHJpewUGGmGDxawAXVE4G5mpLHNcpezmDEFg2o3sXIRrdOlymY47itMwqTJyGCSxcoUw3OOVFiMA29VZWSTLjB_hCqMUhTTgxDAo1ykF - kjG - Q84X9xpdx1VEbyBK7LCMYR2h0fcrl0 - qV7MRAhJvKcy7YJ62CXfKm5Nq1PWJ4qTONFYRtL1Z5X8rJ_jLzLYFy3I4EykDqw\",\"scope\":\"READSYSTEM\"}";
            ApiStub apiNibeStub = new ApiStub();
            apiNibeStub.Post(   
                "/oauth/token",
                (request, args) =>
                {
                    return fileContentExp;
                }
                );

                      
         /*   apiNibeStub.Get(
                "/api/v1/systems/27401/parameters",
                (request, args) =>
                {
                    return reading;
                }
                );*/

            //  string refreshToken = "8_Ln!IAAAAEvNGW1MzKQh8toJ6TV5WdVKuxJonjN61Z5EAo-KiAjasQAAAAGHs3grS10tACRrl8OiGRrCizwJga5ILsEoqVOluvCkb92Dp_s_KuSAtUSIOpgNgWdQGBB5z_1outPSbbZfEJHp7QZRgT2aLI1rOgYNvBAvMATjUat75SYq0ODCrXpkLZVZQB7PwCEypSWW5WBRoARxmvXwhHXtavrAb1ab5H6ASvjSapjuRzURGmVTku8-kOgw8yBl2P994EDvaL5DE1OSfePv6DElVj8OvxjayTTenA";
            


            //.IfHeader("refresh_token", refreshToken);
            apiNibeStub.Start();

            AppKeyConfig configs = new AppKeyConfig();
            configs.NibeHost = apiNibeStub.Address;

            ClimateItem item = nibeUnit.CurrentReading(configs);
            Assert.Null(item);
            /*Assert.Equal(item.IndoorValue, "22.7");
            Assert.Equal(item.OutdoorValue, "2.7");*/
        }
        
    }
}
