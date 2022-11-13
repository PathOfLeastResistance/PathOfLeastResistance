using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;

namespace Game
{
    public class SimpleCameraInteraction : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private float m_distance = 1f;
        [SerializeField] private float2 m_MinMaxDistance = new float2(1f, 10f);
        [SerializeField] private PlanarColliderPositionClamper m_Clamper;

        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
        [SerializeField] private Transform m_TargetTransform;

        [SerializeField] private InteractionObject m_InteractionObject;
        
        private Plane m_InteractionPlane = new Plane(Vector3.up, Vector3.zero);
        private CinemachineTransposer m_Transposer;
        private Vector3 m_TargetDragStartPoint;
        private Vector3 m_TotalDelta;
        private float m_WheelSensitive = 0.01f;
        
        public float Distance
        {
            get => m_distance;
            set
            {
                m_distance = math.clamp(value, m_MinMaxDistance.x, m_MinMaxDistance.y);
                m_Transposer.m_FollowOffset = new Vector3(0, 0, m_distance);
            }
        }

        public Vector3 TargetPosition
        {
            get => m_TargetTransform.position;
            set => m_TargetTransform.position = m_Clamper.ClampPosition(value);
        }

        private void Awake()
        {
            InputManager.Instance.RegisterCamera(m_camera);
            GameExtraInput.Instance.OnWheelEvent += OnWheel;
            m_Transposer = m_virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            m_InteractionObject.SubscribePointerDragEvent(OnPointerDragStart, OnPointerDrag, OnPointerDragEnd);

            TargetPosition = Vector3.zero;
            Distance = 1;
        }
        
        private void OnPointerDragStart(object sender, PointerDragInteractionEventArgs args)
        {
            var ray = m_camera.ScreenPointToRay(args.PointerPosition);
            if (m_InteractionPlane.Raycast(ray, out var enter))
            {
                m_TargetDragStartPoint = TargetPosition;
                m_TotalDelta = Vector3.zero;
            }
        }

        private void OnWheel(float delta)
        {
            Distance += delta * m_WheelSensitive;
        }
        
        private void OnPointerDrag(object sender, PointerDragInteractionEventArgs args)
        {
            m_TotalDelta += ProjectDeltaOnPlane(args.PointerPrevPosition, args.PointerPosition);
            TargetPosition = m_TargetDragStartPoint - m_TotalDelta;
        }

        private void OnPointerDragEnd(object sencer, PointerDragInteractionEventArgs args)
        {
        }
        
        private Vector3 ProjectDeltaOnPlane(Vector3 prevPosition2d, Vector3 newPosition2d)
        {
            var prevRay = m_camera.ScreenPointToRay(prevPosition2d);
            var newRay = m_camera.ScreenPointToRay(newPosition2d);
            if (m_InteractionPlane.Raycast(prevRay, out var prevEnter) && m_InteractionPlane.Raycast(newRay, out var newEnter))
            {
                var prevPoint = prevRay.GetPoint(prevEnter);
                var newPoint = newRay.GetPoint(newEnter);
                return newPoint - prevPoint;
            }
            return Vector3.zero;
        }
    }
}