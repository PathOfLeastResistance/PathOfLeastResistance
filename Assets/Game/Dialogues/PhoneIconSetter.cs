using UnityEngine;
using UnityEngine.UI;

public class PhoneIconSetter : MonoBehaviour
{
    [SerializeField] private Sprite m_Left = default;
    [SerializeField] private Sprite m_Right = default;
    [SerializeField] private Image m_Image = default;

    public void Init(Phrase phrase)
    {
        switch (phrase.Speaker)
        {
            case BubbleAlignment.Left:
                m_Image.sprite = m_Left;
                m_Image.transform.SetAsFirstSibling();
                break;

            case BubbleAlignment.Right:
            m_Image.sprite = m_Right;
                m_Image.transform.SetAsLastSibling();
                break;

            case BubbleAlignment.Center:
            default:
                break;
        }

        m_Image.color = Color.white;
    }
}
