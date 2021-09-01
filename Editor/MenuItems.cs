using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rehawk.ServiceInjection.Editor
{
    public static class MenuItems
    {
        [MenuItem("Assets/Create/Service Injection/ProjectContext")]
        private static void CreateProjectContext()
        {
            string path = GetSelectedPath();
            path += "/ProjectContext.prefab";
            
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
            {
                GameObject prefabObj = new GameObject("ProjectContext");
                prefabObj.AddComponent<ProjectContext>();

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(prefabObj, path);

                Object.DestroyImmediate(prefabObj);

                Selection.activeObject = prefab;
            }
        }

        [MenuItem("GameObject/Service Injection/SceneContext", false, 51)]
        private static void CreateSceneContext()
        {
            GameObject obj = new GameObject("SceneContext");
            obj.AddComponent<SceneContext>();

            Selection.activeObject = obj;
        }

        private static string GetSelectedPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return path;
        }
    }
}