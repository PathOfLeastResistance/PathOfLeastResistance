using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour, ICoroutineRunner
{
    private PlainAnimation mAnimation;

    private Dictionary<BackgroundController, BackgroundController> m_Backgrounds = new Dictionary<BackgroundController, BackgroundController>();

    [SerializeField] private Transform m_Root = default;
    [SerializeField] private float m_Duration = 0.5f;
    [SerializeField] private AnimationCurve m_Alpha = default;
    [SerializeField] private CanvasGroup m_CanvasGroup = default;

    private void Awake()
    {
        mAnimation = new PlainAnimation(this, m_Duration, SetAnimationState);
    }

    public void ActivateBackground(BackgroundController request)
    {
        m_CanvasGroup.blocksRaycasts = true;

        if (request != null)
        {
            var back = GetOrCreate(request);
            back.ShowAnimated();
        }
    }

    internal void HideAnimated()
    {
        mAnimation.StartAnimation();
        m_CanvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy() =>
        mAnimation?.Dispose();

    private void SetAnimationState(float state)
    {
        m_CanvasGroup.alpha = m_Alpha.Evaluate(state);
    }

    private BackgroundController GetOrCreate(BackgroundController request)
    {
        if (!m_Backgrounds.TryGetValue(request, out var back))
        {
            back = Instantiate(request, m_Root);
            m_Backgrounds.Add(request, back);
        }

        return back;
    }
}
