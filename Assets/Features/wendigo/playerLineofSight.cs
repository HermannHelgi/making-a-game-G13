using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class playerLineofSight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public float audioIncreaseRate, audioDecreaseRate, audioMaxVolume;

    public bool isLookingAtWendigo;

    public AudioSource staticSound;


    public wendigoRaycast detectedScript;

    private Camera playerCamera;
    private GameObject wendigo;

    public float detectionDistance = 60f;
    
    void Start()
    {
        playerCamera = Camera.main;
        wendigo = GameObject.Find("Wendigo");

        if(staticSound == null)
        {
            staticSound = GetComponent<AudioSource>();
        }
            
    }

    void PlayMusic()
    {
        if(isLookingAtWendigo)
        {
            staticSound.Play();
            while(staticSound.volume < audioMaxVolume)
            {
                staticSound.volume = Mathf.MoveTowards(staticSound.volume, audioMaxVolume, audioIncreaseRate * Time.deltaTime);
            }
            if(staticSound.volume >= audioMaxVolume)
            {
                staticSound.volume = audioMaxVolume;
            }
        }
    }

    void StopMusic()
    {
        if(!isLookingAtWendigo)
        {
            while(staticSound.volume > 0)
            {
                staticSound.volume = Mathf.MoveTowards(staticSound.volume, 0, audioDecreaseRate * Time.deltaTime);
                staticSound.Stop();
            }
            if(staticSound.volume == 0)
            {
                staticSound.Stop();
            }
        }
    }


    void Update()

    {

        if (playerCamera == null || wendigo == null)
        {
            return;
        }

        Vector3 direction = wendigo.transform.position - playerCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out hit))
        {   
            Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, Mathf.Infinity);
            if (hit.transform == wendigo.transform && hit.distance <= detectionDistance)
            {   
                if(!isLookingAtWendigo)
                {
                    Debug.Log("Player is looking at Wendigo");
                    isLookingAtWendigo = true;

                }
            }
            else
            {
                Debug.Log("Player is not looking at Wendigo");
                isLookingAtWendigo = false;

            }
        }
        else
        {
            isLookingAtWendigo = false;
        }
        AdjustAudio();

    }
    void AdjustAudio()
    {
        if (isLookingAtWendigo)
        {
            // Play only if it's not already playing
            if (!staticSound.isPlaying)
            {
                staticSound.Play();
            }

            // Increase volume smoothly
            staticSound.volume = Mathf.MoveTowards(staticSound.volume, audioMaxVolume, audioIncreaseRate * Time.deltaTime);
        }
        else
        {
            // Reduce volume smoothly
            staticSound.volume = Mathf.MoveTowards(staticSound.volume, 0f, audioDecreaseRate * Time.deltaTime);

            // Stop audio when volume reaches 0
            if (staticSound.volume == 0 && staticSound.isPlaying)
            {
                staticSound.Stop();
            }
        }
    }
}


