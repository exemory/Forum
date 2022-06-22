using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class ThreadStatusUpdateDto
    {
        [Required]
        public bool Closed { get; set; }
    }
}