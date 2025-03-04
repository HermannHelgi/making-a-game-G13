using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.AI;
using Unity.VisualScripting;

public class WendigoFollowPlayer : MonoBehaviour
{
    public AudioSource staticSound;

    public WendigoRaycast wendigoRaycast;
    public Transform wendigoTransform;
    public float speed = 5f;
    public float caughtDistance = 3f;
    public float speedMultiplier = 1.5f;
    public float maxSpeed = 10f;

    public bool lostPlayer = false;
    public bool attackPlayer = false;

    public NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        
    }

    private void Update()
    {

    }



    public void FollowPlayer()
    {
        // if(agent.isOnNavMesh == false)
        // {
        //     agent.enabled = true;
        // }
        SetVolumeIncrease();
        if (wendigoRaycast.detected)
        {   
            float distance = Vector3.Distance(wendigoRaycast.player.transform.position, wendigoTransform.position);
            // Debug.Log("Distance to player: " + distance);
            if (distance <= caughtDistance)
            {   
                Debug.Log("Attacking playing");
                attackPlayer = true;
            }
            // Debug.Log("Following Player");
            agent.SetDestination(wendigoRaycast.player.transform.position);
            agent.speed = speed * speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, speed, maxSpeed);



        }
        else if (!wendigoRaycast.detected)
        {
            // Debug.Log("Lost Player");
            agent.SetDestination(wendigoRaycast.lastKnownPosition);
            agent.acceleration = speed / speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, speed, maxSpeed);
            
            if(wendigoTransform.position ==  wendigoRaycast.lastKnownPosition)
            {
                lostPlayer = true;
            }


            
        }


    }

    public void SpawnBehindPlayer()
    {
        float sampleRadius = 5f;
        Vector3 behindPlayer = wendigoRaycast.player.transform.position - wendigoRaycast.player.transform.forward * 60f;
        PlayChaseMusic();
        if (NavMesh.SamplePosition(behindPlayer, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            wendigoTransform.forward = -wendigoRaycast.player.transform.forward;
            
            agent.Warp(hit.position);
        }
    }


    private void PlayChaseMusic()
    {
        if (!lostPlayer)
        {
            PlayMusic();
        }
        else
        {
            StopMusic();
        }


    }

    void PlayMusic()
    {

        staticSound.Play();


    }

    void StopMusic()
    {

        staticSound.Stop();


    }

    public void SetVolumeIncrease()
    {
        staticSound.volume = 80 - agent.remainingDistance;

    }

}
