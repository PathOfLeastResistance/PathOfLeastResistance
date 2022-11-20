using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;
using Zenject;

public class HandleController : MonoBehaviour
{
    [Inject] private CameraRaycaster m_cameraRaycaster;

    [SerializeField] private Transform m_handleFrom;
    [SerializeField] private Transform m_handleTo;
    [SerializeField] private Transform m_handle;

    [SerializeField] private InteractionObject m_interactionObject;

    private float m_position;
    private event Action<float> HandlePositionChangeEvent;
    
    // Slider drag data
    private Vector3 m_sliderDragStartPosition;
    private Vector3 m_dragDelta;
    private Plane m_dragPlane;

    public float Position
    {
        get => m_position;
        set
        {
            m_position = math.clamp(value, 0, 1);
            RefreshView();
            HandlePositionChangeEvent?.Invoke(m_position);
        }
    }

    public IDisposable SubscribeHandle(Action<float> positionHandler)
    {
        positionHandler?.Invoke(m_position);
        HandlePositionChangeEvent += positionHandler;
        return new DisposableAction(() => HandlePositionChangeEvent -= positionHandler);
    }

    private void Awake()
    {
        m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);
        Position = 0.5f;
    }

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
    {
        m_cameraRaycaster.RaycastScreenToPhysics(args.PointerPrevPosition, out var hit);
        m_dragPlane = new Plane(Vector3.up, hit);

        m_sliderDragStartPosition = m_handle.position;
        m_dragDelta = Vector3.zero;
    }

    private void OnDrag(object sender, PointerDragInteractionEventArgs args)
    {
        m_cameraRaycaster.RaycastDeltaOnPlane(args.PointerPrevPosition, args.PointerPosition, m_dragPlane, out var delta);
        m_dragDelta += delta;

        var sliderTargetPosition = m_sliderDragStartPosition + m_dragDelta;
        Position = GetT(sliderTargetPosition, m_handleFrom.position, m_handleTo.position);
    }

    private void OnDragEnd(object sender, PointerDragInteractionEventArgs args)
    {
    }

    private void RefreshView()
    {
        var handleFromPos = m_handleFrom.transform.position;
        var handleToPos = m_handleTo.transform.position;
        m_handle.transform.position = handleFromPos + (handleToPos - handleFromPos) * m_position;
    }


    #region Helpers //TODO move to math extensions

    private Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        var lineDirection = (lineEnd - lineStart).normalized;
        var dot = Vector3.Dot(point - lineStart, lineDirection);
        return lineStart + lineDirection * dot;
    }

    private float GetT(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        var lineDirection = lineEnd - lineStart;
        return Vector3.Dot(point - lineStart, lineDirection) / lineDirection.sqrMagnitude;
    }

    #endregion
}