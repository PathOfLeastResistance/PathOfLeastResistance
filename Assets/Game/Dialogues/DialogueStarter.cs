using UnityEngine;
using Zenject;

public class DialogueStarter : MonoBehaviour
{
    [Inject] private readonly DialogueManager m_DialogueManager = default;
    [Inject] private readonly ExecutableScriptableObject m_ExecutableScriptableObject = default;

    private void Start() =>
        m_DialogueManager.Proceed(m_ExecutableScriptableObject);
}
