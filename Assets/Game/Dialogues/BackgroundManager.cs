using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour, ICoroutineRunner
{
    private PlainAnimation m_ShowAnimation;
    private PlainAnimation m_HideAnimation;

    private Dictionary<BackgroundController, BackgroundController> m_Backgrounds = new Dictionary<BackgroundController, BackgroundController>();

    [SerializeField] private BackgroundController m_PreviousBack;
    [SerializeField] private Transform m_Root = default;
    [SerializeField] private float m_Duration = 0.5f;
    [SerializeField] private AnimationCurve m_Alpha = default;
    [SerializeField] private CanvasGroup m_CanvasGroup = default;

    private void Awake()
    {
        m_ShowAnimation = new PlainAnimation(this, m_Duration, SetShowAnimationState);
        m_HideAnimation = new PlainAnimation(this, m_Duration, SetHideAnimationState);
    }

    public void ActivateBackground(BackgroundController request)
    {
        var visible = request != null;
        m_CanvasGroup.blocksRaycasts = visible;
        m_CanvasGroup.interactable = visible;
        
        if (visible)
        {
            var back = GetOrCreate(request);
            back.SetFirst();
            if (m_PreviousBack != null)
                back.ShowAnimated();
        }

        if (m_PreviousBack == null && visible)
        {
            var back = GetOrCreate(request);
            foreach (var backgrond in m_Backgrounds)
                backgrond.Value.gameObject.SetActive(backgrond.Key == request);
            
            m_ShowAnimation.StartAnimation();
        }
        else if (m_PreviousBack != null && !visible)
        {
            foreach (var backgrond in m_Backgrounds)
                backgrond.Value.gameObject.SetActive(backgrond.Key == m_PreviousBack);
            
            m_PreviousBack = null;
            m_HideAnimation.StartAnimation();
        }

        m_PreviousBack = request;
    }

    private void OnDestroy() =>
        m_ShowAnimation?.Dispose();

    private void SetShowAnimationState(float state) =>
        m_CanvasGroup.alpha = 1f - m_Alpha.Evaluate(state);

    private void SetHideAnimationState(float state) =>
        m_CanvasGroup.alpha = m_Alpha.Evaluate(state);

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
