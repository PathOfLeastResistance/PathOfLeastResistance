using UnityEngine;
using UnityEngine.UI;

public class PhoneHelpManager : MonoBehaviour, ICoroutineRunner
{
    private PlainAnimation m_AnimationShow;
    private PlainAnimation m_AnimationHide;
    private int m_CurrentStateIndex;
    private bool m_IsEnded;

    [SerializeField] private Button m_ShowPhoneButton = default;

    [SerializeField] private Transform m_PhrasesRoot = default;
    [SerializeField] private Button m_NextButton = default;

    [SerializeField] private float m_Duration = 0.3f;
    [SerializeField] private AnimationCurve m_Alpha = default;
    [SerializeField] private AnimationCurve m_XPosition = default;
    [SerializeField] private CanvasGroup m_CanvasGroup = default;
    [SerializeField] private Transform m_MovingTransform = default;
    [SerializeField] private Transform m_MovingTransformHideTarget = default;
    [SerializeField] private Transform m_MovingTransformShowTarget = default;

    [SerializeField] private DialogueContainer m_Dialogues = default;

    [SerializeField] private PhraseController m_PhraseControllerPrefab = default;
    [SerializeField] private PhraseController m_ChoiceControllerPrefab = default;

    private void Awake()
    {
        m_AnimationShow = new PlainAnimation(this, m_Duration, SetShowAnimationState);
        m_AnimationHide = new PlainAnimation(this, m_Duration, SetHideAnimationState);

        m_ShowPhoneButton.onClick.AddListener(ShowHelp);
        m_NextButton.onClick.AddListener(Next);
        m_CurrentStateIndex = 0;
        m_IsEnded = false;
    }

    private void ShowHelp()
    {
        m_AnimationShow.StartAnimation();
        m_ShowPhoneButton.gameObject.SetActive(false);
        Next();
    }

    private void Next()
    {
        if (m_CurrentStateIndex < m_Dialogues.Phrases.Length)
        {
            var phrase = m_Dialogues.Phrases[m_CurrentStateIndex++];
            CreateReply(m_PhraseControllerPrefab, phrase);
        }
        else
        {
            if (!m_IsEnded)
            {
                m_IsEnded = true;

                foreach (var end in m_Dialogues.Endings)
                {
                    var choice = CreateReply(m_ChoiceControllerPrefab, end.Phrase);
                    choice.GetComponent<Button>().onClick.AddListener(HideHelp);
                }
            }
        }
    }

    private void HideHelp()
    {
        m_ShowPhoneButton.gameObject.SetActive(true);
        m_AnimationHide.StartAnimation();
    }

    private GameObject CreateReply(PhraseController prefeab, Phrase phrase)
    {
        var reply = Instantiate(prefeab, m_PhrasesRoot);
        reply.Init(phrase);
        return reply.gameObject;
    }

    private void OnDestroy()
    {
        m_NextButton.onClick.RemoveAllListeners();

        m_AnimationShow?.Dispose();
        m_AnimationHide?.Dispose();
    }

    private void SetShowAnimationState(float state)
    {
        var position = m_XPosition.Evaluate(state);
        m_MovingTransform.position = Vector3.Lerp(
            m_MovingTransformHideTarget.position,
            m_MovingTransformShowTarget.position,
            position);

        m_CanvasGroup.alpha = m_Alpha.Evaluate(state);
    }

    private void SetHideAnimationState(float state)
    {
        var position = m_XPosition.Evaluate(state);
        m_MovingTransform.position = Vector3.Lerp(
            m_MovingTransformShowTarget.position,
            m_MovingTransformHideTarget.position,
            position);

        m_CanvasGroup.alpha = 1f - m_Alpha.Evaluate(state);
    }
}
