using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Post : EntityBase
    {
        [Required]
        [StringLength(10000)]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }
        
        public Guid ThreadId { get; set; }
        public Thread Thread { get; set; }
        
        public Guid? AuthorId { get; set; }
        public User Author { get; set; }
    }
}