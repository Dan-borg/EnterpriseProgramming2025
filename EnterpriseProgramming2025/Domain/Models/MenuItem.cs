using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Models
{
    public class MenuItem : IItemValidating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }

        public string Status { get; set; } = "Pending";
        public string? ImagePath { get; set; }

        public List<string> GetValidators()
        {
            return new List<string> { Restaurant?.OwnerEmailAddress ?? "" };
        }

        public string GetCardPartial()
        {
            return "_MenuItemRow";
        }
    }
}
