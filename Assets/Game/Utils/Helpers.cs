using UnityEngine;

namespace Game
{
    public static class Helpers
    {
        public static float AngleDifference(float a, float b)
        {
            return (a - b + 540f) % 360 - 180f;
        }

        public static float ClampAngleTo180(float angle)
        {
            while (angle > 180f)
                angle -= 360f;
            while (angle <= -180f)
                angle += 360f;
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