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

    [SerializeField] private Transform m_CablesPlaneTransform;

    private CableBehaviour m_createdCable;

    private Dictionary<Connection, CircuitElm> m_connections = new Dictionary<Connection, CircuitElm>();
    private Dictionary<ulong, ConnectorPinBehaviour> m_connectors = new Dictionary<ulong, ConnectorPinBehaviour>();
    private CirSim m_sim;

    public Plane PinInteractionPlane { get; private set; }

    public CirSim Sim => m_sim;

    private void Update()
    {
        m_sim.timeDelta = Time.deltaTime;
        m_sim.updateCircuit();
        PinInteractionPlane = new Plane(Vector3.up, m_CablesPlaneTransform.position);
    }

    private void Awake()
    {
        m_sim = new CirSim();
        m_sim.init();
    }

    public bool HasWires(ConnectorPinBehaviour connectorPin)
    {
        return m_connections.Any(c=> c.Key.Connector1Id == connectorPin.Id || c.Key.Connector2Id == connectorPin.Id);
    }
    
    public void Connect(Connection connection)
    {
        if (!m_connections.ContainsKey(connection))
        {
            var pin1 = m_connectors[connection.Connector1Id];
            var pin2 = m_connectors[connection.Connector2Id];
            var elm = new ResistorElm(pin1.Post, pin2.Post, 1e-3);
            Sim.AddElement(elm);
            m_connections.Add(connection, elm);
        }
        else
        {
            Debug.LogWarning($"Connection {connection} already exists");
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
        connectorPin.DisposeEvent += () => RemoveAllConnectionsAssociatedWithPin(connectorPin);
    }
    
    private void RemoveAllConnectionsAssociatedWithPin(ConnectorPinBehaviour connectorPin)
    {
        var connectionsToRemove = m_connections.Where(c => c.Key.Connector1Id == connectorPin.Id || c.Key.Connector2Id == connectorPin.Id).ToList();
        foreach (var connection in connectionsToRemove)
        {
            m_sim.RemoveElement(connection.Value);
            m_connections.Remove(connection.Key);
        }
    }

    #region Pins interaction

    private void OnPinDragStart(ConnectorPinBehaviour pin, Vector3 position)
    {
        m_cameraRaycaster.RaycastPointOnPlane(position, PinInteractionPlane, out var result);

        m_createdCable = cablesFactory.Create();
        m_createdCable.CableEnding1.Pin = pin;
        m_createdCable.CableEnding2.Position = result;
    }

    private void OnPinDrag(ConnectorPinBehaviour pin, Vector3 position)
    {
        PinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        m_cameraRaycaster.RaycastPointOnPlane(position, PinInteractionPlane, out var result);
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
            m_createdCable.Dispose();
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
        return m_connectors.Values.Where(c => c.Id != pinId && !m_connections.Keys.Any(connection => connection.Connector1Id == c.Id || connection.Connector2Id == c.Id));
    }
}