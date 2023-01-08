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
        GUILayout.Label("Data Getters", EditorStyles.boldLabel);
        if (GUILayout.Button("Get Position Data"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetPosition.php");
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Get Hits Data"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetHits.php");
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Get Deaths Data"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetDeaths.php");
        }
        GUILayout.Space(20f);
         GUILayout.Label("Clear", EditorStyles.boldLabel);
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
