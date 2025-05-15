
using UnityEngine;
using UnityEngine.AI;


public class WendigoEffigy : WendigoBehaviour
{   
    public Transform wendigoEffigyPoint;
    public Transform spawnLocation;
    public Animator myAnimator;
    public Transform cavePoint;
    public GameObject player;
    private float aggressionRange = 8.0f;
    private bool reachedEffigy ;
    public NavMeshAgent agent;
    public WendigoFollowPlayer spawner;
    public bool inAggressionRange;
    private bool goingToEffigy = false;

    void Awake()
    {
        
    }
    public override void EnterState()
    {
        base.EnterState();
        agent.enabled = true;
        reachedEffigy = false;
        inAggressionRange = false;
        SpawnWendigo(spawnLocation);
    }

    public void SpawnWendigo(Transform potentialLocation)
    {
        if(NavMesh.SamplePosition(potentialLocation.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            transform.position = potentialLocation.position;
            transform.forward = (spawner.player.transform.position - transform.position).normalized;
            agent.Warp(hit.position);
            Debug.Log("Spawning at : " + potentialLocation);         
            
        }
    }

    void goToEffigy()
    {   

        Debug.Log("going to effigy");
        agent.SetDestination(wendigoEffigyPoint.position);
        goingToEffigy = true;
    }
    public override void Run()
    {
        if (isActive)
        {   


            Debug.Log("checking effigy status");
            if(!goingToEffigy)
            {   
                goToEffigy();
            }
            if(reachedEffigy)
            {
                myAnimator.SetBool("effigy", true);
            }
            if (Vector3.Distance(player.transform.position, agent.transform.position) < aggressionRange)
            {       
                inAggressionRange = true;
                isEnding = true;
                Debug.Log("Exiting effigy state");
            }
            if(Vector3.Distance(wendigoEffigyPoint.position, agent.transform.position) <= 1.0f)
            {   

                reachedEffigy = true;
            }
            if(GameManager.instance.skullPickedUp)
            {
                isEnding = true;
            }
            // if(GameManager.instance.dangerZone)
            // {
            //     SpawnWendigo(cavePoint);
            //     inAggressionRange = true;
            //     isEnding = true;
            // }
            
        }
        if (isEnding)
        {   
            myAnimator.SetBool("effigy", false);
            isEnding = false;
        }
    }
}
