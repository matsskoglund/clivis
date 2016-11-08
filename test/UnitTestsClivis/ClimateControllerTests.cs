using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Moq;
using System;

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
            // Mock the repo
            //            IClimateRepository repo = new ClimateRepository();
            Mock<IClimateRepository> repoMock = new Mock<IClimateRepository>();
            Dictionary<string, ClimateItem> list = new Dictionary<string,ClimateItem>();
            
            list.Add("one", new ClimateItem { TimeStamp = DateTime.Now, IndoorValue = "22.0", OutdoorValue = "10.5" });
            
            list.Add("two", new ClimateItem { TimeStamp = DateTime.Now, IndoorValue = "22.0", OutdoorValue = "10.5" });

            repoMock.Setup(x => x.GetAll()).Returns(list.Values);
        

             ConfigurationBuilder builder = new ConfigurationBuilder();
             builder.AddUserSecrets();
            

           
            Configuration = builder.Build();
            IOptions<AppKeyConfig> options = Options.Create(new AppKeyConfig()
            {    
                UserName = Configuration["NetatmoUserName"],
                Password = Configuration["NetatmoPassword"],
                ClientId = Configuration["NetatmoClientId"],
                ClientSecret = Configuration["NetatmoClientSecret"]
             });

            _climateController = new ClimateController(repoMock.Object, options);
        }
         [Fact]
        public void ClimateController_NotNull()
        {
            Assert.NotNull(_climateController);
        }

        [Fact]
        public void ClimateController_Index()
        {
            ClimateItem res = _climateController.GetClimate();
            Assert.NotNull(res);
        }

        [Fact]
        public void ClimateController_Create()
        {
            Microsoft.AspNetCore.Mvc.CreatedAtRouteResult res = (Microsoft.AspNetCore.Mvc.CreatedAtRouteResult)_climateController.Create(new ClimateItem { TimeStamp=DateTime.Now, IndoorValue="18.6",OutdoorValue="21.2"});
            
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
        public void ClimateController_GetId_For_Non_Existing_Id_Returns_Null(string key)
        {
            
            ObjectResult res = (ObjectResult)_climateController.GetById(key);

            Assert.Null(res.StatusCode);
        }


    }
}
