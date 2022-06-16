using System;
using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class PostUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(10000)]
        public string Content { get; set; }
    }
}