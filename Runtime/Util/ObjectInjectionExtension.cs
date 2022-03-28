using System;
using UnityEngine.SceneManagement;

namespace Rehawk.ServiceInjection
{
    public static class ObjectInjectionExtension
    {
        /// <inheritdoc cref="ServiceLocator.ResolveDependencies{T}(T)"/>
        public static void ResolveDependencies(this object instance)
        {
            ServiceLocator.ResolveDependencies(instance);
        }
        
        /// <inheritdoc cref="ServiceLocator.Resolve{T}"/>
        public static T Resolve<T>(this object _, object label = ServiceLocator.DEFAULT_LABEL, bool includeActiveScene = true)
        {
            return ServiceLocator.Resolve<T>(label, includeActiveScene);
        }

        /// <inheritdoc cref="ServiceLocator.Resolve(Type, object, bool)"/>
        public static object Resolve(this object _, Type type, object label = ServiceLocator.DEFAULT_LABEL, bool includeActiveScene = true)
        {
            return ServiceLocator.Resolve(type, label, includeActiveScene);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}(object)"/>
        public static T ResolveFromScene<T>(this object _, object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene<T>(label);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene(Type, object)"/>
        public static object ResolveFromScene(this object _, Type type, object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene(type, label);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}(Scene, object)"/>
        public static T ResolveFromScene<T>(this object _, Scene scene, object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene<T>(scene, label);
        }
        
        /// <inheritdoc cref="ServiceLocator.ResolveFromScene(Type, Scene, object)"/>
        public static object ResolveFromScene(this object _, Type type, Scene scene, object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene(type, scene, label);
        }
    }
}