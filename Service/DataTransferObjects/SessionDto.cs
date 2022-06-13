using System.Collections.Generic;

namespace Service.DataTransferObjects
{
    public class SessionDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}