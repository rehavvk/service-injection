﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    public abstract class MonoBootstrapper : MonoBehaviour, IBootstrapper
    {
        private readonly Queue<object> queuedForResolve = new Queue<object>();

        void IBootstrapper.Boot()
        {
            Boot();
        }
        
        void IBootstrapper.Resolve()
        {
            while (queuedForResolve.Count > 0)
            {
                object instance = queuedForResolve.Dequeue();
                instance.ResolveDependencies();
            }
        }

        protected abstract void Boot();
        
        protected void QueueForResolve<T>(T instance)
        {
            queuedForResolve.Enqueue(instance);
        }
        
        /// <inheritdoc/>
        public Registry<T, T> Register<T>()
        {
            return ServiceLocator.Register<T>();
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> Register<TContract, TConcrete>() where TConcrete : TContract
        {
            return ServiceLocator.Register<TContract, TConcrete>();
        }

        /// <inheritdoc/>
        public Registry<T, T> RegisterInstance<T>(T instance)
        {
            return ServiceLocator.RegisterInstance(instance);
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> RegisterInstance<TContract, TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            return ServiceLocator.RegisterInstance<TContract, TConcrete>(instance);
        }

        /// <inheritdoc/>
        public Registry<T, T> RegisterFactory<T>(Func<T> factory)
        {
            return ServiceLocator.RegisterFactory(factory);
        }

        /// <inheritdoc/>
        public Registry<TContract, TConcrete> RegisterFactory<TContract, TConcrete>(Func<TConcrete> factory) where TConcrete : TContract
        {
            return ServiceLocator.RegisterFactory<TContract, TConcrete>(factory);
        }
        
        /// <inheritdoc/>
        public T CreateInstance<T>(params object[] args)
        {
            return ServiceLocator.CreateInstance<T>(args);
        }

        /// <inheritdoc/>
        public object CreateInstance(Type type, params object[] args)
        {
            return ServiceLocator.CreateInstance(type, args);
        }
    }
}