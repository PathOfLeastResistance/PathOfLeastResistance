using UnityEditor;
using UnityEngine;
using Zenject;

public class DialogueManager
{
    [Inject] private readonly IExecutableScriptableObjectResolver<SceneAsset> m_SceneResolver = default;
    [Inject] private readonly IExecutableScriptableObjectResolver<DialogueLine> m_LineResolver = default;
    [Inject] private readonly IExecutableScriptableObjectResolver<DialogueTree> m_TreeResolver = default;

    public void Proceed(Object nextState)
    {
        switch (nextState)
        {
            case SceneAsset state:
                m_SceneResolver.Resolve(state);
                break;

            case DialogueLine state:
                m_LineResolver.Resolve(state);
                break;

            case DialogueTree state:
                m_TreeResolver.Resolve(state);
                break;

            default:
                Debug.LogError($"Unsupported type {nextState.GetType()}");
                break;
        }
    }
}
