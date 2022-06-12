using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class SignUpDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string Name { get; set; }
        
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}