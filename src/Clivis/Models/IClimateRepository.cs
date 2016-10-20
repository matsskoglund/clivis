using System.Collections.Generic;

namespace Clivis.Models
{
    public interface IClimateRepository
    {
        void Add(ClimateItem item);
        IEnumerable<ClimateItem> GetAll();
        ClimateItem Find(string key);
        ClimateItem Remove(string key);
        void Update(ClimateItem item);
    }
}