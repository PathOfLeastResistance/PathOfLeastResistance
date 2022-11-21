using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public sealed class BootstrapSettings
{
    [SerializeField] private SceneAsset m_StartScene = default;

    public SceneAsset StartScene => m_StartScene;
}
