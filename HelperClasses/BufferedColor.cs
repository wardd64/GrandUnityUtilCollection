
using System;
using UnityEngine;
/// <summary>
/// Encodes a color while buffering hue and saturation values.
/// This is necessary since these values are singular for some 
/// colors like unsaturated grays and would lead to undesirable 
/// behaviour when moving sliders towards such colors.
/// </summary>
[Serializable]
public class BufferedColor {
    public Color color;
    private float bufferedHue;
    private float bufferedSaturation;

    public float r { get { return color.r; } }
    public float g { get { return color.g; } }
    public float b { get { return color.b; } }
    public float a { get { return color.a; } }
    public float h { get { return bufferedHue; } }
    public float s { get { return bufferedSaturation; } }
    public float v { get { return GUC.Colrs.RGBToHSV(color).z; } }


    public BufferedColor() {
        this.bufferedHue = 0f;
        this.bufferedSaturation = 0f;
        this.color = Color.black;
    }

    public BufferedColor(Color color) : this() {
        this.Set(color);
    }

    public BufferedColor(Color color, float hue, float sat) : this(color) {
        this.bufferedHue = hue;
        this.bufferedSaturation = sat;
    }

    public BufferedColor(Color color, BufferedColor source) :
        this(color, source.bufferedHue, source.bufferedSaturation) {
        this.Set(color);
    }

    public void Set(Color color) {
        this.Set(color, this.bufferedHue, this.bufferedSaturation);
    }

    public void Set(Color color, float bufferedHue, float bufferedSaturation) {
        this.color = color;
        Vector3 hsv = GUC.Colrs.RGBToHSV(color);

        bool hueSingularity = hsv.y == 0f || hsv.z == 0f;
        if(hueSingularity)
            this.bufferedHue = bufferedHue;
        else
            this.bufferedHue = hsv.x;

        bool saturationSingularity = hsv.z == 0f;
        if(saturationSingularity)
            this.bufferedSaturation = bufferedSaturation;
        else
            this.bufferedSaturation = hsv.y;
    }

    public BufferedColor PickR(float value) {
        Color toReturn = this.color;
        toReturn.r = value;
        return new BufferedColor(toReturn, this);
    }

    public BufferedColor PickG(float value) {
        Color toReturn = this.color;
        toReturn.g = value;
        return new BufferedColor(toReturn, this);
    }

    public BufferedColor PickB(float value) {
        Color toReturn = this.color;
        toReturn.b = value;
        return new BufferedColor(toReturn, this);
    }

    public BufferedColor PickA(float value) {
        Color toReturn = this.color;
        toReturn.a = value;
        return new BufferedColor(toReturn, this);
    }

    public BufferedColor PickH(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(value, hsv.y, hsv.z);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, value, bufferedSaturation);
    }

    public BufferedColor PickS(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(bufferedHue, value, hsv.z);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, bufferedHue, value);
    }

    public BufferedColor PickV(float value) {
        Vector3 hsv = GUC.Colrs.RGBToHSV(this.color);
        Color toReturn = GUC.Colrs.HSVToRGB(bufferedHue, bufferedSaturation, value);
        toReturn.a = this.color.a;
        return new BufferedColor(toReturn, bufferedHue, bufferedSaturation);
    }
}