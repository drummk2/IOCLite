using IOCLite.Enums;
using IOCLite.Exceptions;
using IOCLite.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace IOCLite
{
    /// <summary>
    /// A standard IOCLiteContainer. Implements the basic functionality needed for 
    /// implementing DI in an application (object registration and resolution).
    /// </summary>
    public class IOCLiteContainer
    {
        /// <summary>
        /// Handles any object registrations made in the IOCLiteContainer.
        /// Maps interface types to concrete class types.
        /// </summary>
        private Dictionary<Type, IOCLiteContractImplementation> _objectRegistrations;

        /// <summary>
        /// Stores any initialised singleton objects.
        /// </summary>
        private Dictionary<Type, object> _singletonInstances;

        /// <summary>
        /// Initialise the dictionaries needed for container registrations and initialised singletons.
        /// </summary>
        public IOCLiteContainer()
        {
            _objectRegistrations = new Dictionary<Type, IOCLiteContractImplementation>();
            _singletonInstances = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Installs a new IOCLiteModule on the container in question and subsequently registers and 
        /// pre-existing registrations in the module.
        /// </summary>
        public void Install(IIOCLiteModule module) => module.RegisterComponents(this);

        /// <summary>
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// Also ensures that the implementation type implements the contract type. The transient lifespan is defaultly set.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract</typeparam>
        /// <typeparam name="TImplementation">The type of the contract implementation.</typeparam>
        public void Register<TContract, TImplementation>() where TImplementation : TContract
            => Register(typeof(TContract), typeof(TImplementation), IOCLiteLifeSpan.TRANSIENT);

        /// <summary>
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// Also ensures that the implementation type implements the contract type. And a life span is specified.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <typeparam name="TImplementation">The type of the contract implementation.</typeparam>
        /// <param name="lifeSpan">The specified life span for the TImplementation type.</param>
        public void Register<TContract, TImplementation>(IOCLiteLifeSpan lifeSpan) where TImplementation : TContract
            => Register(typeof(TContract), typeof(TImplementation), lifeSpan);

        /// <summary>
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// </summary>
        /// <param name="contract">The type of the contract.</param>
        /// <param name="implementation">The type of the implementation.</param>
        public void Register(Type contract, Type implementation)
            => Register(contract, implementation, IOCLiteLifeSpan.TRANSIENT);

        /// <summary>
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// </summary>
        /// <param name="contract">The type of the contract.</param>
        /// <param name="implementation">The type of the implementation.</param>
        /// <param name="lifeSpan">The specified life span for the TImplementation type.</param>
        public void Register(Type contract, Type implementation, IOCLiteLifeSpan lifeSpan)
            => _objectRegistrations.Add(
                   contract,
                   new IOCLiteContractImplementation
                   {
                       ImplementationType = implementation,
                       LifeSpan = lifeSpan
                   });

        /// <summary>
        /// Register multiple components in the container at once.
        /// </summary>
        /// <param name="registrations">The set of registrations to be made.</param>
        public void RegisterAll(Action[] registrations) => registrations.ToList().ForEach(action => action());

        /// <summary>
        /// Register all classes that implement APIController or Controller.
        /// </summary>
        public void RegisterControllers()
        {
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(c => typeof(Controller).IsAssignableFrom(c) || typeof(ApiController).IsAssignableFrom(c));
                .ToList()
                .ForEach(controllerType => Register(controllerType, controllerType, IOCLiteLifeSpan.SINGLETON));
        }

        /// <summary>
        /// Resolve an object when provided with a generic type parameter rather than a type object.
        /// </summary>
        /// <typeparam name="T">The type of the contract being resolved.</typeparam>
        /// <returns>The resolved object.</returns>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Resolve an object, when an existing contract for said contract has been previously registered in the container.
        /// </summary>
        /// <param name="contractType">The type of the contract.</param>
        /// <returns>An object implementing the specified contract.</returns>
        public object Resolve(Type contractType)
        {
            Type implementationType;
            IOCLiteLifeSpan lifeSpan;

            if (_objectRegistrations.ContainsKey(contractType))
            {
                implementationType = _objectRegistrations[contractType].ImplementationType;
                lifeSpan = _objectRegistrations[contractType].LifeSpan;
            }
            else
                throw new IOCLiteException($"Unable to resolve implementation for contract of type {contractType}");

            if (lifeSpan.Equals(IOCLiteLifeSpan.SINGLETON))
            {
                if (_singletonInstances.ContainsKey(contractType))
                    return _singletonInstances[contractType];
                else
                {
                    object instance = CreateInstance(implementationType);
                    _singletonInstances.Add(contractType, instance);
                    return instance;
                }
            }

            return CreateInstance(implementationType);
        }

        /// <summary>
        /// Create an instance of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="implementationType">The type of object being created/</param>
        /// <returns>An instance of the specified type.</returns>
        private object CreateInstance(Type implementationType)
        {
            ConstructorInfo objectConstructor = implementationType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            object[] constructorArguments = objectConstructor
                .GetParameters()
                .Select(p => Resolve(p.ParameterType))
                .ToArray();

            return Activator.CreateInstance(implementationType, constructorArguments);
        }
    }
}
