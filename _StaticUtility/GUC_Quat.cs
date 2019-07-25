using UnityEngine;

namespace GUC {

    public static class Quat {

        /// <summary>
        /// Returns Quaternion scaled with the given factor, relative 
        /// to identity quaternion.
        /// </summary>
        public static Quaternion Scale(this Quaternion quat, float factor) {
            return Quaternion.LerpUnclamped(Quaternion.identity, quat, factor);
        }

        /// <summary>
        /// Twists Quaternion around an axis until the angle with the given reference is minimized
        /// </summary>
        public static Quaternion MinimizeAngle(this Quaternion quat, Vector3 axis, Quaternion reference) {
            float amount = 45f;
            float lastAngle = Quaternion.Angle(quat, reference);
            for(int i = 0; i < 20; i++) {
                Quaternion pos = Quaternion.AngleAxis(amount, axis) * quat;
                Quaternion neg = Quaternion.AngleAxis(-amount, axis) * quat;
                float posAngle = Quaternion.Angle(pos, reference);
                float negAngle = Quaternion.Angle(neg, reference);
                if(posAngle < lastAngle) {
                    quat = pos;
                    lastAngle = posAngle;
                }
                else if(negAngle < lastAngle) {
                    quat = neg;
                    lastAngle = negAngle;
                }
                else {
                    amount /= 2f;
                }
            }
            return quat;
        }

        /// <summary>
        /// Returns true if given quaternion does not contain NaN and
        /// has size 1, as it should.
        /// </summary>
        public static bool IsLegal(this Quaternion quat) {
            if(float.IsNaN(quat.w))
                return false;
            if(float.IsNaN(quat.x))
                return false;
            if(float.IsNaN(quat.y))
                return false;
            if(float.IsNaN(quat.z))
                return false;
            float sizeSqr = quat.w * quat.w + quat.x * quat.x +
                quat.y * quat.y + quat.z * quat.z;
            return sizeSqr > 0.9f && sizeSqr < 1.1f;
        }

        /// <summary>
        /// Clamp all components of given input euler vector between min and max values
        /// </summary>
        public static Vector3 ClampEuler(Vector3 input, Vector3 min, Vector3 max) {
            return new Vector3(
                Math.ClampAngle(input.x, min.x, max.x),
                Math.ClampAngle(input.y, min.y, max.y),
                Math.ClampAngle(input.z, min.z, max.z));
        }

        public static Quaternion WorldToLocal(this Transform transform, Quaternion input) {
            return Quaternion.Inverse(transform.parent.rotation) * input;
        }

        public static Quaternion LocalToWorld(this Transform transform, Quaternion input) {
            return transform.parent.rotation * input;
        }


        /// <summary>
        /// Returns the smallest angle covered by the given quaternion, 
        /// relative to the identity quaternion. This method has improved accuracy 
        /// when dealing with small angles, when compared the the Unity standard method.
        /// </summary>
        public static float GetQuatAngle(this Quaternion q) {
            float angle = Quaternion.Angle(Quaternion.identity, q);

            if(angle < 45f) {
                Vector3 v = Vector3.forward;
                float sinRad = Vector3.Cross(v, q * v).magnitude;
                return Mathf.Asin(sinRad) * Mathf.Rad2Deg;
            }
            else
                return angle;
        }

        /// <summary>
        /// Converts the given rotation (delta) to an angleAxis format.
        /// The given vector will point in the right-hand-rule direction of the rotation,
        /// and have magnitude corresponding the the angle covered.
        /// </summary>
        public static Vector3 GetAxis(this Quaternion q) {
            Vector3 v = Vector3.forward;
            float angle = GetQuatAngle(q);
            return angle * Vector3.Cross(v, q * v).normalized;
        }

        /// <summary>
        /// Returns a Vector4 containing x,y,z,w values of
        /// the given quaternion
        /// </summary>
        public static Vector4 ToVector(this Quaternion q) {
            return new Vector4(q.x, q.y, q.z, q.w).normalized;
        }

    }

}
