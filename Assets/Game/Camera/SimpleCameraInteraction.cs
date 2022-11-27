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
    [Serializable]
    public struct CameraSettings
    {
        public AnimationCurve m_heightCurve;
        public AnimationCurve m_targetForwardOffsetCurve;
        public AnimationCurve m_cameraForwardOffsetCurve;
        public AnimationCurve m_cameraUpOffsetCurve;
    }

    public class SimpleCameraInteraction : MonoBehaviour
    {
        [Inject] private CameraRaycaster m_cameraRaycaster;

        [SerializeField] private Camera m_camera;
        [SerializeField] private float m_zoom = 1f;
        [SerializeField] private PlanarColliderPositionClamper m_Clamper;

        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
        [SerializeField] private Transform m_TargetTransform;

        [SerializeField] private InteractionObject m_InteractionObject;
        [SerializeField] private CameraSettings m_cameraSettings;
        [SerializeField] private float m_RotationSensetivity = 1f;

        private Plane m_InteractionPlane = new Plane(Vector3.up, Vector3.zero);
        private CinemachineTransposer m_Transposer;
        private CinemachineComposer m_Composer;
        private Vector3 m_TargetDragStartPoint;
        private Vector3 m_TotalDelta;
        private float m_WheelSensitive = 0.01f;

        public float Zoom
        {
            get => m_zoom;
            set
            {
                m_zoom = math.clamp(value, 0, 1);
                
                var heightOffset = m_cameraSettings.m_heightCurve.Evaluate(m_zoom);
                var targetForwardOffset = m_cameraSettings.m_targetForwardOffsetCurve.Evaluate(m_zoom);
                var cameraForwardOffset = m_cameraSettings.m_cameraForwardOffsetCurve.Evaluate(m_zoom);
                var cameraUpOffset = m_cameraSettings.m_cameraUpOffsetCurve.Evaluate(m_zoom);
                
                m_Transposer.m_FollowOffset = new Vector3(0, heightOffset, cameraForwardOffset);
                m_Composer.m_TrackedObjectOffset = new Vector3(0, cameraUpOffset, targetForwardOffset);
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
            SimpleMouseInput.Instance.OnWheelEvent += OnWheel;
            SimpleMouseInput.Instance.SubscribeLeftButtonDrag(OnLeftDragStart, OnLeftDragPerformed, OnLeftDragEnd);
            SimpleMouseInput.Instance.SubscribeRightButtonDrag(OnRightDragStart, OnRightDragPerformed, OnRightDragEnd);

            m_Transposer = m_virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            m_Composer = m_virtualCamera.GetCinemachineComponent<CinemachineComposer>();

            TargetPosition = Vector3.zero;
            Zoom = 1;
        }

        private void OnLeftDragStart(MouseDragEventArgs args)
        {
            var ray = m_camera.ScreenPointToRay(args.MousePosition);
            if (m_InteractionPlane.Raycast(ray, out var enter))
            {
                m_TargetDragStartPoint = TargetPosition;
                m_TotalDelta = Vector3.zero;
            }
        }

        private void OnLeftDragPerformed(MouseDragEventArgs args)
        {
            if (!CheckInputInterruption())
            {
                if (m_cameraRaycaster.RaycastDeltaOnPlane(args.MousePosition - args.MouseDelta, args.MousePosition, m_InteractionPlane, out var delta))
                {
                    m_TotalDelta += delta;
                    TargetPosition = m_TargetDragStartPoint - m_TotalDelta;
                }
            }
        }

        private void OnLeftDragEnd(MouseDragEventArgs args)
        {
        }

        private void OnRightDragStart(MouseDragEventArgs args)
        {
        }

        private void OnRightDragPerformed(MouseDragEventArgs args)
        {
            if (!CheckInputInterruption())
                m_TargetTransform.rotation *= Quaternion.AngleAxis(-args.MouseDelta.x * m_RotationSensetivity, Vector3.up);
        }

        private void OnRightDragEnd(MouseDragEventArgs args)
        {
        }

        private void OnWheel(float delta)
        {
            if (!CheckInputInterruption())
                Zoom += delta * m_WheelSensitive;
        }

        private bool CheckInputInterruption()
        {
            return InputManager.Instance.ActiveGestures.Any(c => c.InteractionObject != m_InteractionObject);
        }
    }
}