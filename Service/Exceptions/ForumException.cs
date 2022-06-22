using System;
using System.Runtime.Serialization;

namespace Service.Exceptions
{
    /// <summary>
    /// Represents the general application error
    /// </summary>
    [Serializable]
    public class ForumException : Exception
    {
        public ForumException()
        {
        }

        protected ForumException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ForumException(string message) : base(message)
        {
        }

        public ForumException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}