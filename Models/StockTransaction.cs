using System;
using System.ComponentModel.DataAnnotations;

namespace StockBoutique.Models
{
    public class StockTransaction
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "In"; // "In" or "Out"

        public int Quantity { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? Reason { get; set; }
    }
}
