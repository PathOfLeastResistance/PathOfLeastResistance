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
    [SerializeField] private CableBehaviour m_cableViewPrefab;
    [SerializeField] private CableView m_cablePreview;

    private HashSet<Connection> m_connections = new HashSet<Connection>();
    private Dictionary<ulong, ConnectorPinBehaviour> m_connectors = new Dictionary<ulong, ConnectorPinBehaviour>();
    private CirSim m_sim;
    
    private ConnectorPinBehaviour mFirstPin;
    private Plane mPinInteractionPlane;

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

    #region Pins interaction

    private void OnPinDragStart(ConnectorPinBehaviour pin, Vector3 position)
    {
        mFirstPin = pin;

        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        CameraRaycaster.Instance.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);
        
        m_cablePreview.gameObject.SetActive(true);
        m_cablePreview.From = mFirstPin.ConnectionPoint;
        m_cablePreview.To = result;
    }

    private void OnPinDrag(ConnectorPinBehaviour pin, Vector3 position)
    {
        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        CameraRaycaster.Instance.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);
        m_cablePreview.From = mFirstPin.ConnectionPoint;
        m_cablePreview.To = result;
    }

    private void OnPinDragEnd(ConnectorPinBehaviour pin, Vector3 position)
    {
        m_cablePreview.gameObject.SetActive(false);

        //FindTheClosestPin
        if (CameraRaycaster.Instance.TryGetComponentUnderPosition(position, out ConnectorPinBehaviour otherPin))
        {
            TryCreateConnection(mFirstPin.Id, otherPin.Id);
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
        return m_connectors.Values.Where(c=> c.Id != pinId && !m_connections.Any(connection => connection.Connector1Id == c.Id || connection.Connector2Id == c.Id));
    }

    public bool TryCreateConnection(ulong id1, ulong id2)
    {
        var connection = new Connection(id1, id2);
        if (m_connections.Contains(connection))
        {
            Debug.LogWarning($"connection exists {id1} {id2}");
            return false;
        }

        m_connections.Add(connection);
        var cable = Instantiate(m_cableViewPrefab);
        var connector1 = m_connectors[id1];
        var connector2 = m_connectors[id2];
        cable.SetPins(connector1, connector2);
        
        return true;
    }
}