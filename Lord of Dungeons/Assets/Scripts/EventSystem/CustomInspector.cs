using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(GameEvent))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameEvent myGameEvent = (GameEvent)target;

        if (GUILayout.Button("Raise()"))
        {
            myGameEvent.Raise();
        }
    }
}
