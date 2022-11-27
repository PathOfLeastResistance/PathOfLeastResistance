using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogueManager : MonoBehaviour, IEventResolver<DialogueContainer>
{
    private DialogueContainer m_CurrentState;
    private int m_CurrentStateIndex;
    private bool m_IsEnded;

    private readonly List<GameObject> m_TemporaryObject = new List<GameObject>();

    [Inject] private readonly EventManager m_DialogueManager = default;
    [Inject] private readonly BackgroundManager m_DialogueBackgroundManager = default;
    [Inject] private readonly CharacterManager m_CharacterManager = default;

    [SerializeField] private Transform m_PhrasesRoot = default;
    [SerializeField] private Button m_NextButton = default;

    [SerializeField] private PhraseController m_PhraseControllerPrefab = default;
    [SerializeField] private PhraseController m_ChoiceControllerPrefab = default;

    private void Awake()
    {
        m_NextButton.onClick.AddListener(Next);
    }

    public void Resolve(DialogueContainer state)
    {
        Clear();
        gameObject.SetActive(true);

        m_CurrentState = state;
        m_CurrentStateIndex = 0;
        m_IsEnded = false;

        m_DialogueBackgroundManager.ActivateBackground(state.Backgroud);
        m_CharacterManager.ActivateCharacter(state.LeftCharacter);
        m_CharacterManager.ActivateCharacter(state.RightCharacter);
        Next();
    }

    private void Next()
    {
        if (m_CurrentStateIndex < m_CurrentState.Phrases.Length)
        {
            var phrase = m_CurrentState.Phrases[m_CurrentStateIndex++];
            CreateReply(m_PhraseControllerPrefab, phrase);
        }
        else
        {
            if (!m_IsEnded)
            {
                m_IsEnded = true;

                foreach (var end in m_CurrentState.Endings)
                {
                    var choice = CreateReply(m_ChoiceControllerPrefab, end.Phrase);
                    choice.GetComponent<Button>().onClick.AddListener(() => Deinit(end.End));
                }
            }
        }
    }

    private GameObject CreateReply(PhraseController prefeab, Phrase phrase)
    {
        var reply = Instantiate(prefeab, m_PhrasesRoot);
        reply.Init(phrase);
        m_TemporaryObject.Add(reply.gameObject);
        return reply.gameObject;
    }

    private void Deinit(Object onEnd)
    {
        Clear();

        if (!(onEnd is DialogueContainer))
        {
            gameObject.SetActive(false);
            m_DialogueBackgroundManager.HideAnimated();
        }

        m_DialogueManager.Proceed(onEnd);
    }

    private void Clear()
    {
        m_CharacterManager.HideAll();

        foreach (var temp in m_TemporaryObject)
            Destroy(temp);

        m_TemporaryObject.Clear();
    }
}
