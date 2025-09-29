using System.Collections.Generic;
using UnityEngine;

namespace Bootlegger
{
    public static class Extentions
    {
        // Vectors
        public static Vector3 FlattenVector(this Vector3 vector) => new(vector.x, 0, vector.z);
        public static Vector3 ProjectOnPlane(ref this Vector3 value, Vector3 planeNormal) => value - planeNormal * Vector3.Dot(value, planeNormal);
        public static Vector3 ProjectOnPlane(Vector3 value, Vector3 planeNormal) => value - planeNormal * Vector3.Dot(value, planeNormal);

        // Quaternions
        public static Quaternion FlattenRotation(this Quaternion rotation, Vector3 up) => Quaternion.AngleAxis(rotation.eulerAngles.y, up);

        // Generics
        public static T GetRandom<T>(this List<T> list) => list[Random.Range(0, list.Count - 1)];
        public static T GetRandom<T>(this T[] list) => list[Random.Range(0, list.Length - 1)];

        public static float NormalizeAngle(this float angle)
        {
            if (angle < -180f)
                angle += 360f;

            if (angle > 180f)
                angle -= 360f;

            return angle;
        }
    }
}
