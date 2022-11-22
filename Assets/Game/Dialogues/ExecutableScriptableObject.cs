using UnityEditor;
using UnityEngine;

public abstract class ExecutableScriptableObject : ScriptableObject
{
#if UNITY_EDITOR
    protected void ValidateEnd(ref Object end)
    {
        if (end == null)
            return;

        switch (end)
        {
            case SceneAsset scene:
                EditorHelper.AddScene(scene, this);
                break;

            case ExecutableScriptableObject executableScriptableObject:
                break;

            default:
                EditorHelper.Erase(ref end, this);
                Debug.LogWarning($"Incorrect object type. Alowed types:\n{typeof(SceneAsset)}\n{typeof(ExecutableScriptableObject)}\n");
                break;
        }
    }
#endif
}
