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
        public static T Resolve<T>(this object _, bool includeActiveScene = true)
        {
            return ServiceLocator.Resolve<T>(includeActiveScene);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}()"/>
        public static T ResolveFromScene<T>(this object _)
        {
            return ServiceLocator.ResolveFromScene<T>();
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}(Scene)"/>
        public static T ResolveFromScene<T>(this object _, Scene scene)
        {
            return ServiceLocator.ResolveFromScene<T>(scene);
        }
    }
}