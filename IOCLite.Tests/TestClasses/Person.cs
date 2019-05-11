﻿namespace IOCLite.Tests
{
    /// <summary>
    /// Test class representing a person. Implements the IPerson interface for DI purposes.
    /// </summary>
    internal class Person : IPerson
    {
        public IName Name { get; set; }

        public Person(IName name) => Name = name;
    }
}
