using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TestWin : EditorWindow
{
    // Start is called before the first frame update
    private MainGeneration Main;
    private Editor MainWindow;
    [MenuItem("Window/MainGenerator")]
    public static void Show()// которая вызовет наше окно
    {
        EditorWindow.GetWindow<TestWin>();

    }
    public void OnGUI()
    {
        Main = (MainGeneration)EditorGUILayout.ObjectField(Main,
            typeof(MainGeneration));
        if (Main != null)
        {
            MainWindow??= Editor.CreateEditor(Main);
            MainWindow.OnInspectorGUI();
        }
    }
}
