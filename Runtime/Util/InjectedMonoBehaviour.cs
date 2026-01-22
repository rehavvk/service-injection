using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Derive from this instead of <c>MonoBehaviour</c> to inject all fields, properties or methods tagged with the inject attribute <see cref="InjectAttribute"/>.
    /// If you want to use Awake() in your script, override the method and call <c>base.Awake();</c> or <c>ResolveDependencies();</c>
    /// </summary>
    public abstract class InjectedMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        ///     If you want to use Awake() in your script, override the method and call <c>base.Awake();</c> or <c>ResolveDependencies();</c>
        /// </summary>
        protected virtual void Awake()
        {
            ResolveDependencies();
        }
        
        /// <inheritdoc cref="ServiceLocator.ResolveDependencies{T}(T)"/>
        public void ResolveDependencies()
        {
            ServiceLocator.ResolveDependencies(this);
        }
        
        /// <inheritdoc cref="ServiceLocator.Resolve{T}"/>
        public T Resolve<T>(object label = ServiceLocator.DEFAULT_LABEL, bool includeActiveScene = true)
        {
            return ServiceLocator.Resolve<T>(label, includeActiveScene);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}(object)"/>
        public T ResolveFromScene<T>(object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene<T>(label);
        }

        /// <inheritdoc cref="ServiceLocator.ResolveFromScene{T}(Scene, object)"/>
        public static T ResolveFromScene<T>(Scene scene, object label = ServiceLocator.DEFAULT_LABEL)
        {
            return ServiceLocator.ResolveFromScene<T>(scene, label);
        }
    }
}