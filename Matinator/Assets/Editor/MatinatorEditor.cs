using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Matinator))]
public class MatinatorEditor : Editor {

    public Object save;

    void OnEnable()
    {
        save = null;
    }

    public override void OnInspectorGUI()
    {
        save = EditorGUILayout.ObjectField("Animation", save, typeof(Object), false);
        if (save != null && GUILayout.Button("Modify Animation"))
            MatinatorWindow.OpenWindow();
        if (GUILayout.Button("Create Animation"))
            MatinatorWindow.OpenWindow();
    }
}
