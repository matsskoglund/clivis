using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using System;

namespace ClivisTests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateItemTest
    {
        private readonly ClimateItem _climateItem;
        private DateTime timeStamp = DateTime.Now;

        public ClimateItemTest()
        {
            _climateItem = new ClimateItem();
            _climateItem.TimeStamp = timeStamp;
            _climateItem.OutdoorValue = "4.5";
            _climateItem.IndoorValue = "22.3";
        }

         [Fact]
        public void ClimateItem_NotNull()
        {
            Assert.NotNull(_climateItem);
        }

        [Fact]
        public void ClimateItem_OutdoorValueIsSet()
        {
            Assert.Equal("4.5",_climateItem.OutdoorValue);
            
        }

        [Fact]
        public void ClimateItem_IndoorValueIsSet()
        {
            Assert.Equal("22.3",_climateItem.IndoorValue);
            
        }


        [Fact]
        public void ClimateItem_TimeStampIsSet()
        {
            Assert.Equal(timeStamp, _climateItem.TimeStamp);

        }
    }
}
