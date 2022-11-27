using UnityEngine;
using Zenject;

public class EventStarter : MonoBehaviour
{
    [Inject] private readonly EventManager m_EventManager = default;
    [Inject] private readonly IEventState m_InitialEvent = default;

    private void Start() =>
        m_EventManager.Proceed(m_InitialEvent);
}
