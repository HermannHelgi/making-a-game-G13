using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.AI;

public class WendigoLookForPlayer: MonoBehaviour
{
    private Transform playerFootsteps;
    public GameObject Wendigo;
    public WendigoFollowPlayer wendigoFollowPlayer;

    public float trackingTimer = 5f;

    NavMeshAgent agent;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    public void TrackFootsteps()
    {   
        float smellTimer = 0f;
        smellTimer += Time.deltaTime;
        if(smellTimer >  trackingTimer)
        {
            smellTimer = 0f;
            playerFootsteps = wendigoFollowPlayer.wendigoRaycast.player.transform;
            GoToTracks();
        }
        
    }


    private void GoToTracks()
    {
        
        wendigoFollowPlayer.agent.SetDestination(playerFootsteps.position);

        wendigoFollowPlayer.agent.speed = 15f;
         

    }




}