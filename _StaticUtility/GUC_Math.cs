using UnityEngine;

namespace GUC {

    public static class Math {

        /// <summary>
        /// Specialized repeat function that better handles exceptional cases
        /// </summary>
        public static float Repeat(float x, float d) {
            if(d == 0f)
                return 0f;
            d = Mathf.Abs(d);

            float r = x % d;
            if(r < 0f)
                return r + d;
            return r;
        }

        /// <summary>
        /// Specialized repeat function that better handles exceptional cases
        /// </summary>
        public static int Repeat(int x, int d) {
            if(d == 0)
                return 0;
            d = Mathf.Abs(d);

            int r = x % d;
            if(r < 0)
                return r + d;
            return r;
        }

        /// <summary>
        /// Specialized 2 sided exponential function, returns
        /// sgn(x) * exp(|x| - 1) for values |x| > 1
        /// and returns smooth antisymetric values in between.
        /// </summary>
        public static float BiExp(float x) {
            return BiExp(x, Mathf.Exp(1f));
        }

        /// <summary>
        /// Specialized 2 sided exponential function, returns
        /// sgn(x) * baseN ^ (|x| - 1) for values |x| > 1
        /// and returns smooth antisymetric values in between.
        /// </summary>
        public static float BiExp(float x, float baseN) {
            if(x > 1f)
                return Mathf.Pow(baseN, x - 1f);
            else if(x < -1f)
                return -Mathf.Pow(baseN, -x - 1f);
            else {
                return x;
            }
        }

        /// <summary>
        /// Specialized 2 sided log function, complementary to BiExp.
        /// </summary>
        public static float BiLog(float x) {
            return BiLog(x, Mathf.Exp(1f));
        }

        /// <summary>
        /// Specialized 2 sided log function, complementary to BiExp.
        /// </summary>
        public static float BiLog(float x, float baseN) {
            if(x > 1f)
                return 1f + Mathf.Log(x, baseN);
            else if(x < -1f)
                return -(1f + Mathf.Log(-x, baseN));
            else {
                return x;
            }
        }

        /// <summary>
        /// Easy integer exponents
        /// </summary>
        public static float Pow(float baseN, int exp) {
            float toReturn = 1f;
            if(exp > 0) {
                for(int i = 0; i <= exp; i++) {
                    toReturn *= baseN;
                }
            }
            else if(exp < 0) {
                for(int i = 0; i <= exp; i++) {
                    toReturn /= baseN;
                }
            }
            return baseN;
        }

        /// <summary>
        /// Returns given value, rounded towards zero.
        /// </summary>
        public static float Truncate(float x) {
            if(x >= 0f)
                return Mathf.Floor(x);
            return Mathf.Ceil(x);
        }

        /// <summary>
        /// Returns given value, rounded away from zero.
        /// </summary>
        public static float AntiTruncate(float x) {
            if(x >= 0f)
                return Mathf.Ceil(x);
            return Mathf.Floor(x);
        }

        /// <summary>
        /// Clamps angle in given range, between -180 and 180 degrees, 
        /// while preventing periodic shenanigans
        /// </summary>
        public static float ClampAngle(float angle, float from, float to) {
            angle = Mathf.Repeat(angle, 360f);
            if(angle > 180f)
                angle = angle - 360f;

            angle = Mathf.Clamp(angle, from, to);

            return angle;
        }

        /// <summary>
        /// Convert SquaredCircle coordinates to the regular normalized format.
        /// </summary>
        /// <param name="angle">Angle in degrees relative to the center [.5, .5]</param>
        /// <param name="radius">Normalized distance along the center: 0 corresponds to the center at [.5, .5], 1 corresponds to the edges of the unit square.</param>
        public static Vector2 UnitSquareToNormalized(float angle, float radius) {
            angle *= Mathf.Deg2Rad;
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);
            float radiusScale = .5f / Mathf.Max(Mathf.Abs(s), Mathf.Abs(c));
            return (radius * radiusScale * new Vector2(c, s)).Shift(.5f);
        }

        /// <summary>
        /// Converts given multiplicative factor to its equivalent 
        /// decibel value. e.g. a factor of 100 yields 40dB, 
        /// a factor .1 yields -20dB 
        /// </summary>
        public static float FactorToDb(float input) {
            return Mathf.Log10(input) * 20f;
        }

        /// <summary>
        /// Converts given decibel value to a multiplicative factor.
        /// E.g. 40dB yields 100, -20dB yields 0.1.
        /// </summary>
        public static float DbToFactor(float input) {
            return Mathf.Pow(10f, input / 20f);
        }

    }

}
