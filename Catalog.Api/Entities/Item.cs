using System;
namespace Catalog.Api.Entities
{
    public class Item
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}