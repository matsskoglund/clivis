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
            _climateItem = new ClimateItem();           
           _climateItem.Name ="Namn";
           _climateItem.Key = "Nyckel";
        }

         [Fact]
        public void ClimateItem_NotNull()
        {
            Assert.NotNull(_climateItem);
        }

        [Fact]
        public void ClimateItem_NameIsSet()
        {
            Assert.Equal("Namne",_climateItem.Name);
            
        }

        [Fact]
        public void ClimateItem_KeyIsSet()
        {
            Assert.Equal("Nyckel",_climateItem.Key);
            
        }
    }
}
