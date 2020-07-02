
using System;
using UnityEngine;

/// <summary>
/// Encodes a color while buffering hue and saturation values.
/// This can be useful to create smooth color changes based on hue 
/// and saturation that pass over singular colors.
/// Provides added functionality for smoothly changing the color 
/// based on one of its components.
/// </summary>
[Serializable]
public class BufferedColor {
    public Color color;

    public float R { get { return color.r; } }
    public float G { get { return color.g; } }
    public float B { get { return color.b; } }
    public float A { get { return color.a; } }
    public float H { get; private set; }
    public float S { get; private set; }
    public float V { get { return GUC.Colrs.RGBToHSV(color).z; } }

    /// <summary>
    /// Create opaque black color with hue and sat 0
    /// </summary>
    public BufferedColor() {
        this.H = 0f;
        this.S = 0f;
        this.color = Color.black;
    }

    /// <summary>
    /// Create buffered color based on given color,
    /// hue and sat will be made up from this color if possible 
    /// and default to 0 otherwise.
    /// </summary>
    public BufferedColor(Color color) : this() {
        this.Set(color);
    }

    /// <summary>
    /// Create opaque buffered color based on given HSV values
    /// </summary>
    public BufferedColor(float h, float s, float v) : this() {
        this.Set(GUC.Colrs.HSVToRGB(h, s, v), h, s);
    }

    /// <summary>
    /// Create buffered color based on given color,
    /// while setting the given hue and sat values, which are assumed 
    /// to correspond to the source color
    /// </summary>
    public BufferedColor(Color color, float hue, float sat) : this(color) {
        this.H = hue;
        this.S = sat;
    }

    /// <summary>
    /// Create buffered color based on the source as a default 
    /// for hue and sat values, then overwrites any values as needed.
    /// </summary>
    public BufferedColor(Color color, BufferedColor source) :
        this(color, source.H, source.S) {
        this.Set(color);
    }

    /// <summary>
    /// Change this buffered color, 
    /// while mainting sat and hue values if appropriate
    /// </summary>
    public void Set(Color color) {
        this.Set(color, this.H, this.S);
    }

    /// <summary>
    /// Change this buffered color, 
    /// while reverting to the given default values for sat and hue if appropriate
    /// </summary>
    public void Set(Color color, float defaultHue, float defaultSaturation) {
        this.color = color;
        Vector3 hsv = GUC.Colrs.RGBToHSV(color);

        bool hueSingularity = hsv.y == 0f || hsv.z == 0f;
        if(hueSingularity)
            this.H = defaultHue;
        else
            this.H = hsv.x;

        bool saturationSingularity = hsv.z == 0f;
        if(saturationSingularity)
            this.S = defaultSaturation;
        else
            this.S = hsv.y;
    }

    /// <summary>
    /// Return buffered color with changed R value while maintaining G and B
    /// </summary>
    public BufferedColor PickR(float value) {
        Color toReturn = this.color;
        toReturn.r = value;
        return new BufferedColor(toReturn, this);
    }

    /// <summary>
    /// Return buffered color with changed G value while maintaining R and B
    /// </summary>
    public BufferedColor PickG(float value) {
        Color toReturn = this.color;
        toReturn.g = value;
        return new BufferedColor(toReturn, this);
    }

    /// <summary>
    /// Return buffered color with changed B value while maintaining R and G
    /// </summary>
    public BufferedColor PickB(float value) {
        Color toReturn = this.color;
        toReturn.b = value;
        return new BufferedColor(toReturn, this);
    }

    /// <summary>
    /// Return buffered color with changed alpha value while maintaining color
    /// </summary>
    public BufferedColor PickA(float value) {
        Color toReturn = this.color;
        toReturn.a = value;
        return new BufferedColor(toReturn, this);
    }

    /// <summary>
    /// Return buffered color with changed hue value while maintaining 
    /// (buffered) saturation and value
    /// </summary>
    public BufferedColor PickH(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(value, hsv.y, hsv.z);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, value, S);
    }

    /// <summary>
    /// Return buffered color with changed saturation value while maintaining 
    /// (buffered) hue and value
    /// </summary>
    public BufferedColor PickS(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(H, value, hsv.z);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, H, value);
    }

    /// <summary>
    /// Return buffered color with changed value while maintaining 
    /// (buffered) hue and saturation
    /// </summary>
    public BufferedColor PickV(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(H, S, value);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, H, S);
    }
}