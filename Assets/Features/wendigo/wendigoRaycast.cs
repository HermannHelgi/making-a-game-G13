
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WendigoRaycast : MonoBehaviour
{


    public GameObject player;
    public Transform wendigoTransform;
    public bool detected;
    public float maxDistance = 70f;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to avoid self-detection

    void Start()
    {
        if (wendigoTransform == null)
            wendigoTransform = transform;
    }

    void Update()
    {
        if (player == null) 
        {
            return;
        }

        Vector3 startPos = wendigoTransform.position + offset;
        Vector3 direction = (player.transform.position - startPos).normalized;
        
        RaycastHit hit;
        // LayerMask mask = ~LayerMask.GetMask("Obstacle"); // Ignore obstacles

        if (Physics.Raycast(startPos, direction, out hit, maxDistance))
        {
            // Debug.DrawLine(startPos, hit.point, Color.red, 2f);
            
            if (hit.transform.CompareTag("Player"))
            {
                detected = true;
                // Debug.Log("Player Detected");

            }
            else
            {
                detected = false;
                // Debug.Log("Player Not Detected");
            }
        }

    
    }
}
