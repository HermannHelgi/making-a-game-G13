
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WendigoSpawnPointTracker : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask wendigoLayer;
    [Header("SpawnPoints distance")]
    public float minChaseDistance = 45f;
    public float maxChaseDistance = 60f;
    public float minStalkDistance = 20f;
    public float maxStalkDistance = 40f;
    public Transform despawnPoint;
    private HashSet<GameObject> stalkPositions;
    private HashSet<GameObject> chasePositions;

    void Start()
    {
    }
    void Awake()
    {
        stalkPositions = new HashSet<GameObject>();
        chasePositions = new HashSet<GameObject>();
    }

    private GameObject SelectRandom(HashSet<GameObject> set)
    {
        if (set.Count > 0)
        {
            int index = Random.Range(0, set.Count);
            foreach (GameObject obj in set)
            {
                if (index == 0)
                {   
                    Debug.Log("Select random: "+ obj.name);
                    return obj;
                }
                index--;
            }
        }
        return null;
    }  

    private void UpdateSpawnPositions(HashSet<GameObject> spawns, float minDistance, float maxDistance)
    {
        HashSet<GameObject> toRemove = new HashSet<GameObject>();

        foreach(GameObject spawn in spawns)
        {   
            // Debug.Log(spawn.transform.position);
            if(spawn == null)
            {
                toRemove.Add(spawn);
            }
            float distance = Vector3.Distance(spawn.transform.position, playerCamera.transform.position);
            if (distance < minDistance || distance > maxDistance)
            {   
                toRemove.Add(spawn);
            }
        }
        foreach(GameObject remove in toRemove)
        {
            spawns.Remove(remove);
        }

        foreach (Collider collision in Physics.OverlapSphere(transform.position, maxDistance, wendigoLayer))
        {   
            if(!GameObjectWithinFrustum(collision.gameObject, playerCamera))
            {   
                spawns.Add(collision.gameObject);
            }
        }
        
    }

    private bool GameObjectWithinFrustum(GameObject go, Camera camera, float offset=20.0f)
    {
        Vector3 dir = go.transform.position - camera.transform.position;
        dir.y = 0;
        Vector3 cameraForward = camera.transform.forward;
        cameraForward.y = 0;
        float adjustedFOV = camera.fieldOfView + offset;
        return Vector3.Angle(dir.normalized , cameraForward) <= camera.fieldOfView + offset;
    }


    public bool GameObjectWithinFrustum(GameObject go, float offset=20.0f)
    {
        Vector3 dir = go.transform.position - playerCamera.transform.position;
        dir.y = 0;
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        float adjustedFOV = playerCamera.fieldOfView + offset;
        return Vector3.Angle(dir.normalized , cameraForward) <= playerCamera.fieldOfView + offset;
    }

    public GameObject SelectRandomSpawn(bool chasing)
    {   
        HashSet<GameObject> currentSpawnPosition;
        float currentMin;
        if(chasing)
        {
            currentSpawnPosition = chasePositions;
            currentMin = minChaseDistance;         
        }
        else
        {
            currentSpawnPosition = stalkPositions;
            currentMin = minStalkDistance;
        }
        HashSet<GameObject> potentialSpawns = new HashSet<GameObject>();
        foreach (GameObject wendigo in currentSpawnPosition)
        {
            Vector3 wendigoVector = wendigo.transform.position - playerCamera.transform.position;
            wendigoVector.y = 0;
            if (wendigoVector.magnitude < currentMin)
            {
                continue;
            }
            if (GameObjectWithinFrustum(wendigo, playerCamera, 35f))
            {
                potentialSpawns.Add(wendigo);
            }
        }
        if (potentialSpawns.Count > 0)
        {   
             
            return SelectRandom(potentialSpawns);
        }
        else 
        {
            return null;
        }
    }

    void Update()
    {   
        UpdateSpawnPositions(stalkPositions, minStalkDistance, maxStalkDistance);
        UpdateSpawnPositions(chasePositions, minChaseDistance, maxChaseDistance);
        // foreach(GameObject spawns in stalkPositions)
        // {
        //     Debug.Log("stalkingpoint: " + spawns.name);
        // }
        // foreach(GameObject chases in chasePositions)
        // {
        //     Debug.Log("chasepoint: " + chases.name);
        // }
    }
}