using System;

namespace IOCLite
{
    /// <summary>
    /// Represents an implementation of a registered service (interface) in the IOCLiteContainer.
    /// </summary>
    public class IOCLiteContractImplementation
    {
        /// <summary>
        /// The type of object that implements the interface in question.
        /// </summary>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// The life span of the service implementation.
        /// </summary>
        public IOCLiteLifeSpan LifeSpan { get; set; }
    }
}
