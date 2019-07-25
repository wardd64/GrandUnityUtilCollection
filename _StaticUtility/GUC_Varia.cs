using UnityEngine;

namespace GUC { 

    /// <summary>
    /// Temporary class for functions that do not yet have proper classification
    /// </summary>
    public static class Varia {

        /// <summary>
        /// If value is true, sets cursor hidden, locked in the middle of the screen.
        /// When false, the cursor is released and visible.
        /// </summary>
        /// <param name="value"></param>
        public static void SetFPSCursor(bool value) {
            if(value) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
