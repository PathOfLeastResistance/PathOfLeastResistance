using UnityEngine;
using Zenject;

public class DialogueStarter : MonoBehaviour
{
    [Inject] private readonly DialogueManager m_DialogueManager = default;
    [Inject] private readonly IExecutableObject m_ExecutableObject = default;

    private void Start() =>
        m_DialogueManager.Proceed(m_ExecutableObject);
}
