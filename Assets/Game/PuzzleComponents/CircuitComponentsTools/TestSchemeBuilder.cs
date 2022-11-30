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

/// <summary>
/// This class takes pins that you save in the inspector and connects them together.
/// </summary>
public class TestSchemeBuilder : DisposableMonobehaviour
{
    [Inject] private CableBehaviour.Factory m_cablesFactory;

    [SerializeField] private Transform m_connectorsRoot;
    [SerializeField] private List<PinPair> m_pinPairs = new List<PinPair>();
    [SerializeField] private bool m_isHidden;

    private List<CableBehaviour> m_cables = new List<CableBehaviour>();

    public void CreateConnections()
    {
        foreach (var pair in m_pinPairs)
        {
            if (!pair.Pin1.HasId)
                Debug.LogError($"Pin {pair.Pin1.name} has no id", pair.Pin1);
            if (!pair.Pin2.HasId)
                Debug.LogError($"Pin {pair.Pin2.name} has no id", pair.Pin2);

            var cable = m_cablesFactory.Create();
            cable.CableEnding1.Pin = pair.Pin1;
            cable.CableEnding2.Pin = pair.Pin2;
            cable.transform.SetParent(m_isHidden ? m_connectorsRoot : null);
        }
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        foreach (var cable in m_cables)
            cable.Dispose();
        m_cables.Clear();
    }

    private void OnDrawGizmos()
    {
        foreach (var pair in m_pinPairs)
        {
            if (pair.Pin1 != null && pair.Pin2 != null)
            {
                Gizmos.DrawLine(pair.Pin1.ConnectionPoint, pair.Pin2.ConnectionPoint);
            }
        }
    }
}