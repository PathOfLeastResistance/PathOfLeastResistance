using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class CircuitComponent : MonoBehaviour
{
    [Inject] protected ConnectionManager m_connectionsManager;
    [Inject] protected UniquePostProvider m_uniquePostProvider;

    private bool m_isInited = false;

    protected abstract void InitComponent();

    protected abstract void DeinitComponent();

    private void Init()
    {
        if (!m_isInited)
        {
            InitComponent();
            m_isInited = true;
        }
    }

    protected virtual void PreInit()
    {
        
    }

    private void OnDestroy()
    {
        DeinitComponent();
    }

    private void Awake()
    {
        PreInit();
    }

    private void Start()
    {
        Init();
    }
}