using System.Collections.Generic;
using System.Collections;
// using System.Threading;
using UnityEngine;
using UnityEngine.AI;
// using UnityEngine.UIElements;


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
    public Transform caveChaseSpot;
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
                if (GameManager.instance.skullPickedUp)
                {
                    wendigoFollowPlayer.SpawnBehindPlayer(finalChaseSpot.transform.position);
                    spawned = true;
                }
                if (GameManager.instance.lureCrafted && GameManager.instance.dangerZone && !GameManager.instance.lurePlaced)
                {
                    wendigoFollowPlayer.SpawnBehindPlayer(caveChaseSpot.transform.position);
                    spawned = true;
                }
                spawnBehindTimer -= Time.deltaTime;
                if (spawnBehindTimer < 0.0f)
                {
                    if (wendigoFollowPlayer.SpawnBehindPlayer())
                    {
                        spawned = true;
                        spawnBehindTimer = spawnBehindCooldown;
                    }
                }
            }
            else
            {
                if (GameManager.instance.skullPickedUp)
                {
                    Debug.Log("final chase!");
                    internalTimer += Time.deltaTime;
                    if (internalTimer <= 0.2)
                    {
                        wendigoFollowPlayer.FollowPlayer();
                        internalTimer = 0;
                    }
                }
                if (!wendigoRaycasts.detected && !GameManager.instance.skullPickedUp)
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
            if (wendigoFollowPlayer.selectedRetreat != null)
            {
                isRetreating = true;

                agent.SetDestination(wendigoFollowPlayer.selectedRetreat.transform.position);

                if (Vector3.Distance(transform.parent.transform.position, wendigoFollowPlayer.selectedRetreat.transform.position) <= 5f)
                {
                    Debug.Log("DISSAPEARS");
                    // agent.enabled = false;
                    agent.enabled = false;
                    transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
                    // isRetreating = true;
                    isEnding = false;

                }

            }
            if (wendigoFollowPlayer.selectedRetreat == null)
            {
                isRetreating = true;
                wendigoFollowPlayer.selectedRetreat = caveChaseSpot.gameObject;
            }
            
            // if (Vector3.Distance(transform.parent.transform.position, wendigoRaycasts.target.transform.position) >= 20f)
            // {
            //     transform.parent.transform.position = wendigoSpawnPointTracker.despawnPoint.transform.position;
            //     // isRetreating = true;
            //     isEnding = false;
            // }
        }
    }



    private IEnumerator PlayScream(float pause)
    {
        agent.isStopped = true;
        myAnimator.SetTrigger("scream");
        yield return new WaitForSeconds(pause);   // or use the state’s real length
        agent.isStopped = false;
    }


    private void SpawnBehindPlayer()
    {
        spawnBehindTimer = spawnBehindCooldown;
        if (stalkingBehaviour.inAggressionRange)
        {

            Vector3 currentPosition = stalkingBehaviour.ReturnCurrentPosition();
            if (currentPosition != Vector3.zero)
            {
                wendigoFollowPlayer.SpawnBehindPlayer(currentPosition);
                StartCoroutine(PlayScream(4f));
                stalkingBehaviour.DespawnWendigo();
                spawned = true;
            }
        }
        if (effigyBehavior.inAggressionRange || GameManager.instance.dangerZone)
        {   
            
            StartCoroutine(PlayScream(4f));
            stalkingBehaviour.DespawnWendigo();
            // myAnimator.SetBool("scream",false);
            spawned = true;

        }
    }
    
}
