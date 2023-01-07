using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum HeatMapType
{
    HITS,
    DEATH,
    POSITION,
}
public class AnalysisInspector : EditorWindow
{
    string myString = "Here goes php";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    DataManager[] heatMap;
    HeatMapType type;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/HeatMaps")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AnalysisInspector));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("PHP name", myString);
        //type = EditorGUILayout.field
        if (GUILayout.Button("Get Info from PHP"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap(myString);
        }
        if (GUILayout.Button("Clear Heatmap"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorFinishHeatMap();
        }
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}
