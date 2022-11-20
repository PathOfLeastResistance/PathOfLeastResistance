using UnityEngine;

public sealed class AppLoadingInstaller : ScriptableObjectExtendedInstaller
{
    [SerializeField] private LoaderCurtain m_LoaderCurtain;

    public override void InstallBindings()
    {
        base.InstallBindings();

        BindSingle<AppStateManager>();
        BindSingle<SceneLoader>();

        BindPrefab(m_LoaderCurtain);
    }
}
