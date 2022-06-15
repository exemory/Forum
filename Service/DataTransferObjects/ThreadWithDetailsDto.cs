using System;

namespace Service.DataTransferObjects
{
    public class ThreadWithDetailsDto
    {
        public Guid Id { get; set; }
        public string Topic { get; set; }
        public bool Closed { get; set; }
        public DateTime CreationDate { get; set; }
        public string AuthorUsername { get; set; }
    }
}