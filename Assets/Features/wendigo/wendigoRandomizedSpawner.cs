
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class wendigoRandomizedSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject wendigoPrefab;
    public float spawnRadius = 60f;
    // public float minSpawnDistance = 10f;
    public float spawnInterval = 20f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public playerLineofSight playerLineOfSight;
    public float spawnTimer;
    public int playerSightings;
    public int maxPlayerSightings = 3;
    private GameObject wendigoInstance;

    void Start()
    {
        SpawnSlenderman();
    }


    public void SpawnSlenderman()
    {
        if(!IsVisibleToPlayer())
        {
            wendigoInstance = Instantiate(wendigoPrefab, FindValidSpawnPosition(), Quaternion.identity);
        }

    }

    private Vector3 FindValidSpawnPosition()
    {
        if (player == null) return Vector3.zero;

        Vector3 spawnPosition = Vector3.zero;
        Vector3 playerPosition = player.position;

        return spawnPosition;
    }

    private bool IsVisibleToPlayer()
    {
        if (playerLineOfSight == null) return false;
        if(playerLineOfSight.isLookingAtWendigo) return playerLineOfSight.isLookingAtWendigo;
        return playerLineOfSight.isLookingAtWendigo;

    }

    void Spawn()
    {
        Debug.Log("Spawning Wendigo");
        Vector3 spawnPosition = player.position + Random.insideUnitSphere * spawnRadius;
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            spawnPosition = hit.point;
        }
        else
        {
            Debug.Log("Failed to spawn Wendigo");
            return;
        }
    }


}