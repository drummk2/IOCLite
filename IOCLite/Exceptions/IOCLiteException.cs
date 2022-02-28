using System;

namespace IOCLite.Exceptions
{
    /// <summary>
    /// A custom exception to highlight that an error has occurred internally 
    /// due to an issue with the IOCLiteContainer in use.
    /// </summary>
    public class IOCLiteException : Exception
    {
        /// <summary>
        /// Throws an IOCLiteException with the specified message.
        /// </summary>
        /// <param name="message">An appropriate error message.</param>
        public IOCLiteException(string message) : base(message) { }
    }
}