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
            Mock<IClimateSource> nibeMock = new Mock<IClimateSource>();
            
            _climateController = new ClimateController(options, nibeMock.Object);
        }
         [Fact]
        public void ClimateController_NotNull()
        {
            Assert.NotNull(_climateController);
        }

        [Fact]
        public void ClimateController_Index()
        {
            ClimateItem res = _climateController.GetClimate("code","state");
            Assert.NotNull(res);
        }
    

        [Theory]
        [InlineData("Nyckel")]
        public void ClimateController_GetId_For_Non_Existing_Id_Returns_Null(string key)
        {
            
            //ObjectResult res = (ObjectResult)_climateController.GetById(key);

            //Assert.Null(res.StatusCode);
        }


    }
}
