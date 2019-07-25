using System;
using UnityEngine;

[Serializable]
public class ColorData{
    float r, g, b, a;

	public ColorData(Color c) {
        r = c.r;
        g = c.g;
        b = c.b;
        a = c.a;
	}

	public Color Regenerate() {
        return new Color(r, g, b, a);
	}

	public Color Lerp(ColorData other, float factor) {
		return Color.Lerp(this.Regenerate(), other.Regenerate(), factor);
	}

	public ColorData Clone() {
		return new ColorData(this.Regenerate());
	}

	public override string ToString() {
		return Regenerate().ToString();
	}
}
