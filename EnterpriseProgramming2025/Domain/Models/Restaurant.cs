using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Models
{
    public class Restaurant : ItemValidating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string OwnerEmailAddress { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public List<string> GetValidators()
        {
            // Owner + hardcoded Site Admin
            return new List<string> { OwnerEmailAddress, "admin@site.com" };
        }

        public string GetCardPartial()
        {
            return "/Presentation/Views/Items/_RestaurantCard.cshtml";
        }

    }
}
