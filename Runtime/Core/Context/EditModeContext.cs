#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Rehawk.ServiceInjection
{
    [InitializeOnLoad]
    public static class EditModeContext
    {
        private static readonly List<IBootstrapper> bootstrapper = new List<IBootstrapper>();

        static EditModeContext()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Run();
        }
        
        public static void AddBootstrapper(IBootstrapper bootstrapper)
        {
            EditModeContext.bootstrapper.Add(bootstrapper);
            Run();
        }
        
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