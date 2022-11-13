using System;
using System.Collections.Generic;
using CircuitJSharp;
using Game;
using UnityEngine;
using Zenject;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private LineRenderer m_LineRenderer;
    
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

    private ConnectorPinBehaviour mFirstPin;
    private Plane mPinInteractionPlane;
    
    private void OnPinDragStart(ConnectorPinBehaviour pin, Vector3 position)
    {
        m_LineRenderer.gameObject.SetActive(true);
        mFirstPin = pin;
        
        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        CameraRaycaster.Instance.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);
        m_LineRenderer.SetPositions(new Vector3[] { mFirstPin.ConnectionPoint, result} );
    }

    private void OnPinDrag(ConnectorPinBehaviour pin, Vector3 position)
    {
        mPinInteractionPlane = new Plane(Vector3.up, pin.ConnectionPoint);
        CameraRaycaster.Instance.RaycastPointOnPlane(position, mPinInteractionPlane, out var result);
        m_LineRenderer.SetPositions(new Vector3[] { mFirstPin.ConnectionPoint, result} );
    }

    private void OnPinDragEnd(ConnectorPinBehaviour pin, Vector3 position)
    {
        m_LineRenderer.gameObject.SetActive(false);
         
        //FindTheClosestPin

        if (CameraRaycaster.Instance.TryGetComponentUnderPosition(position, out ConnectorPinBehaviour otherPin))
        {
            
        }

    }
}