using Main;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class StarWindowEditor : EditorWindow
{
    private int _countCrystal;
    private int _min = 50;
    private int _max = 100;
    private Camera _cam;
    private RenderTexture _rt;

    [MenuItem("Window/Star Window Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StarWindowEditor));
        
    }
    private void OnGUI()
    {
        _cam = FindObjectOfType<MiniMap>().Camera;
        GUILayout.Label("Настройка кристалов", EditorStyles.boldLabel);
        _countCrystal = (int)EditorGUILayout.Slider("Количество кристалов", _countCrystal, 1, 10);
        SolarSystemNetworkManager SSNM = FindObjectOfType<SolarSystemNetworkManager>();
        SSNM._countCrystal = _countCrystal;

        _min = (int)EditorGUILayout.Slider("Минимальное расстояние появление кристаллов", _min, 10, _max);
        SSNM.min = _min;
        _max = (int)EditorGUILayout.Slider("Максимальное расстояние появление кристаллов", _max, _min, 1000);
        SSNM.max = _max;

        
        //_rt = EditorGUILayout.
    }
}
