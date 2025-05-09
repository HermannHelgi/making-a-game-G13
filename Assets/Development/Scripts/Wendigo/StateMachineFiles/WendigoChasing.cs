using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


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
    public PlayerDeathHandler playerDeathHandler;
    public SoundManager soundManager;
    public bool isRetreating = false;
    private float internalTimer = 0.0f;


    public override void EnterState()
    {
        base.EnterState();
        agent.enabled = true;
        // stalkingBehaviour.DespawnWendigo();

        spawned = false;
        wendigoLookForPlayer.MarkPlayerSighting();
        SpawnBehindPlayer();
        spawnBehindTimer = spawnBehindCooldown;
        isRetreating = false; 
    }

    public override void Run()
    {   
        if (isActive)
        {   
            if (!spawned)
            {
                spawnBehindTimer -= Time.deltaTime;
                if (spawnBehindTimer < 0.0f)
                {   
                    wendigoFollowPlayer.SpawnBehindPlayer();
                    spawnBehindTimer = spawnBehindCooldown;
                    spawned = true;
                    
                }
            }
            else
            {
                if (!wendigoRaycasts.detected)
                {   
                    soundManager.ExitSoundsnapshot(1f);
                    searchTime += Time.deltaTime;
                    wendigoLookForPlayer.TrackFootsteps();
                }

                else if (wendigoRaycasts.detected)
                {   
                    soundManager.ChangeSoundsnapshot("SPOOKY", 3f);
                    soundManager.PlayGroup("WENDIGO_FOLLOW");
                    searchTime = 0.0f;
                    internalTimer += Time.deltaTime;
                    if(internalTimer <= 0.2)
                    {
                        wendigoFollowPlayer.FollowPlayer();
                        internalTimer = 0;
                    }
                    // wendigoFollowPlayer.justSpawned = false;
                    wendigoLookForPlayer.MarkPlayerSighting();
                    
                }

                if (wendigoRaycasts.detected && Vector3.Distance(wendigoRaycasts.target.transform.position, transform.parent.transform.position) < attackDistance)
                {   
                    soundManager.ChangeSoundsnapshot("SPOOKY", 1f);
                    soundManager.PlayGroup("WENDIGO_KILL");
                    playerDeathHandler.die("You were slain by the monster!");
                    soundManager.ExitSoundsnapshot(0f);
                }
            }
        }
        else if (isEnding)
        {
            searchTime = 0;
            if(!isRetreating)
            {
                wendigoFollowPlayer.Retreat();
                
            }
            if (wendigoFollowPlayer.selectedRetreat == null)
            {
                isRetreating = true;
                agent.SetDestination(wendigoFollowPlayer.selectedRetreat.transform.position);                
            }
            if (Vector3.Distance(transform.parent.transform.position, wendigoFollowPlayer.selectedRetreat.transform.position) <= 5f)
            {
                Debug.Log("DISSAPEARS");
                agent.enabled = false;
                transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
                // isRetreating = true;
                spawned = false;
                isEnding = false;
                
            }
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
            }
        }
    }
}
