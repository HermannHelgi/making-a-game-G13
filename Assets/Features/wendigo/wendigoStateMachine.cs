
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class wendigoStateMachine : MonoBehaviour
{   

    wendigoRaycast wendigoRaycast;
    wendigoRandomizedSpawner wendigoRandomizedSpawner;
    playerLineofSight playerLineOfSight;
    
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
        // Resting state logic
    }

    private void Teleporting()
    {
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