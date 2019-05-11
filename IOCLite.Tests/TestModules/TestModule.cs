using System;

namespace IOCLite.Tests
{
    /// <summary>
    /// Test module to be installed into a test container during unit testing.
    /// </summary>
    internal class TestModule : IIOCLiteModule
    {
        /// <summary>
        /// Installs the module in questions into the intended container by making a 
        /// call to RegisterAll() in the container.
        /// </summary>
        /// <param name="container"></param>
        public void RegisterComponents(IOCLiteContainer container)
        {
            container.RegisterAll(new Action[]
            {
                () => container.Register<IPerson, Person>(IOCLiteLifeSpan.TRANSIENT),
                () => container.Register<IName, Name>(IOCLiteLifeSpan.TRANSIENT)
            });
        }
    }
}
