using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

[CustomEditor(typeof(SoundManager))]
public class SoundEditor : Editor
{
    private Dictionary<string, bool> headerFoldouts = new();
    private Dictionary<string, bool> audioSourceFoldouts = new();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SoundManager soundManager = (SoundManager)target;
        FieldInfo[] fields = typeof(SoundManager).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        string currentHeader = "";

        foreach (FieldInfo field in fields)
        {   
            // Detect new headers and create foldouts
            HeaderAttribute headerAttr = field.GetCustomAttribute<HeaderAttribute>();
            if (headerAttr != null)
            {
                currentHeader = headerAttr.header;

                if (!headerFoldouts.ContainsKey(currentHeader))
                    headerFoldouts[currentHeader] = true;

                headerFoldouts[currentHeader] = EditorGUILayout.Foldout(headerFoldouts[currentHeader], currentHeader, true);
            }

            // Only draw fields inside an expanded foldout
            if (headerFoldouts.ContainsKey(currentHeader) && headerFoldouts[currentHeader])
            {
                DrawField(field, soundManager, currentHeader);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawField(FieldInfo field, SoundManager sm, string currentHeader)
    {
        object value = field.GetValue(sm);
        System.Type fieldType = field.FieldType;
        string fieldName = field.Name;

        EditorGUI.BeginChangeCheck();

        if (fieldType == typeof(AudioClip))
        {
            AudioClip newClip = (AudioClip)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(fieldName), (AudioClip)value, typeof(AudioClip), false);
            if (EditorGUI.EndChangeCheck())
                field.SetValue(sm, newClip);
        }
        else if (fieldType == typeof(AudioSource))
        {
            AudioSource newSource = (AudioSource)EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(fieldName), (AudioSource)value, typeof(AudioSource), true);
            if (EditorGUI.EndChangeCheck())
                field.SetValue(sm, newSource);

            if (newSource != null)
            {
                if (!audioSourceFoldouts.ContainsKey(fieldName))
                    audioSourceFoldouts[fieldName] = false;

                audioSourceFoldouts[fieldName] = EditorGUILayout.Foldout(audioSourceFoldouts[fieldName], currentHeader + " Audio Settings", true);

                if (audioSourceFoldouts[fieldName])
                {
                    DrawAudioSourceControls(newSource);
                }
            }
        }
    }

    private void DrawAudioSourceControls(AudioSource source)
    {
        EditorGUILayout.BeginVertical("box");
        source.volume = EditorGUILayout.Slider("Volume", source.volume, 0f, 1f);
        source.pitch = EditorGUILayout.Slider("Pitch", source.pitch, -3f, 3f);
        source.loop = EditorGUILayout.Toggle("Loop", source.loop);
        source.spatialBlend = EditorGUILayout.Slider("Spatial Blend", source.spatialBlend, 0f, 1f);
        EditorGUILayout.EndVertical();
    }
}
