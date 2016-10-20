using Xunit;
using Clivis.Controllers;
using System.Collections.Generic;
using Clivis.Models;
using System.Linq;

namespace ClivisTests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class ClimateValueRepositoryTest
    {
        private readonly ClimateValueRepository _climateRepo;
        public ClimateValueRepositoryTest()
        {
            _climateRepo = new ClimateValueRepository("climateItemKey");
            ClimateValue item = new ClimateValue { Key = "Value1", ClimateItemKey = "climateItemKey", TimeStamp = System.DateTime.Now, ValueType = "Indoor", Value = "22" };
            _climateRepo.Add(item);
        }

        [Fact]
        public void ClimateValueRepository_NotNull()
        {
            Assert.NotNull(_climateRepo);
        }

        [Fact]
        public void ClimateValueRepository_GetAllReturnsAllItems()
        {
            IEnumerable<ClimateValue> list = _climateRepo.GetAll();
            Assert.Equal(2,list.Count());
        }

        [Fact]
        public void ClimateValueRepository_ExistingItemIsFound()
        {
            ClimateValue item = _climateRepo.Find("Value1");
            Assert.Equal("22", item.Value);
        }

        [Fact]
        public void ClimateValueRepository_NonExistingItemReturnsNull()
        {
            ClimateValue item = _climateRepo.Find("Nonexist");
            Assert.Null(item);
        }

        [Fact]
        public void ClimateValueRepository_ExistingValueUpdatedValueIsChanged()
        {
            // Create and add a new item that should be changed
            ClimateValue item = new ClimateValue();
            item.Key = "ChangeKey";
            item.Value = "22";
            _climateRepo.Add(item);

            // Make sure the item is in the repo
            ClimateValue existingItem = _climateRepo.Find("ChangeKey");
            Assert.Equal("22", existingItem.Value);

            // Change the item
            item.Value = "23";

            // Update the item in the repo
            _climateRepo.Update(item);

            // Get the item from the repo
            ClimateValue afterItem = _climateRepo.Find("ChangeKey");

            // Check that the item name with the key has changed
            Assert.Equal("23", item.Value);
            Assert.Equal("ChangeKey", item.Key);

            // Cleanup, remove the item and make sure it is gone
            item = _climateRepo.Remove("ChangeKey");
            Assert.Null(_climateRepo.Find("ChangeKey"));
        }

        [Fact]
        public void ClimateValueRepository_RemovedItemIsGone()
        {
            ClimateValue item = new ClimateValue();
            item.Key = "RemoveKey";
            item.Value = "22";
            _climateRepo.Add(item);

            item = _climateRepo.Remove("RemoveKey");
            Assert.Equal("22", item.Value);
            item = _climateRepo.Find("RemoveKey");
            Assert.Null(item);
        }

    }
}
