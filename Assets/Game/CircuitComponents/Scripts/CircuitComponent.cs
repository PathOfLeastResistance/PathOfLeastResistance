using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;
using Zenject;

public abstract class CircuitComponent : DisposableMonobehaviour
{
    [Inject] protected ConnectionManager m_connectionsManager;
    [Inject] protected UniquePostProvider m_uniquePostProvider;

    private bool m_isInited = false;

    protected abstract void InitComponent();

    protected abstract void DeinitComponent();

    public void Init()
    {
        if (!m_isInited)
        {
            InitComponent();
            m_isInited = true;
        }
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        DeinitComponent();
    }
}