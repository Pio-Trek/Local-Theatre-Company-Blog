using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(60), MinLength(5)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}