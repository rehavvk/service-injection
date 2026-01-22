using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Represents an abstract base class for managing the lifecycle and dependency injection of services
    /// within a specific context in a Unity application.
    /// </summary>
    /// <remarks>
    /// The Context class is designed to facilitate dependency injection by utilizing a service locator
    /// pattern. It ensures that services and dependencies are properly registered, bootstrapped, and resolved.
    /// This class should be inherited by specific contexts that need to manage their own set of services,
    /// such as project-wide or scene-specific contexts.
    /// </remarks>
    /// <example>
    /// Subclasses may use this base class to implement unique functionalities for managing their specific contexts.
    /// </example>
    public abstract class Context : MonoBehaviour
    {
        [SerializeField] private List<MonoBootstrapper> monoBootstrapper = new List<MonoBootstrapper>();
        
        private bool isInitialized;

        protected void Run()
        {
            Assert.IsFalse(isInitialized, "Tried to run a context twice.");
            
            ServiceLocator.BeginRegister();
            foreach (IBootstrapper bootstrapper in monoBootstrapper)
            {
                bootstrapper.Boot();
            }
            ServiceLocator.EndRegister();
            
            foreach (IBootstrapper bootstrapper in monoBootstrapper)
            {
                bootstrapper.BootCompleted();
            }

            foreach (IBootstrapper bootstrapper in monoBootstrapper)
            {
                bootstrapper.Resolve();
            }
            
            isInitialized = true;
        }
    }
}