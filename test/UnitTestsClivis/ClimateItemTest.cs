using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace UnitTestClivis
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateItemTest
    {
        private readonly ClimateItem _climateItem;
        public ClimateItemTest()
        {
           ClimateItem item = new ClimateItem();           
           
        }
         [Fact]
        public void ClimateItem_NotNull()
        {
            Assert.NotNull(_climateItem);
        }
    }
}
