using IOCLite.Enums;
using IOCLite.Exceptions;
using IOCLite.Tests.TestClasses;
using IOCLite.Tests.TestInterfaces;
using IOCLite.Tests.TestModules;
using Xunit;

namespace IOCLite.Tests
{
    /// <summary>
    /// Unit tests for the IOCLiteContainer class.
    /// </summary>
    public class IOCLiteContainerTests
    {
        /// <summary>
        /// The user should be able to install an IOCLiteModule containing multiple registrations.
        /// Those registrations should then be registered in bulk in the intended container.
        /// </summary>
        [Fact]
        public void Install_ValidModuleProvided_RegistersModuleCorrectly()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Install(new TestModule());
            IPerson p = container.Resolve<IPerson>();
            Assert.True(Equals(typeof(Person), p.GetType()));
        }

        /// <summary>
        /// When a class implementing the <see cref="ApiController"/> base class is present in the assembly,
        /// this function should register an instance of the class in question in the container.
        /// </summary>
        [Fact]
        public void RegisterControllers_SuccessfullyRegistersApiControllerImplementation_InstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.RegisterControllers();
            PersonApiController apiController = container.Resolve<PersonApiController>();
            Assert.Equal(typeof(PersonApiController), apiController.GetType());
        }

        /// <summary>
        /// When a class implementing the <see cref="Controller"/> base class is present in the assembly,
        /// this function should register an instance of the class in question in the container.
        /// </summary>
        [Fact]
        public void RegisterControllers_SuccessfullyRegistersControllerImplementation_InstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.RegisterControllers();
            PersonController controller = container.Resolve<PersonController>();
            Assert.Equal(typeof(PersonController), controller.GetType());
        }

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
        /// When a valid container registration exists. And the constructor of that registration's implementation 
        /// also contains a separate container registration, verify that an implementation of a contract is resolved 
        /// by the container recursively using the correct constructors.
        /// </summary>
        [Fact]
        public void Resolve_NestedRegistrationsExist_NestedInstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>();
            container.Register<IName, Name>();
            IPerson p = container.Resolve<IPerson>();
            Assert.NotNull(p.Name);
        }

        /// <summary>
        /// When a valid singleton container registration exists that is a dependency of another container registration.
        /// Verify that when the nested dependency is resolved to create two separate parent registrations, that the 
        /// resolved dependency is still a singleton as expected.
        /// </summary>
        [Fact]
        public void Resolve_NestedSingletonRegistrationExists_NestedSingletonReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>();
            container.Register<IName, Name>(IOCLiteLifeSpan.SINGLETON);
            IPerson firstInstance = container.Resolve<IPerson>();
            IPerson secondInstance = container.Resolve<IPerson>();
            Assert.Equal(firstInstance.Name, secondInstance.Name);
        }

        /// <summary>
        /// When a valid container registration exists, the container should return an instance 
        /// of it's implementation type when asked to resolve that contract type.
        /// </summary>
        [Fact]
        public void Resolve_RegistrationExists_InstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>();
            container.Register<IName, Name>();
            IPerson p = container.Resolve<IPerson>();
            Assert.Equal(typeof(Person), p.GetType());
        }

        /// <summary>
        /// When a valid container singleton registration exists, the container should return a singleton 
        /// instance of it's implementation type when asked to resolve that contract type.
        /// </summary>
        [Fact]
        public void Resolve_SingletonRegistrationExists_SingletonInstanceReturned()
        {
            IOCLiteContainer container = new IOCLiteContainer();
            container.Register<IPerson, Person>(IOCLiteLifeSpan.SINGLETON);
            container.Register<IName, Name>();
            IPerson firstInstance = container.Resolve<IPerson>();
            IPerson secondInstance = container.Resolve<IPerson>();
            Assert.Equal(firstInstance, secondInstance);
        }
    }
}