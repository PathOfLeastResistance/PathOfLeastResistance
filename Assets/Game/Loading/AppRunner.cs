using UnityEngine;
using Zenject;

public sealed class AppRunner : MonoBehaviour
{
    [Inject] private readonly SceneLoader m_SceneLoader = default;
    [Inject] private readonly BootstrapSettings m_Settings = default;

    private void Start() =>
        m_SceneLoader.Load(m_Settings.StartScene);

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
