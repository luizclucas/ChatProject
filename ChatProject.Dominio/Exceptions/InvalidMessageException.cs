using System;

namespace ChatProject.Domain.Exceptions
{
    public class InvalidMessageException : Exception
    {
        public InvalidMessageException(string message) : base (message)
        {

        }
    }
}
