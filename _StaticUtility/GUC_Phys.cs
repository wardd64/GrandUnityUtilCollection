using UnityEngine;

namespace GUC {

    public static class Phys {

        /// <summary>
        /// prevents all children colliders in the given transform
        /// from colliding with eachother
        /// </summary>
        public static void IgnoreAllCrossCollisions(Transform parent) {
            Collider[] colliders = parent.GetComponentsInChildren<Collider>();
            int nboColliders = colliders.Length;
            for(int i = 0; i < nboColliders; i++) {
                for(int j = i + 1; j < nboColliders; j++) {
                    Physics.IgnoreCollision(colliders[i], colliders[j]);
                }
            }

        }

    }

}
