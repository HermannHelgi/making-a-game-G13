using UnityEngine;

using UnityEngine.AI;

public class WendigoFollowPlayer : MonoBehaviour
{
    public WendigoRaycast wendigoRaycast;
    public Transform wendigoTransform;
    public float speed = 5f;
    public float caughtDistance = 3f;
    public float speedMultiplier = 1.5f;
    public float maxSpeed = 10f;

    public bool lostPlayer = false;
    public bool attackPlayer = false;

    public NavMeshAgent agent;
    private wendigoRandomizedSpawner spawnerLogic;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;


    }

    void Awake()
    {
        spawnerLogic = FindFirstObjectByType<wendigoRandomizedSpawner>();

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
        // SetVolumeIncrease();
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
            float distance = Vector3.Distance(wendigoTransform.position, wendigoRaycast.lastKnownPosition);
            if (distance <= caughtDistance)
            {
                lostPlayer = true;

            }
            agent.SetDestination(wendigoRaycast.lastKnownPosition);
            agent.acceleration = speed / speedMultiplier;
            agent.speed = Mathf.Clamp(agent.speed, speed, maxSpeed);



        }


    }

    public void SpawnBehindPlayer()
    {

        float sampleRadius = 10f;
        GameObject spawnPoint = spawnerLogic.FindStartPoint();
        // Ray spawnspot =  
        // PlayChaseMusic();
        if (NavMesh.SamplePosition(spawnPoint.transform.position, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            wendigoTransform.position = spawnPoint.transform.position;
            wendigoTransform.forward = -wendigoRaycast.player.transform.forward;
            agent.Warp(hit.position);
        }
    }

    
}
