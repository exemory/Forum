using System;
using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class ThreadUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Topic { get; set; }
    }
}