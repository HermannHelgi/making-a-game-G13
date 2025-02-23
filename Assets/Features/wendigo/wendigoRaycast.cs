
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WendigoRaycast : MonoBehaviour
{


    public GameObject player;
    public Transform wendigoTransform;
    public bool detected;
    public float radius; 
    [Range(0, 360)]
    public float fovAngle = 60f;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to avoid self-detection
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    void Start()
    {
        if (wendigoTransform == null)
            wendigoTransform = transform;
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while(true)
            {
                yield return wait;
                ScanForPlayer();
            }
    }

    private void ScanForPlayer()
    {
        Collider[] playerInRadius = Physics.OverlapSphere(wendigoTransform.position, radius, playerMask);
        if (playerInRadius.Length > 0)
        {
            Transform target = playerInRadius[0].transform;
            Vector3 directionToPlayer = (target.position - wendigoTransform.position).normalized;

            if (Vector3.Angle(wendigoTransform.forward, directionToPlayer) < fovAngle / 2)
            {

                float distanceToPlayer = Vector3.Distance(wendigoTransform.position, target.position);

                if (!Physics.Raycast(wendigoTransform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    detected = true;
                }
                else
                {
                    detected = false;
                }

            }
            else
            {
                detected = false;
            }
        }
        else if (detected)
        {
            detected = false;
        }   
    }

    void Update()
    {

    }
}
