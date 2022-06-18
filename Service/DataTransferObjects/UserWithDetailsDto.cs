using System;
using System.Collections.Generic;

namespace Service.DataTransferObjects
{
    public class UserWithDetailsDto
    {
        public Guid Id { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Name { get; set; }
        
        public DateTime RegistrationDate { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}