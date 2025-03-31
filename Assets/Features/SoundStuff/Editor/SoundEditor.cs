using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(SoundManager))]
public class SoundEditor : Editor
{
    private Dictionary<int, bool> audioSourceFoldouts = new();
    SerializedProperty soundGroupsProp;

    void OnEnable()
    {
        soundGroupsProp = serializedObject.FindProperty("soundGroups");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < soundGroupsProp.arraySize; i++)
        {
            SerializedProperty groupProp = soundGroupsProp.GetArrayElementAtIndex(i);
            SerializedProperty headerProp = groupProp.FindPropertyRelative("headerName");
            SerializedProperty sourceProp = groupProp.FindPropertyRelative("source");
            SerializedProperty soundsProp = groupProp.FindPropertyRelative("sounds");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            headerProp.stringValue = EditorGUILayout.TextField("Header", headerProp.stringValue);

            if (GUILayout.Button("Remove Group"))
            {
                soundGroupsProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(sourceProp, new GUIContent("Audio Source"));
            
            AudioSource src = sourceProp.objectReferenceValue as AudioSource;
            if (src != null)
            {
                if (!audioSourceFoldouts.ContainsKey(i))
                    audioSourceFoldouts[i] = false;

                audioSourceFoldouts[i] = EditorGUILayout.Foldout(audioSourceFoldouts[i], "Audio Source Settings", true);

                if (audioSourceFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    src.mute = EditorGUILayout.Toggle("Mute", src.mute);
                    src.loop = EditorGUILayout.Toggle("Loop", src.loop);
                    src.playOnAwake = EditorGUILayout.Toggle("OnAwake", src.playOnAwake);
                    src.priority = (int)EditorGUILayout.Slider("Priority", src.priority, 0, 256);
                    GUILayout.Label("Lower = higher priority in Audio System", EditorStyles.miniLabel);
                    src.volume = EditorGUILayout.Slider("Volume", src.volume, 0f, 1f);
                    src.pitch = EditorGUILayout.Slider("Pitch", src.pitch, -3f, 3f);
                    src.panStereo = EditorGUILayout.Slider("Stereo Pan",src.panStereo,-1, 1);
                    GUILayout.Label("Left                               Right");
                    src.spatialBlend = EditorGUILayout.Slider("Spatial Blend", src.spatialBlend, 0f, 1f);
                    GUILayout.Label("Lower spatialBlend is 2D higher is 3D");    
                    src.
                    
                    
                }
                audioSourceFoldouts[i] = EditorGUILayout.Foldout(audioSourceFoldouts[i],"3D Sound Settings");
                if(audioSourceFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    src.dopplerLevel = (int)EditorGUILayout.Slider("Doppler level", src.dopplerLevel, 0, 1);
                    src.spread = (int)EditorGUILayout.Slider("Spread", src.spread, 0, 1);
                    src.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", src.rolloffMode); 
                    src.minDistance = EditorGUILayout.FloatField("Min Distance", src.minDistance);
                    src.maxDistance = EditorGUILayout.FloatField("Max Distance", src.maxDistance);
        
                    EditorGUI.indentLevel--;
                }



        }


            EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);

            for (int j = 0; j < soundsProp.arraySize; j++)
            {
                SerializedProperty entryProp = soundsProp.GetArrayElementAtIndex(j);
                SerializedProperty nameProp = entryProp.FindPropertyRelative("soundName");
                SerializedProperty clipProp = entryProp.FindPropertyRelative("clip");

                EditorGUILayout.BeginHorizontal();
                nameProp.stringValue = EditorGUILayout.TextField(nameProp.stringValue);
                clipProp.objectReferenceValue = EditorGUILayout.ObjectField(clipProp.objectReferenceValue, typeof(AudioClip), false);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    soundsProp.DeleteArrayElementAtIndex(j);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Sound"))
            {
                soundsProp.arraySize++;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (GUILayout.Button("Add Sound Group"))
        {
            soundGroupsProp.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
