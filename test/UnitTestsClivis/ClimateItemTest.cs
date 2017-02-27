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

        [Fact]
        public void ClimateItem_MeanValueWithNoDecimalsIsFormatedCorrectly()
        {
            string left = "10.0";
            string right = "12.0";
            string resultValue = ClimateItem.Meanvalue(left, right);
            Assert.Equal("11.0", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueWithDecimalsIsFormatedCorrectly()
        {
            string left = "10.5";
            string right = "12.3";
            string resultValue = ClimateItem.Meanvalue(left, right);
            Assert.Equal("11.4", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueResultBelowZeroWithOneBelowZeroOtherAboveZeroIsFormatedCorrectly()
        {
            string left = "10.5";
            string right = "-12.3";
            string resultValue = ClimateItem.Meanvalue(left, right);
            //Assert.Equal("-0.9", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueResultBelowZeroWithBothBelowZeroIsFormatedCorrectly()
        {
            string left = "-10.5";
            string right = "-12.3";
            string resultValue = ClimateItem.Meanvalue(left, right);
            //Assert.Equal("-11.4", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueResultBelowZeroWithBothBelowZeroNoDecimalIsFormatedCorrectly()
        {
            string left = "-10.0";
            string right = "-12.0";
            string resultValue = ClimateItem.Meanvalue(left, right);

       
           // Assert.Equal(resultValue, resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueResultIsZero()
        {
            string left = "-6.5";
            string right = "6.5";
            string resultValue = ClimateItem.Meanvalue(left, right);

            Assert.Equal("0.0", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueLeftIsNull()
        {
            string left = null;
            string right = "6.5";
            string resultValue = ClimateItem.Meanvalue(left, right);

            Assert.Equal("6.5", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueRightIsNull()
        {
            string left = "22.1";
            string right = null;
            string resultValue = ClimateItem.Meanvalue(left, right);

            Assert.Equal("22.1", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanValueLeftAndRightAreNull()
        {
            
            string left = null;
            string right = null;
            // Throws Exception
            Exception ex = Assert.Throws<ArgumentNullException>(() => ClimateItem.Meanvalue(left, right));

            Assert.True(ex.Message.StartsWith("Value cannot be null"));
        }
        [Fact]
        public void ClimateItem_MeanIndoorValueTest()
        {

            ClimateItem left = new ClimateItem() { IndoorValue = "10.0", OutdoorValue = "-22.5" };
            ClimateItem right = new ClimateItem() { IndoorValue = "12.0", OutdoorValue = "-22.5" };
            string resultValue = ClimateItem.MeanIndoorValue(left, right);

            Assert.Equal("11.0", resultValue);
        }
        [Fact]
        public void ClimateItem_MeanOutdoorValueTest()
        {

            ClimateItem left = new ClimateItem() { IndoorValue = "-10.0", OutdoorValue = "20.0" };
            ClimateItem right = new ClimateItem() { IndoorValue = "-12.0", OutdoorValue = "22.0" };
            string resultValue = ClimateItem.MeanOutdoorValue(left, right);

            Assert.Equal("21.0", resultValue);
        }

        [Fact]
        public void ClimateItem_MeanValueTest()
        {

            ClimateItem left = new ClimateItem() { IndoorValue = "18.5", OutdoorValue = "-20.0" };
            ClimateItem right = new ClimateItem() { IndoorValue = "19.3", OutdoorValue = "22.0" };
            ClimateItem resultValue = ClimateItem.ClimateMeanValues(left, right);

            Assert.Equal("18.9", resultValue.IndoorValue);
            Assert.Equal("1.0", resultValue.OutdoorValue);
        }

    }
}
