using UnityEngine;

namespace GUC {

    public static class Vect {

        /// <summary>
        /// returns source vector, extended along the given direction, so that its component
        /// along that direction is equal to the given magnitude
        /// </summary>
        public static Vector3 SetComponent(this Vector3 source, Vector3 direction, float magnitude) {
            Vector3 perpendicular = Vector3.ProjectOnPlane(source, direction);
            Vector3 parallel = direction.normalized * magnitude;
            return parallel + perpendicular;
        }

        /// <summary>
        /// returns source vector, extended along the given direction, so that its component
        /// along that direction is equal to the same component of the reference vector.
        /// </summary>
        public static Vector3 SetComponent(this Vector3 source, Vector3 direction, Vector3 reference) {
            float magnitude = Vector3.Dot(reference, direction.normalized);
            return SetComponent(source, direction, magnitude);
        }

        /// <summary>
        /// returns source vector, extended along the given reference, so that its component
        /// along that direction is at least as big as the reference vector.
        /// </summary>
        public static Vector3 MinComponent(this Vector3 source, Vector3 reference) {
            float magnitude = Vector3.Dot(source, reference.normalized);
            float refMag = reference.magnitude;

            if(magnitude >= refMag)
                return reference;
            else
                return SetComponent(source, reference, refMag);
        }

        /// <summary>
        /// Returns true if given vector does not have any illegal components (NaN or Infinity)
        /// </summary>
        public static bool IsLegal(this Vector3 vector) {
            if(float.IsNaN(vector.x) || float.IsInfinity(vector.x))
                return false;
            if(float.IsNaN(vector.y) || float.IsInfinity(vector.y))
                return false;
            if(float.IsNaN(vector.z) || float.IsInfinity(vector.z))
                return false;
            return true;
        }

        /// <summary>
        /// Returns vector that applies the opposite scaling effect as the given vector.
        /// Note that off-axis scaling cannot be undone with a single scaling vector.
        /// </summary>
        public static Vector3 InvertScaling(this Vector3 source) {
            float x = 1f / source.x;
            float y = 1f / source.y;
            float z = 1f / source.z;
            return new Vector3(x, y, z);
        }



        /// <summary>
        /// Scales given vector to the appropriate length, so that the 
        /// sum with given base vector results in a normalized vector.
        /// </summary>
        public static Vector3 NormalizeSum(this Vector3 vec, Vector3 baseVec) {
            vec = vec.normalized;
            float dot = Vector3.Dot(baseVec, vec);
            float baseSqr = baseVec.sqrMagnitude;

            float rootFac = 4f * (dot * dot - baseSqr);
            float s = (-2f * dot * Mathf.Sqrt(rootFac)) / 2f;
            return s * vec;
        }

        /// <summary>
        /// Returns a new vector which is the input scaled by scale.
        /// (Component-wise multiplication)
        /// </summary>
        public static Vector2 NewScale(this Vector2 input, Vector2 scale) {
            return new Vector2(input.x * scale.x, input.y * scale.y);
        }

        /// <summary>
        /// Returns a new vector which is the input inversely scaled by scale. 
        /// (Component-wise division)
        /// </summary>
        public static Vector2 InverseScale(this Vector2 input, Vector2 scale) {
            return new Vector2(input.x / scale.x, input.y / scale.y);
        }

        /// <summary>
        /// Returns a new vector which is the input with all components shifted 
        /// by the value a.
        /// </summary>
        public static Vector2 Shift(this Vector2 input, float a) {
            return new Vector2(input.x + a, input.y + a);
        }

        /// <summary>
        /// Returns a new vector which is the input vector with its 
        /// components rounded
        /// </summary>
        public static Vector2 Round(this Vector2 input) {
            return new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));
        }



        /// <summary>
        /// True if both components of the given vector lie between 0 and 1 including.
        /// </summary>
        public static bool InRange01(Vector2 position) {
            Rect bounds = new Rect(0f, 0f, 1f, 1f);
            return bounds.Contains(position);
        }

        /// <summary>
        /// Returns true if the given point lies in the given sphere.
        /// </summary>
        public static bool InsideSphere(this Vector3 point, Vector3 center, float radius) {
            return (point - center).sqrMagnitude < radius * radius;
        }

        /// <summary>
        /// Returns position of the given point after rotating it around the pivot with the given rotation.
        /// </summary>
        public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation) {
            Vector3 dir = point - pivot;
            Vector3 rotatedDir = rotation * dir;
            return pivot + rotatedDir;
        }

        /// <summary>
        /// Returns a uniform scaling vector based on the given value.
        /// The value used for the new vector is always the 'odd one out' 
        /// if appropriate. In this way, you can use this method to keep 
        /// the scale of a Unity Transform uniform while a user edits 
        /// the value in the editor.
        /// </summary>
        public static Vector3 EqualizeScale(this Vector3 scale) {
            float EPSILON = 1e-4f;
            float factor = Vector3.Dot(scale, Vector3.one) / 3f;
            if(Mathf.Abs(scale.x - scale.y) < EPSILON)
                factor = scale.z;
            else if(Mathf.Abs(scale.z - scale.y) < EPSILON)
                factor = scale.x;
            else if(Mathf.Abs(scale.x - scale.z) < EPSILON)
                factor = scale.y;
            return factor * Vector3.one;
        }
    }

}
