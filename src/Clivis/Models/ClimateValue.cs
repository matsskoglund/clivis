using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clivis.Models
{
    public class ClimateValue
    {
        public string Value { get; set; } = "0";

        public string ValueType { get; set; } = "Indoor";

        public string ClimateItemKey { get ; set; }

        public DateTime TimeStamp { get; set;  } 

        public string Key { get; set;  }
    }
}
