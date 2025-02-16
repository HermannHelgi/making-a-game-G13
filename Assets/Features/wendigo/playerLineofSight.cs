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

    public bool isLooking;

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
        if(isLooking)
        {
            staticSound.Play();

        }
    }

    void StopMusic()
    {

        staticSound.Stop();

        
    }


    void Update()

    {
        if (wendigo == null || playerCamera == null)
        {
            return;
        }
        if(IsLookingAtWendigo(wendigo.transform.position, playerCamera.transform.position))
        {
            Debug.Log("Player is looking at Wendigo");
        }
        else
        {
            Debug.Log("Player is not looking at Wendigo");
        }

        AdjustAudio();

    }

    public bool IsLookingAtWendigo(Vector3 wendigoPosition, Vector3 playerPosition)
    {


        Vector3 direction = wendigoPosition - playerPosition;
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out hit))
        {   
            Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, Mathf.Infinity);
            if (hit.transform == wendigo.transform && hit.distance <= detectionDistance)
            {   
                if(!isLooking)
                {
                    Debug.Log("Player is looking at Wendigo");
                    isLooking = true;
                

                }
            }
            else
            {
                Debug.Log("Player is not looking at Wendigo");
                isLooking = false;
                

            }
        }
        else
        {
            isLooking = false;
            
        }
        return isLooking;
    }
    void AdjustAudio()
    {
        if (isLooking)
        {
            // Play only if it's not already playing
            if (!staticSound.isPlaying)
            {
                PlayMusic();
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
                StopMusic();
            }
        }
    }
}


