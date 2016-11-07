using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clivis.Models.Nibe
{
    public class NibeAuth
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public List<string> scope { get; set; }
        public int token_type { get; set; }
    }
}
