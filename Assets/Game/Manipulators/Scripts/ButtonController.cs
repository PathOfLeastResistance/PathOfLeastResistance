using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private InteractionObject m_interactionObject;
    [SerializeField] private Transform m_buttonTransform;
    [SerializeField] private Transform m_pressedTransform;
    [SerializeField] private Transform m_releasedTransform;

    public event Action PressEvent;
    
    private void Awake()
    {
        m_interactionObject.SubscribePointerGrabEvent(OnGrabStart, OnGrabEnd);
        m_interactionObject.SubscribePointerPressEvent(OnPress);
        m_interactionObject.SubscribePointerDragEvent(null, OnDrag, null);
        m_buttonTransform.localPosition = Vector3.zero;
    }

    private void OnDrag(object sender, PointerDragInteractionEventArgs args)
    {
        m_buttonTransform.localPosition = Vector3.zero;
        SetReleasedPos();
    }

    private void OnPress(object sender, PointerInteractionEventArgs args)
    {
        PressEvent?.Invoke();
    }

    private void OnGrabStart(object sender, PointerInteractionEventArgs args)
    {
        SetPressedPos();
    }

    private void OnGrabEnd(object sender, PointerInteractionEventArgs args)
    {
        SetReleasedPos();
    }

    private void SetPressedPos()
    {
        var position = ConvertPosition(Vector3.zero, m_pressedTransform, m_releasedTransform);
        m_buttonTransform.localPosition = position;
    }
    
    private void SetReleasedPos()
    {
        m_buttonTransform.localPosition = Vector3.zero;
    }
    
    private Vector3 ConvertPosition(Vector3 position, Transform from, Transform to)
    {
        return to.InverseTransformPoint(from.TransformPoint(position));
    }
}