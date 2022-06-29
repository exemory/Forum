using System;

namespace Service.DataTransferObjects
{
    public class PostCreationDto
    {
        public string Content { get; set; }
        public Guid ThreadId { get; set; }
    }
}