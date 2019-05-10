using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// Also ensures that the implementation type implements the contract type. The transient lifespan is defaultly set.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract</typeparam>
        /// <typeparam name="TImplementation">The type of the contract implementation.</typeparam>
        public void Register<TContract, TImplementation>() where TImplementation : TContract 
            => Register<TContract, TImplementation>(IOCLiteLifeSpan.TRANSIENT);

        /// <summary>
        /// Adds a new registration to the container with both a contract type and implementation type.
        /// Also ensures that the implementation type implements the contract type. And a life span is specified.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract</typeparam>
        /// <typeparam name="TImplementation">The type of the contract implementation.</typeparam>
        /// <param name="lifeSpan">The specified life span for the TImplementation type.</param>
        public void Register<TContract, TImplementation>(IOCLiteLifeSpan lifeSpan) where TImplementation : TContract
            => _objectRegistrations.Add(
                   typeof(TContract),
                   new IOCLiteContractImplementation
                   {
                       ImplementationType = typeof(TImplementation),
                       LifeSpan = lifeSpan
                   });

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
            Type implementationType = default;
            IOCLiteLifeSpan lifeSpan = default;

            if (_objectRegistrations.ContainsKey(contractType))
            {
                implementationType = _objectRegistrations[contractType].ImplementationType;
                lifeSpan = _objectRegistrations[contractType].LifeSpan;
            }
            else
                throw new IOCLiteException($"Unable to resolve implementation for contract of type {contractType.ToString()}");

            ConstructorInfo objectConstructor = implementationType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            object[] constructorArguments = objectConstructor
                .GetParameters()
                .Select(p => Resolve(p.ParameterType))
                .ToArray();

            object instance = Activator.CreateInstance(implementationType, constructorArguments);

            if (lifeSpan.Equals(IOCLiteLifeSpan.SINGLETON))
            {
                if (_singletonInstances.ContainsKey(contractType))
                    return _singletonInstances[contractType];
                else
                    _singletonInstances.Add(contractType, instance);
            }

            return instance;
        }
    }
}