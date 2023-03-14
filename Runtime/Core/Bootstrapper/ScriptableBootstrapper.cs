using System;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
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