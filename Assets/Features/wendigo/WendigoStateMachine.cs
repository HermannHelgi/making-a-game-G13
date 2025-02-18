
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

    private float idleTimer;
    public float respawnTimer;
    
    public GameManager gameManager;
    private enum State
    {
        Resting,
        Teleporting,
        Idle,
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
        
        respawnTimer += Time.deltaTime;
        if(wendigoRandomizedSpawner.spawnInterval < respawnTimer)
        {
            
            
            wendigoRandomizedSpawner.DespawnWendigo();
            
        }
            currentState = State.Teleporting;
    }

    private void Resting()
    {
        if(gameManager.isNight)
        {
            currentState = State.Teleporting;
        }
    }

    private void Teleporting()
    {   
        // teleport once, start timer once it reaches 30 seconds teleport again

        if(wendigoRandomizedSpawner.playerSightings < wendigoRandomizedSpawner.maxPlayerSightings && respawnTimer >= wendigoRandomizedSpawner.spawnInterval)
        {   
            wendigoRandomizedSpawner.SpawnWendigo();
        }
        else
        {
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