using System;
using UnityEngine;

public class DialogueTree : ExecutableScriptableObject
{
    [Serializable]
    public class Answer
    {
        [Multiline]
        public string Text;
        public UnityEngine.Object End;
    }

    [SerializeField] private string m_Reply = default;
    [SerializeField] private Answer[] m_Answers = default;

    public string Reply => m_Reply;
    public Answer[] Answers => m_Answers;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var answer in m_Answers)
            ValidateEnd(ref answer.End);
    }
#endif
}
