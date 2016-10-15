using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace UnitTestClivis
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateControllerTests
    {
        private readonly ClimateController _climateController;
        public ClimateControllerTests()
        {
            IClimateRepository repo = new ClimateRepository();            
            _climateController = new ClimateController(repo);
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

        [Theory]
        [InlineData("Nyckel")]
        public void ClimateController_GetId_Returns_NotNull(string key)
        {
            string res = _climateController.GetById(key).ToString();
                        
            Assert.NotNull(res);
        }

    }
}
