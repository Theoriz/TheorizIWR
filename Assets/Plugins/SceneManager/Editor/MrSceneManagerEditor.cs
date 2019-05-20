using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MrSceneManager))]
public class MrSceneManagerEditor : Editor {

    private MrSceneManager sceneManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (sceneManager == null)
        {
            sceneManager = target as MrSceneManager;
        }

        if (GUILayout.Button("Next"))
        {
            sceneManager.Next();
        }

        if (GUILayout.Button("Previous"))
        {
            sceneManager.Previous();
        }
    }
}
