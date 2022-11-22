using UnityEngine;
using Zenject;

public class DialoguePanel :
    MonoBehaviour,
    IExecutableScriptableObjectResolver<DialogueLine>,
    IExecutableScriptableObjectResolver<DialogueTree>
{
    [Inject] private readonly DialogueManager m_DialogueManager = default;

    public void Resolve(DialogueLine state)
    {
        throw new System.NotImplementedException();
    }

    public void Resolve(DialogueTree state)
    {
        throw new System.NotImplementedException();
    }
}
