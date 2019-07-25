using System.IO;
using UnityEngine;

namespace GUC {

    public static class Text {

        /// <summary>
        /// Returns string representing the given float, to precision number 
        /// of digits behind the '.'
        /// </summary>
        public static string FloatString(float x, int precision) {
            bool negative = x < 0f;
            x = Mathf.Abs(x);

            float pow = Mathf.Pow(10f, precision);
            int round = Mathf.RoundToInt(x * pow);
            if(round == 0)
                return "0";
            string toReturn = round.ToString();

            if(precision > 0)
                toReturn = toReturn.Insert(toReturn.Length - precision, ".");
            else if(precision < 0)
                toReturn = toReturn.PadRight(toReturn.Length + precision, '0');
            return (negative ? "-" : "") + toReturn;
        }


        /// <summary>
        /// Returns string representing the given number of seconds in an 
        /// hours:minutes:seconds.frac format where frac is a fractional value
        /// consisting of prec amount of digits.
        /// </summary>
        public static string GetTimeString(float seconds, int prec) {
            return GetTimeString(seconds, prec, seconds);
        }

        /// <summary>
        /// Returns string representing the given number of seconds in an 
        /// hours:minutes:seconds.frac format where frac is a fractional value
        /// consisting of prec amount of digits. The format will be the same as 
        /// it would be for the value refSeconds.
        /// </summary>
        public static string GetTimeString(float seconds, int prec, float refSeconds) {
            int secInt = Mathf.FloorToInt(seconds);
            int refSecInt = Mathf.FloorToInt(refSeconds);

            float frac = seconds % 1f;
            string fracString = frac.ToString("F" + prec);
            if(fracString.Length <= 2)
                fracString = ".".PadRight(prec + 1, '0');
            else
                fracString = fracString.Substring(1).PadRight(prec + 1, '0');
            return GetTimeString(secInt, refSecInt) + fracString;
        }

        /// <summary>
        /// Returns string representing the given number of seconds in an 
        /// hours:minutes:seconds format
        /// </summary>
        public static string GetTimeString(int seconds) {
            return GetTimeString(seconds, seconds);
        }

        /// <summary>
        /// Returns string representing the given number of seconds in an 
        /// hours:minutes:seconds format, making sure the format is the same 
        /// as that of refSeconds
        /// </summary>
        public static string GetTimeString(int seconds, int refSeconds) {
            if(refSeconds < 60)
                return seconds.ToString();
            int minutes = seconds / 60;
            seconds = seconds % 60;
            if(refSeconds < 3600)
                return minutes + ":" + seconds.ToString().PadLeft(2, '0');
            int hours = minutes / 60;
            minutes = minutes % 60;
            return hours + ":" + minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Returns the same string as the input, but with the first 
        /// character capitalized if possible.
        /// </summary>
        public static string Capitalize(string input) {
            if(string.IsNullOrEmpty(input))
                return input;
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }


        /// <summary>
        /// Returns a simple string that roughly encodes the given vector.
        /// </summary>
        public static string GetVecStr(Vector2 value) {
            return value.x.ToString("n2") + "_" + value.y.ToString("n2");
        }



        /// <summary>
        /// Returns string that represents given floating point value as well as possible.
        /// The string will consist of the given amount of charcaters at most.
        /// </summary>
        public static string GetShortFormat(float value, int charCount) {
            if(charCount <= 0)
                return "";

            if(charCount >= 16f)
                return value.ToString();

            float max = Mathf.Pow(10f, charCount);
            int decCount = Mathf.Max(0, charCount - 2);
            float min = Mathf.Pow(0.1f, decCount);

            if(value > max) {
                if(charCount < 4)
                    return max.ToString();
                int pwr = Mathf.FloorToInt(Mathf.Log10(value));
                value = Mathf.Round(value / Mathf.Pow(10f, pwr));
                return value + "e+" + pwr;
            }

            if(value < min) {
                if(charCount < 4)
                    return min.ToString();
                int pwr = -Mathf.FloorToInt(Mathf.Log10(value));
                value = Mathf.Round(value * Mathf.Pow(10f, pwr));
                return value + "e-" + pwr;

            }

            if(value > max / 100f)
                return Mathf.Round(value).ToString();
            return value.ToString("F" + (charCount - 2));
        }


        /// <summary>
        /// Trims away characters in the given path that are not legal for paths
        /// </summary>
        public static string TrimIllegalPathChars(string input) {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string toReturn = input;

            foreach(char c in invalid) {
                toReturn = toReturn.Replace(c.ToString(), "");
            }

            return toReturn;
        }
    }

}
