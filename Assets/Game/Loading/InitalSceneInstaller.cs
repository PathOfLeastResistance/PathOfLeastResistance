using UnityEngine;

/// <summary>
/// Installer for bootstrap scene, will be destroyed
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
    private void OnValidate() =>
        EditorHelper.AddScene(m_BootsTrapSettings.StartScene, this);
#endif
}
