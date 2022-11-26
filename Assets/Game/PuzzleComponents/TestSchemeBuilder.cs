using System;
using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;
using UnityTools;
using Zenject;

[Serializable]
public struct PinPair
{
    public ConnectorPinBehaviour Pin1;
    public ConnectorPinBehaviour Pin2;
}

public class TestSchemeBuilder : DisposableMonobehaviour
{
    [SerializeField] private List<PinPair> m_pinPairs = new List<PinPair>();
    [Inject] private CableBehaviour.Factory m_cablesFactory;

    public void InitComponent()
    {
        foreach (var pair in m_pinPairs)
        {
            var cable = m_cablesFactory.Create();
            cable.CableEnding1.Pin = pair.Pin1;
            cable.CableEnding2.Pin = pair.Pin2;
        }
    }

    public void DeinitComponent()
    {
    }

    private void OnDrawGizmos()
    {
        foreach (var pair in m_pinPairs)
        {
            if (pair.Pin1 != null && pair.Pin2 != null)
            {
                Gizmos.DrawLine(pair.Pin1.transform.position, pair.Pin2.transform.position);
            }
        }
    }
}