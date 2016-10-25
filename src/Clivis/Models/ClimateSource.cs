namespace Clivis.Models
{
    public interface IClimateSource
    {
         string clientID { get; set; }
         string secret { get; set; }
         string userName { get; set; }
         string passWord { get; set; }
         void init();
    }
    public class Netatmo:IClimateSource
    {
        public string clientID { get; set; }
        public string secret { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }       
        public void init()
        {
            
        }
    }
}