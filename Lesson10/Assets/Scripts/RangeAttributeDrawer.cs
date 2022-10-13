using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(RangeAttribute))]
public class RangeAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position,SerializedProperty property,GUIContent label)
    {
        var range = (RangeAttribute)attribute;
        if (property.propertyType == SerializedPropertyType.Float)
        {
            EditorGUI.Slider(position, property, range.Min, range.Max, label);
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.IntSlider(position, property, (int)range.Min, (int)range.Max, label);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "USE ME INT OR FLOAT YES");
        }
    }
}
