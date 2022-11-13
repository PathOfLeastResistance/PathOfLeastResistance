using System;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    private HashSet<Connection> m_connections = new HashSet<Connection>();
    private Dictionary<uint, ConnectorPinBehaviour> m_connectors = new Dictionary<uint, ConnectorPinBehaviour>();

    private CirSim m_sim;

    public CirSim Sim => m_sim;

    private void Update()
    {
        m_sim.timeDelta = Time.deltaTime;
        m_sim.updateCircuit();
        // Debug.Log(m_sim.t);
    }

    private void Awake()
    {
        m_sim = new CirSim();
        m_sim.init();
    }

    public void RegisterPin(ConnectorPinBehaviour connectorPin)
    {
        m_connectors.Add(connectorPin.Id, connectorPin);
        connectorPin.SubscribePinDrag(OnPinDragStart, OnPinDrag, OnPinDragEnd);
    }

    private void OnPinDragStart(ConnectorPinBehaviour pin, Vector3 position)
    {
    }

    private void OnPinDrag(ConnectorPinBehaviour pin, Vector3 position)
    {
    }

    private void OnPinDragEnd(ConnectorPinBehaviour pin, Vector3 position)
    {
    }
}