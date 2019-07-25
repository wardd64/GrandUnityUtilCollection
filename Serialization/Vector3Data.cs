using System;
using UnityEngine;

[Serializable]
public class Vector3Data{
	float x, y, z;

	public Vector3Data(Vector3 v) {
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public Vector3 Regenerate() {
		return new Vector3(x, y, z);
	}

	public Vector3 Lerp(Vector3Data other, float factor) {
		return Vector3.Lerp(this.Regenerate(), other.Regenerate(), factor);
	}

	public Vector3Data Clone() {
		return new Vector3Data(this.Regenerate());
	}

	public override string ToString() {
		return Regenerate().ToString();
	}
}
