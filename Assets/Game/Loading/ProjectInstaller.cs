using UnityEngine;

/// <summary>
/// Installer for common dependencies
/// </summary>
public sealed class ProjectInstaller : ScriptableObjectExtendedInstaller
{
    [Header("Installer for common dependencies")]
    [SerializeField] private LoaderCurtain m_LoaderCurtain;

    public override void InstallBindings()
    {
        BindSingle<SceneLoader>();

        BindPrefab(m_LoaderCurtain);

        BindMonoBehaviour<CoroutineRunner>();
    }
}
