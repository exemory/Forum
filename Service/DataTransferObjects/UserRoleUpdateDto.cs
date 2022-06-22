using System;
using System.ComponentModel.DataAnnotations;

namespace Service.DataTransferObjects
{
    public class UserRoleUpdateDto
    {
        [Required]
        [RegularExpression("User|Moderator", ErrorMessage = "Role must be either User or Moderator")]
        public string Role { get; set; }
    }
}