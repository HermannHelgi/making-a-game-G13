using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundEntry
{
    public string soundName;
    public AudioClip clip;
    public int soundPriority;
}

[System.Serializable]
public class SoundGroup
{
    public string headerName;
    public AudioSource source;
    public AudioMixerGroup mixer; 
    public List<SoundEntry> sounds = new();
}

[System.Serializable]
public class SnapshotGroup
{
    public AudioMixerSnapshot audioSnapshot;
}



[System.Serializable]
public class AudioMixerReference
{
    public string name = "AudioMixer";	
    public AudioMixer mixer;
}

public class SoundManager : MonoBehaviour
{   
    public AudioMixer mainMixer;
    public List<SnapshotGroup> soundSnapshots = new();
    private AudioMixerSnapshot defaultSnapshot;
    private string currentScene;

    public static SoundManager instance;

    public List<SoundGroup> soundGroups = new();

    private Dictionary<string, AudioClip> soundLookup = new();
    private Dictionary<string, AudioSource> sourceLookup = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        BuildLookup();

        if (mainMixer != null) defaultSnapshot = mainMixer.FindSnapshot("Main");
    }

    private void Update()
    {
     if (Input.GetKeyDown(KeyCode.V))
        {
            PlayGroup("STOMP");
        }
        else if (Input.GetKeyDown(KeyCode.B))
            {
                PlayGroup("FOOD");
            }
        else if (Input.GetKeyDown(KeyCode.N))
            {
                ChangeSoundsnapshot("SPOOKY", 0.9f);
            }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            ExitSoundsnapshot(0.6f);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            PlayGroup("WALK");
        }
 
    }


    private void BuildLookup()
    {
        soundLookup.Clear();
        sourceLookup.Clear();

        foreach (var group in soundGroups)
        {
            foreach (var sound in group.sounds)
            {
                if (!string.IsNullOrEmpty(sound.soundName))
                {
                    soundLookup[sound.soundName] = sound.clip;
                    sourceLookup[sound.soundName] = group.source;
                }
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundLookup.TryGetValue(soundName, out var clip) &&
            sourceLookup.TryGetValue(soundName, out var source))
        {
            source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found or missing source.");
        }
    }

    public void PlayGroup(string groupName)
    {
        SoundGroup group = soundGroups.Find(g => g.headerName == groupName);

        if (group != null && group.sounds.Count > 0)
        {

            SoundEntry randomSound = group.sounds[Random.Range(0, group.sounds.Count)];

            if (randomSound.clip != null && group.source != null)
            {
                if(!group.source.isPlaying)
                {
                    group.source.PlayOneShot(randomSound.clip);
                }
            }
            else
            {
                Debug.LogWarning($"[SoundManager] Missing clip or AudioSource in group '{groupName}'.");
            }
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Sound group '{groupName}' not found or empty.");
        }
    }


    public void ChangeSoundsnapshot(string snapshotName, float timing)
    {
        var snapshot = soundSnapshots.Find(s => s.audioSnapshot != null && s.audioSnapshot.name == snapshotName);

        Debug.Log("Snapshot" + snapshotName + " found");
        if (snapshot != null)
        {
            snapshot.audioSnapshot.TransitionTo(timing);
            currentScene = snapshotName;
        }
        else
        {
            Debug.LogWarning($"Snapshot '{snapshotName}' not found.");
        }
    }   

    public void ExitSoundsnapshot(float timing)
    {   
        Debug.Log("returning to default snapshot");
        if (defaultSnapshot != null)
        {
            defaultSnapshot.TransitionTo(timing);
            currentScene = null;
        }
    }




}
