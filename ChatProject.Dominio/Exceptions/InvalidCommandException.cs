using System;

namespace ChatProject.Domain.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message)
        {

        }
    }
}
