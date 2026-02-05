using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bai1.Models
{
   public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]  
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 2)")]  
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }
}