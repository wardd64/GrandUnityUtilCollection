using UnityEngine;

namespace GUC {

    public static class Cam {

        /// <summary>
        /// Returns vector2 containing width and height of the near clip 
        /// plane of the given camera. Useful for matching a quad to layer 
        /// on a camera view
        /// </summary>
        public static Vector2 GetNearClipExtents(this Camera camera) {
            float halfFOV = camera.fieldOfView * .5f * Mathf.Deg2Rad;
            float aspect = camera.aspect;

            float height = Mathf.Tan(halfFOV);
            float width = height * aspect;

            return new Vector2(width, height);
        }

    }

}
