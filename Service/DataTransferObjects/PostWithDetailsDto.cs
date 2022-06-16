using System;

namespace Service.DataTransferObjects
{
    public class PostWithDetailsDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid ThreadId { get; set; }
        public UserDto Author { get; set; }
    }
}