using System;
using System.Collections;
using System.Collections.Generic;
using PID_Controller;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityTools;
using Zenject;

namespace Game
{
    /// <summary>
    /// This class behaviours like a rigidbody in the scene.
    /// It becomes kinematic when is not touched, and is dynamic on user interaction
    /// </summary>
    public class RigidBodySceneElement : MonoBehaviour
    {
        private InteractionObject m_interactionObject;
        private Rigidbody m_rigidbody;

        [Inject] private CameraRaycaster m_cameraRaycaster;

        // Object positioning
        [SerializeField] private bool m_ResetPids = false;
        [SerializeField] private PidSettings m_positionPidSettings = PidSettings.Default;
        private Vector3PidController m_positionPidController;

        private Vector3 m_LocalInitialTouchPoint;
        private Vector3 m_CurrentWorldTouchPoint;
        private Plane m_InteractionPlane;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_interactionObject = GetComponent<InteractionObject>();
            m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);
            m_interactionObject.SubscribePointerGrabEvent(OnObjectGrabStart, OnObjectGrabEnd);

            m_rigidbody.isKinematic = true;
            m_positionPidController = new Vector3PidController(m_positionPidSettings);
        }

        private void OnObjectGrabStart(object sender, PointerInteractionEventArgs args)
        {
            GameExtraInput.Instance.OnWheelEvent += OnWheel;
        }

        private void OnObjectGrabEnd(object sender, PointerInteractionEventArgs args)
        {
            GameExtraInput.Instance.OnWheelEvent -= OnWheel;
        }

        private void OnWheel(float delta)
        {
        }
        
        private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
        {

            m_rigidbody.isKinematic = false;
            m_cameraRaycaster.RaycastScreenToPhysics(args.PointerPrevPosition, out var worldTouchPoint);
            m_CurrentWorldTouchPoint = worldTouchPoint;
            m_LocalInitialTouchPoint = m_rigidbody.transform.InverseTransformPoint(worldTouchPoint);
            m_InteractionPlane = new Plane(Vector3.up, worldTouchPoint);
            m_positionPidController.ResetAllDimensions();
        }

        private void OnDrag(object sender, PointerDragInteractionEventArgs args)
        {
            m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_InteractionPlane, out var targetTouchPoint);
            m_CurrentWorldTouchPoint = targetTouchPoint;
        }

        private void OnDragEnd(object sender, PointerDragInteractionEventArgs args)
        {
            m_rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (m_rigidbody.isKinematic)
                return;

            var initialWorldTouchPoint = m_rigidbody.transform.TransformPoint(m_LocalInitialTouchPoint);
            var velocity = m_positionPidController.Iterate(m_CurrentWorldTouchPoint, initialWorldTouchPoint, Time.fixedDeltaTime);

            m_rigidbody.velocity = velocity;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            var from = m_rigidbody.transform.TransformPoint(m_LocalInitialTouchPoint);
            var to = m_CurrentWorldTouchPoint;

            Gizmos.DrawLine(from, to);
        }

        private void Update()
        {
            m_positionPidController.SetSettings(m_positionPidSettings);

            if (m_ResetPids)
            {
                m_positionPidController.ResetAllDimensions();
                m_ResetPids = false;
            }
        }
    }

    public static class Helpers
    {
        public static Vector3 ClampAngleTo180(this Vector3 angle)
        {
            while (angle.x > 180f)
                angle.x -= 360f;
            while (angle.y > 180f)
                angle.y -= 360f;
            while (angle.z > 180f)
                angle.z -= 360f;
            return angle;
        }

        public static void DrawTestSphere(Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = position;
            go.transform.localScale = Vector3.one * .01f;
            go.GetComponent<Collider>().enabled = false;
        }
    }
}