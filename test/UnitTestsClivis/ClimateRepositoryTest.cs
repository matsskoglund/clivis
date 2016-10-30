using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using System.Linq;
using System;

namespace ClivisTests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateRepositoryTest
    {
        private DateTime timeStamp = DateTime.Now;
        private readonly ClimateRepository _climateRepo;
        public ClimateRepositoryTest()
        {
            _climateRepo = new ClimateRepository();     
            ClimateItem item = new ClimateItem { TimeStamp = timeStamp, IndoorValue = "20.4", OutdoorValue = "11.6" };
            _climateRepo.Add(item);
       }

         [Fact]
        public void ClimateRepository_NotNull()
        {
            Assert.NotNull(_climateRepo);
        }

        [Fact]
        public void ClimateRepository_GetAllReturnsAllItems()
        {
            IEnumerable<ClimateItem> list = _climateRepo.GetAll();
         //   Assert.Equal(1,list.Count());
        }

        [Fact]
        public void ClimateRepository_ExistingItemIsFound()
        {
            ClimateItem item = _climateRepo.Find(timeStamp.ToString());
            Assert.Equal("20.4", item.IndoorValue);
        }

        [Fact]
        public void ClimateRepository_NonExistingItemReturnsNull()
        {
            ClimateItem item = _climateRepo.Find("Nonexist");
            Assert.Null(item);
        }

        [Fact]
        public void ClimateRepository_ExistingItemUpdatedItemIsChanged()
        {
            // Create and add a new item that should be changed
            ClimateItem item = new ClimateItem();
            DateTime newKey = DateTime.Now;
            item.TimeStamp = DateTime.Now;
            item.IndoorValue = "11.9";
            item.OutdoorValue = "5.3";
            _climateRepo.Add(item);

            // Make sure the item is in the repo
            ClimateItem existingItem = _climateRepo.Find(newKey.ToString());
            Assert.Equal("11.9", existingItem.IndoorValue);

            // Change the item
            item.IndoorValue = "28.3";

            // Update the item in the repo
            _climateRepo.Update(item);

            // Get the item from the repo
            ClimateItem afterItem =  _climateRepo.Find(newKey.ToString());
            
            // Check that the item name with the key has changed
            Assert.Equal("28.3", item.IndoorValue);
            Assert.Equal(newKey.ToString(),item.TimeStamp.ToString());

            // Cleanup, remove the item and make sure it is gone
            item = _climateRepo.Remove(newKey.ToString());
            Assert.Null(_climateRepo.Find(newKey.ToString()));
        }

      [Fact]
        public void ClimateRepository_RemovedItemIsGone()
        {
            ClimateItem item = new ClimateItem();
            DateTime timeStamp = DateTime.Now;
            item.TimeStamp = timeStamp;
            item.IndoorValue = "19.0";      
           _climateRepo.Add(item);
 
            item = _climateRepo.Remove(timeStamp.ToString());
            Assert.Equal("19.0",item.IndoorValue);
            item = _climateRepo.Find(timeStamp.ToString());
            Assert.Null(item);
        }

    }
}
