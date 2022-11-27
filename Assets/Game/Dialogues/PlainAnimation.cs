using System;
using System.Collections;
using UnityEngine;

public class PlainAnimation : IDisposable
{
    private readonly ICoroutineRunner m_CoroutineRunner = default;
    private readonly float mDuration = default;

    private float mState;

    public PlainAnimation(ICoroutineRunner coroutineRunner, float duration)
    {
        m_CoroutineRunner = coroutineRunner;
        mDuration = duration;
    }

    public PlainAnimation(ICoroutineRunner coroutineRunner, float duration, Action<float> action)
    {
        m_CoroutineRunner = coroutineRunner;
        mDuration = duration;
        OnProgress += action;
    }

    private event Action<float> OnProgress;

    public void StartAnimation()
    {
        mState = 0f;
        m_CoroutineRunner.StartCoroutine(OnTick());
    }

    public void Subscribe(Action<float> action) =>
        OnProgress += action;

    public void Unsubscribe(Action<float> action) =>
        OnProgress -= action;

    public void Dispose() =>
        OnProgress = null;

    private IEnumerator OnTick()
    {
        while (mState < 1f)
        {
            mState = Mathf.Clamp01(mState + Time.deltaTime / mDuration);
            OnProgress?.Invoke(mState);

            yield return null;
        }
    }
}
