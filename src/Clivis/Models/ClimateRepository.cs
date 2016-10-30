using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Clivis.Models
{
    public class ClimateRepository : IClimateRepository
    {
        private static ConcurrentDictionary<string, ClimateItem> _items =
              new ConcurrentDictionary<string, ClimateItem>();

        public ClimateRepository()
        {
            Add(new ClimateItem { TimeStamp = DateTime.Now, IndoorValue = "22.0", OutdoorValue="10.5"});
            Add(new ClimateItem { TimeStamp = DateTime.Now, IndoorValue = "22.0", OutdoorValue = "10.5" });
        }

        public IEnumerable<ClimateItem> GetAll()
        {
            return _items.Values;
        }

        public void Add(ClimateItem item)
        {

            _items[item.TimeStamp.ToString()] = item;
        }

        public ClimateItem Find(string key)
        {
            ClimateItem item;
            _items.TryGetValue(key, out item);
            return item;
        }

        public ClimateItem Remove(string key)
        {
            ClimateItem item;
            _items.TryRemove(key, out item);
            return item;
        }

        public void Update(ClimateItem item)
        {
            _items[item.TimeStamp.ToString()] = item;
        }

        public ClimateItem Latest(AppKeyConfig config)
        {
            return new ClimateItem() { TimeStamp = DateTime.Now, IndoorValue = "22.0", OutdoorValue = "10.5" };
        }
    }
}