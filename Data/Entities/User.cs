using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        [StringLength(20)]
        public string Name { get; set; }

        public DateTime RegistrationDate { get; set; }

        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Thread> Threads { get; set; }
    }
}