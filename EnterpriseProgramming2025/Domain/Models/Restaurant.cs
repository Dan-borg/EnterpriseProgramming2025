using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Models
{
    public class Restaurant : IItemValidating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string OwnerEmailAddress { get; set; }

        public string? Description { get; set; }
        public string? ImagePath { get; set; }

        public string Status { get; set; } = "Pending";

        public List<string> GetValidators()
        {
            return new List<string> { OwnerEmailAddress };
        }

        public string GetCardPartial()
        {
            return "_RestaurantCard";
        }
    }
}
