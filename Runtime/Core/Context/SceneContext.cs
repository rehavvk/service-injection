using UnityEngine;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Represents a specific context in a Unity scene that facilitates the injection
    /// of dependencies through the Service Injection system. This class inherits
    /// from the abstract Context base class and is designed to manage service
    /// registration and resolution specific to the Unity scene it is part of.
    /// SceneContext is automatically executed at a very early stage of the Unity
    /// execution order due to its DefaultExecutionOrder attribute to ensure that all
    /// dependency injection setup is completed before other components in the scene
    /// are initialized.
    /// This class acts as a foundational element for the dependency injection system,
    /// allowing the registration and resolution of services within the scope of the
    /// current Unity scene.
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class SceneContext : Context
    {
        [SerializeField] private bool autoRun = true;
        
        private void Awake()
        {
            ProjectContext.Instance.EnsureIsInitialized();

            if (autoRun)
            {
                Run();
            }
        }
    }
}