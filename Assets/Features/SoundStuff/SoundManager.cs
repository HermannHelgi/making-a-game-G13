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
    public string name = "SnapshotName";
    public AudioMixerSnapshot snapshot;
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
     if (Input.GetKeyDown(KeyCode.B))
        {
            PlayGroup("PEE");
        }
    else if (Input.GetKeyDown(KeyCode.N))
        {
            PlayGroup("POOP");
        }
    else if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeSoundsnapshot("Ambience");
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
                Debug.Log($"[SoundManager] Playing from group '{groupName}': {randomSound.soundName}");
                group.source.PlayOneShot(randomSound.clip);
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


    public void ChangeSoundsnapshot(string snapshotName)
    {
    var newsnapshot = soundSnapshots.Find(s => s.name == snapshotName);
    Debug.Log("Snapshot" + snapshotName + " found");
    if (newsnapshot != null && newsnapshot.snapshot != null)
    {
        newsnapshot.snapshot.TransitionTo(0.1f); // smooth transition
        currentScene = snapshotName;
    }
    else
    {
        Debug.LogWarning($"Snapshot '{snapshotName}' not found or missing snapshot.");
    }
    }   

    public void ExitSoundsnapshot()
    {   
        Debug.Log("returning to default snapshot");
        if (defaultSnapshot != null)
        {
            defaultSnapshot.TransitionTo(0.1f);
            currentScene = null;
        }
    }




}
