using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TransformInfo : MonoBehaviour { }


#if UNITY_EDITOR
[CustomEditor(typeof(TransformInfo))]
public class TransformInfoEditor : Editor {

	Transform transform	{ get {
		return ((TransformInfo)this.target).transform;
	}}

	public override void OnInspectorGUI() {
		serializedObject.Update();

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Vector3 scale = transform.lossyScale;

        GUILayout.Label("Absolute transform:");
		GUILayout.Label(pos.ToString());
		GUILayout.Label(rot.eulerAngles.ToString());
		GUILayout.Label(scale.ToString());
		GUILayout.Label("");

		GUILayout.Label("Transform index path:");
		GUILayout.Label(GUC.Trnsf.GetIndexPath(transform));
		GUILayout.Label("");

        GUILayout.Label("Absolute transform matrix:");
        Matrix4x4 mat = Matrix4x4.TRS(pos, rot, scale);
        GUILayout.Label(mat.ToString());
        GUILayout.Label("");

        GUILayout.Label("Absolute rotation matrix:");
        Matrix4x4 rotMat = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
        GUILayout.Label(rotMat.ToString());
        GUILayout.Label("");

    }
}
#endif


