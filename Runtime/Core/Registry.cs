using System;
using UnityEngine.SceneManagement;

namespace Rehawk.ServiceInjection
{
    public abstract class Registry
    {
        protected bool asSingle = true;

        protected event Action<object> onInstantiate;

        protected object label;
        protected Func<object> factory;
        protected object[] arguments;
        protected Func<object[]> getLazyArguments;
        protected Scene? scene;

        internal abstract Type ContractType { get; }
        internal abstract Type ConcreteType { get; }

        internal bool AsSingle => asSingle;

        internal object Label => label;

        internal bool IsSceneScoped => scene.HasValue;

        internal Action<object> OnInstantiate => onInstantiate;

        internal Func<object> Factory => factory;

        public object[] Arguments => arguments;

        internal Func<object[]> GetLazyArguments => getLazyArguments;

        internal Scene Scene => scene.GetValueOrDefault();
    }

    public class Registry<TContract, TConcrete> : Registry
    {
        internal override Type ContractType => typeof(TContract);

        internal override Type ConcreteType => typeof(TConcrete);

        /// <summary>
        /// The contract type of <typeparamref name="TContract" /> will always resolved as the same instance of the type <typeparamref name="TConcrete" />.
        /// </summary>
        public Registry<TContract, TConcrete> AsSingleton()
        {
            asSingle = true;
            return this;
        }
        
        /// <summary>
        /// The contract type of <typeparamref name="TContract" /> will always resolved as a new instance of the type <typeparamref name="TConcrete" />.
        /// </summary>
        public Registry<TContract, TConcrete> AsTransient()
        {
            asSingle = false;
            return this;
        }
        
        /// <summary>
        /// Registers a specific instance to the contract type of <typeparamref name="TContract" />.
        /// <seealso cref="ServiceLocator.RegisterInstance{T}(T)" />
        /// <seealso cref="ServiceLocator.RegisterInstance{TContract, TConcrete}(TConcrete)" />
        /// </summary>
        /// <param name="instance">The instance which will be resolved.</param>
        public Registry<TContract, TConcrete> FromInstance(TConcrete instance)
        {
            factory = () => instance;
            return this;
        }
        
        /// <summary>
        /// Registers a specific factory to the contract type of <typeparamref name="TContract" />.
        /// <seealso cref="ServiceLocator.RegisterFactory{T}(Func{T})" />
        /// <seealso cref="ServiceLocator.RegisterFactory{TContract, TConcrete}(Func{TConcrete})" />
        /// </summary>
        /// <param name="factory">The factory which will be called to create a new instance.</param>
        public Registry<TContract, TConcrete> FromFactory(Func<TConcrete> factory)
        {
            this.factory = () => factory();
            return this;
        }

        /// <summary>
        /// Registers a scene-specific resolver of the type <typeparamref name="TConcrete" /> for the contract type <typeparamref name="TContract" />.
        /// <remarks>The resolver is registered for the active scene according to <see cref="SceneManager.GetActiveScene()" />.</remarks>
        /// </summary>
        public Registry<TContract, TConcrete> SceneScoped()
        {
            scene = SceneManager.GetActiveScene();
            return this;
        }
        
        /// <summary>
        /// Registers a scene-specific resolver of the type <typeparamref name="TConcrete" /> for the contract type <typeparamref name="TContract" />.
        /// </summary>
        /// <param name="scene">The scene to register the type for.</param>
        public Registry<TContract, TConcrete> SceneScoped(Scene scene)
        {
            this.scene = scene;
            return this;
        }

        /// <summary>
        /// Registers a concrete singleton of the given type.
        /// </summary>
        public Registry<TContract, TConcrete> GlobalScoped()
        {
            scene = null;
            return this;
        }

        /// <summary>
        /// Registers a method which will be called each time the concrete type <typeparamref name="TConcrete" /> is instantiated and fully injected.
        /// <remarks>Multiple callbacks are possible.</remarks>
        /// </summary>
        /// <param name="method">The method which will be called.</param>
        public Registry<TContract, TConcrete> WithCallback(Action<TConcrete> method)
        {
            onInstantiate += o => method((TConcrete) o);
            return this;
        }
        
        /// <summary>
        /// Registers a set of arguments which will be used during the injection process. Arguments are preferred and only if they do not contain a matching instance will an attempt be made to pull a registered instance.
        /// </summary>
        /// <param name="arguments">The additional arguments.</param>
        public Registry<TContract, TConcrete> WithArguments(params object[] arguments)
        {
            this.arguments = arguments;
            return this;
        }
        
        /// <summary>
        /// Registers a set of arguments which will be used during the injection process. Arguments are preferred and only if they do not contain a matching instance will an attempt be made to pull a registered instance.
        /// </summary>
        /// <param name="method">The method which will be called to receive arguments.</param>
        public Registry<TContract, TConcrete> WithLazyArguments(Func<object[]> method)
        {
            this.getLazyArguments = method;
            return this;
        }
        
        public Registry<TContract, TConcrete> WithLabel(object label)
        {
            this.label = label;
            return this;
        }
    }
}