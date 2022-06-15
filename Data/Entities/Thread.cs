using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Thread : EntityBase
    {
        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Topic { get; set; }

        public bool Closed { get; set; }
        
        public DateTime CreationDate { get; set; }

        public IEnumerable<Post> Posts { get; set; }

        public Guid? AuthorId { get; set; }
        public User Author { get; set; }
    }
}