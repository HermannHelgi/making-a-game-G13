using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class wendigoRaycast : MonoBehaviour
{

// public GameObject player;
// public Transform wendigoTransform;
// public bool detected;
// public Vector3 offset;

// // Start is called once before the first execution of Update after the MonoBehaviour is created
// void Start()
// {
    
// }

// // Update is called once per frame
// void Update()
// {
//     Vector3 direction  = player.transform.position - wendigoTransform.position;
//     RaycastHit hit;
//     if(Physics.Raycast(wendigoTransform.position,direction,out hit))
//     {
//         Debug.DrawLine(wendigoTransform.position,hit.point,Color.red, Mathf.Infinity);
//         if(hit.transform.tag == "Player")
//         {
//             detected = true;
//             Debug.Log("Player Detected");
//         }
//         else
//         {
//             detected = false;
//             Debug.Log("Player Not Detected");
//         }
//     }
    
// }

    public GameObject player;
    public Transform wendigoTransform;
    public bool detected;
    public float maxDistance = 50f;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to avoid self-detection

    void Start()
    {
        if (wendigoTransform == null)
            wendigoTransform = transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 startPos = wendigoTransform.position + offset;
        Vector3 direction = (player.transform.position - startPos).normalized;
        
        RaycastHit hit;
        LayerMask mask = ~LayerMask.GetMask("Obstacle"); // Ignore obstacles

        if (Physics.Raycast(startPos, direction, out hit, maxDistance, mask))
        {
            Debug.DrawLine(startPos, hit.point, Color.red, 0.1f);
            
            if (hit.transform.CompareTag("Player"))
            {
                detected = true;
                Debug.Log("Player Detected");
            }
            else
            {
                detected = false;
                Debug.Log("Player Not Detected");
            }
        }
    }
}
