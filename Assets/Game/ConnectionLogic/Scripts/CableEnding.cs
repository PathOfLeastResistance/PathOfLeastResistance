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

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
    {
        Pin = null; //Disconnect element on drag start
        if (m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_connectionManager.PinInteractionPlane, out var position))
            Position = position;

        //TODO: Notify that connection is dragged
    }

    private void OnDrag(object sender, PointerDragInteractionEventArgs args)
    {
        if (m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_connectionManager.PinInteractionPlane, out var position))
            Position = position;

        //TODO: Search for pin to snap
    }

    private void OnDragEnd(object sender, PointerDragInteractionEventArgs args)
    {
        //TODO: Notify that connection is dropped, create new connection
        if (m_cameraRaycaster.TryGetComponentUnderPosition(args.PointerPosition, out ConnectorPinBehaviour pin))
            Pin = pin;
        
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