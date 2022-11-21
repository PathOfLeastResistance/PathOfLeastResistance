#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

using UnityEngine;

/// <summary>
/// Installer for bootstrap scene
/// </summary>
public sealed class InitalSceneInstaller : ScriptableObjectExtendedInstaller
{
    [Header("Installer for bootstrap scene")]
    [SerializeField] private BootstrapSettings m_BootsTrapSettings;

    public override void InstallBindings()
    {
        BindInstance(m_BootsTrapSettings);
        BindMonoBehaviour<AppRunner>().NonLazy();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_BootsTrapSettings.StartScene == null)
            return;

        var path = AssetDatabase.GetAssetPath(m_BootsTrapSettings.StartScene);
        var scenes = EditorBuildSettings.scenes;

        if (!scenes.Any(scene => scene.path == path))
        {
            var scenesList = scenes.ToList();
            var newScene = new EditorBuildSettingsScene(path, true);
            scenesList.Add(newScene);
            EditorBuildSettings.scenes = scenesList.ToArray();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
