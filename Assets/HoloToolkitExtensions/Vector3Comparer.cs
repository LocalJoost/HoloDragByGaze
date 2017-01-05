using System;
using UnityEngine;
using System.Collections.Generic;

namespace LocalJoost.HoloToolkitExtensions
{
    public class Vector3Comparer : IEqualityComparer<Vector3>
    {
        const float Delta = 0.00001f;

        public bool Equals(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.x - v2.x) < Delta && Math.Abs(v1.y - v2.y) < Delta && Math.Abs(v1.z - v2.z) < Delta;
        }

        public int GetHashCode(Vector3 obj)
        {
            return obj.GetHashCode();
        }
    }
}
