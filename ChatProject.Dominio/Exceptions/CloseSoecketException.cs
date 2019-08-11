using System;

namespace ChatProject.Domain.Exceptions
{
    public class CloseSocketException : Exception
    {
        public CloseSocketException(string message, Exception e) : base(message, e)
        {

        }
    }
}
