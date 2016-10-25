using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clivis.Models
{
    public interface IClimateSource
    {
        string clientID { get; set; }
        string secret { get; set; }
        string userName { get; set; }
        string passWord { get; set; }
        void init(AppKeyConfig config);
        string outDoorTemperature { get; }
        string inDoorTemperature { get; }
    }
}
