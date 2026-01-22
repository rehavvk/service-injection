using System;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// Marks constructors, methods, fields or properties for dependency injection.
    /// Any member marked with this attribute will attempt to resolve its dependency through a service container.
    /// For this the instance has to be created via <see cref="ServiceLocator.CreateInstance"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectAttribute : PropertyAttribute
    {
        public object Label { get; set; }
    }
}