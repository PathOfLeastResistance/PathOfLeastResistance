using UnityEngine;

public class DialogueContainer : ScriptableObject, IEventState
{
    [System.Serializable]
    public class Choice
    {
        public Phrase Phrase;
        public Object End;
    }

    [SerializeField] private CharacterIconController m_LeftCharacter = default;
    [SerializeField] private CharacterIconController m_RightCharacter = default;

    [SerializeField] private BackgroundController m_Backgroud = default;

    [SerializeField] private Phrase[] m_Phrases = default;
    [SerializeField] private Choice[] m_Endings = default;

    public CharacterIconController LeftCharacter => m_LeftCharacter;
    public CharacterIconController RightCharacter => m_RightCharacter;
    public BackgroundController Backgroud => m_Backgroud;

    public Phrase[] Phrases => m_Phrases;
    public Choice[] Endings => m_Endings;
}
