using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;
using Zenject;

public class CableBehaviour : DisposableMonobehaviour
{
    public class Factory : PlaceholderFactory<CableBehaviour>
    {
    }

    [Inject] private ConnectionManager m_connectionsManager;

    [SerializeField] private CableView m_cableView;
    [SerializeField] private CableEnding m_cableEnding1;
    [SerializeField] private CableEnding m_cableEnding2;

    private ConnectorPinBehaviour m_pin1;
    private ConnectorPinBehaviour m_pin2;

    public CableEnding CableEnding1 => m_cableEnding1;

    public CableEnding CableEnding2 => m_cableEnding2;

    protected override void OnAwake()
    {
        base.OnAwake();

        m_cableEnding1.SubscribePosition((pos) => OnPositionsChanged()).DisposeWhenNotifierDisposed(this);
        m_cableEnding2.SubscribePosition((pos) => OnPositionsChanged()).DisposeWhenNotifierDisposed(this);

        m_cableEnding1.SubscribePin(OnPin1Changed);
        m_cableEnding2.SubscribePin(OnPin2Changed);
    }
    
    private void OnPin1Changed(ConnectorPinBehaviour pin)
    {
        SetPins(pin, m_pin2);
    }

    private void OnPin2Changed(ConnectorPinBehaviour pin)
    {
        SetPins(m_pin1, pin);
    }

    private void SetPins(ConnectorPinBehaviour pin1, ConnectorPinBehaviour pin2)
    {
        if (pin1 != null && pin2 != null)
        {
            //We had the connection before. remove it (not sure we need this part)
            if (m_pin1 != null && m_pin2 != null)
            {
                m_connectionsManager.Disconnect(new Connection(m_pin1.Id, m_pin2.Id));
                Debug.Log("Disconnected");
            }

            // the connection is not the same as before. add it
            if (m_pin1 != pin1 || m_pin2 != pin2)
            {
                m_connectionsManager.Connect(new Connection(pin1.Id, pin2.Id));
                Debug.Log("Connected");
            }
        }
        //We lost the connection. remove it
        else if (m_pin1 != null && m_pin2 != null)
        {
            m_connectionsManager.Disconnect(new Connection(m_pin1.Id, m_pin2.Id));
            Debug.Log("Disconnected");
        }

        m_pin1 = pin1;
        m_pin2 = pin2;
    }

    private void OnPositionsChanged()
    {
        m_cableView.From = m_cableEnding1.Position;
        m_cableView.To = m_cableEnding2.Position;
    }
}