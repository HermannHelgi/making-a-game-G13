
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
    public float spawnRadius = 50f;
    // public float minSpawnDistance = 10f;
    public float spawnInterval = 30f;
    public LayerMask groundLayer;
    public PlayerLineofSight playerLineOfSight;
    private bool ispositionvisible;
    public float spawnTimer;
    public int playerSightings;
    public int maxPlayerSightings = 3;
    private GameObject wendigoInstance;

    void Start()
    {   
        wendigoInstance = null;

        SpawnWendigo();
    }


    public void SpawnWendigo()
    {   
        if (wendigoInstance != null) return;
        if (player == null) return;

        Vector3 spawnpoint = FindValidSpawnPosition();

        if (spawnpoint != Vector3.zero)
        {
            wendigoInstance = Instantiate(wendigoPrefab,spawnpoint,Quaternion.identity);
        }
        

    }

    public void DespawnWendigo()
    {
        while (wendigoInstance != null)
        {
            Debug.Log("Despawning Wendigo.");
            Destroy(wendigoInstance);
        }
            // wendigoInstance = null; //  Ensure reference is cleared
    }


    private Vector3 FindValidSpawnPosition()
    {
        if (player == null) return Vector3.zero;

        Vector3 spawnPosition = Vector3.zero;
        Vector3 playerPosition = player.position;
        const int maxAttempts = 20;
        for(int i = 0; i < maxAttempts; i++)
        {
            //find valid random spawn position
            spawnPosition = playerPosition * Random.Range(spawnRadius,spawnRadius * 2);
            if(IsPositionVisibleToPlayer(spawnPosition))
            {
                continue;
    
            }
            else
            {
                RaycastHit hit;
                if(Physics.Raycast(spawnPosition,Vector3.down,out hit,Mathf.Infinity,groundLayer))
                {
                    if(hit.collider != null)
                    {
                        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            return hit.point;
                        }
                    }
                }
            }
        }


        return spawnPosition;
    }

    private bool IsPositionVisibleToPlayer(Vector3 spawnPosition)
    {
        if (playerLineOfSight == null) 
        {
            // Debug.Log("Player Line of Sight is null");	
            return false;
        }

                
            ispositionvisible = playerLineOfSight.IsLookingAtWendigo(spawnPosition);
            return ispositionvisible;

    }



}