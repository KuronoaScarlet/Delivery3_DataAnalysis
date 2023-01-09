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
    HeatMapType heatMap;
    Gradient gradient = new Gradient();
    float resolution;
    float transparency;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/HeatMaps")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AnalysisInspector));
    }

    void OnGUI()
    {
        EditorGUILayout.PrefixLabel("Attributes");

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2f));

        GUILayout.Label("Data Getters", EditorStyles.boldLabel);

        heatMap = (HeatMapType)EditorGUILayout.EnumPopup("Example Enum", heatMap);
        if(heatMap == HeatMapType.DEATH || heatMap == HeatMapType.HITS)
        {
            GUILayout.Space(10f);
            EditorGUILayout.GradientField(gradient);
            resolution = EditorGUILayout.Slider("Resolution", resolution, 25, 45);
            transparency = EditorGUILayout.Slider("Tranparency", transparency, 0.5f, 1f);
        }

        GUILayout.Space(10f);

        if (GUILayout.Button("Get " + heatMap.ToString().ToLower() +" Data"))
        {
            switch (heatMap)
            {
                case HeatMapType.HITS:
                    Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetHits.php", gradient,resolution,transparency);
                    break;
                case HeatMapType.DEATH:
                    Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetDeaths.php",gradient,resolution,transparency);
                    break;
                case HeatMapType.POSITION:
                    Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorStartHeatMap("GetPosition.php");
                    break;
            }
            
        }
        GUILayout.Space(10f);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2f));
        GUILayout.Label("Clear", EditorStyles.boldLabel);
        GUILayout.Space(10f);
        if (GUILayout.Button("Clear Heatmap"))
        {
            Resources.FindObjectsOfTypeAll<DataManager>()[0].GetComponent<DataManager>().EditorFinishHeatMap();
        }
    }
}
