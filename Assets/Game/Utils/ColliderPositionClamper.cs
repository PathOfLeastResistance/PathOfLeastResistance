using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public interface IPositionClamper
    {
        Vector3 ClampPosition(Vector3 position);
    }

    public class ColliderPositionClamper : MonoBehaviour, IPositionClamper
    {
        [SerializeField] protected List<Collider> mClamerColliders = new List<Collider>();

        /// <summary>
        /// This method takes all colliders and clamps position to the nearest of all of them.
        /// It does not clamp in case of empty {mClamerColliders} list.
        /// </summary>
        /// <param name="position">position to clamp</param>
        /// <returns></returns>
        public virtual Vector3 ClampPosition(Vector3 position)
        {
            var minDeltaSqr = float.MaxValue;
            var result = position;

            foreach (var clamperCollider in mClamerColliders)
            {
                var clamped = clamperCollider.ClosestPoint(position);
                var deltaSqr = Vector3.SqrMagnitude(position - clamped);
                if (deltaSqr < minDeltaSqr)
                {
                    minDeltaSqr = deltaSqr;
                    result = clamped;
                }
            }
            return result;
        }
    }
}
