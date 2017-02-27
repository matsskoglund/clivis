namespace Clivis.Models
{
    public interface IClimateSource
    {
        string code { get; set; }
        string clientId { get; set; }
        string secret { get; set; }
        string userName { get; set; }
        string passWord { get; set; }
        string CodeFilePath { get; set; }
        void init(AppKeyConfig config);
        ClimateItem CurrentReading(AppKeyConfig AppConfigs);
        
    }
}
