using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerLineofSight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float audioIncreaseRate, audioDecreaseRate, audioMaxVolume;

    public bool isLooking;
    

    public float fovThreshold = 0.75f; // 45 degrees

    public AudioSource staticSound;

    public WendigoRaycast detectedScript;

    public Camera playerCamera;
    private GameObject wendigo;

    public float detectionDistance = 60f;
    private MeshRenderer isWendigoVisible;
    
    void Start()
    {
        wendigo = GameObject.Find("Wendigo");
        isWendigoVisible = wendigo.GetComponent<MeshRenderer>();

        if(staticSound == null)
        {
            staticSound = GetComponent<AudioSource>();
        }
        if (wendigo == null)
        {
            Debug.Log("Wendigo not found by name in the scene!");
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
        bool wasLooking = isLooking;
        // isLooking = IsLookingAtWendigo(wendigo.transform.position);
        if(isWendigoVisible.enabled)
        {   
            Debug.Log("Wendigo is visible");
            isLooking = IsLookingAtWendigo(wendigo.transform.position);
            if(isLooking && !wasLooking)
            {   
                
                Debug.Log("Player is looking at Wendigo");
            }
            else if(!isLooking && wasLooking)
            {
                Debug.Log("Player is not looking at Wendigo");
            }
        }

        AdjustAudio();

    }

    public bool IsLookingAtWendigo(Vector3 wendigoPosition)
    {
        
        Vector3 directionToWendigo = (wendigoPosition - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, directionToWendigo);

        if (dot < fovThreshold)
        {   
            // Debug.Log("Wendigo is OUTSIDE the player's field of view.");
            return false; // Wendigo is outside the field of view
        }
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, directionToWendigo, out hit, detectionDistance))        {   
            // Debug.DrawLine(cameraForward, hit.point, Color.red, 2f);
            if (hit.transform.CompareTag("Wendigo"))
            {
                if(isWendigoVisible.enabled)
                {
                    return true;
                }
                return false;
            }
        return false;
        }
        return false;

    }

    public bool IsPositionVisibleToCamera(Vector3 position) 
    {
        Vector3 direction = (position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, direction);

        // Check if within FOV
        if (dot < fovThreshold)
        {
            return false;
        }

        // Check if there is no obstruction
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, direction, out hit, detectionDistance))
        {
            // If the ray hits 'position' or hits very close to it without hitting other geometry first
            float distanceToPosition = Vector3.Distance(playerCamera.transform.position, position);
            if (hit.distance < distanceToPosition - 1f) // Some small offset
            {
                return true; // Something else is in the way
            }
            
        }
        return false;
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


