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
        [Inject] private CameraRaycaster m_CameraRaycaster;

        [SerializeField] private CinemachineBrain m_brain;
        [SerializeField] private Camera m_camera;
        [SerializeField] private PlanarColliderPositionClamper m_clamper;

        [SerializeField] private Transform m_cameraMainRoot;
        [SerializeField] private Transform m_cameraRotationXRoot;
        [SerializeField] private Transform m_cameraDistanceRoot;

        [SerializeField] private float m_rotationSensitivity = 1f;
        [SerializeField] private float m_wheelSensitive = 0.0001f;
        [SerializeField] private float m_movementDuration = 10;
        [SerializeField] private Vector2 m_MinMaxAngle = new Vector2(30, 89);
        [SerializeField] private AnimationCurve m_ZoomCurve = AnimationCurve.EaseInOut(0, 0.05f, 1, 1);

        private Plane m_InteractionPlane = new Plane(Vector3.up, Vector3.zero);
        private Vector3 m_TargetDragStartPoint;
        private Vector3 m_TotalDelta;
        private InteractionObject m_InteractionObject;

        private Vector3 m_CurrentPosition;
        private float m_CurrentZoom;
        private float m_CurrentYRotation = 0;
        private float m_CurrentXRotation = 0;

        private Vector3 m_TargetPosition;
        private float m_TargetZoom = 1f;
        private float m_TargetYRotation = 0;
        private float m_TargetXRotation = 0;

        public float TargetZoom
        {
            get => m_TargetZoom;
            set => m_TargetZoom = math.clamp(value, 0, m_ZoomCurve.keys.Last().time);
        }

        public Vector3 TargetPosition
        {
            get => m_TargetPosition;
            set => m_TargetPosition = m_clamper.ClampPosition(value);
        }

        public float TargetYRotation
        {
            get => m_TargetYRotation;
            set => m_TargetYRotation = value;
        }

        public float TargetXRotation
        {
            get => m_TargetXRotation;
            set => m_TargetXRotation = math.clamp(m_TargetXRotation = value, m_MinMaxAngle.x, m_MinMaxAngle.y);
        }

        private void Awake()
        {
            InputManager.Instance.RegisterCamera(m_camera);
            InputManager.Instance.CameraTracer.LayerSettings.RemoveLayers("LevelLimits", "TableThings");

            m_InteractionObject = m_camera.GetComponent<InteractionObject>();
            SimpleMouseInput.Instance.OnWheelEvent += OnWheel;
            SimpleMouseInput.Instance.SubscribeLeftButtonDrag(OnLeftDragStart, OnLeftDragPerformed, OnLeftDragEnd);
            SimpleMouseInput.Instance.SubscribeRightButtonDrag(OnRightDragStart, OnRightDragPerformed, OnRightDragEnd);

            ResetCameraPos();
        }

        public void ResetCameraPos()
        {
            m_CurrentPosition = TargetPosition = Vector3.zero;
            m_CurrentZoom = TargetZoom = 1f;
            m_CurrentYRotation = TargetYRotation = 0;
            m_CurrentXRotation = TargetXRotation = m_cameraRotationXRoot.eulerAngles.x;

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
            if (!IsCameraIntercepted())
            {
                if (m_CameraRaycaster.RaycastDeltaOnPlane(args.MousePosition - args.MouseDelta, args.MousePosition, m_InteractionPlane, out var delta))
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
            if (!IsCameraIntercepted())
            {
                TargetYRotation += args.MouseDelta.x * m_rotationSensitivity;
                TargetXRotation += args.MouseDelta.y * m_rotationSensitivity;
            }
        }

        private void OnRightDragEnd(MouseDragEventArgs args)
        {
        }

        private void OnWheel(float delta)
        {
            if (!IsCameraIntercepted())
                TargetZoom -= delta * m_wheelSensitive;
        }

        private bool IsCameraIntercepted() => InputManager.Instance.ActiveGestures.Any(c => c.InteractionObject != m_InteractionObject);

        #endregion

        private void Update()
        {
            var t = Time.deltaTime / m_movementDuration;
            m_CurrentPosition = math.lerp(m_CurrentPosition, m_TargetPosition, t);
            m_CurrentZoom = math.lerp(m_CurrentZoom, m_TargetZoom, t);
            m_CurrentXRotation = math.lerp(m_CurrentXRotation, m_TargetXRotation, t);
            m_CurrentYRotation = math.lerp(m_CurrentYRotation, m_TargetYRotation, t);

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            //Apply position and rotation settings
            m_cameraMainRoot.localPosition = m_CurrentPosition;
            m_cameraMainRoot.localRotation = Quaternion.AngleAxis(m_CurrentYRotation, Vector3.up);
            m_cameraRotationXRoot.localRotation = Quaternion.AngleAxis(m_CurrentXRotation, Vector3.right);

            //Apply zoom
            m_cameraDistanceRoot.transform.localPosition = Vector3.back * m_ZoomCurve.Evaluate(m_CurrentZoom);

            m_brain.ManualUpdate();
        }
    }
}