using UnityEngine;

public class DialogueLine : ExecutableScriptableObject
{
    [SerializeField] private PhrasesCouple[] m_PhrasesCouples = default;
    [SerializeField] private Object m_OnEnd = default;

    public PhrasesCouple[] PhrasesCouples => m_PhrasesCouples;
    public Object OnEnd => m_OnEnd;

#if UNITY_EDITOR
    private void OnValidate() =>
        ValidateEnd(ref m_OnEnd);
#endif
}
