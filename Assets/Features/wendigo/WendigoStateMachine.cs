
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

    public GameObject Wendigo;

    private float idleTimer;

    
    private GameManager gameManager = GameManager.instance;
    private enum State
    {
        Resting,
        Teleporting,
        Idle,
        Despawned,
        FollowingPlayer,
        LookingForPlayer,
        AttackPlayer

    }

    private State currentState;

    void Start()
    {
        currentState = State.Resting;

    }

    void Update()
    {
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
        Debug.Log("Idle");
        idleTimer += Time.deltaTime;
        // Idle state logic until seen by player then switch to teleport
        if(playerLineOfSight.IsLookingAtWendigo(Wendigo.transform.position))
        {   
            if(idleTimer >= wendigoRandomizedSpawner.maxStareTime){
                wendigoRandomizedSpawner.DespawnWendigo();
                wendigoRandomizedSpawner.playerSightings++;
                Debug.Log("Player has seen Wendigo " + wendigoRandomizedSpawner.playerSightings + " times");
                currentState = State.Despawned;
                idleTimer = 0;

            }
        
        }
        else if(idleTimer > wendigoRandomizedSpawner.spawnTimer)
        {   
            wendigoRandomizedSpawner.DespawnWendigo();
            idleTimer = 0;
            currentState = State.Despawned;

        }
    }

    private void Despawned()
    {
        Debug.Log("Despawned");
        // Despawned state logic
        idleTimer += Time.deltaTime;
        playerLineOfSight.isLooking = false;
        if(idleTimer > wendigoRandomizedSpawner.spawnTimer)
        {
            idleTimer = 0;
            currentState = State.Teleporting;
        }
    }

    private void Resting()
    {   
        Debug.Log("Resting");
        if(gameManager.isNight)
        {
            currentState = State.Teleporting;
        }
    }

    private void Teleporting()
    {   
        
        // teleport once, start timer once it reaches 30 seconds teleport again
        if(wendigoRandomizedSpawner.playerSightings < wendigoRandomizedSpawner.maxPlayerSightings)
        {   
            Debug.Log("Teleporting");
            wendigoRandomizedSpawner.SpawnWendigo();
            currentState = State.Idle;
        }
        else
        {   
            Debug.Log("going to follow");
            // currentState = State.FollowingPlayer;
        }
        // Teleporting state logic
           

    }

    private void FollowingPlayer()
    {
        // FollowingPlayer state logic
    }


    private void LookingForPlayer()
    {
        // LookingForPlayer state logic
    }

    private void AttackPlayer()
    {
        // AttackPlayer state logic
    }


    private bool IsPlayerVisible()
    {
        // Check if player is visible
        return false;
    }

    private bool IsPlayerInRange()
    {
        // Check if player is in range
        return false;
    }





}