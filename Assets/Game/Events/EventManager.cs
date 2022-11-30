using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class EventManager
{
    [Inject] private readonly IEventResolver<DialogueContainer> m_DialogueResolver = default;
    [Inject] private readonly IEventResolver<Level> m_LevelResolver = default;
    [Inject] private readonly BackgroundManager m_DialogueBackgroundManager = default;

    public void Proceed(IEventState nextState) =>
        Proceed(nextState as Object);

    public void Proceed(Object nextState)
    {
        switch (nextState)
        {
            case DialogueContainer state:
                m_DialogueResolver.Resolve(state);
                m_DialogueBackgroundManager.ActivateBackground(state.Backgroud);
                break;

            case Level state:
                m_LevelResolver.Resolve(state);
                m_DialogueBackgroundManager.ActivateBackground(null);
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
