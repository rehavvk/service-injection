using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rehawk.ServiceInjection
{
    public static class PrefabUtils
    {
        private static GameObject disabledIndestructibleGameObject;
        
        // Returns a Transform in the DontDestroyOnLoad scene (or, if we're not in play mode, within the current active scene)
        // whose GameObject is inactive, and whose hide flags are set to HideAndDontSave. We can instantiate prefabs in here
        // without any of their Awake() methods firing.
        internal static Transform GetOrCreateInactivePrefabParent()
        {
            if (disabledIndestructibleGameObject == null || (!Application.isPlaying && disabledIndestructibleGameObject.scene != SceneManager.GetActiveScene()))
            {
                var go = new GameObject("UtilInternal_PrefabParent");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.SetActive(false);

                if (Application.isPlaying)
                {
                    Object.DontDestroyOnLoad(go);
                }

                disabledIndestructibleGameObject = go;
            }

            return disabledIndestructibleGameObject.transform;
        }
    }
}