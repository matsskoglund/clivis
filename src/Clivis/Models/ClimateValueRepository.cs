using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Clivis.Models
{
    public class ClimateValueRepository : IClimateValueRepository
    {
        private static ConcurrentDictionary<string, ClimateValue> _items =
              new ConcurrentDictionary<string, ClimateValue>();

        public ClimateValueRepository(string climateItemKey)
        {
            Add(new ClimateValue { Key = "Value1", ClimateItemKey = climateItemKey, TimeStamp = System.DateTime.Now, ValueType = "Indoor", Value = "22"});
            Add(new ClimateValue { Key = "Value2", ClimateItemKey = climateItemKey, TimeStamp = System.DateTime.Now, ValueType = "Outdoor", Value = "6" });
        }

        public IEnumerable<ClimateValue> GetAll()
        {
            return _items.Values;
        }

        public void Add(ClimateValue item)
        {

            _items[item.Key] = item;
        }

        public ClimateValue Find(string key)
        {
            ClimateValue item;
            _items.TryGetValue(key, out item);
            return item;
        }

        public ClimateValue Remove(string key)
        {
            ClimateValue item;
            _items.TryRemove(key, out item);
            return item;
        }

        public void Update(ClimateValue item)
        {
            _items[item.Key] = item;
        }
    }
}