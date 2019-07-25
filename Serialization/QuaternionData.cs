using System;
using UnityEngine;

[Serializable]
public class QuaternionData {
	float x, y, z, w;

    public QuaternionData(Quaternion q) {
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}

    public Quaternion Regenerate() {
		return new Quaternion(x, y, z, w);
	}

	public Quaternion Lerp(QuaternionData other, float factor) {
		return Quaternion.Slerp(this.Regenerate(), other.Regenerate(), factor);
	}

	public QuaternionData Clone() {
		return new QuaternionData(this.Regenerate());
	}

	public override string ToString() {
		return Regenerate().eulerAngles.ToString();
	}
}
