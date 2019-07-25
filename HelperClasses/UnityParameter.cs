using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Encodes a parameter with a name and a flexible value type, 
/// such as floating point, integer, vector, quaternion etc.
/// </summary>
[Serializable]
public class UnityParameter {

    public string name;
    public byte[] value;

    public Type type;
    public enum Type {
        Byte, Float, Int, Bool, Vector3, Quaternion
    }

    /* To add a new type, say 'MyObject'
     * Add MyObject to the Type enum above
     * Add a new TypeGetSet called MyObject to the class below
     * Fill in the appropriate values and design a getter and setter 
     * that converts to and from byte data.
     */

    private class TypeGetSet {
        public static readonly TypeGetSet Byte = new TypeGetSet(
            1, typeof(byte), Type.Byte,
            delegate (byte[] value) { return value[0]; },
            delegate (object value) { return new byte[] { value is byte ? (byte)value : (byte)0x00 }; });

        public static readonly TypeGetSet Bool = new TypeGetSet( 
            1, typeof(bool), Type.Bool, 
            delegate (byte[] value) { return BitConverter.ToBoolean(value, 0); },
            delegate (object value) { return BitConverter.GetBytes((bool)value); });

        public static readonly TypeGetSet Float = new TypeGetSet( 
            4, typeof(float), Type.Float,
            delegate (byte[] value) { return BitConverter.ToSingle(value, 0); },
            delegate (object value) { return BitConverter.GetBytes((float)value); });

        public static readonly TypeGetSet Int = new TypeGetSet( 
            4, typeof(int), Type.Int,
            delegate (byte[] value) { return BitConverter.ToInt32(value, 0); },
            delegate (object value) { return BitConverter.GetBytes((int)value); });

        public static readonly TypeGetSet Vector3 = new TypeGetSet( 
            12, typeof(Vector3), Type.Vector3,
            delegate (byte[] value) { return GUC.Bits.GetVector3(value, 0); },
            delegate (object value) { return GUC.Bits.GetBytes((Vector3)value); });

        public static readonly TypeGetSet Quaternion = new TypeGetSet( 
            16, typeof(Quaternion), Type.Quaternion,
            delegate (byte[] value) { return GUC.Bits.GetQuaternion(value, 0); },
            delegate (object value) { return GUC.Bits.GetBytes((Quaternion)value); });

        public int byteLength;
        public System.Type systemType;
        public Type type;
        public Get get;
        public Set set;

        public delegate object Get(byte[] value);
        public delegate byte[] Set(object value);

        public TypeGetSet(int byteLength, System.Type systemType, Type type, Get get, Set set) {
            this.byteLength = byteLength;
            this.systemType = systemType;
            this.type = type;
            this.get = get;
            this.set = set;
        }
    }

    /* NOTE: get/set/type naming conventions are designed to be 
     * as unreadable and confusing as humanly possible. If you 
     * want to add more types you only need to be concerned with 
     * the code above.
     */

    private TypeGetSet GetSet(Type type) {
        System.Type st = typeof(TypeGetSet);
        string typeName = type.ToString();
        FieldInfo info = st.GetField(typeName);
        if(info == null) {
            Debug.LogWarning("Parameter type " + type + " does not have a valid UnityParameter implementation.");
            return TypeGetSet.Byte;
        }
        return (TypeGetSet)info.GetValue(null);
    }

    private TypeGetSet GetSet(System.Type systemType) {
        System.Type st = typeof(TypeGetSet);
        foreach(Type type in Enum.GetValues(typeof(Type))) {
            if(GetSet(type).systemType == systemType)
                return GetSet(type);
        }
        Debug.LogWarning("System type " + systemType + " does not have a valid UnityParameter implementation.");
        return TypeGetSet.Byte;
    }

    /// <summary>
    /// Return stored value
    /// </summary>
    public object Get() {
        if(!HasValue())
            return Activator.CreateInstance(GetSet(type).systemType);
        return GetSet(type).get(value);
    }

    /// <summary>
    /// Return stored value of given type
    /// </summary>
    public T Get<T>(){
        return (T)Get();
    }

    /// <summary>
    /// Set stored value, assuming that types match.
    /// </summary>
    public void Set(object value) {
        this.value = GetSet(type).set(value);
    }

    /// <summary>
    /// Set stored value while changing the parameter type if necessary
    /// </summary>
    public void Set<T>(T value) {
        this.type = GetSet(typeof(T)).type;
        this.value = GetSet(type).set(value);
    }

    private bool HasValue() {
        if(value == null)
            return false;
        return value.Length >= GetSet(type).byteLength;
    }

    /// <summary>
    /// Create empty byte parameter with empty name
    /// </summary>
    public UnityParameter() : this("") { }

    /// <summary>
    /// Create default value byte parameter with given name
    /// </summary>
    /// <param name="name"></param>
    public UnityParameter(string name) {
        this.name = name;
        this.type = Type.Byte;
        this.Set((byte)0x00);
    }

    /// <summary>
    /// Create default value parameter of given type
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    public UnityParameter(string name, Type type) : this(name) {
        this.type = type;
        this.value = null;
    }

    /// <summary>
    /// Create parameter encoding given value. 
    /// (Automatically deduces target type from given value)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public UnityParameter(string name, object value) : this(name) {
        this.type = GetSet(value.GetType()).type;
        Set(value);
    }

    /// <summary>
    /// Create parameter of given type, encoding given value.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public UnityParameter(string name, Type type, object value) : this(name) {
        this.type = type;
        Set(value);
    }
}
