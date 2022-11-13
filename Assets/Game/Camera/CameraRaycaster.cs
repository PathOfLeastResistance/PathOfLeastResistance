using UnityEngine;
using UnityTools;

namespace Game
{
    public class CameraRaycaster : SingletonMonobehaviour<CameraRaycaster>
    {
        private Camera m_camera;

        private void Awake()
        {
            m_camera = Camera.main;
        }

        public bool RaycastScreenToPhysics(Vector3 point, out Vector3 result)
        {
            var ray = m_camera.ScreenPointToRay(point);
            if (Physics.Raycast(ray, out var hit))
            {
                result = hit.point;
                return true;
            }
            result = Vector3.zero;
            return  false;
        }

        /// <summary>
        /// Raycast from screen position to given plane, and returns the world position
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool RaycastPointOnPlane(Vector3 point, Plane plane, out Vector3 result)
        {
            var ray = m_camera.ScreenPointToRay(point);
            if (plane.Raycast(ray, out var enter))
            {
                result = ray.GetPoint(enter);
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Raycasts from screen positions to given plane, returns delta in 3d
        /// </summary>
        /// <param name="prevPosition2d"></param>
        /// <param name="newPosition2d"></param>
        /// <param name="plane"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool ProjectDeltaOnPlane(Vector3 prevPosition2d, Vector3 newPosition2d, Plane plane, out Vector3 result)
        {
            var prevRay = m_camera.ScreenPointToRay(prevPosition2d);
            var newRay = m_camera.ScreenPointToRay(newPosition2d);
            if (plane.Raycast(prevRay, out var prevEnter) && plane.Raycast(newRay, out var newEnter))
            {
                var prevPoint = prevRay.GetPoint(prevEnter);
                var newPoint = newRay.GetPoint(newEnter);
                result = newPoint - prevPoint;
                return true;
            }

            result = Vector3.zero;
            return false;
        }
    }
}