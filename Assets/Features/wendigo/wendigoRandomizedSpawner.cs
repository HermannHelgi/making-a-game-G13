
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
    public float spawnInterval = 10f;
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
        if (playerLineOfSight.isLookingAtWendigo == true)
        {
            playerSightings++;
            return false;
        }
        else
        {   
            
            return true;
        }

    }
}