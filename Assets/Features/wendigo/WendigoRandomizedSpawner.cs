
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

    public WendigoRaycast wendigoRaycast;

    private MeshRenderer meshRenderer;



    void Start()
    {   
        meshRenderer = wendigoPrefab.GetComponent<MeshRenderer>();
        
    }


    public void SpawnWendigo()
    {   
        
        if (player == null) return;

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
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


            if (meshRenderer != null)
            {   
                wendigoPrefab.transform.position = despawnPoint.position;
                meshRenderer.enabled = false;
                Debug.Log("Despawning Wendigo.");
            }        
    }

    //spawn outside of player's line of sight and near a gameobject with tree tag
    private Vector3 FindValidSpawnPosition()
    {   
        if (player == null) return Vector3.zero;
        
        Vector3 spawnPosition;
        int attempts = 0;
        while (attempts < 15)
        {
            spawnPosition = player.position + Random.insideUnitSphere * maxSpawnDistance;
            // if (playerLineOfSight.IsPositionVisibleToPlayer(spawnPosition))
            float distance = Vector3.Distance(spawnPosition, player.position);
            Vector3 direction = Random.onUnitSphere;
            spawnPosition = player.position + direction * distance;
            if (distance < minSpawnDistance)
            {
                attempts++;
                continue;
            }
        // if(Vector3.Distance(spawnPosition, player.position) < minSpawnDistance)
        // {
        //     attempts++;
        //     continue;
        // }

            if(HasLineOfSight(wendigoPrefab.transform.position, spawnPosition, 60f))
            {
                attempts++;
                continue;
            }
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 200, Vector3.down, out hit, 400, groundLayer))
            {
                spawnPosition = hit.point;
                spawnPosition.y = hit.point.y + 2;
                // attempts = 0;
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

    // private bool IsPositionVisibleToPlayer(Vector3 spawnPosition)
    // {
    //     // if (playerLineOfSight == null) 
    //     // {
    //     //     // Debug.Log("Player Line of Sight is null");	
    //     //     return false;
    //     // }

                
    //     //     ispositionvisible = playerLineOfSight.IsLookingAtWendigo(spawnPosition);
    //     //     return ispositionvisible;

        

    // }



}