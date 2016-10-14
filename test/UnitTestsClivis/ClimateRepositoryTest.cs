using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using System.Linq;

namespace UnitTestClivis
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateRepositoryTest
    {
        private readonly ClimateRepository _climateRepo;
        public ClimateRepositoryTest()
        {
            _climateRepo = new ClimateRepository();     
            ClimateItem item = new ClimateItem();
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
            Assert.Equal(1,list.Count());
        }

        [Fact]
        public void ClimateRepository_ExistingItemIsFound()
        {
            ClimateItem item = _climateRepo.Find("Nyckel");
            Assert.Equal("Item1", item.Name);
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
            item.Key = "ChangeKey";
            item.Name = "ToBeChanged";      
            _climateRepo.Add(item);

            // Make sure the item is in the repo
            ClimateItem existingItem = _climateRepo.Find("ChangeKey");
            Assert.Equal("ToBeChanged", existingItem.Name);

            // Change the item
            item.Name = "Changed";

            // Update the item in the repo
            _climateRepo.Update(item);

            // Get the item from the repo
            ClimateItem afterItem =  _climateRepo.Find("ChangeKey");
            
            // Check that the item name with the key has changed
            Assert.Equal("Changed",item.Name);
            Assert.Equal("ChangeKey",item.Key);

            // Cleanup, remove the item and make sure it is gone
            item = _climateRepo.Remove("ChangeKey");
            Assert.Null(_climateRepo.Find("ChangeKey"));
        }

      [Fact]
        public void ClimateRepository_RemovedItemIsGone()
        {
            ClimateItem item = new ClimateItem();
            item.Key = "RemoveKey";
            item.Name = "ToBeRemoved";      
           _climateRepo.Add(item);
 
            item = _climateRepo.Remove("RemoveKey");
            Assert.Equal("ToBeRemoved",item.Name);
            item = _climateRepo.Find("ToBeRemoved");
            Assert.Null(item);
        }

    }
}
