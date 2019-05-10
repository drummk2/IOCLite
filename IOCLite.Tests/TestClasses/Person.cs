namespace IOCLite.Tests
{
    /// <summary>
    /// Test class representing a person. Implements the IPerson interface for DI purposes.
    /// </summary>
    internal class Person : IPerson
    {
        /// <summary>
        /// The person's full name.
        /// </summary>
        public string Name { get; set; }
    }
}
