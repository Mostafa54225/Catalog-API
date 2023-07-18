using Moq;
using Catalog.Api.Interfaces;
using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Catalog.Api.DTOs;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositorySetub = new();
        private readonly Mock<ILogger<ItemsController>> loggerSetub = new();
        private readonly Random random = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // Arrange
            repositorySetub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);
            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithexistingItem_ReturnsExpectedItem()
        {
            // Arrange
            var expectedItem = CreateRandomItem();
            repositorySetub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);
            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<ItemDTO>(result.Value);
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemsAsync_WithexistingItems_ReturnsAllItems()
        {
            // Arrange
            var expectedItems = new[] {CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};
            repositorySetub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);
            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);
            // Act
            var actualItems = await controller.GetItemsAsync();

            // Assert
            actualItems.Should().BeEquivalentTo(expectedItems);
        }


        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {
            // Arrange
            var itemToCreate = new CreateItemDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), random.Next(1000));

            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);

            // Act
            var result = await controller.PostItem(itemToCreate);

            // Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDTO;
            itemToCreate.Should().BeEquivalentTo(createdItem, options => options.ComparingByMembers<ItemDTO>().ExcludingMissingMembers());
            createdItem?.Id.Should().NotBeEmpty();
            createdItem?.Description.Should().NotBeEmpty();
            createdItem?.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }


        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnNotUpdatedItem()
        {
            // Arrange 
            var existingItem = CreateRandomItem();
            repositorySetub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);

            // Act
            var result = await controller.PutItem(itemId, itemToUpdate);

            // Assert
            var updadedItem = (result.Result as CreatedAtActionResult).Value;
            itemToUpdate.Should().BeEquivalentTo(updadedItem, options => options.ComparingByMembers<ItemDTO>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdateItemAsync_WithUnexistingItem_ReturnNotFound()
        {
            // Arrange 
            var existingItem = CreateRandomItem();
            repositorySetub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositorySetub.Object, loggerSetub.Object);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);
            // Act
            var result = await controller.PutItem(itemId, itemToUpdate);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        private Item CreateRandomItem(){
            return new(){
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = random.Next(1000),
                CreatedDate = DateTime.Now
            };
        }
    }
}
