using Unity.Mathematics;
using UnityEngine;

namespace ViJApps.CanvasTexture.Utils
{
    /// <summary>
    /// Line to line intersection
    /// </summary>
    public enum LineIntersectionType
    {
        Empty,      //No intersection
        Point,      //The intersection has just one point
        Line,       //Lines are parallel and collinear
    }
    
    public static class Geometry2d
    {
        public static float DistanceToSegment(float2 p0, float2 p1, float2 p)
            => math.distance(NearestPointToSegment(p0, p1, p), p); 
        
        public static float2 NearestPointToSegment(float2 p0, float2 p1, float2 p)
        {
            var a = p.x - p0.x;
            var b = p.y - p0.y;
            var c = p1.x - p0.x;
            var d = p1.y - p0.y;

            var dot = a * c + b * d;
            var lenSq = c * c + d * d;
            var param = -1.0f;

            //in case of 0 length line
            if (lenSq != 0)
                param = dot / lenSq;

            float2 nearest;
            if (param < 0)
                nearest = p0;
            else if (param > 1)
                nearest = p1;
            else
                nearest = new float2(p0.x + param * c, p0.y + param * d);
            return nearest;
        }
        
        public static float DotPerpendicular(this float2 point, float2 direction) => point.x * direction.y - point.y * direction.x;
        
        /// <summary>
        /// Checks the line intersection. The lines are represented as point and direction.
        /// </summary>
        /// <param name="p0">Point 0</param>
        /// <param name="d0">Direction 0</param>
        /// <param name="p1">Point 1</param>
        /// <param name="d1">Direction 1</param>
        /// <param name="dotThreshold">threshold for dot product</param>
        /// <param name="result">the result t[0] for p0d0 and t[1] for p1d1</param>
        /// <returns></returns>
        public static LineIntersectionType LineLineIntersection(float2 p0, float2 d0, float2 p1, float2 d1, float dotThreshold, ref float2 result)
        {
            var diff = p1 - p0;
            var d0DotPerpD1 = d0.DotPerpendicular(d1);
            if (math.abs(d0DotPerpD1) > dotThreshold)
            {
                // Lines intersect in a single point.
                var invD0DotPerpD1 = 1 / d0DotPerpD1;
                var diffDotPerpD0 = diff.DotPerpendicular(d0);
                var diffDotPerpD1 = diff.DotPerpendicular(d1);
                result[0] = diffDotPerpD1 * invD0DotPerpD1;
                result[1] = diffDotPerpD0 * invD0DotPerpD1;
                return LineIntersectionType.Point;
            }

            // Lines are parallel.
            diff = math.normalizesafe(diff, d1);
            var diffNDotPerpD1 = diff.DotPerpendicular(d1);
            
            // Lines are collinear.
            if (math.abs(diffNDotPerpD1) <= dotThreshold)
                return LineIntersectionType.Line;

            // Lines are parallel, but distinct.
            return LineIntersectionType.Empty;
        }
    }
}
