using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PonyColorManager))]
[CanEditMultipleObjects]
public class PonyColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // or DrawDefaultInsepctor() ? 

        PonyColorManager script = (PonyColorManager)target;
        if (GUILayout.Button("ApplyNewColors"))
        {
            script.Start();
            script.Update();
        }
    }
}
