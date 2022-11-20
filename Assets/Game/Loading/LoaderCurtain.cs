using UnityEngine;

public sealed class LoaderCurtain : MonoBehaviour
{
    private bool m_IsHiding = false;

    [SerializeField] private float m_FadeTime = 0.5f;
    [SerializeField] private CanvasGroup m_CoverCanvas = default;

    private void Awake() =>
        DontDestroyOnLoad(this);

    public void Show()
    {
        m_CoverCanvas.alpha = 1f;
        m_IsHiding = false;
        gameObject.SetActive(true);
    }

    public void Hide() =>
        m_IsHiding = true;

    private void Update()
    {
        if (m_IsHiding)
        {
            var alpha = m_CoverCanvas.alpha;
            alpha = Mathf.Clamp01(alpha - Time.deltaTime / m_FadeTime);
            m_CoverCanvas.alpha = alpha;

            if (alpha <= 0f)
                gameObject.SetActive(false);
        }
    }
}
