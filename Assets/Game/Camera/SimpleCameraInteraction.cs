using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;
using Zenject;

namespace Game
{
    public class SimpleCameraInteraction : MonoBehaviour
    {
        [Inject] private CameraRaycaster m_cameraRaycaster;
        
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
            //We can scroll if no object is under interaction. not a good way to do like this :(
            var inputManager = InputManager.Instance;
            var canScroll = inputManager.ActiveGesturesCount == 1 && inputManager.ActiveGestures.All(c => c.InteractionObject == m_InteractionObject);
            if (canScroll)
            {
                Distance += delta * m_WheelSensitive;
            }
        }

        private void OnPointerDrag(object sender, PointerDragInteractionEventArgs args)
        {
            if (m_cameraRaycaster.ProjectDeltaOnPlane(args.PointerPrevPosition, args.PointerPosition, m_InteractionPlane, out var detla))
            {
                m_TotalDelta += detla;
                TargetPosition = m_TargetDragStartPoint - m_TotalDelta;
            }
        }

        private void OnPointerDragEnd(object sencer, PointerDragInteractionEventArgs args)
        {
        }
    }
}