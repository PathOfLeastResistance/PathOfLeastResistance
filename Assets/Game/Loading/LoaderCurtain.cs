using UnityEngine;
using UnityEngine.UI;

public sealed class LoaderCurtain : MonoBehaviour, ILoaderCurtain
{
    private bool m_IsHiding = false;

    [SerializeField] private float m_FadeTime = 0.5f;
    [SerializeField] private CanvasGroup m_CoverCanvas = default;
    [SerializeField] private Text m_ProgressBar = default;

    public void Show()
    {
        m_CoverCanvas.alpha = 1f;
        m_IsHiding = false;
        gameObject.SetActive(true);
    }

    public void Hide() =>
        m_IsHiding = true;

    //TODO Add a progress bar
    public void SetProgress(float progress) =>
        m_ProgressBar.text = $"{progress:0.0} %";

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
