using UnityEngine;

namespace GUC {

    public static class Anim {

        public static bool AnimatorHasParam(string paramName, Animator animator) {
            foreach(AnimatorControllerParameter param in animator.parameters) {
                if(param.name == paramName)
                    return true;
            }
            return false;
        }

    }

}
