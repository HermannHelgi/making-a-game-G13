using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

[CustomEditor(typeof(SoundManager))]
public class SoundEditor : Editor
{
    private Dictionary<int, bool> soundGroupFoldouts = new();
    private Dictionary<int, bool> audioSourceFoldouts = new();
    private Dictionary<int, bool> source3Dsound = new();
    private Dictionary<string, bool> soundFoldout = new();

    SerializedProperty soundGroupsProp;

    SerializedProperty mainMixerProp;

    SerializedProperty snapshotsGroupProp;


    void OnEnable()
    {
        soundGroupsProp = serializedObject.FindProperty("soundGroups");
        mainMixerProp = serializedObject.FindProperty("mainMixer");
        snapshotsGroupProp = serializedObject.FindProperty("soundSnapshots");


    }

    public override void OnInspectorGUI()
    {   
        
        serializedObject.Update();

        // ===== TOP SECTION: MAIN MIXER + SOUND SCENES =====
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Main Audio Mixer", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mainMixerProp, new GUIContent("Main Mixer"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

        // SerializedProperty snapshotProp = serializedObject.FindProperty("Snapshot");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Snapshots", EditorStyles.boldLabel);

        for (int i = 0; i < snapshotsGroupProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal("box");
            SerializedProperty snapshotsProp = snapshotsGroupProp.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = snapshotsProp.FindPropertyRelative("name");

            
            EditorGUILayout.PropertyField(snapshotsProp, new GUIContent("Snapshot"));
            if (GUILayout.Button("Remove snapshot"))
            {
                snapshotsGroupProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Snapshot"))
        {
            snapshotsGroupProp.arraySize++;
        }

        EditorGUILayout.Space(15);

        // ===== BELOW: SOUND GROUPS UI =====
        for (int i = 0; i < soundGroupsProp.arraySize; i++)
        {
            SerializedProperty groupProp = soundGroupsProp.GetArrayElementAtIndex(i);
            SerializedProperty headerProp = groupProp.FindPropertyRelative("headerName");
            SerializedProperty sourceProp = groupProp.FindPropertyRelative("source");
            SerializedProperty mixerProp = groupProp.FindPropertyRelative("mixer");
            SerializedProperty soundsProp = groupProp.FindPropertyRelative("sounds");

            if (!soundGroupFoldouts.ContainsKey(i))
                soundGroupFoldouts[i] = true;

            EditorGUILayout.BeginVertical("box");

            soundGroupFoldouts[i] = EditorGUILayout.Foldout(soundGroupFoldouts[i], headerProp.stringValue, true);

            //soundgroup foldout 
            if (soundGroupFoldouts[i])
            {
                EditorGUI.indentLevel++;

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

                EditorGUILayout.PropertyField(mixerProp, new GUIContent("Audio Mixer Group"));
                AudioMixerGroup mixer = mixerProp.objectReferenceValue as AudioMixerGroup;
                
                if (src != null)
                {   
                    if(mixer != null)
                    {
                        src.outputAudioMixerGroup = mixer;
                    }
                    if (!audioSourceFoldouts.ContainsKey(i))
                        audioSourceFoldouts[i] = false;
                    
                    //audiosource foldout
                    audioSourceFoldouts[i] = EditorGUILayout.Foldout(audioSourceFoldouts[i], "Audio Source Settings", true);
                    if (audioSourceFoldouts[i])
                    {
                        EditorGUI.indentLevel++;
                        src.mute = EditorGUILayout.Toggle("Mute", src.mute);
                        src.loop = EditorGUILayout.Toggle("Loop", src.loop);
                        src.playOnAwake = EditorGUILayout.Toggle("Play On Awake", src.playOnAwake);
                        src.priority = (int)EditorGUILayout.Slider("Priority", src.priority, 0, 256);
                        GUILayout.Label("Lower = higher priority", EditorStyles.miniLabel);
                        src.volume = EditorGUILayout.Slider("Volume", src.volume, 0f, 1f);
                        src.pitch = EditorGUILayout.Slider("Pitch", src.pitch, -3f, 3f);
                        src.panStereo = EditorGUILayout.Slider("Stereo Pan", src.panStereo, -1f, 1f);
                        GUILayout.Label("Left                              Right", EditorStyles.miniLabel);
                        src.spatialBlend = EditorGUILayout.Slider("Spatial Blend", src.spatialBlend, 0f, 1f);
                        GUILayout.Label("0 = 2D | 1 = 3D", EditorStyles.miniLabel);
                        EditorGUI.indentLevel--;
                    }

                    if (!source3Dsound.ContainsKey(i))
                        source3Dsound[i] = false;
                    
                    // 3D sound setting foldout
                    source3Dsound[i] = EditorGUILayout.Foldout(source3Dsound[i], "3D Sound Settings", true);
                    if (source3Dsound[i])
                    {
                        EditorGUI.indentLevel++;
                        src.dopplerLevel = EditorGUILayout.Slider("Doppler Level", src.dopplerLevel, 0f, 5f);
                        src.spread = EditorGUILayout.Slider("Spread", src.spread, 0f, 360f);
                        src.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", src.rolloffMode);
                        src.minDistance = EditorGUILayout.FloatField("Min Distance", src.minDistance);
                        src.maxDistance = EditorGUILayout.FloatField("Max Distance", src.maxDistance);

                        // if (src.rolloffMode == AudioRolloffMode.Custom)
                        // {
                        //     EditorGUILayout.Space(5);
                        //     EditorGUILayout.LabelField("Custom Attenuation Curves", EditorStyles.boldLabel);

                        //     src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, 
                        //         EditorGUILayout.CurveField("Volume Rolloff", src.GetCustomCurve(AudioSourceCurveType.CustomRolloff)));

                        //     src.SetCustomCurve(AudioSourceCurveType.SpatialBlend, 
                        //         EditorGUILayout.CurveField("Spatial Blend", src.GetCustomCurve(AudioSourceCurveType.SpatialBlend)));

                        //     src.SetCustomCurve(AudioSourceCurveType.Spread, 
                        //         EditorGUILayout.CurveField("Spread", src.GetCustomCurve(AudioSourceCurveType.Spread)));

                        //     src.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, 
                        //         EditorGUILayout.CurveField("Reverb Zone Mix", src.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix)));
                        // }
                        EditorGUI.indentLevel--;
                    }
                }

                // Sounds list
                EditorGUILayout.Space();
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

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        if (GUILayout.Button("Add Sound Group"))
        {
            soundGroupsProp.arraySize++;
            
        }
        serializedObject.ApplyModifiedProperties();

    }
}
