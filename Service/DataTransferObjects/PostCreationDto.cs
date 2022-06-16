using System;
using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class PostCreationDto
    {
        [Required]
        [StringLength(10000)]
        public string Content { get; set; }

        [Required]
        public Guid ThreadId { get; set; }
    }
}