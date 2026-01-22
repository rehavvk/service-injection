using System;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Provides an abstract base class for creating scriptable bootstrappers in Unity.
    /// </summary>
    /// <remarks>
    /// The <c>ScriptableBootstrapper</c> class serves as a scriptable object-based implementation
    /// for managing service registration and dependency injection in Unity projects.
    /// It leverages an underlying <c>IBootstrapper</c> to provide functionality for
    /// registering, resolving, and creating instances of services or objects.
    /// Derived classes must implement the <see cref="Boot"/> method to specify
    /// the registration logic for the services or dependencies.
    /// </remarks>
    public abstract class ScriptableBootstrapper : ScriptableObject, IBootstrapper
    {
        private readonly IBootstrapper bootstrapper = new Bootstrapper();
        
        void IBootstrapper.Boot()
        {
            bootstrapper.Boot();
            Boot();
        }
        
        void IBootstrapper.Resolve()
        {
            bootstrapper.Resolve();
        }

        void IBootstrapper.BootCompleted()
        {
            bootstrapper.BootCompleted();
            OnBootCompleted();
        }

        protected abstract void Boot();
        protected virtual void OnBootCompleted() {}
        
        /// <inheritdoc/>
        public Registry<T, T> Register<T>()
        {
            return bootstrapper.Register<T>();
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> Register<TContract, TConcrete>() where TConcrete : TContract
        {
            return bootstrapper.Register<TContract, TConcrete>();
        }

        /// <inheritdoc/>
        public Registry<T, T> RegisterInstance<T>(T instance)
        {
            return bootstrapper.RegisterInstance(instance);
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> RegisterInstance<TContract, TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            return bootstrapper.RegisterInstance<TContract, TConcrete>(instance);
        }

        /// <inheritdoc/>
        public Registry<T, T> RegisterFactory<T>(Func<T> factory)
        {
            return bootstrapper.RegisterFactory(factory);
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> RegisterFactory<TContract, TConcrete>(Func<TConcrete> factory) where TConcrete : TContract
        {
            return bootstrapper.RegisterFactory<TContract, TConcrete>(factory);
        }
        
        /// <inheritdoc/>
        public T CreateInstance<T>(params object[] args)
        {
            return bootstrapper.CreateInstance<T>(args);
        }

        /// <inheritdoc/>
        public object CreateInstance(Type type, params object[] args)
        {
            return bootstrapper.CreateInstance(type, args);
        }
        
        /// <inheritdoc/>
        public void QueueForResolve<T>(T instance)
        {
            bootstrapper.QueueForResolve<T>(instance);
        }
    }
}