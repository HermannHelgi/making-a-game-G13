using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


public class WendigoChasing : WendigoBehaviour
{
    public float spawnBehindCooldown = 20.0f;
    public float attackDistance = 3.0f;
    public float searchTime = 0.0f;
    public GameObject wendigo;
    public NavMeshAgent agent;
    private float spawnBehindTimer = 0.0f;
    private bool spawned = false;
    public WendigoSpawnPointTracker wendigoSpawnPointTracker;
    public StalkingBehaviour stalkingBehaviour;
    public WendigoRaycast wendigoRaycasts;
    public WendigoFollowPlayer wendigoFollowPlayer;
    public WendigoLookForPlayer wendigoLookForPlayer;
    public WendigoAttack wendigoAttack;
    public PlayerDeathHandler playerDeathHandler;


    public override void EnterState()
    {
        base.EnterState();
        spawned = false;
        agent.enabled = true;
        SpawnBehindPlayer();
        spawnBehindTimer = spawnBehindCooldown;
    }

    public override void Run()
    {   
        if (isActive)
        {   
            Debug.Log("spawn Status: "+ spawned);
            if (!spawned)
            {
                spawnBehindTimer -= Time.deltaTime;
                if (spawnBehindTimer < 0.0f)
                {   
                    Debug.Log("Spawning wendigo!"); 
                    wendigoFollowPlayer.SpawnBehindPlayer();
                    spawnBehindTimer = spawnBehindCooldown;
                    spawned = true;
                }
            }
            else
            {
                if (wendigoRaycasts.detected && Vector3.Distance(wendigoRaycasts.target.transform.position, transform.parent.transform.position) < attackDistance)
                {   
                    playerDeathHandler.die("You were slain by the monster!");
                }
                else if (wendigoRaycasts.detected)
                {
                    searchTime = 0.0f;
                    wendigoFollowPlayer.FollowPlayer();
                    wendigoLookForPlayer.MarkPlayerSighting();
                }
                else if (!wendigoRaycasts.detected)
                {
                    searchTime += Time.deltaTime;
                    wendigoLookForPlayer.TrackFootsteps();
                }
            }
        }
        else if (isEnding)
        {
            isEnding = false;	
            agent.enabled = false;
            transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
            searchTime = 0;

            // Debug.Log("DOING000");
            // if (wendigoFollowPlayer.selectedRetreat == null)
            // {
            //     wendigoFollowPlayer.Retreat();
            // }

            // if (Vector3.Distance(transform.position, wendigoFollowPlayer.selectedRetreat.transform.position) <= 5f)
            // {
            //     Debug.Log("DISSAPEARS");
            //     agent.enabled = false;
            //     transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
            //     spawned = false;
            // }
            // else if (wendigoRaycasts.detected)
            // {
            //     isEnding = false;
            // }
        }        
    }

    private void SpawnBehindPlayer()
    {
        spawnBehindTimer = spawnBehindCooldown;
        if (stalkingBehaviour.inAggressionRange)
        {   
            Vector3 currentPosition = stalkingBehaviour.ReturnCurrentPosition();
            if(currentPosition != Vector3.zero)
            {
                wendigoFollowPlayer.SpawnBehindPlayer(currentPosition);
                spawned = true;
                stalkingBehaviour.DespawnWendigo();
            }
        }
    }
}
