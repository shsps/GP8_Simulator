using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StepInfoGenerator))]
public class StepInfoGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if(GUILayout.Button("Generate"))
        {
            StepInfoGenerator.StepInfoWriter();
        }
    }
}
