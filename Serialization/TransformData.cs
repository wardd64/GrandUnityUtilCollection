using System;
using UnityEngine;

[Serializable]
public class TransformData : ComponentData{

	Vector3Data position;
	QuaternionData rotation;
	Vector3Data scale;

	public TransformData(Vector3Data pos, QuaternionData rot, Vector3Data scale) {
		this.position = pos;
		this.rotation = rot;
		this.scale = scale;
	}

	public TransformData(Vector3 pos, Quaternion rot, Vector3 scale) {
		this.position = new Vector3Data(pos);
		this.rotation = new QuaternionData(rot);
		this.scale = new Vector3Data(scale);
	}

	public TransformData(Transform t) :
		this(t.localPosition, t.localRotation, t.localScale) {}

	public override void Apply(Component target) {
		Transform t = (Transform)target;
		t.localPosition = this.position.Regenerate();
		t.localRotation = this.rotation.Regenerate();
		t.localScale = this.scale.Regenerate();
	}

	public override void ApplyLerp(Component target, ComponentData other, float factor) {
		Transform t = (Transform)target;
		TransformData right = (TransformData)other;

		t.localPosition = this.position.Lerp(right.position, factor);
		t.localRotation = this.rotation.Lerp(right.rotation, factor);
		t.localScale = this.scale.Lerp(right.scale, factor);
	}
}
