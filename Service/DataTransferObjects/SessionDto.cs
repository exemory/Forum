using System;
using System.Collections.Generic;

namespace Service.DataTransferObjects
{
    public class SessionDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
        public string AccessToken { get; set; }
    }
}