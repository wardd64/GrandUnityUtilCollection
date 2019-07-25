using System;
using System.Text;
using UnityEngine;

namespace GUC {

    public static class Bits {

        /// <summary>
        /// Starts reading string encoded in the given bytes until we hit a null byte.
        /// Also moves the pointer passed this null terminator
        /// </summary>
        public static string ReadNullTerminatedString(byte[] bytes, ref int pointer) {
            StringBuilder toReturn = new StringBuilder();
            char nextChar = Encoding.UTF8.GetString(bytes, pointer++, 1).ToCharArray()[0];
            while(nextChar != '\0') {
                toReturn.Append(nextChar);
                nextChar = Encoding.UTF8.GetString(bytes, pointer++, 1).ToCharArray()[0];
            }
            return toReturn.ToString();
        }

        /// <summary>
        /// Starts reading string encoded in the given bytes until we hit a null byte.
        /// </summary>
        public static string ReadNullTerminatedString(byte[] bytes, int pointer) {
            return ReadNullTerminatedString(bytes, ref pointer);
        }

        /// <summary>
        /// Reads and returns string with 2 byte number denoting its length.
        /// Moves the pointer to the first byte after the string.
        /// </summary>
        public static string ReadLengthHeaderString(byte[] bytes, ref int pointer) {
            short length = BitConverter.ToInt16(bytes, pointer);
            pointer += 2;
            string toReturn = Encoding.UTF8.GetString(bytes, pointer, length);
            pointer += length;
            return toReturn;
        }

        /// <summary>
        /// Reads and returns string with 2 byte number denoting its length.
        /// </summary>
        public static string ReadLengthHeaderString(byte[] bytes, int pointer) {
            return ReadLengthHeaderString(bytes, pointer);
        }

        /// <summary>
        /// Reads 3 bytes and returns the color they would encode (with max alpha)
        /// </summary>
        public static Color GetRGBColor(byte[] bytes, int start) {
            float f = 1f / 255;
            float r = f * bytes[start];
            float g = f * bytes[start + 1];
            float b = f * bytes[start + 2];
            return new Color(r, g, b);
        }

        /// <summary>
        /// Reads 4 bytes and returns the color they would encode (including alpha value)
        /// </summary>
        public static Color GetRGBAColor(byte[] bytes, int start) {
            float f = 1f / 255;
            float r = f * bytes[start];
            float g = f * bytes[start + 1];
            float b = f * bytes[start + 2];
            float a = f * bytes[start + 3];
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Reads 4 consecutive floats and returns them inside a vector.
        /// </summary>
        public static Vector4 GetVector4(byte[] bytes, int start) {
            float x = BitConverter.ToSingle(bytes, start);
            float y = BitConverter.ToSingle(bytes, start + 4);
            float z = BitConverter.ToSingle(bytes, start + 8);
            float w = BitConverter.ToSingle(bytes, start + 12);
            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Reads 3 consecutive floats and returns them inside a vector.
        /// </summary>
        public static Vector3 GetVector3(byte[] bytes, int start) {
            float x = BitConverter.ToSingle(bytes, start);
            float y = BitConverter.ToSingle(bytes, start + 4);
            float z = BitConverter.ToSingle(bytes, start + 8);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Reads 2 consecutive floats and returns them inside a vector.
        /// </summary>
        public static Vector3 GetVector2(byte[] bytes, int start) {
            float x = BitConverter.ToSingle(bytes, start);
            float y = BitConverter.ToSingle(bytes, start + 4);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Read 4 consecutive floats and returns them inside a Quaternion
        /// </summary>
        public static Quaternion GetQuaternion(byte[] bytes, int start) {
            float x = BitConverter.ToSingle(bytes, start);
            float y = BitConverter.ToSingle(bytes, start + 4);
            float z = BitConverter.ToSingle(bytes, start + 8);
            float w = BitConverter.ToSingle(bytes, start + 12);
            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// True if the given bytes can be reasonably expected to encode
        /// an index for an array with the size 'max'
        /// </summary>
        public static bool IsPlausibleIndex(byte[] bytes, int start, int max) {
            int value = BitConverter.ToInt32(bytes, start);
            return value >= 0 && value < max;
        }

        /// <summary>
        /// True if the given bytes can be reasonably expected to encode
        /// a floating point value such as a coordinate or matrix element.
        /// </summary>
        public static bool IsPlausibleFloat(byte[] bytes, int start) {
            float value = Mathf.Abs(BitConverter.ToSingle(bytes, start));
            return value == 0f || (value > 1e-10f && value < 1e+10f);
        }

        /// <summary>
        /// Return hexadecimal form of the given number
        /// </summary>
        public static string GetHex(int value) {
            return value.ToString("X");
        }

        /// <summary>
        /// Return hexadecimal form of the given number
        /// </summary>
        public static string GetHex(float value) {
            return value.ToString("X");
        }

        /// <summary>
        /// Return given byte as a string containing its binary form
        /// </summary>
        public static string GetBinary(byte[] bytes, int pointer) {
            return Convert.ToString(bytes[pointer], 2).PadLeft(8, '0');
        }

        /// <summary>
        /// Return given length of bytes as a string containg hexadecimal
        /// </summary>
        public static string GetHex(byte[] bytes, int start, int length) {
            StringBuilder toReturn = new StringBuilder();
            for(int i = 0; i < length; i++)
                toReturn.Append(BitConverter.ToString(bytes, start + i, 1));
            return toReturn.ToString();
        }

        /// <summary>
        /// Return value of a specific bit
        /// </summary>
        public static bool GetFlag(byte[] bytes, int pointer, int bit) {
            byte flags = bytes[pointer];
            byte match = (byte)(1 << bit);
            return (flags & match) != 0;
        }

        /// <summary>
        /// Return 4-bit integer encoded as a half of a byte
        /// </summary>
        public static byte GetNibble(byte[] bytes, int pointer, bool first) {
            byte value = bytes[pointer];

            if(first)
                return (byte)(value % 16);
            else
                return (byte)(value / 16);
        }

        /// <summary>
        /// Returns true if given byte encodes a readable character
        /// </summary>
        public static bool IsReadable(byte b) {
            return b >= 32;
        }

        /// <summary>
        /// Returns true only if all bytes in the given range encode reabable characters
        /// </summary>
        public static bool IsReadable(byte[] bytes, int start, int length) {
            for(int i = start; i < start + length; i++) {
                if(!IsReadable(bytes[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get byte form of string in a Length Header format
        /// </summary>
        public static byte[] GetBytes(string value) {
            return GetBytesLengthHeader(value);
        }

        public static byte[] GetBytesNullTerminated(string value) {
            byte[] toReturn = new byte[value.Length + 1];
            WriteBytes(toReturn, Encoding.UTF8.GetBytes(value), 0);
            return toReturn;
        }

        public static byte[] GetBytesLengthHeader(string value) {
            byte[] toReturn = new byte[value.Length + 2];
            byte[] lengthHeader = BitConverter.GetBytes((short)value.Length);
            WriteBytes(toReturn, lengthHeader, 0);
            WriteBytes(toReturn, Encoding.UTF8.GetBytes(value), 2);
            return toReturn;
        }

        /// <summary>
        /// Get byte form of this color in RGB bytes format
        /// </summary>
        public static byte[] GetBytes(Color value) {
            return GetBytesRGB(value);
        }

        public static byte[] GetBytesRGB(Color value) {
            byte[] toReturn = new byte[3];
            for(int i = 0; i < 3; i++)
                toReturn[i] = (byte)(value[i] * 255);
            return toReturn;
        }

        public static byte[] GetBytesRGBA(Color value) {
            byte[] toReturn = new byte[4];
            for(int i = 0; i < 4; i++)
                toReturn[i] = (byte)(value[i] * 255);
            return toReturn;
        }

        public static byte[] GetBytes(Vector2 value) {
            byte[] toReturn = new byte[8];
            for(int i = 0; i < 2; i++)
                WriteBytes(toReturn, BitConverter.GetBytes(value[i]), i * 4);
            return toReturn;
        }


        public static byte[] GetBytes(Vector3 value) {
            byte[] toReturn = new byte[12];
            for(int i = 0; i < 3; i++)
                WriteBytes(toReturn, BitConverter.GetBytes(value[i]), i * 4);
            return toReturn;
        }


        public static byte[] GetBytes(Vector4 value) {
            byte[] toReturn = new byte[16];
            for(int i = 0; i < 4; i++)
                WriteBytes(toReturn, BitConverter.GetBytes(value[i]), i * 4);
            return toReturn;
        }

        public static byte[] GetBytes(Quaternion value) {
            byte[] toReturn = new byte[16];
            for(int i = 0; i < 4; i++)
                WriteBytes(toReturn, BitConverter.GetBytes(value[i]), i * 4);
            return toReturn;
        }



        /// <summary>
        /// Write all data from 'bytes' to 'array'
        /// </summary>
        /// <param name="array">Target array to write to</param>
        /// <param name="bytes">Source bites to be written</param>
        /// <param name="writeOffset">The starting index in the target array at which bytes should be written</param>
        public static void WriteBytes(byte[] array, byte[] bytes, int writeOffset) {
            WriteBytes(array, bytes, writeOffset, 0, bytes.Length);
        }

        /// <summary>
        /// Write a length of data from 'bytes' to 'array'
        /// </summary>
        /// <param name="array">Target array to write to</param>
        /// <param name="bytes">Source bites to be written</param>
        /// <param name="writeOffset">The starting index in the target array at which bytes should be written</param>
        /// <param name="readOffset">The starting index in source bytes from which reading should start</param>
        /// <param name="length">The number of bites to write</param>
        public static void WriteBytes(byte[] array, byte[] bytes, int writeOffset, int readOffset, int length) {
            for(int i = 0; i < length; i++)
                array[writeOffset + i] = bytes[readOffset + i];
        }

    }
}
