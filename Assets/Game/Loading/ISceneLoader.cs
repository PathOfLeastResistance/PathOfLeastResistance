using System;
using UnityEditor;

public interface ISceneLoader
{
    void Load(SceneAsset scene, Action onLoaded = null, bool useCurtain = true);
}