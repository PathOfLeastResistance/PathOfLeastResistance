using System;
using System.Collections.Generic;
using System.Linq;
using CircuitJSharp;
using Game;
using UnityEngine;
using UnityTools;
using Zenject;

public class ConnectionManager : MonoBehaviour
{
    [Inject] private CableBehaviour.Factory cablesFactory;
    [Inject] private CameraRaycaster m_cameraRaycaster;

    private CableBehaviour m_createdCable;

    private Dictionary<Connection, CircuitElm> m_connections = new Dictionary<Connection, CircuitElm>();
    private Dictionary<ulong, ConnectorPinBehaviour> m_connectors = new Dictionary<ulong, ConnectorPinBehaviour>();
    private CirSim m_sim;

    private Plane mPinInteractionPlane;

    public CirSim Sim => m_sim;

    private void Update()
    {
        m_sim.timeDelta = Time.deltaTime;
        m_sim.updateCircuit();
    }

    private void Awake()
    {
        m_sim = new CirSim();
        m_sim.init();
    }

    public void Connect(Connection connection)
    {
        if (!m_connections.ContainsKey(connection))
        {
            var pin1 = m_connectors[connection.Connector1Id];
            var pin2 = m_connectors[connection.Connector2Id];
            var elm = new ResistorElm(pin1.Post, pin2.Post, 1e-4);
            Sim.AddElement(elm);
            m_connections.Add(connection, elm);
        }
    }

    public void Disconnect(Connection connection)
    {
        if (m_connections.ContainsKey(connection))
        {
            var elm = m_connections[connection];
            m_sim.RemoveElement(elm);
            m_connections.Remove(connection);
        }
    }

    public void RegisterPin(ConnectorPinBehaviour connectorPin)
    {
        m_connectors.Add(connectorPin.Id, connectorPin);
        connectorPin.SubscribePinDrag(OnPinDragStart, OnPinDrag, OnPinDragEnd);
    }

    #region Pins interaction

    private void OnPinDragStart(ConnectorPinBehaviour pin, Vector3 position)
    {
        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        m_cameraRaycaster.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);

        m_createdCable = cablesFactory.Create();
        m_createdCable.CableEnding1.Pin = pin;
        m_createdCable.CableEnding2.Position = result;
    }

    private void OnPinDrag(ConnectorPinBehaviour pin, Vector3 position)
    {
        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        m_cameraRaycaster.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);
        m_createdCable.CableEnding2.Position = result;
    }

    private void OnPinDragEnd(ConnectorPinBehaviour pin, Vector3 position)
    {
        if (m_cameraRaycaster.TryGetComponentUnderPosition(position, out ConnectorPinBehaviour otherPin))
        {
            m_createdCable.CableEnding2.Pin = otherPin;
        }
        else
        {
            Destroy(m_createdCable.gameObject);
        }
    }

    #endregion

    /// <summary>
    /// Returns pins that are not the same as given id, and that are not already connected to the given id
    /// </summary>
    /// <param name="pinId"></param>
    /// <returns></returns>
    public IEnumerable<ConnectorPinBehaviour> GetAvailablePins(ulong pinId)
    {
        var pin = m_connectors[pinId];
        return m_connectors.Values.Where(c => c.Id != pinId && !m_connections.Keys.Any(connection => connection.Connector1Id == c.Id || connection.Connector2Id == c.Id));
    }

    public bool TryCreateConnection(ulong id1, ulong id2)
    {
        // var connection = new Connection(id1, id2);
        // if (m_connections.Contains(connection))
        // {
        //     Debug.LogWarning($"connection exists {id1} {id2}");
        //     return false;
        // }
        //
        // m_connections.Add(connection);
        // var cable = Instantiate(m_cableViewPrefab);
        // var connector1 = m_connectors[id1];
        // var connector2 = m_connectors[id2];
        // cable.SetPins(connector1, connector2);
        //
        return true;
    }
}