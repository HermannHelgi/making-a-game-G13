
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;

public class wendigoRandomizedSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject wendigoPrefab;
    public float maxSpawnDistance = 50f;
    public float minSpawnDistance = 30f;
    public float maxStareTime = 10f;
    public LayerMask groundLayer;
    public PlayerLineofSight playerLineOfSight;
    private bool ispositionvisible;
    public float spawnTimer;
    public int playerSightings;
    public int maxPlayerSightings = 3;

    public Transform despawnPoint;

    private CapsuleCollider capsuleCollider;

    public float heightOffset = 1.84f;  

    void Start()
    {   
        capsuleCollider = wendigoPrefab.GetComponent<CapsuleCollider>();
        
    }


    public void SpawnWendigo()
    {   
        
        if (player == null) return;

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }

        Vector3 spawnpoint = FindValidSpawnPosition();

        if (spawnpoint != Vector3.zero)
        {
            wendigoPrefab.transform.position = spawnpoint;
        }
        

    }

    public void DespawnWendigo()
    {
        
        if (wendigoPrefab == null) return;


            if (capsuleCollider != null)
            {   
                wendigoPrefab.transform.position = despawnPoint.position;
                capsuleCollider.enabled = false;
                Debug.Log("Despawning Wendigo.");
            }        
    }

    private Vector3 FindValidSpawnPosition()
    {   
        if (player == null) return Vector3.zero;
        
        Vector3 spawnPosition;
        int attempts = 0;
        Debug.Log("Finding valid spawn position");
        while (attempts < 15)
        {   
            
            spawnPosition = player.position  + Random.insideUnitSphere * 80f;
            spawnPosition.y -= heightOffset;
            float distance = Vector3.Distance(player.position, spawnPosition);
            Debug.Log("Checking point: " + spawnPosition + " Distance: " + distance);	
            wendigoPrefab.transform.forward = -player.transform.forward;
            if (distance < minSpawnDistance || distance > maxSpawnDistance)
            {   
                Debug.Log("Distance is too close or too far");
                attempts++;
                continue;
            }

            if(HasLineOfSight(wendigoPrefab.transform.position, spawnPosition, maxSpawnDistance))
            {   
                Debug.Log("No line of sight to player");
                attempts++;
                continue;
            }
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 200, Vector3.down, out hit, 400, groundLayer))
            {
                spawnPosition = hit.point;
                spawnPosition.y = hit.point.y + heightOffset;
                Debug.Log("Spawn position: " + spawnPosition);
                return spawnPosition;
            }
            attempts++;
        }
        return Vector3.zero;
    }

    private bool HasLineOfSight(Vector3 fromPos, Vector3 toPos, float maxDist)
    {
    Vector3 dir = (toPos - fromPos).normalized;
    if (Physics.Raycast(fromPos, dir, out RaycastHit hit, maxDist))
    {
        return hit.transform.CompareTag("Player");
    }
    return false;
    }


}