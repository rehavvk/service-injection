using System;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectAttribute : PropertyAttribute
    {
        public object Label { get; set; }
    }
}