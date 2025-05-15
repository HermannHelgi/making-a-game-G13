using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

using UnityEngine.AI;

public class WendigoFollowPlayer : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    public WendigoSpawnPointTracker spawnPointTracker;
    public List<GameObject> retreatPositions;
    public GameObject selectedRetreat;
    public float retreatDistance;
    public SoundManager soundManager;
    public bool justSpawned;
    private float repathInterval = 0.15f;
    private float nextRepath;
    private Transform target;
    private Vector3 playerVelocity;
    private StalkingBehaviour stalkingBehaviour;
    // private AudioSource source;
    public AudioClip audioClip;
    private float internalDelay;
    public AudioSource spawnAudioSource;
    Animator myAnimator;

    private void Awake()
    {
        if (player == null)
        player = GameObject.FindGameObjectWithTag("Player");

        if (spawnPointTracker == null)
        {
            spawnPointTracker = FindFirstObjectByType<WendigoSpawnPointTracker>();
        }

        if (soundManager == null)
        {
        soundManager = FindFirstObjectByType<SoundManager>();
        }
        // if(source == null)
        // {
        //     source = GetComponentInParent<AudioSource>();
        // }

        if ( stalkingBehaviour == null)
        {
            stalkingBehaviour = FindAnyObjectByType<StalkingBehaviour>();
        }

        if (myAnimator == null)
        {
            myAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        selectedRetreat = null;

        
       
    }


    public void FollowPlayer()
    {
        selectedRetreat = null;
        target = player.transform;            
        if (Time.time < nextRepath) return;   
        nextRepath = Time.time + repathInterval;

        // === predictive intercept ===
        Vector3 toTarget = target.position - transform.position;
        Vector3 v = target.GetComponent<CharacterController>().velocity;        // player velocity
        float speedSquared = agent.speed * agent.speed;
        float a = Vector3.Dot(v, v) - speedSquared;
        float b = 2f * Vector3.Dot(toTarget, v);
        float c = Vector3.Dot(toTarget, toTarget);
        float t = SolveSmallestPositiveRoot(a, b, c);   
        Vector3 intercept = (t > 0f) ? target.position + v * t : target.position;
        // Vector3 intercept;
        agent.SetDestination(intercept);
        // soundManager.PlayGroup("WENDIGO_WALKING");
    }

    float SolveSmallestPositiveRoot(float a, float b, float c)
    {
        // Handle near‑linear case to avoid dividing by ~0
        if (Mathf.Abs(a) < 1e-6f)
        {
            if (Mathf.Abs(b) < 1e-6f)          
                return -1f;

            float t = -c / b;                 
            return t > 0f ? t : -1f;
        }

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f)                
            return -1f;

        float sqrtD = Mathf.Sqrt(discriminant);
        float denom = 2f * a;

        float t1 = (-b - sqrtD) / denom;      
        float t2 = (-b + sqrtD) / denom;      

        bool t1Positive = t1 > 0f;
        bool t2Positive = t2 > 0f;

        if (t1Positive && t2Positive)        
            return Mathf.Min(t1, t2);
        if (t1Positive)
            return t1;
        if (t2Positive)
            return t2;

        return -1f;                           
    }

    public bool SpawnBehindPlayer()
    {
        selectedRetreat = null;
        GameObject potentialSpawn = spawnPointTracker.SelectRandomSpawn(true);
        if(potentialSpawn != null)
        {
            SpawnBehindPlayer(potentialSpawn.transform.position);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void SpawnBehindPlayer(Vector3 spawnPoint, float sampleRadius=10.0f)
    {   
        if(spawnPoint == null)
        {
            return;
        }
        if(NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            transform.position = spawnPoint;
            transform.forward = (player.transform.position - transform.position).normalized;
            agent.Warp(hit.position);
            nextRepath = 0f;
            justSpawned = true;
            Debug.Log("Spawning at : " + spawnPoint);

            spawnAudioSource.PlayOneShot(audioClip);
            
        }
    }
    public void Retreat()
    {   

        foreach(GameObject spot in retreatPositions)
        {
            float distance = Vector3.Distance(spot.transform.position, transform.position);
            if (distance >= retreatDistance)
            {
                selectedRetreat = spot;
                break;
            }
        }
    
    }

    void Update()
    {
        float currentAgentForwardSpeed = Vector3.Dot(agent.velocity.normalized, agent.transform.forward) * agent.velocity.magnitude / agent.speed;
        myAnimator.SetFloat("speed", currentAgentForwardSpeed);
        // myAnimator.applyRootMotion = currentAgentForwardSpeed <= 0.1f;
        
        if(currentAgentForwardSpeed <= 0.1f)
        {   
            // myAnimator.applyRootMotion = true;
            myAnimator.speed = 1f;
            // soundManager.PlayGroup("WENDIGO_WALKING");

        }
        else if (currentAgentForwardSpeed > 0.1f && currentAgentForwardSpeed <= 0.5f)
        {
            // myAnimator.applyRootMotion = false;
            myAnimator.speed = currentAgentForwardSpeed / 0.5f;
            soundManager.PlayGroup("WENDIGO_WALKING");
        }
        else if( currentAgentForwardSpeed > 0.5)
        {
            // myAnimator.applyRootMotion = false;
            soundManager.PlayGroup("WENDIGO_WALKING");
            myAnimator.speed = currentAgentForwardSpeed;
        }

    }

}
