using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Post : EntityBase
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
        
        public Guid ThreadId { get; set; }
        public Thread Thread { get; set; }
        
        public Guid? UserId { get; set; }
        public User User { get; set; }
    }
}