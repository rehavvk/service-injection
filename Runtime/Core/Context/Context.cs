using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rehawk.ServiceInjection
{
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