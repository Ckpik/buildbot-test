using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.Reflection;

public class EventWindow : ScriptableWizard {

    private GameObject obj;
    static private List<MethodInfo> methods;
    static private bool canValidate;

    static public List<string> listParameter;

    private bool editing = false;
    static private Rect Box;
    static private MethodInfo selectedItem;
    static private Vector2 scrollValue;

    static Key currentKey;

    static public void CreateWizard(Key key)
    {
        ScriptableWizard.DisplayWizard<EventWindow>("Chose your event", "Create", "Apply");
        methods = new List<MethodInfo>();
        GameObject[] allObjects = FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy)
            {
                Component[] compo = obj.GetComponents<Component>();
                foreach (Component c in compo)
                {
                     GetMethodsFromClass(c.GetType());
                }
            }
        }
        currentKey = key;
        if (currentKey.function == null && methods.Count > 0)
            selectedItem = methods[0];
        else
        {
            selectedItem = currentKey.function;
            listParameter = currentKey.parameterList;
        }
        canValidate = true;
        scrollValue = Vector2.zero;
    }

    private Vector2 scrollPos = new Vector2(0, 0);

    void OnGUI()
    {
        if (methods.Count == 0)
            return;
        if (listParameter == null)
            CreateParameterList();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUIComboBox();
        if (editing == false)
        {
            DisplayParametersField();
            if (GUILayout.Button("Save"))
                SaveChangement();
            if (GUILayout.Button("Cancel"))
                CancelChangement();
            if (GUILayout.Button("Reset"))
                ResetEvent();
        }
        EditorGUILayout.EndScrollView();
    }

    void GUIComboBox()
    {
        if (GUILayout.Button(PrintMethod(selectedItem)))
        {
            editing = !editing;
        }

        if (editing)
        {
            for (int x = 0; x < methods.Count; x++)
            {
                if (GUILayout.Button(PrintMethod(methods[x])))
                {
                    selectedItem = methods[x];
                    editing = false;
                    CreateParameterList();
                    canValidate = true;
                }
            }
        }
    }

    void DisplayParametersField()
    {
        ParameterInfo[] parameters = selectedItem.GetParameters();
        int i = 0;
        foreach (ParameterInfo pi in parameters)
        {
            System.Type t = pi.ParameterType;
            if (t != typeof(bool) && t != typeof(float) &&
                t != typeof(int) && t != typeof(double) &&
                t != typeof(string))
            {
                GUILayout.Label("You can't add this event. Your event must not have parameters or parameters with basic type (bool, int, float, string)");
                canValidate = false;
                break;
            }
            else if (t == typeof(bool))
            {
                GUILayout.Label(pi.Name);
                listParameter[i] = (GUILayout.Toggle(listParameter[i] == "True" ? true : false, "")).ToString();
            }
            else if (t == typeof(string))
            {
                GUILayout.Label(pi.Name);
                listParameter[i] = GUILayout.TextArea(listParameter[i]);
            }
            else if (t == typeof(int))
            {
                GUILayout.Label(pi.Name);
                listParameter[i] = (EditorGUILayout.IntField(int.Parse(listParameter[i]))).ToString();
            }
            else if (t == typeof(float) || t == typeof(double))
            {
                GUILayout.Label(pi.Name);
                listParameter[i] = (EditorGUILayout.FloatField(float.Parse(listParameter[i]))).ToString();
            }
            ++i;
        }
    }

    void CreateParameterList()
    {
        ParameterInfo[] parameters = selectedItem.GetParameters();
        listParameter = new List<string>();
        foreach (ParameterInfo pi in parameters)
        {
            System.Type t = pi.ParameterType;
            if (t == typeof(bool))
                listParameter.Add(false.ToString());
            else if (t == typeof(string))
                listParameter.Add(string.Empty);
            else if (t == typeof(int))
                listParameter.Add(0.ToString());
            else if (t == typeof(float) || t == typeof(double))
                listParameter.Add(0.0f.ToString());
        }
    }

    string PrintMethod(MethodInfo mi)
    {
        var parameterDescriptions = string.Join
            (", ", mi.GetParameters()
                         .Select(x => x.ParameterType + " " + x.Name)
                         .ToArray());
        return mi.ReturnType + "   " + mi.Name + "(" + parameterDescriptions + ")";
    }

    void SaveChangement()
    {
        if (canValidate)
        {
            currentKey.AddEvent(selectedItem, listParameter);
            Close();
        }
    }

    void CancelChangement()
    {
        Close();
    }

    void ResetEvent()
    {
        selectedItem = methods[0];
        listParameter = new List<string>();
        CreateParameterList();
    }

    static void GetMethodsFromClass(System.Type type)
    {
        BindingFlags bf = BindingFlags.Static | BindingFlags.Public | BindingFlags.SuppressChangeType;
        foreach (var method in type.GetMethods(bf))
        {
            /* This should be done by reflection but *
             * I didn't find the great flags ....    */

            string toCompare = method.Name.Substring(0, 3);
            if (string.Compare(toCompare, "get", true) == 0 ||
                string.Compare(toCompare, "set", true) == 0)
                continue;
            methods.Add(method);
        }
    }
}
