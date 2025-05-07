
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WendigoSpawnPointTracker : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask wendigoLayer;
    [Header("SpawnPoints distance")]
    public float minSpawnDistance;
    public float maxSpawnDistance;
    public Transform despawnPoint;
    private HashSet<GameObject> spawnPositions;

    void Start()
    {
    }
    void Awake()
    {
        spawnPositions = new HashSet<GameObject>();
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
                    return obj;
                }
                index--;
            }
        }
        return null;
    }


    private void UpdateSpawnPositions()
    {
        HashSet<GameObject> toRemove = new HashSet<GameObject>();
        foreach(GameObject spawn in spawnPositions)
        {   
            // Debug.Log(spawn.transform.position);
            if(spawn == null)
            {
                toRemove.Add(spawn);
            }
            float distance = Vector3.Distance(spawn.transform.position, playerCamera.transform.position);
            if (distance < minSpawnDistance || distance > maxSpawnDistance)
            {   
                toRemove.Add(spawn);
            }
        }
        foreach(GameObject potentialRemove in toRemove)
        {
            spawnPositions.Remove(potentialRemove);
        }

        foreach (Collider collision in Physics.OverlapSphere(transform.position, maxSpawnDistance, wendigoLayer))
        {   
            if(!GameObjectWithinFrustum(collision.gameObject, playerCamera))
            {   
                spawnPositions.Add(collision.gameObject);
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

    public GameObject SelectRandomSpawn()
    {
        HashSet<GameObject> potentialSpawns = new HashSet<GameObject>();
        foreach (GameObject wendigo in spawnPositions)
        {
            Vector3 wendigoVector = wendigo.transform.position - playerCamera.transform.position;
            wendigoVector.y = 0;
            if (wendigoVector.magnitude < minSpawnDistance)
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
        UpdateSpawnPositions();
    }
}