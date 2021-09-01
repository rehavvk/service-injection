using System;

namespace Rehawk.ServiceInjection
{
    public interface IBootstrapper
    {
        void Boot();
        void Resolve();

        /// <inheritdoc cref="ServiceLocator.Register{T}()"/>
        Registry<T, T> Register<T>();

        /// <inheritdoc cref="ServiceLocator.Register{TContract, TConcrete}()"/>
        Registry<TContract, TConcrete> Register<TContract, TConcrete>() where TConcrete : TContract;

        /// <inheritdoc cref="ServiceLocator.RegisterInstance{T}(T)"/>
        Registry<T, T> RegisterInstance<T>(T instance);
        
        /// <inheritdoc cref="ServiceLocator.RegisterInstance{TContract, TConcrete}(TConcrete)"/>
        Registry<TContract, TConcrete> RegisterInstance<TContract, TConcrete>(TConcrete instance) where TConcrete : TContract;
        
        /// <inheritdoc cref="ServiceLocator.RegisterFactory{T}(Func{T})"/>
        Registry<T, T> RegisterFactory<T>(Func<T> factory);
        
        /// <inheritdoc cref="ServiceLocator.RegisterFactory{TContract, TConcrete}(Func{TConcrete})"/>
        Registry<TContract, TConcrete> RegisterFactory<TContract, TConcrete>(Func<TConcrete> factory) where TConcrete : TContract;

        /// <inheritdoc cref="ServiceLocator.CreateInstance{T}(object[])"/>
        T CreateInstance<T>(params object[] args);

        /// <inheritdoc cref="ServiceLocator.CreateInstance(Type, object[])"/>
        object CreateInstance(Type type, params object[] args);
    }
}