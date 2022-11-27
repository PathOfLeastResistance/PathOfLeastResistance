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
    [Inject] private CableBehaviour.Factory m_cablesFactory;
    
    [SerializeField] private Transform m_connectorsRoot;
    [SerializeField] private List<PinPair> m_pinPairs = new List<PinPair>();

    public void CreateConnections()
    {
        foreach (var pair in m_pinPairs)
        {
            var cable = m_cablesFactory.Create();
            cable.CableEnding1.Pin = pair.Pin1;
            cable.CableEnding2.Pin = pair.Pin2;
            cable.transform.SetParent(m_connectorsRoot);
        }
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