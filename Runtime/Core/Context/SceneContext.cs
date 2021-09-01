using UnityEngine;

namespace Rehawk.ServiceInjection
{
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