using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerLineofSight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isLooking;

    public float fovThreshold = 0.75f; // 45 degrees

    public Camera playerCamera;
    public List<GameObject> spawnBoxes;

    public float detectionDistance = 60f;
    
    void Start()
    {
            
    }

    void Update()

    {

        bool wasLooking = isLooking;
        // isLooking = IsLookingAtWendigo(wendigo.transform.position);

        // isLooking = IsLookingAtWendigo(wendigo.transform.position);
        if(isLooking && !wasLooking)
        {   
            
            Debug.Log("Player is looking at Wendigo");
        }
        else if(!isLooking && wasLooking)
        {
            Debug.Log("Player is not looking at Wendigo");
        }
        



    }



    public bool IsLookingAtWendigo(Vector3 wendigoPosition)
    {
        
        Vector3 directionToWendigo = (wendigoPosition - playerCamera.transform.position).normalized;
        if (IsPositionVisibleToCamera(directionToWendigo))
        {
            return false;
        }
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, directionToWendigo, out hit, detectionDistance))        {   
            if (hit.transform.CompareTag("Wendigo"))
            {
                return true;
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


}


