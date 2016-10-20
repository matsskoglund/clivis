using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;

namespace ClivisTests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateValueTest
    {
        private readonly ClimateValue _climateValue;

        public ClimateValueTest()
        {
            _climateValue = new ClimateValue();
            _climateValue.ClimateItemKey = "Nibe";
            _climateValue.TimeStamp = System.DateTime.Now;
            _climateValue.ValueType = "Indoor";
            _climateValue.Value = "22";
        }

        [Fact]
        public void ClimateItem_NotNull()
        {
            Assert.NotNull(_climateValue);
        }

        [Fact]
        public void ClimateItem_TimeStampIsSet()
        {
            Assert.NotNull(_climateValue.TimeStamp);

        }

        [Fact]
        public void ClimateItem_KeyIsSet()
        {
            Assert.Equal("Nibe", _climateValue.ClimateItemKey);

        }
         [Fact]
         public void ClimateValue_ValueIsSet()
         {
             Assert.Equal("22", _climateValue.Value);

         }
         [Fact]
         public void ClimateValue_ValueTypeIsSet()
         {
             Assert.Equal("Indoor", _climateValue.ValueType);
         }
    }
}
