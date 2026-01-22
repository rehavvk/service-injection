using System;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Represents the contract for a bootstrapper that defines methods for registering,
    /// resolving, and managing service dependencies for injection.
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        /// Is called to register all types which should be injected or receive injection.
        /// </summary>
        void Boot();
        
        /// <summary>
        /// Is called after all bootstrappers of the context are booted and before the resolve of already queued instances happens.
        /// </summary>
        void BootCompleted();

        /// <summary>
        /// Is called to handle resolving of queued instances by cref="QueueForResolve{T}(T instance)"
        /// </summary>
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

        /// <summary>
        /// Registers a instance to which will be injected after the boot process.
        /// </summary>
        /// <param name="instance">The instance which will injected to.</param>
        void QueueForResolve<T>(T instance);
    }
}
