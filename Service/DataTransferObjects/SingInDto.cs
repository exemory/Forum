using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class SingInDto
    {
        [Required]
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}