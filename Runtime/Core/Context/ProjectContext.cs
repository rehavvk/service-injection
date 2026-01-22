using UnityEngine;
using UnityEngine.Assertions;

namespace Rehawk.ServiceInjection
{
    /// <summary>
    /// The ProjectContext class serves as the main context for the project, managing dependency injection
    /// and maintaining a singleton instance throughout the application's lifecycle.
    /// It is responsible for ensuring that only one instance is created and properly initialized.
    /// </summary>
    public class ProjectContext : Context
    {
        private const string PROJECT_CONTEXT_RESOURCE_PATH = "ProjectContext";

        private static ProjectContext instance;
        
        public static ProjectContext Instance
        {
            get
            {
                if (instance == null)
                {
                    InstantiateAndInitialize();
                    Assert.IsNotNull(instance);
                }

                return instance;
            }
        }
        
        private static void InstantiateAndInitialize()
        {
            Assert.IsNull(FindObjectOfType<ProjectContext>(), "Tried to create multiple instances of ProjectContext!");

            GameObject prefab = TryGetPrefab();

            var prefabWasActive = false;

            if (prefab == null)
            {
                instance = new GameObject("ProjectContext").AddComponent<ProjectContext>();
            }
            else
            {
                prefabWasActive = prefab.activeSelf;

                GameObject gameObjectInstance;
                
#if UNITY_EDITOR
                if (prefabWasActive)
                {
                    // This ensures the prefab's Awake() methods don't fire (and, if in the editor, that the prefab file doesn't get modified)
                    gameObjectInstance = Instantiate(prefab, PrefabUtils.GetOrCreateInactivePrefabParent());
                    gameObjectInstance.SetActive(false);
                    gameObjectInstance.transform.SetParent(null, false);
                }
                else
                {
                    gameObjectInstance = Instantiate(prefab);
                }
#else
                if (prefabWasActive)
                {
                    prefab.SetActive(false);
                    gameObjectInstance = Instantiate(prefab);
                    prefab.SetActive(true);
                }
                else
                {
                    gameObjectInstance = Instantiate(prefab);
                }
#endif

                instance = gameObjectInstance.GetComponent<ProjectContext>();

                Assert.IsNotNull(instance, $"Could not find ProjectContext component on prefab 'Resources/{PROJECT_CONTEXT_RESOURCE_PATH}.prefab'");
            }

            DontDestroyOnLoad(instance.gameObject);
            
            // Note: We use Initialize instead of awake here in case someone calls
            // ProjectContext.Instance while ProjectContext is initializing
            instance.Initialize();

            if (prefabWasActive)
            {
                // We always instantiate it as disabled so that Awake and Start events are triggered after inject
                instance.gameObject.SetActive(true);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            instance = null;
        }

        public static GameObject TryGetPrefab()
        {
            ProjectContext[] prefabs = Resources.LoadAll<ProjectContext>(PROJECT_CONTEXT_RESOURCE_PATH);

            if (prefabs.Length > 0)
            {
                Assert.IsTrue(prefabs.Length == 1, $"Found multiple project context prefabs at resource path '{PROJECT_CONTEXT_RESOURCE_PATH}'");
                return prefabs[0].gameObject;
            }
            
            return null;
        }

		private void Awake() 
		{
			if (Application.isPlaying) 
			{
				DontDestroyOnLoad(gameObject);
			}
		}
		
        public void EnsureIsInitialized()
        {
            // Do nothing - Initialize occurs in Instance property
        }

        private void Initialize()
        {
            Run();
        }
    }
}