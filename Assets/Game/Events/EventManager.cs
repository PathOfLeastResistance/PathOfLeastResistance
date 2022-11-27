using UnityEditor;
using UnityEngine;
using Zenject;

public class EventManager
{
    [Inject] private readonly IEventResolver<SceneAsset> m_SceneResolver = default;
    [Inject] private readonly IEventResolver<DialogueContainer> m_DialogueResolver = default;
    [Inject] private readonly IEventResolver<Level> m_LevelResolver = default;

    public void Proceed(IEventState nextState) =>
        Proceed(nextState as Object);

    public void Proceed(Object nextState)
    {
        switch (nextState)
        {
            case SceneAsset state:
                m_SceneResolver.Resolve(state);
                break;

            case DialogueContainer state:
                m_DialogueResolver.Resolve(state);
                break;

            case Level state:
                m_LevelResolver.Resolve(state);
                break;

            default:
                Debug.LogError($"Unsupported type {nextState.GetType()}");
                break;
        }
    }
}
