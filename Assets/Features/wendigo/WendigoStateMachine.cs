
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

    public float respawnTimer;
    
    public GameManager gameManager;
    private enum State
    {
        Resting,
        Teleporting,
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
        respawnTimer += Time.deltaTime;

        if(wendigoRandomizedSpawner.playerSightings < wendigoRandomizedSpawner.maxPlayerSightings && respawnTimer >= wendigoRandomizedSpawner.spawnInterval)
        {   
            wendigoRandomizedSpawner.DespawnWendigo();
            wendigoRandomizedSpawner.SpawnWendigo();
            respawnTimer = 0;
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