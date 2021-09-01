using System;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : PropertyAttribute {}
}