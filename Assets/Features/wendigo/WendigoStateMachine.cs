
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WendigoStateMachine : MonoBehaviour
{

    public WendigoRaycast wendigoRaycasts;
    public wendigoRandomizedSpawner wendigoRandomizedSpawner;
    public PlayerLineofSight playerLineOfSight;
    public WendigoFollowPlayer wendigoFollowPlayer;

    public WendigoLookForPlayer wendigoLookForPlayer;

    public GameObject Wendigo;

    private float idleTimer;


    private GameManager gameManager;
    private enum State
    {
        Resting,
        Teleporting,
        Idle,
        Despawned,
        SpawnBehindPlayer,
        FollowingPlayer,
        LookingForPlayer,
        AttackPlayer

    }



    private State currentState;

    void Start()
    {
        currentState = State.Resting;
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

    }

    void Update()
    {        
        if(gameManager.isNight == false){
            currentState = State.Resting;
        }
        if(gameManager.safeArea == true){
            currentState = State.Resting;
        }

        switch (currentState)
        {
            case State.Resting:
                Resting();
                break;
            case State.Teleporting:
                Teleporting();
                break;
            case State.Idle:
                Idle();
                break;
            case State.Despawned:
                Despawned();
                break;
            case State.SpawnBehindPlayer:
                SpawnBehindPlayer();
                break;
            case State.FollowingPlayer:
                FollowingPlayer();
                break;
            case State.LookingForPlayer:
                LookingForPlayer();
                break;
            case State.AttackPlayer:
                AttackPlayer();
                break;
        }
    }

    private void Idle()
    {
        // Debug.Log("Idle");
        idleTimer += Time.deltaTime;
        if (wendigoRandomizedSpawner.playerSightings >= wendigoRandomizedSpawner.maxPlayerSightings)
        {
            currentState = State.SpawnBehindPlayer;
        }
        // Idle state logic until seen by player then switch to teleport
        if (playerLineOfSight.IsLookingAtWendigo(Wendigo.transform.position))
        {
            if (idleTimer >= wendigoRandomizedSpawner.maxStareTime)
            {
                wendigoRandomizedSpawner.DespawnWendigo();
                wendigoRandomizedSpawner.playerSightings++;
                Debug.Log("Player has seen Wendigo " + wendigoRandomizedSpawner.playerSightings + " times");
                currentState = State.Despawned;
                idleTimer = 0;

            }

        }
        else if (idleTimer > wendigoRandomizedSpawner.spawnTimer)
        {
            wendigoRandomizedSpawner.DespawnWendigo();
            idleTimer = 0;
            currentState = State.Despawned;

        }
    }

    private void Despawned()
    {
        // Debug.Log("Despawned");
        // Despawned state logic
        idleTimer += Time.deltaTime;
        playerLineOfSight.isLooking = false;
        if (idleTimer > wendigoRandomizedSpawner.spawnTimer)
        {
            idleTimer = 0;
            currentState = State.Teleporting;
        }
    }

    private void Resting()
    {
        // Debug.Log("Resting");
        wendigoRandomizedSpawner.DespawnWendigo();
        if(gameManager.isNight == true && gameManager.safeArea == false)
        {
            currentState = State.Teleporting;
        }
        
    }

    private void Teleporting()
    {



        // Debug.Log("Teleporting");
        wendigoRandomizedSpawner.SpawnWendigo();
        currentState = State.Idle;


        // Teleporting state logic


    }

    private void SpawnBehindPlayer()
    {
        Debug.Log("SpawnBehindPlayer");
        wendigoFollowPlayer.SpawnBehindPlayer();
        currentState = State.FollowingPlayer;
        // SpawnBehindPlayer state logic
    }

    private void FollowingPlayer()
    {
        Debug.Log("FollowingPlayer");
        wendigoFollowPlayer.FollowPlayer();
        // Follow player until player is out of sight
        if (wendigoFollowPlayer.attackPlayer)
        {
            Debug.Log("Attack Player");
            currentState = State.AttackPlayer;
        }
        else if (wendigoFollowPlayer.lostPlayer)
        {
            Debug.Log("Lost Player");
            currentState = State.LookingForPlayer;
        }

        // FollowingPlayer state logic
    }


    private void LookingForPlayer()
    {   
        
        idleTimer += Time.deltaTime;
        wendigoLookForPlayer.TrackFootsteps();
        if(wendigoLookForPlayer.trackingTimer > idleTimer)
        {   
            Debug.Log("Looking for player");	
            if(wendigoRaycasts.detected)
            {   
                Debug.Log("Player detected");
                currentState = State.FollowingPlayer;
            }
        }
        else
        {
            currentState = State.Idle;
        }

        // LookingForPlayer state logic
    }

    private void AttackPlayer()
    {
        Debug.Log("Attacking player");

        // AttackPlayer state logic
    }







}