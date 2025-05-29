using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ChildGeneratror))]
[CanEditMultipleObjects]
public class ChildGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // or DrawDefaultInsepctor() ? 

        ChildGeneratror script = (ChildGeneratror)target;
        if (GUILayout.Button("GenerateChild"))
        {
            script.Start();
            script.GenerateChild();
        }
    }
}
