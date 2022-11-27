using UnityEditor;
using Zenject;

public class TransitorToNextScene : IEventResolver<SceneAsset>
{
    [Inject] private readonly ISceneLoader m_SceneLoader = default;

    public void Resolve(SceneAsset state) =>
        m_SceneLoader.Load(state);
}
