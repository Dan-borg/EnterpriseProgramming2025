using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Models
{
    public class MenuItem : ItemValidating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = string.Empty;

        public double Price { get; set; }

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public string? ImagePath { get; set; }

        public List<string> GetValidators()
        {
            // Validated by restaurant owner
            // This assumes Restaurant is loaded with OwnerEmailAddress
            return new List<string> { Restaurant?.OwnerEmailAddress ?? string.Empty };
        }

        public string GetCardPartial()
        {
            return "/Presentation/Views/Items/_MenuItemRow.cshtml";
        }

    }
}
