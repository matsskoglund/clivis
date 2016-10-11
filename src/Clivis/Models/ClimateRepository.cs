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
            Add(new ClimateItem { Name = "Item1" });
        }

        public IEnumerable<ClimateItem> GetAll()
        {
            return _items.Values;
        }

        public void Add(ClimateItem item)
        {
            item.Key = "Nyckel";
            _items[item.Key] = item;
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
            _items[item.Key] = item;
        }
    }
}