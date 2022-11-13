using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitComponent : MonoBehaviour
{
    [SerializeField] private List<ConnectorPinBehaviour> m_pins = new List<ConnectorPinBehaviour>();

    private ConnectionManager m_connectionsManager;
    
    public virtual void InitPins()
    {
    
    }

    private void RegisterPins()
    {
        foreach (var pin in m_pins)
        {
            m_connectionsManager.RegisterPin(pin);
        }
    }

    public void Init()
    {
        InitPins();
    }
}
