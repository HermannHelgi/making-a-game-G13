using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class WendigoLookForPlayer: MonoBehaviour
{
    public GameObject player;
    public float trackingTimer = 10.0f;
    private float smellTimer = 0.0f;
    public float markerTimer = 4.0f;
    private float trackingMarkerTimer = 0.0f;
    NavMeshAgent agent;
    public int numMarkers = 5;
    private Queue<Vector3> markers;
    public SoundManager soundManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        markers = new Queue<Vector3>();
    }

    public void TrackFootsteps()
    {   
        smellTimer += Time.deltaTime;
        if(smellTimer > trackingTimer)
        {   
            smellTimer = 0.0f;
            soundManager.PlayGroup("WENDIGO_TRACKING");
            agent.SetDestination(markers.Dequeue()); 
        }
    }

    public void MarkPlayerSighting()
    {
        markers.Clear();
        markers.Enqueue(player.transform.position);
    }

    public void Update()
    {
        trackingMarkerTimer -= Time.deltaTime;
        if (trackingMarkerTimer < 0.0)
        {
            trackingMarkerTimer = markerTimer;
            markers.Enqueue(player.transform.position);
            while (markers.Count > numMarkers)
            {
                markers.Dequeue();
            }
        }
    }
}