using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.AI;

public class WendigoLookForPlayer: MonoBehaviour
{
    List<Transform> playerFootsteps = new List<Transform>();
    public WendigoRaycast wendigoRaycast;
    public GameObject Wendigo;

    public float trackingTimer = 15f;
    private int stepscounter = 0;

    NavMeshAgent agent;


    private void Start()
    {
        StartCoroutine(GoToTracks());
        agent = GetComponent<NavMeshAgent>();
        stepscounter = 0;
        playerFootsteps.Clear();
    }

    public void TrackFootsteps()
    {   

        wendigoRaycast.player.transform.position = playerFootsteps[stepscounter].position;
        stepscounter++;
        
    }


    private IEnumerator GoToTracks()
    {
        TrackFootsteps();
        WaitForSeconds wait = new WaitForSeconds(2f);

        while (true)
        {
            yield return wait;
            TrackFootsteps();
            agent.SetDestination(playerFootsteps[stepscounter].position);
            
        }
    }




}