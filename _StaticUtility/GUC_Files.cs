using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GUC {

    public static class Files {


        /// <summary>
        /// Compresses given object and writes it to the given file path.
        /// </summary>
        public static void CompressAndSave(string path, object obj) {
            FileStream fs = new FileStream(path, FileMode.Create);

            //serialize the object
            MemoryStream objectSerialization = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(objectSerialization, obj);

            //compressed the serialized object
            byte[] compressed = Compr.Compress(objectSerialization.GetBuffer());

            //write compressed object (length first)
            BinaryWriter binaryWriter = new BinaryWriter(fs);
            binaryWriter.Write(compressed.Length);
            binaryWriter.Write(compressed);

            //and close everything
            binaryWriter.Close();
            objectSerialization.Close();
            fs.Close();
        }

        /// <summary>
        /// Decompresses object saved at the given file path and returns it.
        /// </summary>
        public static object LoadAndDecompress(string path) {
            FileStream fs = new FileStream(path, FileMode.Open);

            //read compressed object (length first)
            BinaryReader binaryReader = new BinaryReader(fs);
            int length = binaryReader.ReadInt32();
            byte[] compressed = binaryReader.ReadBytes(length);

            //decompress it
            byte[] bytesUncompressed = Compr.Decompress(compressed);

            //deserialize the decompressed object
            MemoryStream objectDeSerialization = new MemoryStream(bytesUncompressed);
            BinaryFormatter bformatter = new BinaryFormatter();
            object toReturn = bformatter.Deserialize(objectDeSerialization);

            //close everything
            objectDeSerialization.Close();
            binaryReader.Close();
            fs.Close();

            //return result
            return toReturn;
        }

        /// <summary>
        /// Write given object to the given file path (uncompressed).
        /// </summary>
        public static void Save(string path, object obj) {
            if(string.IsNullOrEmpty(path))
                throw new System.ArgumentException("Trying to save to undefined path");
            else if(obj == null)
                throw new System.ArgumentNullException("Trying to save null object to " + path);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, obj);
            file.Close();
        }

        /// <summary>
        /// Reads (uncompressed) object at the given file path and returns it.
        /// Returns null if file could not be loaded for whatever reason.
        /// </summary>
        public static object Load(string path) {
            FileStream file = null;
            object toReturn = null;
            try {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Open(path, FileMode.Open);
                toReturn = bf.Deserialize(file);
            }
            catch(System.Exception) { }

            if(file != null)
                file.Close();
            return toReturn;

        }

        /// <summary>
        /// Returns true if a file exists at the given path, and it matches the given type.
        /// </summary>
        public static bool Exists<T>(string path) {
            if(!File.Exists(path))
                return false;

            object o = Load(path);
            return o != null && o is T;
        }

        /// <summary>
        /// True if given string is an absolute path that leads into the assets folder.
        /// </summary>
        public static bool IsAssetPath(string path) {
            return path.StartsWith(Application.dataPath);
        }

        /// <summary>
        /// Converts given absolute path to a relative path, starting with Assets/
        /// </summary>
        public static string GetRelativeUnityPath(string path) {
            return "Assets" + path.Substring(Application.dataPath.Length);
        }

        /// <summary>
        /// Converts given relative path (inside assets folder) to an absolute path
        /// </summary>
        public static string GetAbsoluteUnityPath(string path) {
            if(path.StartsWith("Assets"))
                return Application.dataPath + path.Substring(6);
            return Application.dataPath + "/" + path;

        }
    }
}
