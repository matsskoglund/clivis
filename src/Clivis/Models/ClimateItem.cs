using System;
using System.Globalization;

namespace Clivis.Models
{
    public class ClimateItem
    {
        public DateTime TimeStamp { get; set; }
        public string OutdoorValue { get; set; }
        public string IndoorValue { get; set; }

        public static string Meanvalue(string left, string right)
        {
            if ((left == null) && (right != null))
                return right;

            if ((right == null) && (left != null))
                return left;

            if ((left == null) && (right == null))
                throw new ArgumentNullException("Both parameters cannot be null");

            double leftIndoorValue = Double.Parse(left, System.Globalization.NumberStyles.AllowDecimalPoint| System.Globalization.NumberStyles.AllowLeadingSign , CultureInfo.InvariantCulture);
            double rightIndoorValue = Double.Parse(right, System.Globalization.NumberStyles.AllowDecimalPoint| System.Globalization.NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            double indoorValue = (leftIndoorValue + rightIndoorValue) / 2;
            string stringFormatedValue = string.Format("{0:0.0}", indoorValue);
            string stringPointed = stringFormatedValue.Replace(",", ".");
            return stringPointed;
        }  
        
        public static string MeanOutdoorValue(ClimateItem left, ClimateItem right)
        {
            return ClimateItem.Meanvalue(left.OutdoorValue, right.OutdoorValue);
        }

        public static string MeanIndoorValue(ClimateItem left, ClimateItem right)
        {
            return ClimateItem.Meanvalue(left.IndoorValue, right.IndoorValue);
        }

        public static ClimateItem ClimateMeanValues(ClimateItem left, ClimateItem right)
        {
            string indoorMean = ClimateItem.MeanIndoorValue(left, right);
            string outDoorMean = ClimateItem.MeanOutdoorValue(left, right);
            ClimateItem mean = new ClimateItem() { IndoorValue = indoorMean, OutdoorValue = outDoorMean, TimeStamp = DateTime.Now };
            return mean;
        }
    }
}