
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WendigoRaycast : MonoBehaviour
{


    public GameObject target;
    public Transform wendigoTransform;
    public bool detected;
    [Header("FOV Settings")]
    public float listenRadius = 50f;
    public float followRange = 45f;
    [Range(0, 360)]
    public float fovAngle = 60f;

    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to avoid self-detection

    [Tooltip("Layer mask for obstacles")]
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    [Header("Lose Sight Delay")]
    public float loseSightDelay = 6f;
    private float loseSightTimer = 0f;
    private bool sawPlayerThisScan = false;

    private float scanDelay = 0;

    public Vector3 lastKnownPosition;

    void Start()
    {
        if (wendigoTransform == null)
            wendigoTransform = transform;
    }

    private bool ScanForPlayer()
    {
        Collider[] playerInRadius = Physics.OverlapSphere(wendigoTransform.position, followRange, playerMask);
        if (playerInRadius.Length > 0)
        {
            Transform target = playerInRadius[0].transform;
            Vector3 directionToPlayer = (target.position - wendigoTransform.position).normalized;

            if (Vector3.Angle(wendigoTransform.forward, directionToPlayer) < fovAngle / 2)
            {

                float distanceToPlayer = Vector3.Distance(wendigoTransform.position, target.position);
                Debug.DrawRay(wendigoTransform.position, target.transform.position, Color.red);

                if (!Physics.Raycast(wendigoTransform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;

                }

            }

        }
        return false;
    }

    private bool ListenForPlayer()
    {   
        Collider[] playerInRadius = Physics.OverlapSphere(wendigoTransform.position, listenRadius, playerMask);

        if(playerInRadius.Length > 0)
        {
            Transform target = playerInRadius[0].transform;
            float distanceToPlayer = Vector3.Distance(wendigoTransform.position, target.position);
            if(distanceToPlayer < listenRadius)
            {
                return false;
            }
            else{
                return true;
            }
        }
        return false;

    }

    void Update()
    {   
        scanDelay += Time.deltaTime;
        
        if(scanDelay >= 0.2f)
        {   
            sawPlayerThisScan = ScanForPlayer();
            if (sawPlayerThisScan)
            {   
                Debug.Log("Player detected!");
                detected = true;
                loseSightTimer = 0;
            }
            else if(ListenForPlayer())
            {
                Debug.Log("Player heard!");
                lastKnownPosition = target.transform.position;
                detected = true;
                loseSightTimer = 0;
            }
            else
            {
                loseSightTimer += Time.deltaTime;
                if(loseSightTimer >= loseSightDelay)
                {
                    Debug.Log("Player Lost!");
                    detected = false;
                }
            }
            scanDelay = 0f;
        }
    }
}
