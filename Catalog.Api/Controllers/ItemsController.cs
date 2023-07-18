using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
            
        }


        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDTO());
            _logger.LogInformation($"{DateTime.Now.ToShortDateString()}: Retrieved {items.Count()} items");
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id){
            var item = await _repository.GetItemAsync(id);
            if(item is null)
                return NotFound();
            return item.AsDTO();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDTO>> PostItem(CreateItemDTO itemDTO){
            Item item = new(){
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                Description = itemDTO.Description,
                CreatedDate = DateTime.Now
            };
            await _repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { Id = item.Id }, item.AsDTO());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDTO>> PutItem(Guid id, UpdateItemDTO itemDTO)
        {
            var existingItem = await _repository.GetItemAsync(id);
            if(existingItem is null) return NotFound("Item is not found");
            existingItem.Name = itemDTO.Name;
            existingItem.Price = itemDTO.Price;
            existingItem.Description = itemDTO.Description;

            await _repository.UpdateItemAsync(existingItem);
            return CreatedAtAction(nameof(GetItemAsync), new { Id = existingItem.Id }, existingItem.AsDTO());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            await _repository.DeleteItemAsync(id);
            return Ok("Item deleted Successfully");
        }
    }
}