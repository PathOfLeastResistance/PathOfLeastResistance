using UnityEditor;
using Zenject;

public class TransitorToNextScene : IExecutableScriptableObjectResolver<SceneAsset>
{
    [Inject] private readonly ISceneLoader m_SceneLoader = default;

    public void Resolve(SceneAsset state) =>
        m_SceneLoader.Load(state);
}
