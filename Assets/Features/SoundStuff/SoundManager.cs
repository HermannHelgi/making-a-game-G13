using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public string soundName;
    public AudioClip clip;
}

[System.Serializable]
public class SoundGroup
{
    public string headerName;
    public AudioSource source;
    public List<SoundEntry> sounds = new();
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<SoundGroup> soundGroups = new();

    private Dictionary<string, AudioClip> soundLookup = new();
    private Dictionary<string, AudioSource> sourceLookup = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        BuildLookup();
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



}
