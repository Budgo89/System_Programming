using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(Star)), CanEditMultipleObjects]
public class StarEditor : Editor
{
    private SerializedProperty _center;
    private SerializedProperty _points;
    private SerializedProperty _frequency;

    private ReorderableList _reorderableList;

    private void OnEnable()
    {
        _center = serializedObject.FindProperty("_center");
        _points = serializedObject.FindProperty("_points");
        _frequency = serializedObject.FindProperty("_frequency");

        _reorderableList = new ReorderableList(serializedObject, _points, true, true, true, true)
        {
            drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(rect, element.displayName);
                rect.x += rect.width / 1.5f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("Color"), GUIContent.none);
            },
            drawHeaderCallback = rect =>
            {
                name = "Points";
                EditorGUI.LabelField(rect, name);
            }
        };
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _reorderableList.DoLayoutList();
        EditorGUILayout.PropertyField(_center);
        EditorGUILayout.PropertyField(_points);
        EditorGUILayout.IntSlider(_frequency, 1, 20);

        var totalPoints = _frequency.intValue * _points.arraySize;
        if (totalPoints < 3)
        {
            EditorGUILayout.HelpBox("At least three points are needed.",
            MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox(totalPoints + " points in total.",
            MessageType.Info);
        }
        serializedObject.ApplyModifiedProperties();
        
    }
    
    
    private void OnSceneGUI()
    {
        if (!(target is Star star))
        {
            return;
        }

        var starTransform = star.transform;
        var angle = -360f / (star.frequency * star.points.Length);

        for (var i = 0; i < star.points.Length; i++)
        {
            var rotation = Quaternion.Euler(0f, 0f, angle * i);
            var oldPoint = starTransform.TransformPoint(rotation * star.points[i].Position);
            Vector3 snap = Vector3.one * 0.5f;
            var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, 0.02f, snap, Handles.DotHandleCap);
            if (oldPoint == newPoint)
            {
                continue;
            }
            star.points[i].Position = Quaternion.Inverse(rotation) * starTransform.InverseTransformPoint(newPoint);
            
        }

        star.UpdateMesh();
    }
    
}
