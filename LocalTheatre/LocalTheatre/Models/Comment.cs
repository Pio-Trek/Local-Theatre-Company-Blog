using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocalTheatre.Models
{
    public class Comment : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500), MinLength(5)]
        [DisplayName("Comment")]
        public string Text { get; set; }

        [Required]
        [DisplayName("Author")]
        public string AuthorId { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime PublishDate { get; set; }

        [Required]
        public int BlogId { get; set; }

        public bool IsModerated { get; set; }

        public bool DeleteFromListHelpColumn { get; set; }
    }
}