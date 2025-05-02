using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class WendigoChasing : WendigoBehaviour
{
    public float spawnBehindCooldown = 20.0f;
    public float attackDistance = 5.0f;
    public float searchTime = 0.0f;
    public GameObject wendigo;

    private float spawnBehindTimer = 0.0f;
    private bool spawned = false;
    public WendigoSpawnPointTracker wendigoSpawnPointTracker;
    public StalkingBehaviour stalkingBehaviour;
    public WendigoRaycast wendigoRaycasts;
    public WendigoFollowPlayer wendigoFollowPlayer;
    public WendigoLookForPlayer wendigoLookForPlayer;
    public WendigoAttack wendigoAttack;

    void Awake()
    {
       
    }
    public override void EnterState()
    {
        base.EnterState();
        spawned = false;
        SpawnBehindPlayer();
    }

    public override void Run()
    {   
        spawnBehindTimer = spawnBehindCooldown;
        if (isActive)
        {
            if (!spawned)
            {
                spawnBehindTimer -= Time.deltaTime;
                if (spawnBehindTimer < 0.0f)
                {
                    wendigoFollowPlayer.SpawnBehindPlayer();
                    spawnBehindTimer = spawnBehindCooldown;
                }
            }
            else
            {
                if (wendigoRaycasts.detected && Vector3.Distance(wendigoRaycasts.target.transform.position, transform.position) < attackDistance)
                {
                    wendigoAttack.Attack();
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
            wendigoFollowPlayer.Retreat();

            if (!wendigoRaycasts.detected)
            {
                wendigo.transform.position = wendigoSpawnPointTracker.despawnPoint.position;
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
                wendigoFollowPlayer.SpawnBehindPlayer();
                spawned = true;
                stalkingBehaviour.DespawnWendigo();
            }
        }
    }
}
