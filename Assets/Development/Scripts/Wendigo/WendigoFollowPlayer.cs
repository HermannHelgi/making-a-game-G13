using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class WendigoFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    public WendigoSpawnPointTracker spawnPointTracker;
    public List<GameObject> retreatPositions;
    public GameObject selectedRetreat;
    public float retreatDistance;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        selectedRetreat = null;
    }

    public void FollowPlayer()
    {
        selectedRetreat = null;
        agent.SetDestination(player.transform.position);
    }

    public void SpawnBehindPlayer()
    {
        selectedRetreat = null;
        SpawnBehindPlayer(spawnPointTracker.SelectRandomSpawn().transform.position);
    }

    public void SpawnBehindPlayer(Vector3 spawnPoint, float sampleRadius=10.0f)
    {
        if(NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            transform.position = spawnPoint;
            transform.forward = (player.transform.position - transform.position).normalized;
            agent.Warp(hit.position);
            Debug.Log("Spawning at : " + spawnPoint);
        }
    }
    public void Retreat()
    {
        foreach(GameObject spot in retreatPositions)
        {
            float distance = Vector3.Distance(spot.transform.position, transform.position);
            if (distance >= retreatDistance)
            {
                selectedRetreat = spot;
                break;
            }
        }
    }
}
