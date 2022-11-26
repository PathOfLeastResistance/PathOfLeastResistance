using UnityEditor;
using UnityEngine;
using Zenject;

public class DialogueManager
{
    [Inject] private readonly IExecutableScriptableObjectResolver<SceneAsset> m_SceneResolver = default;
    [Inject] private readonly IExecutableScriptableObjectResolver<DialogueContainer> m_DialogueResolver = default;

    public void Proceed(IExecutableObject nextState) =>
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

            default:
                Debug.LogError($"Unsupported type {nextState.GetType()}");
                break;
        }
    }
}
