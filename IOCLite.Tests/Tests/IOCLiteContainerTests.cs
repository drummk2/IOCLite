using Xunit;

namespace IOCLite.Tests
{
    /// <summary>
    /// Unit tests for the IOCLiteContainer class.
    /// </summary>
    public class IOCLiteContainerTests
    {
        /// <summary>
        /// The IOCLiteContainer should throw an IOCLiteException when it tries to resolve a 
        /// type that has not been registered. In this case the Resolve(Type type) function is called.
        /// </summary>
        [Fact]
        public void Resolve_NoRegistrationExists_ThrowsIOCLiteException() 
            => Assert.Throws<IOCLiteException>(() => new IOCLiteContainer().Resolve(typeof(IOCLiteException)));

        /// <summary>
        /// The IOCLiteContainer should throw an IOCLiteException when it tries to resolve a 
        /// type that has not been registered. In this case the Resolve<T> function is called.
        /// </summary>
        [Fact]
        public void ResolveGenericArgument_NoRegistrationExists_ThrowsIOCLiteException()
            => Assert.Throws<IOCLiteException>(() => new IOCLiteContainer().Resolve<IOCLiteException>());

        /// <summary>
        /// When a valid container registration exists, the container should return an instance 
        /// of it's implementation type when asked to resolve that contract type.
        /// </summary>
        [Fact]
        public void ResolveGenericArgument_RegistrationExists_InstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>();
            IPerson p = container.Resolve<IPerson>();
            Assert.True(Equals(typeof(Person), p.GetType()));
        }

        /// <summary>
        /// When a valid container singleton registration exists, the container should return a singleton 
        /// instance of it's implementation type when asked to resolve that contract type.
        /// </summary>
        [Fact]
        public void ResolveGenericArgument_SingletonRegistrationExists_SingletonInstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>(IOCLiteLifeSpan.SINGLETON);
            IPerson firstInstance = container.Resolve<IPerson>();
            IPerson secondInstance = container.Resolve<IPerson>();
            Assert.True(Equals(firstInstance, secondInstance));
        }
    }
}
