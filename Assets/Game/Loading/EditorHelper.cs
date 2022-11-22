#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorHelper
{
    public static void Erase(ref Object value, Object target)
    {
        if (value != null)
        {
            value = null;
            EditorUtility.SetDirty(target);
        }
    }

    public static void AddScene(SceneAsset scene, Object target)
    {
        if (scene == null)
            return;

        var path = AssetDatabase.GetAssetPath(scene);
        var scenes = EditorBuildSettings.scenes;

        if (!scenes.Any(scene => scene.path == path))
        {
            var scenesList = scenes.ToList();
            var newScene = new EditorBuildSettingsScene(path, true);
            scenesList.Add(newScene);
            EditorBuildSettings.scenes = scenesList.ToArray();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
