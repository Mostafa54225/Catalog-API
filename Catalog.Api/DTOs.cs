using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record ItemDTO(Guid Id, string Name, string Description, double Price, DateTime CreatedDate);
    public record CreateItemDTO([Required]string Name, string Description, [Range(1, 1000)] double Price);
    public record UpdateItemDTO([Required]string Name, string Description, [Range(1, 1000)] double Price);
}