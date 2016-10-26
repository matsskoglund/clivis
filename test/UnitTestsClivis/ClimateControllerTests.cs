using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace ClivisTests
{
    

    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateControllerTests
    {
        public IConfigurationRoot Configuration { get; }
        private readonly ClimateController _climateController;
        public ClimateControllerTests()
        {
            IClimateRepository repo = new ClimateRepository();
             ConfigurationBuilder builder = new ConfigurationBuilder();
             builder.AddUserSecrets();
            
           
            Configuration = builder.Build();
            IOptions<AppKeyConfig> options = Options.Create(new AppKeyConfig()
            {    
                NetatmoUserName = Configuration["NetatmoUserName"],
                NetatmoPassword = Configuration["NetatmoPassword"],
                NetatmoClientId = Configuration["NetatmoClientId"],
                NetatmoClientSecret = Configuration["NetatmoClientSecret"]
             });

            _climateController = new ClimateController(repo, options);
        }
         [Fact]
        public void ClimateController_NotNull()
        {
            Assert.NotNull(_climateController);
        }

        [Fact]
        public void ClimateController_Index()
        {
            IEnumerable<ClimateItem> res = _climateController.GetAll();
            Assert.NotNull(res);
        }

        [Fact]
        public void ClimateController_Create()
        {
            Microsoft.AspNetCore.Mvc.CreatedAtRouteResult res = (Microsoft.AspNetCore.Mvc.CreatedAtRouteResult)_climateController.Create(new ClimateItem { Key = "Nyckel2", SourceName = "WeatherStation"});
            
            Assert.Equal(201, res.StatusCode);
        }

        [Fact]
        public void ClimateController_Create_with_null_result_in_bad_request()
        {
            //Microsoft.AspNetCore.Mvc.CreatedAtRouteResult res = (Microsoft.AspNetCore.Mvc.CreatedAtRouteResult)_climateController.Create(null);
            IActionResult res = _climateController.Create(null);

            Assert.IsType<BadRequestResult>(res);

            Assert.Equal(400, ((BadRequestResult) res).StatusCode);
        }

        [Theory]
        [InlineData("Nyckel")]
        public void ClimateController_GetId_Returns_NotNull(string key)
        {
            string res = _climateController.GetById(key).ToString();
                        
            Assert.NotNull(res);
        }


    }
}
