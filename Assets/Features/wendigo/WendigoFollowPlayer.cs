using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.AI;

public class WendigoFollowPlayer : MonoBehaviour
{
    public AudioSource staticSound;

    public WendigoRaycast wendigoRaycast;
    public Transform wendigoTransform;
    public float speed = 5f;
    public float caughtDistance = 2f;
    public float speedMultiplier = 1.5f;
    public float maxSpeed = 10f;

    public bool lostPlayer = false;
    public bool attackPlayer = false;

    NavMeshAgent agent;

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
            if (Vector3.Distance(wendigoRaycast.player.transform.position, wendigoTransform.position) <= caughtDistance)
            {
                attackPlayer = true;
            }
            Debug.Log("Following Player");
            agent.SetDestination(wendigoRaycast.player.transform.position);
            agent.speed = speed * speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, speed, maxSpeed);

        }
        else if (!wendigoRaycast.detected)
        {
            Debug.Log("Lost Player");
            agent.SetDestination(wendigoRaycast.lastKnownPosition);
            agent.acceleration = speed / speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, speed, maxSpeed);
            if(Vector3.Distance(wendigoRaycast.lastKnownPosition, wendigoTransform.position) < 1f)
            {
                lostPlayer = true;
            }

            
        }


    }

    public void SpawnBehindPlayer()
    {
        float sampleRadius = 5f;
        Vector3 behindPlayer = wendigoRaycast.player.transform.position - wendigoRaycast.player.transform.forward * 50f;
        PlayChaseMusic();
        if (NavMesh.SamplePosition(behindPlayer, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            transform.position = hit.position + Vector3.up * 2f;
            transform.forward = wendigoRaycast.player.transform.forward;
            wendigoTransform.forward = wendigoRaycast.player.transform.forward;
            agent.Warp(transform.position);
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
