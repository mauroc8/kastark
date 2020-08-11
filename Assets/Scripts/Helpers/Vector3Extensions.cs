using UnityEngine;
using System;

namespace Vector3Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        public static Vector3 Floor(this Vector3 vector)
        {
            return new Vector3(
                x: Mathf.Floor(vector.x),
                y: Mathf.Floor(vector.y),
                z: Mathf.Floor(vector.z)
            );
        }
    }
}

