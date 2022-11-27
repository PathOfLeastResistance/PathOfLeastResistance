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
        [SerializeField] private PlanarColliderPositionClamper m_Clamper;

        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;
        [SerializeField] private Transform m_TargetTransform;

        [SerializeField] private InteractionObject m_InteractionObject;
        [SerializeField] private CameraSettings m_cameraSettings;
        [SerializeField] private float m_RotationSensetivity = 1f;
        [SerializeField] private float m_WheelSensitive = 0.001f;
        [SerializeField] private float m_movementDuration = 10;

        private Plane m_InteractionPlane = new Plane(Vector3.up, Vector3.zero);
        private CinemachineTransposer m_Transposer;
        private CinemachineComposer m_Composer;
        private Vector3 m_TargetDragStartPoint;
        private Vector3 m_TotalDelta;

        private Vector3 m_currentPosition;
        private Quaternion m_currentRotation;
        private float m_currentZoom;

        private Vector3 m_targetPosition;
        private Quaternion m_targetRotation;
        private float m_targetZoom = 1f;

        public float TargetZoom
        {
            get => m_targetZoom;
            set => m_targetZoom = math.clamp(value, 0, 1);
        }

        public Vector3 TargetPosition
        {
            get => m_targetPosition;
            set => m_targetPosition = m_Clamper.ClampPosition(value);
        }

        public Quaternion TargetRotation
        {
            get => m_targetRotation;
            set => m_targetRotation = value;
        }

        private void Awake()
        {
            InputManager.Instance.RegisterCamera(m_camera);
            SimpleMouseInput.Instance.OnWheelEvent += OnWheel;
            SimpleMouseInput.Instance.SubscribeLeftButtonDrag(OnLeftDragStart, OnLeftDragPerformed, OnLeftDragEnd);
            SimpleMouseInput.Instance.SubscribeRightButtonDrag(OnRightDragStart, OnRightDragPerformed, OnRightDragEnd);

            m_Transposer = m_virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            m_Composer = m_virtualCamera.GetCinemachineComponent<CinemachineComposer>();

            ResetCameraPos();
        }

        public void ResetCameraPos()
        {
            m_currentPosition = TargetPosition = Vector3.zero;
            m_currentRotation = TargetRotation = Quaternion.identity;
            m_currentZoom = TargetZoom = 1f;

            UpdateCameraPosition();
        }

        #region Input handling

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
                TargetRotation *= Quaternion.AngleAxis(-args.MouseDelta.x * m_RotationSensetivity, Vector3.up);
        }

        private void OnRightDragEnd(MouseDragEventArgs args)
        {
        }

        private void OnWheel(float delta)
        {
            if (!CheckInputInterruption())
                TargetZoom += delta * m_WheelSensitive;
        }

        private bool CheckInputInterruption()
        {
            return InputManager.Instance.ActiveGestures.Any(c => c.InteractionObject != m_InteractionObject);
        }

        #endregion

        private void Update()
        {
            var t = Time.deltaTime / m_movementDuration;
            m_currentPosition = math.lerp(m_currentPosition, m_targetPosition, t);
            m_currentRotation = Quaternion.Lerp(m_currentRotation, m_targetRotation, t);
            m_currentZoom = math.lerp(m_currentZoom, m_targetZoom, t);

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            //Apply position and rotation settings
            m_TargetTransform.SetPositionAndRotation(m_currentPosition, m_currentRotation);

            //Apply zoom
            var cameraUpOffset = m_cameraSettings.m_heightCurve.Evaluate(m_targetZoom);
            var cameraForwardOffset = m_cameraSettings.m_cameraForwardOffsetCurve.Evaluate(m_targetZoom);

            var targetUpOffset = m_cameraSettings.m_cameraUpOffsetCurve.Evaluate(m_targetZoom);
            var targetForwardOffset = m_cameraSettings.m_targetForwardOffsetCurve.Evaluate(m_targetZoom);

            m_Transposer.m_FollowOffset = new Vector3(0, cameraUpOffset, cameraForwardOffset);
            m_Composer.m_TrackedObjectOffset = new Vector3(0, targetUpOffset, targetForwardOffset);
        }
    }
}