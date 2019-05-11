namespace IOCLite.Tests
{
    /// <summary>
    /// An interface to be implemented by the Person class for DI purposes.
    /// </summary>
    internal interface IPerson
    {
        IName Name { get; set; }
    }
}
