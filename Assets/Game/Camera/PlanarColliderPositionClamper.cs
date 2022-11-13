using UnityEngine;

namespace Game
{
    public class PlanarColliderPositionClamper : ColliderPositionClamper
    {
        [SerializeField] private Vector3 m_normal = Vector3.up;
        [SerializeField] private Vector3 m_point = Vector3.zero;
        
        public override Vector3 ClampPosition(Vector3 position)
        {
            var plane = new Plane(m_normal, m_point);
            var result = base.ClampPosition(position);
            return plane.ClosestPointOnPlane(result);
        }
    }
}