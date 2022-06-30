using System;
using System.Runtime.Serialization;

namespace Service.Exceptions
{
    /// <summary>
    /// The exception that is thrown when registration failed
    /// </summary>
    [Serializable]
    public class RegistrationException : ForumException
    {
        public RegistrationException()
        {
        }

        protected RegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RegistrationException(string message) : base(message)
        {
        }

        public RegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}