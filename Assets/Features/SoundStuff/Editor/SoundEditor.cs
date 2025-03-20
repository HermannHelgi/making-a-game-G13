using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SoundManager))]
public class SoundEditor : Editor
{   
    private void OnSceneGUI
    {
        SoundManager soundManager = (SoundManager)target;
        Handles.color = Color.white';

    }
}
