using UnityEngine;
using System.Collections;
using System.Reflection;
using  System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class Key {

    public enum KeyType
    {
        E_POS_X = 0, E_POS_Y = 1, E_POS_Z = 2,
        E_ROT_X = 3, E_ROT_Y = 4, E_ROT_Z = 5,
        E_SCALE_X = 6, E_SCALE_Y = 7, E_SCALE_Z = 8,
        E_EVENT
    };

    public Key()
    {
    }

    public Vector2 pos;
    public float time;
    public float value;
    public KeyType type;
    public int nbCase;
    public float pourcentage;
    public float nbTime;
    public int nbTtlCase;

    public MethodInfo function;
    public List<string> parameterList;
    public System.Object[] parameterArray;
    public bool eventTrigger;

    public void SetValues(Vector2 _pos, float _time, float _value, KeyType _type, int _nbCase, float _pourcentage, int _nbTime)
    {
        pos = _pos;
        time = _time;
        value = _value;
        type = _type;
        nbCase = _nbCase;
        pourcentage = _pourcentage;
        nbTime = _nbTime;
        eventTrigger = false;
    }

    public void AddEvent(MethodInfo _function, List<string> _parameterList)
    {
        Debug.Log("Add event is called");
        function = _function;
        parameterList = _parameterList;
        parameterArray = new System.Object[parameterList.Count];
        ParameterInfo[] pi = function.GetParameters();
        for (int i = 0; i < parameterList.Count; ++i)
        {
                System.Type t = pi[i].ParameterType;
                if (t == typeof(bool))
                    AddToArrayObject(bool.Parse(parameterList[i]), i);
                else if (t == typeof(string))
                    AddToArrayObject(parameterList[i], i);
                else if (t == typeof(int))
                    AddToArrayObject(int.Parse(parameterList[i]), i);
                else if (t == typeof(float) || t == typeof(double))
                    AddToArrayObject(float.Parse(parameterList[i]), i);
            Debug.Log(parameterArray[i]);
        }
        
    }

    private void AddToArrayObject(System.Object obj, int i)
    {
        parameterArray[i] = obj;
    }
}
