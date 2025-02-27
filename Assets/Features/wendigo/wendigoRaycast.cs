
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WendigoRaycast : MonoBehaviour
{


    public GameObject player;
    public Transform wendigoTransform;
    public bool detected;
    [Header("FOV Settings")]
    public float radius;
    [Range(0, 360)]
    public float fovAngle = 60f;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to avoid self-detection

    [Tooltip("Layer mask for obstacles")]
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    [Header("Lose Sight Delay")]
    public float loseSightDelay = 4f;
    private float loseSightTimer = 0f;
    private bool sawPlayerThisScan = false;
    public float listenRadius = 60f;

    public Vector3 lastKnownPosition;

    void Start()
    {
        if (wendigoTransform == null)
            wendigoTransform = transform;
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            sawPlayerThisScan = ScanForPlayer();
            if (sawPlayerThisScan)
            {
                lastKnownPosition = player.transform.position;
            }
        }
    }

    private bool ScanForPlayer()
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
                    return true;

                }

            }

        }
        if(ListenForPlayer())
        {
            return true;
        }
        return false;
    }

    private bool ListenForPlayer()
    {
        if (Vector3.Distance(wendigoTransform.position, player.transform.position) < listenRadius && loseSightTimer < loseSightDelay)
        {
            return true;
        }
        return false;
    }

    void Update()
    {

        if (sawPlayerThisScan)
        {
            detected = true;
            loseSightTimer = 0;
        }
        else
        {
            loseSightTimer += Time.deltaTime;
            if (loseSightTimer >= loseSightDelay)
            {
                detected = false;
            }
        }

    }
}
