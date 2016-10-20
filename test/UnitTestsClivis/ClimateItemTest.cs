using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace ClivisTests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateItemTest
    {
        private readonly ClimateItem _climateItem;
        public ClimateItemTest()
        {
            _climateItem = new ClimateItem();           
           _climateItem.SourceName ="Heatpump"; 
            _climateItem.Key = "Nyckel";
        }

         [Fact]
        public void ClimateItem_NotNull()
        {
            Assert.NotNull(_climateItem);
        }

        [Fact]
        public void ClimateItem_SourceNameIsSet()
        {
            Assert.Equal("Heatpump",_climateItem.SourceName);
            
        }

        [Fact]
        public void ClimateItem_KeyIsSet()
        {
            Assert.Equal("Nyckel",_climateItem.Key);
            
        }       
    }
}
