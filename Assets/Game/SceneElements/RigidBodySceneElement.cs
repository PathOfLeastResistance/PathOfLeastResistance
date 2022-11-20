using System;
using System.Collections;
using System.Collections.Generic;
using PID_Controller;
using Unity.Mathematics;
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
        [SerializeField] private bool m_resetPids = false;
        [SerializeField] private float m_rotationSensitivity = 0.1f;
        [SerializeField] private PidSettings m_positionPidSettings = PidSettings.Default;
        [SerializeField] private PidSettings m_rotationPidSettings = PidSettings.Default;

        //Position PID controller
        private Vector3PidController m_positionPidController;
        private Vector3 m_LocalInitialTouchPoint;
        private Vector3 m_CurrentWorldTouchPoint;
        private Plane m_InteractionPlane;

        //Rotation PID controller
        private DoublePidController m_rotationPidController;
        private float m_TargetRotation;
        private float m_CurrentRotation;
        private float m_lastRotation;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_interactionObject = GetComponent<InteractionObject>();
            m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);
            m_interactionObject.SubscribePointerGrabEvent(OnObjectGrabStart, OnObjectGrabEnd);

            m_rigidbody.isKinematic = true;
            m_positionPidController = new Vector3PidController(m_positionPidSettings);
            m_rotationPidController = new DoublePidController(m_rotationPidSettings);
        }

        private void OnObjectGrabStart(object sender, PointerInteractionEventArgs args)
        {
            m_rigidbody.isKinematic = false;
            
            // Save rotation data
            GameExtraInput.Instance.OnWheelEvent += OnWheel;
            m_lastRotation = m_CurrentRotation = m_TargetRotation = m_rigidbody.rotation.eulerAngles.y;
            m_rotationPidController.ResetAllDimensions();

            //Save position data
            m_cameraRaycaster.RaycastScreenToPhysics(args.PointerPosition, out var worldTouchPoint);
            m_CurrentWorldTouchPoint = worldTouchPoint;
            m_LocalInitialTouchPoint = m_rigidbody.transform.InverseTransformPoint(worldTouchPoint);
            m_InteractionPlane = new Plane(Vector3.up, worldTouchPoint);
            m_positionPidController.ResetAllDimensions();
        }

        private void OnObjectGrabEnd(object sender, PointerInteractionEventArgs args)
        {
            GameExtraInput.Instance.OnWheelEvent -= OnWheel;
            m_rigidbody.isKinematic = true;
        }

        private void OnWheel(float delta)
        {
            m_TargetRotation += delta * m_rotationSensitivity;
        }

        private void OnDragStart(object sender, PointerDragInteractionEventArgs args)
        {
        }

        private void OnDrag(object sender, PointerDragInteractionEventArgs args)
        {
            m_cameraRaycaster.RaycastPointOnPlane(args.PointerPosition, m_InteractionPlane, out var targetTouchPoint);
            m_CurrentWorldTouchPoint = targetTouchPoint;
        }

        private void OnDragEnd(object sender, PointerDragInteractionEventArgs args)
        {
        }

        private void FixedUpdate()
        {
            if (m_rigidbody.isKinematic)
                return;

            //PID position
            var initialWorldTouchPoint = m_rigidbody.transform.TransformPoint(m_LocalInitialTouchPoint);
            var velocity = m_positionPidController.Iterate(m_CurrentWorldTouchPoint, initialWorldTouchPoint, Time.fixedDeltaTime);
            m_rigidbody.velocity = velocity;

            //PID rotation
            var delta = Helpers.AngleDifference(m_lastRotation, m_rigidbody.rotation.eulerAngles.y);
            m_CurrentRotation += delta;
            m_lastRotation = m_rigidbody.rotation.eulerAngles.y;
            var angularVelocity = (float)m_rotationPidController.Iterate(m_CurrentRotation, m_TargetRotation, Time.fixedDeltaTime);
            m_rigidbody.angularVelocity = Vector3.up * angularVelocity;

            // Debug.Log($"{m_TargetRotation}, {m_CurrentRotation}, {m_TargetRotation - m_CurrentRotation}");
        }

#if UNITY_EDITOR
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
            m_rotationPidController.SetSettings(m_rotationPidSettings);

            if (m_resetPids)
            {
                m_positionPidController.ResetAllDimensions();
                m_rotationPidController.ResetAllDimensions();
                m_resetPids = false;
            }
        }
#endif
    }
}