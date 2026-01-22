#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// The EditModeContext class provides a static context for managing and running dependency
    /// injection bootstrappers in Unity's edit mode through the Service Injection framework.
    /// </summary>
    [InitializeOnLoad]
    public static class EditModeContext
    {
        private static readonly List<IBootstrapper> bootstrapper = new List<IBootstrapper>();

        static EditModeContext()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Run();
        }

        /// <summary>
        /// Adds a bootstrapper to the EditModeContext and immediately runs the context to handle service registration and resolution.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper instance to be added to the context. This instance defines registration and dependency management for services.</param>
        public static void AddBootstrapper(IBootstrapper bootstrapper)
        {
            EditModeContext.bootstrapper.Add(bootstrapper);
            Run();
        }

        /// <summary>
        /// Adds a bootstrapper of the specified type to the EditModeContext and immediately
        /// runs the context to handle service registration and resolution.
        /// </summary>
        /// <typeparam name="T">The type of the bootstrapper to be added. This type must implement the IBootstrapper interface and have a parameterless constructor.</typeparam>
        public static void AddBootstrapper<T>() where T : IBootstrapper
        {
            AddBootstrapper(Activator.CreateInstance<T>());
        }
        
        public static void Run()
        {
            ServiceLocator.Reset();
            
            ServiceLocator.BeginRegister();
            foreach (IBootstrapper bootstrapper in bootstrapper)
            {
                bootstrapper.Boot();
            }
            ServiceLocator.EndRegister();
            
            foreach (IBootstrapper bootstrapper in bootstrapper)
            {
                bootstrapper.BootCompleted();
            }

            foreach (IBootstrapper bootstrapper in bootstrapper)
            {
                bootstrapper.Resolve();
            }
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                Run();
            }
        }
    }
}

#endif