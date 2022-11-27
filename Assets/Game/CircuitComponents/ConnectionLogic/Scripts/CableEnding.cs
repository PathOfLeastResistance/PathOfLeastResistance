using System;
using Game;
using UnityEngine;
using UnityTools;
using Zenject;

public class CableEnding : MonoBehaviour
{
    [Inject] private CameraRaycaster m_cameraRaycaster;
    [Inject] private ConnectionManager m_connectionManager;
    [SerializeField] private InteractionObject m_interactionObject;

    private ConnectorPinBehaviour m_pin;
    private Vector3 m_position;
    private bool m_isDragged;
    private IDisposable m_pinSubscription;
    private CableBehaviour m_owner;

    private event Action<ConnectorPinBehaviour> m_pinChangeEvent;
    private event Action<Vector3> m_positionChangeEvent;
    
    private CableEnding m_otherCableEnding;

    public ConnectorPinBehaviour Pin
    {
        get => m_pin;
        set
        {
            if (m_pin != value)
            {
                m_pinSubscription?.Dispose();
                m_pin = value;

                if (m_pin != null)
                {
                    m_pinSubscription = m_pin.SubscribePinPosition(SetPositionInternal);
                }

                m_pinChangeEvent?.Invoke(value);
            }
        }
    }

    public Vector3 Position
    {
        get => m_position;
        set
        {
            if (m_pin == null)
                SetPositionInternal(value);
            else
                Debug.LogWarning("You are trying to set position, but this ending is controlled by pin");
        }
    }

    private void Awake()
    {
        m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);
        m_owner = GetComponentInParent<CableBehaviour>();
        
        m_otherCableEnding = m_owner.CableEnding1 == this ? m_owner.CableEnding2 : m_owner.CableEnding1;
    }

    public IDisposable SubscribePin(Action<ConnectorPinBehaviour> pinChangeHandle)
    {
        pinChangeHandle?.Invoke(m_pin);
        m_pinChangeEvent += pinChangeHandle;
        return new DisposableAction(() => m_pinChangeEvent -= pinChangeHandle);
    }

    public IDisposable SubscribePosition(Action<Vector3> positionChangeHandle)
    {
        positionChangeHandle?.Invoke(m_position);
        m_positionChangeEvent += positionChangeHandle;
        return new DisposableAction(() => m_positionChangeEvent -= positionChangeHandle);
    }

    //TODO: Think how to unify with ConnectionManager. The difference that we do not create new cable here

    private ConnectorPinBehaviour m_connectionCandidate;

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
    {
        Pin = null; //Disconnect element on drag start

        m_connectionCandidate = null;
        if (m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_connectionManager.PinInteractionPlane, out var position))
            Position = position;
    }

    private void OnDrag(object sender, PointerDragInteractionEventArgs args)
    {
        var connectionPosition = Position;
        if (m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_connectionManager.PinInteractionPlane, out var worldPosition))
            connectionPosition = worldPosition;

        var connectorFound = m_cameraRaycaster.TryGetComponentUnderPosition(args.PointerPosition, out ConnectorPinBehaviour otherPin);
        if (connectorFound && m_connectionManager.IsConnectionAvailable(m_otherCableEnding.Pin, otherPin))
        {
            m_connectionCandidate = otherPin;
            connectionPosition = otherPin.ConnectionPoint;
        }

        Position = connectionPosition;
    }

    private void OnDragEnd(object sender, PointerDragInteractionEventArgs args)
    {
        if (m_connectionCandidate != null)
            Pin = m_connectionCandidate;

        m_connectionCandidate = null;
        if (Pin == null)
            m_owner.Dispose();
    }

    private void SetPositionInternal(Vector3 position)
    {
        if (m_position != position)
        {
            m_position = position;
            m_positionChangeEvent?.Invoke(position);
        }
    }
}