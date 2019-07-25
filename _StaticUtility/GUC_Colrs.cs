using UnityEngine;

namespace GUC {

    public static class Colrs {

        /// <summary>
        /// Returns the same color, but with given alpha value
        /// </summary>
        public static Color SetAlpha(this Color source, float alpha) {
            return new Color(source.r, source.g, source.b, alpha);
        }

        /// Get color from hue, saturation, value format
        /// </summary>
        /// <param name="hsv">Vector containing h, s and v values.</param>
        public static Color HSVToRGB(Vector3 hsv) {
            return HSVToRGB(hsv.x, hsv.y, hsv.z);
        }

        /// <summary>
        /// Get color from hue, saturation, value format
        /// </summary>
        /// <param name="h">hue value, ranging from 0 to 6; red to red</param>
        /// <param name="s">saturation, 0 to 1; gray to colored</param>
        /// <param name="v">value, 0 to 1; black to white</param>
        public static Color HSVToRGB(float h, float s, float v) {
            float c = s * v;
            float m = v - c;
            float x = c * (1f - Mathf.Abs(h % 2f - 1f)) + m;
            c += m;

            int range = Mathf.FloorToInt(h % 6f);

            switch(range) {
            case 0:
            return new Color(c, x, m);
            case 1:
            return new Color(x, c, m);
            case 2:
            return new Color(m, c, x);
            case 3:
            return new Color(m, x, c);
            case 4:
            return new Color(x, m, c);
            case 5:
            return new Color(c, m, x);
            default:
            return Color.black;
            }
        }

        /// <summary>
        /// Get hue, saturation and value of a color.
        /// Complementary to HSVToRGB
        /// </summary>
        public static Vector3 RGBToHSV(Color color) {
            float r = color.r;
            float g = color.g;
            float b = color.b;
            return RGBToHSV(r, g, b);
        }

        /// <summary>
        /// Get hue, saturation and value of a color.
        /// Complementary to HSVToRGB
        /// </summary>
        public static Vector3 RGBToHSV(float r, float g, float b) {
            float cMax = Mathf.Max(r, g, b);
            float cMin = Mathf.Min(r, g, b);
            float delta = cMax - cMin;
            float h = 0f;
            if(delta > 0f) {
                if(r >= b && r >= g)
                    h = Mathf.Repeat((g - b) / delta, 6f);
                else if(g >= r && g >= b)
                    h = (b - r) / delta + 2f;
                else if(b >= r && b >= g)
                    h = (r - g) / delta + 4f;
            }
            float s = cMax == 0f ? 0f : delta / cMax;
            float v = cMax;
            return new Vector3(h, s, v);
        }

        /// <summary>
        /// Moves color from start to target, by at most the given amount of delta.
        /// if delta is 1; it takes 1 second to move from say; black to red.
        /// </summary>
        public static Color MoveTowards(Color startColor, Color targetColor, float delta) {
            Vector4 vecStart = new Vector4(startColor.r, startColor.g, startColor.b, startColor.a);
            Vector4 vecTarget = new Vector4(targetColor.r, targetColor.g, targetColor.b, targetColor.a);
            Vector4 vecReturn = Vector4.MoveTowards(vecStart, vecTarget, delta);
            return new Color(vecReturn.x, vecReturn.y, vecReturn.z, vecReturn.w);
        }

        /// <summary>
        /// Returns a simple color gradient that progresses from the given startColor to
        /// the given endColor. This effect includes alpha values.
        /// </summary>
        public static Gradient GetLinearGradient(Color startColor, Color endColor) {
            Gradient toReturn = new Gradient();
            GradientAlphaKey startAlpha = new GradientAlphaKey(startColor.a, 0f);
            GradientAlphaKey endAlpha = new GradientAlphaKey(startColor.a, 1f);
            GradientColorKey startKey = new GradientColorKey(startColor, 0f);
            GradientColorKey endKey = new GradientColorKey(endColor, 1f);
            toReturn.alphaKeys = new GradientAlphaKey[] { startAlpha, endAlpha };
            toReturn.colorKeys = new GradientColorKey[] { startKey, endKey };
            return toReturn;
        }


    }

}
