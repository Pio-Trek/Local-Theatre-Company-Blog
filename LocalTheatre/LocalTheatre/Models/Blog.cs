using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocalTheatre.Models
{
    public class Blog : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(80), MinLength(5)]
        public string Title { get; set; }

        [Required]
        [MinLength(650)]
        public string Article { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime PublishDate { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [DisplayName("Cover Image")]
        public string CoverUrl { get; set; }

        public string CoverFileName { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}