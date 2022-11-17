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
        [SerializeField] private double Kp;
        [SerializeField] private double Ki;
        [SerializeField] private double Kd;
        [SerializeField] private double N;

        private PID mPidX;
        private PID mPidZ;
        private Vector3 m_LocalInitialTouchPoint;
        private Vector3 m_CurrentWorldTouchPoint;
        private Plane m_InteractionPlane;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_interactionObject = GetComponent<InteractionObject>();
            m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);

            m_rigidbody.isKinematic = true;
            mPidX = new PID(Kp, Ki, Kd, N, double.PositiveInfinity, Double.NegativeInfinity);
            mPidZ = new PID(Kp, Ki, Kd, N, double.PositiveInfinity, Double.NegativeInfinity);
        }

        private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
        {
            m_rigidbody.isKinematic = false;
            m_cameraRaycaster.RaycastScreenToPhysics(args.PointerPrevPosition, out var worldTouchPoint);
            m_CurrentWorldTouchPoint = worldTouchPoint;
            m_LocalInitialTouchPoint = m_rigidbody.transform.InverseTransformPoint(worldTouchPoint);
            m_InteractionPlane = new Plane(Vector3.up, worldTouchPoint);
            
            mPidX.ResetController();
            mPidZ.ResetController();
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
            var xVelocity = mPidX.PID_iterate(m_CurrentWorldTouchPoint.x, initialWorldTouchPoint.x, Time.fixedDeltaTime);
            var zVelocity = mPidZ.PID_iterate(m_CurrentWorldTouchPoint.z, initialWorldTouchPoint.z, Time.fixedDeltaTime);

            m_rigidbody.velocity = new Vector3((float)xVelocity, 0, (float)zVelocity);
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
            mPidX.Kd = Kd;
            mPidX.Ki = Ki;
            mPidX.Kp = Kp;
            mPidX.N = N;
            
            mPidZ.Kd = Kd;
            mPidZ.Ki = Ki;
            mPidZ.Kp = Kp;
            mPidZ.N = N;

            if (m_ResetPids)
            {
                mPidX.ResetController();
                mPidZ.ResetController();
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