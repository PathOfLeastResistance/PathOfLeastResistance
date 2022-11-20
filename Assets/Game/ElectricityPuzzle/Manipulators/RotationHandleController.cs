using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;

public class RotationHandle : MonoBehaviour
{
    [SerializeField] private Transform m_rotatorTransform;

    [SerializeField] private InteractionObject m_interactionObject;

    [SerializeField] private float m_minValueAngle = 0f;
    [SerializeField] private float m_maxValueAngle = 180;

    [SerializeField] private float m_sensitivity = 1f;

    private float m_value;
    private float m_dragStartValue;
    private float m_delta;
    private event Action<float> ValueChangeEvent;

    public float Value
    {
        get => m_value;
        set
        {
            m_value = math.clamp(value, 0, 1);
            ValueChangeEvent?.Invoke(m_value);
            RefreshView();
        }
    }

    public IDisposable SubscribeValue(Action<float> valueHandler)
    {
        ValueChangeEvent += valueHandler;
        valueHandler?.Invoke(m_value);
        return new DisposableAction(() => ValueChangeEvent -= valueHandler);
    }

    private void Awake()
    {
        RefreshView();
        m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, null);
    }

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
    {
        m_dragStartValue = Value;
    }

    private void OnDrag(object sender, PointerDragInteractionEventArgs args)
    {
        var screenDelta = (args.PointerPosition - args.PointerPrevPosition) / Screen.width;
        var delta = screenDelta.x + screenDelta.y;
        m_delta += delta;
        Value = m_dragStartValue + m_delta * m_sensitivity;
    }

    private void RefreshView()
    {
        var angle = math.lerp(m_minValueAngle, m_maxValueAngle, m_value);
        m_rotatorTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}