﻿using UnityEngine;

public class BackgroundController : MonoBehaviour, ICoroutineRunner
{
    private PlainAnimation mAnimation;

    [SerializeField] private float m_Duration = 0.7f;
    [SerializeField] private AnimationCurve m_Alpha = default;
    [SerializeField] private CanvasGroup m_CanvasGroup = default;

    private void Awake()
    {
        m_CanvasGroup.blocksRaycasts = false;
        m_CanvasGroup.interactable = false;
    }

    public void SetFirst()
    {
        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void ShowAnimated()
    {
        mAnimation ??= new PlainAnimation(this, m_Duration, SetAnimationState);
        mAnimation.StartAnimation();
    }

    private void OnDestroy() =>
        mAnimation?.Dispose();

    private void SetAnimationState(float state)
    {
        m_CanvasGroup.alpha = m_Alpha.Evaluate(state);
    }
}
