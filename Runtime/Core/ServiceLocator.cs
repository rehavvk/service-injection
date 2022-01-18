using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Rehawk.ServiceInjection
{
    public static class ServiceLocator
    {
        public const string MULTISCENE_GAME_OBJECT_NAME = "ServiceLocator - Multi-scene";
        public const object DEFAULT_LABEL = null;
        
        private static readonly Dictionary<Type, ResolverCollection> globalResolvers = new();
        private static readonly Dictionary<Scene, SceneData> sceneResolvers = new();
        
        private static readonly List<Registry> register = new();

        private static readonly List<Scene> tempSceneList = new();
        
        private static GameObject multiSceneGameObject;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            ResetGlobal();
            ResetScenes();
            
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene unloadedScene)
        {
            ResetScene(unloadedScene);
        }
        
        internal static void BeginRegister()
        {
            register.Clear();
        }
        
        internal static void EndRegister()
        {
            foreach (Registry registry in register)
            {
                CreateResolver(registry);
            }
            
            register.Clear();
        }
        
        /// <summary>
        ///     Registers a concrete resolver of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        public static Registry<T, T> Register<T>()
        {
            var registry = new Registry<T, T>();
            register.Add(registry);
            return registry;
        }
        
        /// <summary>
        ///     Registers a concrete resolver of the type <typeparamref name="TConcrete" /> for the contract type <typeparamref name="TContract" />.
        /// </summary>
        /// <typeparam name="TContract">The contract type that will be mapped to <typeparamref name="TConcrete" />.</typeparam>
        /// <typeparam name="TConcrete">The concrete singleton implementation.</typeparam>
        public static Registry<TContract, TConcrete> Register<TContract, TConcrete>() where TConcrete : TContract
        {
            var registry = new Registry<TContract, TConcrete>();
            register.Add(registry);
            return registry;
        }
        
        /// <summary>
        ///     Registers a specific instance to the concrete type.
        /// </summary>
        /// <param name="instance">The instance which will be resolved.</param>
        public static Registry<T, T> RegisterInstance<T>(T instance)
        {
            Registry<T, T> registry = Register<T, T>();
            registry.FromInstance(instance);
            return registry;
        }

        /// <summary>
        ///     Registers a specific instance of the concrete type <typeparamref name="TConcrete" /> to the contract type of <typeparamref name="TContract" />.
        /// </summary>
        /// <typeparam name="TContract">The contract type that will be mapped to <typeparamref name="TConcrete" />.</typeparam>
        /// <typeparam name="TConcrete">The concrete singleton implementation.</typeparam>
        /// <param name="instance">The instance which will be resolved.</param>
        public static Registry<TContract, TConcrete> RegisterInstance<TContract, TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            Registry<TContract, TConcrete> registry = Register<TContract, TConcrete>();
            registry.FromInstance(instance);
            return registry;
        }

        /// <summary>
        ///     Registers a specific factory to the concrete type.
        /// </summary>
        /// <param name="factory">The factory which will be called to create a new instance.</param>
        public static Registry<T, T> RegisterFactory<T>(Func<T> factory)
        {
            Registry<T, T> registry = Register<T, T>();
            registry.FromFactory(factory);
            return registry;
        }

        /// <summary>
        ///     Registers a specific factory of the concrete type <typeparamref name="TConcrete" /> to the contract type of <typeparamref name="TContract" />.
        /// </summary>
        /// <typeparam name="TContract">The contract type that will be mapped to <typeparamref name="TConcrete" />.</typeparam>
        /// <typeparam name="TConcrete">The concrete singleton implementation.</typeparam>
        /// <param name="factory">The factory which will be called to create a new instance.</param>
        public static Registry<TContract, TConcrete> RegisterFactory<TContract, TConcrete>(Func<TConcrete> factory) where TConcrete : TContract
        {
            Registry<TContract, TConcrete> registry = Register<TContract, TConcrete>();
            registry.FromFactory(factory);
            return registry;
        }

        /// <summary>
        ///     Locates and returns a transient object or singleton of the specified type.
        ///     Searches for a global object first, if nothing is found and <paramref name="includeActiveScene" /> is true then it
        ///     searches for a scene specific resolver.
        ///     Make sure the type is registered first.
        ///     <seealso cref="ResolveFromScene{T}(string)" />
        /// </summary>
        /// <typeparam name="T">The type to locate an implementation for.</typeparam>
        /// <param name="label">Whether to search for a labeled resolver.</param>
        /// <param name="includeActiveScene">Whether to search for a scene specific resolver if a global one isn't found.</param>
        /// <returns>
        ///     The transient object or singleton that is mapped to the specified type.
        ///     If nothing is registered for <typeparamref name="T" /> the default value for the type is returned.
        /// </returns>
        public static T Resolve<T>(object label = DEFAULT_LABEL, bool includeActiveScene = true)
        {
            return (T) ResolveByType(typeof(T), label, includeActiveScene);
        }

        /// <summary>
        ///     Locates and returns a transient object or singleton of the specified type for the currently active scene.
        ///     Make sure the type is registered first.
        /// </summary>
        /// <typeparam name="T">The type to locate an implementation for.</typeparam>
        /// <param name="label">Whether to search for a labeled resolver.</param>
        /// <returns>
        ///     The transient object or singleton that is mapped to the specified type.
        ///     If nothing is registered for <typeparamref name="T" /> the default value for the type is returned.
        /// </returns>
        public static T ResolveFromScene<T>(object label = DEFAULT_LABEL)
        {
            return ResolveFromScene<T>(GetActiveScene(), label);
        }
        
        /// <summary>
        ///     Locates and returns a transient object or singleton of the specified type for the given scene.
        ///     Make sure the type is registered first.
        /// </summary>
        /// <typeparam name="T">The type to locate an implementation for.</typeparam>
        /// <param name="scene">Specify the scene in which the resolver should be searched for.</param>
        /// <param name="label">Whether to search for a labeled resolver.</param>
        /// <returns>
        ///     The transient object or singleton that is mapped to the specified type.
        ///     If nothing is registered for <typeparamref name="T" /> the default value for the type is returned.
        /// </returns>
        public static T ResolveFromScene<T>(Scene scene, object label = DEFAULT_LABEL)
        {
            return (T) ResolveByTypeForScene(typeof(T), scene, label);
        }

        /// <summary>
        ///     Finds all fields, properties and methods tagged with the inject attribute <see cref="InjectAttribute"/> and try's to resolve them.
        /// </summary>
        /// <param name="instance">The instance to inject their dependencies.</param>
        public static void ResolveDependencies<T>(T instance)
        {
            if (instance != null)
            {
                InjectToFields(instance, null);
                InjectToProperties(instance, null);
                InjectToMethods(instance, null);
            }
        }
        
        private static object ResolveByType(Type contractType, object label, bool includeActiveScene = true)
        {
            // Can happen if a registered type depends on another registered type.
            Registry registry = register.FirstOrDefault(r => r.ContractType == contractType && r.Label == label);
            if (registry != null)
            {
                CreateResolver(registry);
            }

            if (globalResolvers.TryGetValue(contractType, out ResolverCollection resolverCollection))
            {
                if (resolverCollection.TryGetByLabel(label, out Resolver resolver))
                {
                    return resolver.Instantiate();
                }
            }

            if (includeActiveScene && sceneResolvers.TryGetValue(GetActiveScene(), out SceneData sceneData))
            {
                if (sceneData.Resolvers.TryGetValue(contractType, out resolverCollection))
                {
                    if (resolverCollection.TryGetByLabel(label, out Resolver resolver))
                    {
                        return resolver.Instantiate();
                    }
                }
            }
            
            return default;
        }
        
        private static object ResolveByTypeForScene(Type contractType, Scene scene, object label)
        {
            // Can happen if a registered type depends on another registered type.
            Registry registry = register.FirstOrDefault(r => r.ContractType == contractType && r.Label == label);
            if (registry != null)
            {
                CreateResolver(registry);
            }

            if (sceneResolvers.TryGetValue(scene, out SceneData sceneData))
            {
                if (sceneData.Resolvers.TryGetValue(contractType, out ResolverCollection resolverCollection))
                {
                    if (resolverCollection.TryGetByLabel(label, out Resolver resolver))
                    {
                        return resolver.Instantiate();
                    }
                }
            }

            return default;
        }

        private static void CreateResolver(Registry registry)
        {
            if (registry.AsSingle)
            {
                CreateSingletonResolver(registry);
            }
            else
            {
                CreateTransientResolver(registry);
            }
        }
        
        private static void CreateSingletonResolver(Registry registry)
        {
            if ((!registry.IsSceneScoped && HasResolver(registry.ContractType, registry.Label, false)) || (registry.IsSceneScoped && HasSceneResolver(registry.Scene, registry.ContractType, registry.Label)))
            {
                return;
            }

            object instance = registry.Factory?.Invoke();
            if (instance == null)
            {
                instance = CreateInstance(registry.ConcreteType, !registry.IsSceneScoped, registry.Scene, registry.OnInstantiate, registry.Arguments, registry.GetLazyArguments);
            }

            if (!registry.IsSceneScoped)
            {
                AddGlobalResolver(registry.ContractType, registry.Label, () => instance);
            }
            else
            {
                AddSceneResolver(registry.Scene, registry.ContractType, registry.Label, () => instance);
            }
        }

        private static void CreateTransientResolver(Registry registry)
        {
            Func<object> factory = registry.Factory;
            if (factory == null)
            {
                factory = () => CreateInstance(registry.ConcreteType, !registry.IsSceneScoped, registry.Scene, registry.OnInstantiate, registry.Arguments, registry.GetLazyArguments);
            }

            if (!registry.IsSceneScoped)
            {
                AddGlobalResolver(registry.ContractType, registry.Label, () => factory());
            }
            else
            {
                AddSceneResolver(registry.Scene, registry.ContractType, registry.Label, () => factory());
            }
        }

        private static void AddGlobalResolver(Type contractType, object label, Func<object> resolver)
        {
            if (!globalResolvers.TryGetValue(contractType, out ResolverCollection collection))
            {
                collection = new ResolverCollection();
                globalResolvers.Add(contractType, collection);
            }
            
            collection.Add(label, new Resolver
            {
                Instantiate = resolver
            });
        }

        private static void AddSceneResolver(Scene scene, Type contractType, object label, Func<object> resolver)
        { 
            SceneData sceneData = GetOrCreateSceneData(scene);
            
            if (!sceneData.Resolvers.TryGetValue(contractType, out ResolverCollection collection))
            {
                collection = new ResolverCollection();
                sceneData.Resolvers.Add(contractType, collection);
            }
            
            collection.Add(label, new Resolver
            {
                Instantiate = resolver
            });
        }

        private static SceneData GetOrCreateSceneData(Scene scene)
        {
            if (!sceneResolvers.TryGetValue(scene, out SceneData sceneData))
            {
                var sceneObj  = new GameObject($"ServiceLocator - Scene: {scene.name}");
                
                sceneResolvers[scene] = sceneData = new SceneData
                {
                    GameObject = sceneObj
                };
            }

            return sceneData;
        }

        /// <summary>
        ///     Creates a instance of the given generic type and injects it.
        /// </summary>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T) CreateInstance(typeof(T), args);
        }

        /// <summary>
        ///     Creates a instance of the given type and injects it.
        /// </summary>
        public static object CreateInstance(Type type, params object[] args)
        {
            return CreateGeneralClass(type, args);
        }

        private static object CreateInstance(Type concreteType, bool inGlobalScope, Scene scene, Action<object> onInstantiate, object[] arguments, Func<object[]> getLazyArguments)
        {
            object instance;

            // Get Lazy arguments and merge them with the basic arguments.
            object[] lazyArguments = getLazyArguments?.Invoke();

            if (lazyArguments != null && lazyArguments.Length > 0)
            {
                if (arguments == null)
                {
                    arguments = lazyArguments;
                }
                else
                {
                    int argumentsLength = arguments.Length;
                    Array.Resize(ref arguments, arguments.Length + lazyArguments.Length);
                    Array.Copy(lazyArguments, 0, arguments, argumentsLength, lazyArguments.Length);
                }
            }
            
            if (IsComponent(concreteType))
            {
                instance = CreateComponent(concreteType, inGlobalScope, scene);
            }
            else
            {
                instance = CreateGeneralClass(concreteType, arguments);
            }

            if (instance != null)
            {
                InjectToFields(instance, arguments);
                InjectToProperties(instance, arguments);
                InjectToMethods(instance, arguments);
            }

            onInstantiate?.Invoke(instance);

            return instance;
        }

        private static object CreateComponent(Type concreteType, bool asGlobal, Scene scene)
        {
            object instance;
            
            if (asGlobal)
            {
                if (multiSceneGameObject == null)
                {
                    multiSceneGameObject = new GameObject(MULTISCENE_GAME_OBJECT_NAME);
                    Object.DontDestroyOnLoad(multiSceneGameObject);
                }

                instance = multiSceneGameObject.AddComponent(concreteType);
            }
            else
            {
                SceneData sceneData = GetOrCreateSceneData(scene);
                instance = sceneData.GameObject.AddComponent(concreteType);
            }

            return instance;
        }

        private static object CreateGeneralClass(Type concreteType, object[] arguments)
        {
            object instance;

            ConstructorInfo constructor = GetInjectOrDefaultConstructor(concreteType);
            if (constructor != null)
            {
                ParameterInfo[] parameterInfos = constructor.GetParameters();
                var parameters = new List<object>();

                var argumentCollection = new ArgumentCollection(arguments);

                foreach (ParameterInfo parameter in parameterInfos)
                {
                    // TODO: Handle IEnumerator exception.
                    
                    object label = DEFAULT_LABEL;
                    
                    var injectAttribute = parameter.GetCustomAttribute<InjectAttribute>();
                    if (injectAttribute != null)
                    {
                        label = injectAttribute.Label;
                    }
                    
                    // Try to resolve the parameter from the arguments.
                    if (argumentCollection.TryResolve(parameter.ParameterType, out object parameterInstance))
                    {
                        parameters.Add(parameterInstance);
                    }
                    else
                    {
                        parameterInstance = ResolveByType(parameter.ParameterType, label);
                        parameters.Add(parameterInstance);
                    }
                }
                
                instance = Activator.CreateInstance(concreteType, parameters.ToArray());
            }
            else
            {
                instance = Activator.CreateInstance(concreteType);
            }
            
            return instance;
        }

        private static void InjectToFields(object instance, object[] arguments)
        {
            var argumentCollection = new ArgumentCollection(arguments);

            Type type = instance.GetType();
            
            // TODO: Maybe add some kind of cache for the fields of a type. 
            
            foreach (FieldInfo field in type.GetAllFields())
            {
                var injectAttribute = field.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute != null)
                {
                    if (field.GetValue(instance) == null)
                    {
                        // Try to resolve the parameter from the arguments.
                        if (argumentCollection.TryResolve(field.FieldType, out object parameterInstance))
                        {
                            field.SetValue(instance, parameterInstance);
                        }
                        else
                        {
                            parameterInstance = ResolveByType(field.FieldType, injectAttribute.Label);
                            field.SetValue(instance, parameterInstance);
                        }
                    }
                }
            }
        }

        private static void InjectToProperties(object instance, object[] arguments)
        {
            var argumentCollection = new ArgumentCollection(arguments);

            Type type = instance.GetType();

            // TODO: Maybe add some kind of cache for the properties of a type. 

            foreach (PropertyInfo property in type.GetAllProperties())
            {
                var injectAttribute = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute != null)
                {
                    if (property.GetValue(instance) == null)
                    {
                        // Try to resolve the parameter from the arguments.
                        if (argumentCollection.TryResolve(property.PropertyType, out object parameterInstance))
                        {
                            property.SetValue(instance, parameterInstance);
                        }
                        else
                        {
                            parameterInstance = ResolveByType(property.PropertyType, injectAttribute.Label);
                            property.SetValue(instance, parameterInstance);
                        }
                    }
                }
            }
        }

        private static void InjectToMethods(object instance, object[] arguments)
        {
            Type type = instance.GetType();

            // TODO: Maybe add some kind of cache for the methods of a type. 

            foreach (MethodInfo method in type.GetAllMethods())
            {
                var argumentCollection = new ArgumentCollection(arguments);

                var injectAttribute = method.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute != null)
                {
                    ParameterInfo[] parameterInfos = method.GetParameters();
                    var parameters = new List<object>();

                    foreach (ParameterInfo parameter in parameterInfos)
                    {
                        // TODO: Handle Array/List exception.

                        // Try to resolve the parameter from the arguments.
                        if (argumentCollection.TryResolve(parameter.ParameterType, out object parameterInstance))
                        {
                            parameters.Add(parameterInstance);
                        }
                        else
                        {
                            parameterInstance = ResolveByType(parameter.ParameterType, injectAttribute.Label);
                            parameters.Add(parameterInstance);
                        }
                    }

                    method.Invoke(instance, parameters.ToArray());
                }
            }
        }

        /// <summary>
        ///     Clears all resolvers.
        /// </summary>
        public static void Reset()
        {
            ResetScenes();
            ResetGlobal();
        }

        /// <summary>
        ///     Clears global resolvers.
        /// </summary>
        public static void ResetGlobal()
        {
            globalResolvers.Clear();

            if (multiSceneGameObject != null)
            {
                Object.Destroy(multiSceneGameObject);
                multiSceneGameObject = null;
            }
        }

        /// <summary>
        ///     Clears the currently active scene's resolvers.
        /// </summary>
        public static void ResetScene()
        {
            ResetScene(GetActiveScene());
        }

        /// <summary>
        ///     Clears a specific scene's resolvers.
        /// </summary>
        public static void ResetScene(Scene scene)
        {
            if (sceneResolvers.TryGetValue(scene, out SceneData sceneData))
            {
                if (sceneData.GameObject != null)
                {
                    Object.Destroy(sceneData.GameObject);
                }

                sceneResolvers.Remove(scene);
            }
        }

        /// <summary>
        ///     Clears all scene specific resolvers for all scenes.
        /// </summary>
        public static void ResetScenes()
        {
            tempSceneList.AddRange(sceneResolvers.Keys);

            for (var i = 0; i < tempSceneList.Count; i++)
            {
                var scene = tempSceneList[i];
                ResetScene(scene);
            }

            tempSceneList.Clear();
        }

        private static ConstructorInfo GetInjectOrDefaultConstructor(Type type)
        {
            ConstructorInfo[] constructorInfos = type.GetConstructors();
            foreach (ConstructorInfo constructor in constructorInfos)
            {
                var autowiredAttribute = Attribute.GetCustomAttribute(constructor, typeof(InjectAttribute));
                if (autowiredAttribute != null)
                {
                    return constructor;
                }
            }

            if (constructorInfos.Length > 1)
            {
                throw new Exception($"Class {type} has more than one constructor. Please define one of those as [Inject]");
            }

            if (constructorInfos.Length == 1)
            {
                return constructorInfos[0];
            }

            return null;
        }

        /// <summary>
        ///     Checks whether there's registered a resolver for a specific type.
        /// </summary>
        /// <param name="contractType">The specific type.</param>
        /// <param name="label">Whether to search for a labeled resolver.</param>
        /// <param name="includeActiveScene">Whether to search for a scene specific resolver if a global one isn't found.</param>
        /// <returns>True if registered, false otherwise.</returns>
        public static bool HasResolver(Type contractType, object label = DEFAULT_LABEL, bool includeActiveScene = true)
        {
            return globalResolvers.TryGetValue(contractType, out ResolverCollection collection) && collection.TryGetByLabel(label, out Resolver _) || (includeActiveScene && HasSceneResolver(GetActiveScene(), contractType, label));
        }
        
        /// <summary>
        ///     Checks whether there's created a scene-specific resolver for a specific type in the specified scene.
        /// </summary>
        /// <param name="scene">The specific scene.</param>
        /// <param name="contractType">The specific type.</param>
        /// <param name="label">Whether to search for a labeled resolver.</param>
        /// <returns>True if resolver exists, false otherwise.</returns>
        public static bool HasSceneResolver(Scene scene, Type contractType, object label = DEFAULT_LABEL)
        {
            if (!sceneResolvers.TryGetValue(scene, out SceneData sceneData))
            {
                return false;
            }

            return sceneData.Resolvers.TryGetValue(contractType, out ResolverCollection collection) && collection.TryGetByLabel(label, out Resolver _);
        }

        private static Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }

        private static bool IsComponent(Type type)
        {
            return type.IsSubclassOf(typeof(Component));
        }
        
        private class SceneData
        {
            public Dictionary<Type, ResolverCollection> Resolvers { get; } = new();
            public GameObject GameObject { get; set; }
        }
        
        private class Resolver
        {
            public Func<object> Instantiate { get; set; }
            public string Label { get; set; }
        }

        private class ResolverCollection
        {
            private readonly Dictionary<object, Resolver> labeledResolvers = new();
            private Resolver defaultResolver;

            public bool TryGetByLabel(object label, out Resolver resolver)
            {
                if (label == null)
                {
                    resolver = defaultResolver;
                    return resolver != null;
                }
                
                return labeledResolvers.TryGetValue(label, out resolver);
            }
            
            public void Add(object label, Resolver resolver)
            {
                if (defaultResolver == null || label == null)
                {
                    defaultResolver = resolver;
                }

                if (label != null)
                {
                    labeledResolvers[label] = resolver;
                }
            }
        }
    }
}