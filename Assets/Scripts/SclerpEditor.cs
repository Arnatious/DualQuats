using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sclerp))]
[CanEditMultipleObjects]
public class SclerpEditor : Editor
{
    SerializedProperty timeStamps;
    SerializedProperty waypoints;

    void OnEnable()
    {
        timeStamps = serializedObject.FindProperty("TimeStamps");
        waypoints = serializedObject.FindProperty("Targets");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if ((target as Sclerp).Targets != null)
        {
            int numKeyframes = (target as Sclerp).Targets.childCount;
            timeStamps.arraySize = Mathf.Max(1, numKeyframes);
        }
        else
        {
            timeStamps.arraySize = 0;
        }
        serializedObject.ApplyModifiedProperties();
        

    }

}
