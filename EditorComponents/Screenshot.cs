using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Screenshot : MonoBehaviour { }

#if UNITY_EDITOR

[CustomEditor(typeof(Screenshot))]
public class ScreenShotExt : Editor {

    private string savePath;
    private int pixWidth, pixHeight;

    Transform transform {
        get { return ((TransformInfo)this.target).transform; }
    }

    public override void OnInspectorGUI() {

        Camera cam = FindObjectOfType<Camera>();
        if(cam != null) {
            pixWidth = cam.pixelWidth;
            pixHeight = cam.pixelHeight;
        }

        GUILayout.Label("Screenshot utility, save image to: ");

        if(string.IsNullOrEmpty(savePath)) {
            savePath = @"C:\tmp";
        }
        savePath = GUILayout.TextField(savePath);

        GUILayout.Label("");
        string resolutionString = pixWidth + " x " + pixHeight;
        GUILayout.Label("Saving at resolution: " + resolutionString);

        if(GUILayout.Button("Screenshot"))
            Shot();
    }

    private void Shot() {

        string folder = savePath;
        if(folder.Substring(folder.Length - 1) != @"\")
            folder += @"\";

        string file = "UnityScreenshot.png";
        string fullPath = folder + file;

        Debug.Log("Attempting to save screenshot to " + fullPath);
        ScreenCapture.CaptureScreenshot(fullPath);
    }
}

#endif