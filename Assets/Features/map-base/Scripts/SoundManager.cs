using UnityEngine;

public class SoundManager : MonoBehaviour
{   
    public static SoundManager instance;

    [Header("Ambiance")]
    // Sounds for ambiance
    public AudioClip Ambiance1;
    public AudioSource AmbianceSource1;
    public AudioClip Ambiance2;
    public AudioSource AmbianceSource2;

    [Header("Asset Sounds")]
    // Sounds for fire, interractions, etc

    public AudioClip FireCrackleAudio;
    public AudioSource FireCrackleSource;


    [Header("Player Sounds")]
    // Oil fucntionality sounds

    public AudioClip OuchSound;
    public AudioClip BreathingSound;
    public AudioClip FootstepsSound;
    public AudioClip RunningSound;
    public AudioClip EatingSound;
    public AudioClip DeadSound;
    public AudioSource Player;


    [Header("Witch")]
    // Sounds for Withceroo

    public AudioClip WitchNoiseSound1;
    public AudioClip WitchNoiseSound2;
    public AudioClip WitchNoiseSound3;
    public AudioSource WitchNoiseSource;
    

    [Header("Wendigo")]
    // Sounds for Wendigo

    public AudioClip WendigoScream1;
    public AudioClip WendigoScream2;
    public AudioClip WendigoScream3;
    public AudioSource WendigoScreamSource;


    [Header("Events")]
    // Sounds for different happenings
    public AudioClip StalkingStateSound;
    public AudioClip ChasingStateSound;
    public AudioClip JumpscareSound;
    public AudioSource EventsSource;



    //############################################//


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayOneShot(AudioClip clip, AudioSource source)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Clip or Source is missing!");
        }
    }

    // Ambiance Sounds
    public void PlayAmbiance1() => PlayOneShot(Ambiance1, AmbianceSource1);
    public void PlayAmbiance2() => PlayOneShot(Ambiance2, AmbianceSource2);
    
    // Asset Sounds
    public void PlayFireCrackle() => PlayOneShot(FireCrackleAudio, FireCrackleSource);

    // Player Sounds
    public void PlayOuch() => PlayOneShot(OuchSound, Player);
    public void PlayBreathing() => PlayOneShot(BreathingSound, Player);
    public void PlayFootsteps() => PlayOneShot(FootstepsSound, Player);
    public void PlayEating() => PlayOneShot(EatingSound, Player);
    public void PlayRunning() => PlayOneShot(RunningSound, Player);
    public void PlayDead() => PlayOneShot(DeadSound, Player);

    // Witch Sounds
    public void PlayWitchNoise1() => PlayOneShot(WitchNoiseSound1, WitchNoiseSource);
    public void PlayWitchNoise2() => PlayOneShot(WitchNoiseSound2, WitchNoiseSource);
    public void PlayWitchNoise3() => PlayOneShot(WitchNoiseSound3, WitchNoiseSource);

    // Wendigo Sounds
    public void PlayWendigoScream1() => PlayOneShot(WendigoScream1, WendigoScreamSource);
    public void PlayWendigoScream2() => PlayOneShot(WendigoScream2, WendigoScreamSource);
    public void PlayWendigoScream3() => PlayOneShot(WendigoScream3, WendigoScreamSource);
    
    // Events Sounds
    public void PlayStalking() => PlayOneShot(StalkingStateSound, EventsSource);
    public void PlayChasing() => PlayOneShot(ChasingStateSound, EventsSource);
    public void PlayJumpscare() => PlayOneShot(JumpscareSound, EventsSource);

}
