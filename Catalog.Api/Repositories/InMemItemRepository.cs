using Catalog.Api.Entities;
using Catalog.Api.Interfaces;

namespace Catalog.Api.Repositories
{
    public class InMemItemRepository: IItemsRepository
    {
        private readonly List<Item> items = new() {
            new Item {Id = Guid.NewGuid(), Name = "Potion", Price = 9.00, CreatedDate = DateTime.Now},
            new Item {Id = Guid.NewGuid(), Name = "Iron Sword", Price = 11.00, CreatedDate = DateTime.Now},
            new Item {Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 5.40, CreatedDate = DateTime.Now}
        };

        public async Task<IEnumerable<Item>> GetItemsAsync() {
            return await Task.FromResult(items);
        }

        public async Task<Item> GetItemAsync(Guid id){
            return await Task.FromResult(items.Find(item => item.Id == id));
        }

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var index = items.FindIndex(item => item.Id == id);
            items.RemoveAt(index);
            await Task.CompletedTask;

        }
    }
}