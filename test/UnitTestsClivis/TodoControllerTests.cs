using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace UnitTestClivis
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class TodoControllersTest
    {
        private readonly ClimateController _climateController;
        public TodoControllersTest()
        {
            IClimateRepository repo = new ClimateRepository();            
            _climateController = new ClimateController(repo);
        }
         [Fact]
        public void TodoController_NotNull()
        {
            Assert.NotNull(_climateController);
        }

        [Fact]
        public void TodoController_Index()
        {
            IEnumerable<ClimateItem> res = _climateController.GetAll();
            Assert.NotNull(res);
        }

        [Fact]
        public void ValuesController_GetId_Returns_NotNull()
        {
            string res = _climateController.GetById("Nyckel").ToString();
                        
            Assert.NotNull(res);
        }

    }
}
