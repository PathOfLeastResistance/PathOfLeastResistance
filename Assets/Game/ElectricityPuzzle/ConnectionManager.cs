using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager
{
    private HashSet<Connection> m_connections = new HashSet<Connection>();
    private Dictionary<uint, ConnectorPinBehaviour> m_connectors = new Dictionary<uint, ConnectorPinBehaviour>();

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