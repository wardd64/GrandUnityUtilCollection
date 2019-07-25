using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnityParameterSet {

    public UnityParameter[] parameters;

    public UnityParameterSet(params UnityParameter[] parameters) {
        this.parameters = parameters;
    }

    public float GetFloat(string name) {
        return Get(name).Get<float>();
    }

    public int GetInt(string name) {
        return Get(name).Get<int>();
    }

    public bool GetBool(string name) {
        return Get(name).Get<bool>();
    }

    private UnityParameter Get(string name) {
        foreach(UnityParameter p in parameters) {
            if(p.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return p;
        }
        Debug.LogWarning("Game Parameter with name " + name + " does not exist in this set.");
        return new UnityParameter("Null");
    }
}
