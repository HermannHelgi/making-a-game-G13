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
    public WendigoEffigy effigyBehavior;
    public StalkingBehaviour stalkingBehaviour;
    public WendigoRaycast wendigoRaycasts;
    public WendigoFollowPlayer wendigoFollowPlayer;
    public WendigoLookForPlayer wendigoLookForPlayer;
    public PlayerDeathHandler playerDeathHandler;
    public SoundManager soundManager;
    public bool isRetreating = false;
    private float internalTimer = 0.0f;
    public Animator myAnimator;
    public Transform finalChaseSpot;

    private void Start()
    {
        
    }
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
                if(GameManager.instance.skullPickedUp)
                {
                    wendigoFollowPlayer.SpawnBehindPlayer(finalChaseSpot.transform.position);
                    spawned = true;
                }
                if(GameManager.instance.lureCrafted && GameManager.instance.dangerZone && !GameManager.instance.lurePlaced)
                {
                    wendigoFollowPlayer.SpawnBehindPlayer(finalChaseSpot.transform.position);
                    spawned = true;
                }
                spawnBehindTimer -= Time.deltaTime;
                if (spawnBehindTimer < 0.0f)
                {
                    if(wendigoFollowPlayer.SpawnBehindPlayer())
                    {
                        spawned = true;
                        spawnBehindTimer = spawnBehindCooldown;
                    }
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
                    if (internalTimer <= 0.2)
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
            if (!isRetreating)
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
                // agent.enabled = false;
                transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
                // isRetreating = true;
                spawned = false;
                isEnding = false;

            }
            else if (Vector3.Distance(transform.parent.transform.position, wendigoRaycasts.target.transform.position) >= 45f)
            {
                transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
                // isRetreating = true;
                spawned = false;
                isEnding = false;
            }
        }
    }

    private void SpawnBehindPlayer()
    {
        float animationDelay = 3.0f;
        spawnBehindTimer = spawnBehindCooldown;
        if (stalkingBehaviour.inAggressionRange)
        {   

            Vector3 currentPosition = stalkingBehaviour.ReturnCurrentPosition();
            if (currentPosition != Vector3.zero)
            {
                wendigoFollowPlayer.SpawnBehindPlayer(currentPosition);
                myAnimator.SetBool("scream", true);
                while(animationDelay < 0.0f)
                {   
                    animationDelay -= Time.deltaTime;
                    // animationDelay --;
                }
                
                myAnimator.SetBool("scream",false);
                spawned = true;
            }
        }
        if(effigyBehavior.inAggressionRange)
        {
            agent.isStopped = true;
            myAnimator.SetBool("scream", true);
            
            while(animationDelay < 0.0f)
            {   
                animationDelay -= Time.deltaTime;
            }
            
            myAnimator.SetBool("scream",false);
            spawned = true;

        }
    }
}
