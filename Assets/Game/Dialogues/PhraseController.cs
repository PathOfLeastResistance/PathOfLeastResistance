using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhraseController : MonoBehaviour, ICoroutineRunner
{
    [Serializable]
    private class BubbleTailObject
    {
        public BubbleTail Tail;
        public GameObject GameObject;
    }

    [Serializable]
    private class BubbleFormSprite
    {
        public BubbleForm Form;
        public Sprite Sprite;
    }

    private PlainAnimation mAnimation;

    [SerializeField] private Image m_BubbleBack = default;

    [SerializeField] private TextMeshProUGUI m_PhraseText = default;
    [SerializeField] private Image m_Sprite = default;
    [SerializeField] private VerticalLayoutGroup m_TextLayout = default;
    [SerializeField] private BubbleTailObject[] m_Tails = default;
    [SerializeField] private BubbleFormSprite[] m_Forms = default;
    [SerializeField] private HorizontalLayoutGroup m_Layout = default;

    [SerializeField] private float m_Duration = 0.3f;
    [SerializeField] private CanvasGroup m_CanvasGroup = default;
    [SerializeField] private LayoutElement m_LayoutElement = default;
    [SerializeField] private RectTransform m_Content = default;

    [SerializeField] private AnimationCurve m_Alpha = default;
    [SerializeField] private AnimationCurve m_YPosition = default;

    public void Init(Phrase phrase)
    {
        if (phrase.SpriteInsteadOfText != null)
        {
            m_Sprite.sprite = phrase.SpriteInsteadOfText;
            var spriteRect = phrase.SpriteInsteadOfText.rect;
            var size = m_Sprite.rectTransform.sizeDelta;
            size.y = spriteRect.height * size.x / spriteRect.width;
            m_Sprite.rectTransform.sizeDelta = size;

            m_TextLayout.childControlHeight = false;
            m_TextLayout.childForceExpandHeight = false;

            m_Sprite.enabled = true;
            m_PhraseText.enabled = false;
        }
        else
        {
            m_PhraseText.text = phrase.Text;

            m_TextLayout.childControlHeight = true;
            m_TextLayout.childForceExpandHeight = true;

            m_PhraseText.enabled = true;
            m_Sprite.enabled = false;
        }

        m_BubbleBack.sprite = m_Forms.FirstOrDefault(f => f.Form == phrase.BubbleForm).Sprite;

        foreach (var tail in m_Tails)
            tail.GameObject.SetActive(tail.Tail == phrase.BubbleTail);

        switch (phrase.Speaker)
        {
            case BubbleAlignment.Left:
                m_Layout.childAlignment =  TextAnchor.MiddleLeft;
                break;

            case BubbleAlignment.Right:
                m_Layout.childAlignment =  TextAnchor.MiddleRight;
                break;

            case BubbleAlignment.Center:
                m_Layout.childAlignment =  TextAnchor.MiddleCenter;
                break;

            default:
                throw new NotImplementedException();
        }

        mAnimation = new PlainAnimation(this, m_Duration, SetAnimationState);
        mAnimation.StartAnimation();
    }

    private void OnDestroy() =>
        mAnimation?.Dispose();

    private void SetAnimationState(float state)
    {
        var targetHeight = LayoutUtility.GetPreferredSize(m_Content, 1);
        var targetWidth = LayoutUtility.GetPreferredSize(m_Content, 0);

        m_LayoutElement.preferredWidth = targetWidth;
        m_LayoutElement.preferredHeight = Mathf.Lerp(0f, targetHeight, m_YPosition.Evaluate(state));

        m_CanvasGroup.alpha = m_Alpha.Evaluate(state);
    }
}
