using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EventManager
{
    [Inject] private readonly IEventResolver<DialogueContainer> m_DialogueResolver = default;
    [Inject] private readonly IEventResolver<Level> m_LevelResolver = default;

    public void Proceed(IEventState nextState) =>
        Proceed(nextState as Object);

    public void Proceed(Object nextState)
    {
        switch (nextState)
        {
            case DialogueContainer state:
                m_DialogueResolver.Resolve(state);
                break;

            case Level state:
                m_LevelResolver.Resolve(state);
                break;

            default:
                var stateInterface = nextState.GetComponent<IEventState>();
                if (stateInterface == null)
                {
                    Debug.LogError($"Unsupported type {nextState.GetType()}");
                }
                else
                {
                    Proceed(stateInterface);
                }
                break;
        }
    }
}
