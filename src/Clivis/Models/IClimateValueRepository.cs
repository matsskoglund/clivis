using System.Collections.Generic;

namespace Clivis.Models
{
    public interface IClimateValueRepository
    {
        void Add(ClimateValue item);
        IEnumerable<ClimateValue> GetAll();
        ClimateValue Find(string key);
        ClimateValue Remove(string key);
        void Update(ClimateValue item);
    }
}