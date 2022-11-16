using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;
using Zenject;

public class CableBehaviour : MonoBehaviour
{
    public class Factory : PlaceholderFactory<CableBehaviour>
    {
    }
    
    [Inject] private ConnectionManager m_connectionManager;
    
    [SerializeField] private InteractionObject m_interactionObject;
    [SerializeField] private InteractionObject m_FromInteractionObject;
    [SerializeField] private InteractionObject m_ToInteractionObject;
    [SerializeField] private CableView m_cableView;

    private Vector3 m_fromPosition;
    private Vector3 m_ToPosition;

    private IDisposable m_Pin1Subscription;
    private IDisposable m_Pin2Subscription;

    public Vector3 FromPosition
    {
        get => m_fromPosition;
        set
        {
            m_fromPosition = value;
            OnPositionsChanged();
        }
    }

    public Vector3 ToPosition
    {
        get => m_ToPosition;
        set
        {
            m_ToPosition = value;
            OnPositionsChanged();
        }
    }

    public void SetPins(ConnectorPinBehaviour pin1, ConnectorPinBehaviour pin2)
    {
        m_Pin1Subscription?.Dispose();
        m_Pin2Subscription?.Dispose();

        m_Pin1Subscription = pin1.SubscribePinPosition((pos) => FromPosition = pos);
        m_Pin2Subscription = pin2.SubscribePinPosition((pos) => ToPosition = pos);
    }

    private void Awake()
    {
        m_FromInteractionObject.SubscribePointerDragEvent(OnFromDragStart, OnFromDrag, OnFromDragEnd);
        m_ToInteractionObject.SubscribePointerDragEvent(OnToDragStart, OnToDrag, OnToDragEnd);
    }

    private void OnPositionsChanged()
    {
        m_cableView.From = m_fromPosition;
        m_cableView.To = m_ToPosition;
    }

    private void OnFromDragStart(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void OnFromDrag(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void OnFromDragEnd(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void OnToDragStart(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void OnToDrag(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void OnToDragEnd(object sender, PointerDragInteractionEventArgs args)
    {
    }
}