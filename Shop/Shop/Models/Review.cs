using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Review : IEntity
    {
        public int Id { get; set; }

        [Range(0, 10)]
        public int Rating { get; set; }

        public string Comment { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
    }
}