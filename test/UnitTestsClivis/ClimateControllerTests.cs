using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Clivis.Models.Netatmo;
using Clivis.Models.Nibe;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Stubbery;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.PlatformAbstractions;
using Clivis;
using System.Reflection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;


namespace ClivisTests
{


    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateControllerTests
    {
        Mock<IClimateSource> nibeMock = new Mock<IClimateSource>();
        Mock<IClimateSource> netatmoMock = new Mock<IClimateSource>();

        public IConfigurationRoot Configuration { get; }
        private readonly ClimateController _climateController;
        public ClimateControllerTests()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
            IOptions<AppKeyConfig> options = Options.Create(new AppKeyConfig()
            {
                UserName = Configuration["NETATMO_USERNAME"],
                Password = Configuration["NETATMO_PASSWORD"],
                NetatmoClientId = Configuration["NETATMO_CLIENTID"],
                NetatmoClientSecret = Configuration["NETATMO_CLIENTSECRET"],
                NibeClientId = Configuration["NIBE_ID"],
                NibeClientSecret = Configuration["NIBE_SECRET"],
                NibeRedirectURI = Configuration["NIBE_REDIRECTURL"],
                NibeHost = Configuration["NIBE_HOST"],
                NetatmoHost = Configuration["NETATMO_HOST"],
                BuildVersion = Configuration["BUILD_VERSION"]
            });
            ConcurrentDictionary<string, IClimateSource> sources = new ConcurrentDictionary<string, IClimateSource>();
            sources["Nibe"] = nibeMock.Object;
            sources["Netatmo"] = netatmoMock.Object;
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            ILogger<ClimateController> logger = new Logger<ClimateController>(loggerFactory);

            _climateController = new ClimateController(options, sources, logger);
        }

        [Fact]
        public void ClimateController_NotNull()
        {
            Assert.NotNull(_climateController);
        }

        [Fact]
        public void ClimateController_IClimateSources_are_of_correct_type()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();

            IConfigurationRoot Configuration = builder.Build();
            IOptions<AppKeyConfig> options = Options.Create(new AppKeyConfig()
            {
                UserName = Configuration["NETATMO_USERNAME"],
                Password = Configuration["NETATMO_PASSWORD"],
                NetatmoClientId = Configuration["NETATMO_CLIENTID"],
                NetatmoClientSecret = Configuration["NETATMO_CLIENTSECRET"],
                NibeClientId = Configuration["NIBE_ID"],
                NibeClientSecret = Configuration["NIBE_SECRET"],
                NibeRedirectURI = Configuration["NIBE_REDIRECTURL"],
                NibeHost = Configuration["NIBE_HOST"],
                NetatmoHost = Configuration["NETATMO_HOST"],
                BuildVersion = Configuration["BUILD_VERSION"]
        });

            ConcurrentDictionary<string, IClimateSource> sources = new ConcurrentDictionary<string, IClimateSource>();
            NibeUnit nibeUnit = new NibeUnit();
    
            sources["Nibe"] = nibeUnit;
            sources["Netatmo"] = new NetatmoUnit();
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            ILogger<ClimateController> logger = new Logger<ClimateController>(loggerFactory);
            ClimateController climateController = new ClimateController(options, sources, logger);

            Assert.IsType<NetatmoUnit>(climateController.netatmo);
            Assert.IsType<NibeUnit>(climateController.nibe);
        }

        [Fact]
        public void ClimateController_GetClimateSetsCodeAndReturnNotNull()
        {


            IActionResult res = _climateController.GetClimate("code", "12345");
            nibeMock.VerifySet(foo => foo.code = "code");

            Assert.NotNull(res);
            Assert.IsType<RedirectResult>(res);
        }

        private IHostingEnvironment CreateHostingEnvironment()
        {
            var hostingEnvironment = new HostingEnvironment();

            var appEnvironment = PlatformServices.Default.Application;

            var applicationName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            hostingEnvironment.Initialize(applicationName, appEnvironment.ApplicationBasePath, new WebHostOptions());

            return hostingEnvironment;
        }

        [Fact]
        public void ClimateController_GetById_WithNibe_As_Source_Calls_CurrentReading_WithConfigs()
        {
            ClimateItem item = new ClimateItem();
            nibeMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(item);


            IActionResult res = _climateController.GetBySource("Nibe");
            nibeMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());

        }

        [Fact]
        public void ClimateController_GetById_WithReading_As_Source_Calls_CurrentReading_WithConfigs()
        {
            ClimateItem nibeitem = new ClimateItem() { IndoorValue = "20", OutdoorValue = "10" };
            nibeMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(nibeitem);

            ClimateItem netatmoitem = new ClimateItem() { IndoorValue = "20", OutdoorValue = "10" };
            netatmoMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(netatmoitem);

            IActionResult res = _climateController.GetBySource("Reading");
            nibeMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());
            netatmoMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ClimateController_GetById_WithNibe_As_Source_Calls_CurrentReading_ThatReturnsNull()
        {
            ClimateItem item = null;

            // nibeMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(item);
            AppKeyConfig ac = new AppKeyConfig()
            {
                UserName = Configuration["NETATMO_USERNAME"],
                Password = Configuration["NETATMO_PASSWORD"],
                NetatmoClientId = Configuration["NETATMO_CLIENTID"],
                NetatmoClientSecret = Configuration["NETATMO_CLIENTSECRET"],
                NibeClientId = Configuration["NIBE_CLIENTID"],
                NibeClientSecret = Configuration["NIBE_CLIENTSECRET"],
                NibeRedirectURI = Configuration["NIBE_REDIRECTURL"],
                NibeHost = Configuration["NIBE_HOST"],
                NetatmoHost = Configuration["NETATMO_HOST"],
                BuildVersion = Configuration["BUILD_VERSION"]
            };
            nibeMock.Setup<ClimateItem>(x => x.CurrentReading(ac)).Returns(item);

            IActionResult res = _climateController.GetBySource("Nibe");
            nibeMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());

            Assert.IsType<RedirectResult>(res);
        }


        [Fact]
        public void ClimateController_GetById_With_Netatmo_As_Source_Calls_CurrentReading_WithConfigs()
        {
            ClimateItem item = new ClimateItem();
            netatmoMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(item);


            IActionResult res = _climateController.GetBySource("Netatmo");
            netatmoMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ClimateController_GetById_With_Netatmo_As_Source_Calls_CurrentReading_IsNullReturnNocontentResult()
        {
            ClimateItem item = null;
            netatmoMock.Setup<ClimateItem>(x => x.CurrentReading(It.IsAny<AppKeyConfig>())).Returns(item);


            IActionResult res = _climateController.GetBySource("Netatmo");
            Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(res);
            netatmoMock.Verify(x => x.CurrentReading(It.IsAny<AppKeyConfig>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ClimateController_GetById_With_Wrong_SourceName_Returns_null()
        {
            IActionResult res = _climateController.GetBySource("Nonexisting");
            Assert.NotNull(res);
            Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(res);
        }
    }
}
