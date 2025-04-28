
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.FullSerializer;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor;

public class wendigoRandomizedSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject wendigo;
    public PlayerLineofSight playerLineOfSight;
    public float spawnTimer;
    public int playerSightings;
    public int maxPlayerSightings = 3;
    public Transform despawnPoint;
    private HashSet<GameObject> spawnPositions;
    public LayerMask wendigoLayer;
    [Header("SpawnPoints distance")]
    public float minSpawnDistance;
    // public float midSpawnDistance;
    public float maxSpawnDistance;
    private List<GameObject> possibleSpawns;
    private GameObject activeWendigo = null;
    public float wendigoRadius = 0.75f;

    // private SkinnedMeshRenderer wendigoMesh;

    // public float heightOffset = 1.84f;  

    void Awake()
    {   

        
    }
    void Start()
    {   
        possibleSpawns = new List<GameObject>();
    }

    void  FindWendigosWithinRange()
    {   
        List<GameObject> temporaryWendigos = new List<GameObject>();
        foreach(GameObject spawn in possibleSpawns)
        {
            float distance = Vector3.Distance(spawn.transform.position , player.transform.position);

            if( distance >= maxSpawnDistance)
            {   
                temporaryWendigos.Add(spawn);
            }
        }

        foreach(GameObject potentialRemove in temporaryWendigos)
        {
            possibleSpawns.Remove(potentialRemove);
        }

        foreach (Collider collision in Physics.OverlapSphere(transform.position, maxSpawnDistance,wendigoLayer))
        {   
            if(!GameObjectWithinFrustum(collision.gameObject, playerLineOfSight.playerCamera))
            {   
                possibleSpawns.Add(collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            }
        }
    }

    public Transform ReturnCurrentPosition()
    {
        if(activeWendigo != null)
        {
            return activeWendigo.transform;

        }
        return null;
    }

    bool GameObjectWithinFrustum(GameObject go, Camera camera)
    {
        Vector3 dir = go.transform.position - camera.transform.position;
        dir.y = 0;
        Vector3 cameraForward = camera.transform.forward;
        cameraForward.y = 0;
        float adjustedMagnitude = dir.magnitude + 10;
        return Vector3.Angle(dir.normalized * adjustedMagnitude, cameraForward) <= camera.fieldOfView;
    }

    public bool isWendigoWithinFrustum()
    {   if(activeWendigo != null)
        {   
        return GameObjectWithinFrustum(activeWendigo, playerLineOfSight.playerCamera);
        }
        return false;
    }

    GameObject SelectRandom(List<GameObject> list)
    {
        if (list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        else
        {
            return null;
        }
    }

    void DeActivateWendigo()
    {
        SkinnedMeshRenderer mesh = activeWendigo.gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh.enabled = false;
        activeWendigo = null;
    }

    void ActivateWendigo()
    {
        SkinnedMeshRenderer mesh = activeWendigo.gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh.enabled = true;
    }
    void UpdateActiveWendigo()
    {
        if (activeWendigo != null)
        {   

            if (!GameObjectWithinFrustum(activeWendigo, playerLineOfSight.playerCamera))
            {
                DeActivateWendigo();
            }
            List<Vector3> pointArray = new List<Vector3>();
            pointArray.Add(activeWendigo.transform.position);
            pointArray.Add(activeWendigo.transform.position - (playerLineOfSight.playerCamera.transform.right * wendigoRadius));
            pointArray.Add(activeWendigo.transform.position + (playerLineOfSight.playerCamera.transform.right * wendigoRadius));
            bool allHit = true;
            foreach(Vector3 point in pointArray)
            {   
                
                Debug.DrawRay(playerLineOfSight.playerCamera.transform.position, point - playerLineOfSight.transform.position, Color.red);

                if(!Physics.Raycast(playerLineOfSight.playerCamera.transform.position, point - playerLineOfSight.transform.position,Mathf.Infinity , wendigoLayer))
                {
                   allHit = false;
                   break;
                } 
            }
            if(allHit)
            {   
                DeActivateWendigo();
            }

             
        }
    }

    void SelectNewActiveWendigo()
    {   
        Debug.Log("selecting new wendigo"); 
        List<GameObject> potentialSpawns = new List<GameObject>();
        foreach (GameObject wendigo in possibleSpawns)
        {
            Vector3 wendigoVector = wendigo.transform.position - playerLineOfSight.playerCamera.transform.position;
            wendigoVector.y = 0;
            if (wendigoVector.magnitude < minSpawnDistance)
            {
                continue;
            }
            if (GameObjectWithinFrustum(wendigo, playerLineOfSight.playerCamera))
            {
                potentialSpawns.Add(wendigo);
            }
        }
        if (potentialSpawns.Count > 0)
        {   
            activeWendigo = SelectRandom(potentialSpawns);
            ActivateWendigo();
        }
    }

    void Update()
    {   
        FindWendigosWithinRange();
        if(activeWendigo != null)
        {
            activeWendigo.transform.forward = (playerLineOfSight.playerCamera.transform.position - activeWendigo.transform.position).normalized;
        }

        // UpdateActiveWendigo();
        // if(activeWendigo == null)
        // {
        //     SelectNewActiveWendigo();
        // }
    }

    public GameObject FindStartPoint()
    {
        return SelectRandom(possibleSpawns);
    }


    public void SpawnWendigo()
    {
        
        UpdateActiveWendigo();
        if(activeWendigo == null)
        {
            SelectNewActiveWendigo();
        }

        if (activeWendigo != null)
        {
            Debug.Log("Wendigo spawned at: " + activeWendigo.transform.position);
        }
        else
        {
            Debug.LogWarning("No valid spawn points for Wendigo!");
        }

    }


    public void DespawnWendigo()
    {   
        if(activeWendigo != null)
        {
            DeActivateWendigo();
        }
       
    }



}