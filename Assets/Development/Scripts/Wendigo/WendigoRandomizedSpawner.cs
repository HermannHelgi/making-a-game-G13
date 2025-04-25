
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
    // public GameObject wendigoPrefab;

    // public float maxStareTime = 10f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
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
    private List<GameObject> activeWendigos = new List<GameObject>();
    private GameObject activeWendigo = null;
    private int maxPossibleSpawns;
    public float wendigoRadius = 0.75f;

    // private SkinnedMeshRenderer wendigoMesh;

    // public float heightOffset = 1.84f;  

    void Awake()
    {   

        
    }
    void Start()
    {   
        // capsuleCollider = wendigoPrefab.GetComponent<CapsuleCollider>();
        // wendigoMesh = wendigoPrefab.GetComponent<SkinnedMeshRenderer>();
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
                Debug.Log(spawn);
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
                Debug.Log(collision);
                possibleSpawns.Add(collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
            }
        }
    }

    bool GameObjectWithinFrustum(GameObject go, Camera camera)
    {
        Vector3 dir = go.transform.position - camera.transform.position;
        dir.y = 0;
        Vector3 cameraForward = camera.transform.forward;
        cameraForward.y = 0;
        return Vector3.Angle(dir, cameraForward) <= camera.fieldOfView;
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
            pointArray.Add(activeWendigo.transform.position - (playerLineOfSight.transform.right * wendigoRadius));
            pointArray.Add(activeWendigo.transform.position + (playerLineOfSight.transform.right * wendigoRadius));
            bool allHit = true;
            foreach(Vector3 point in pointArray)
            {   
                
                Debug.DrawRay(playerLineOfSight.playerCamera.transform.position, point - playerLineOfSight.transform.position, Color.red);

                if(!Physics.Raycast(playerLineOfSight.playerCamera.transform.position, (point - playerLineOfSight.transform.position),  wendigoLayer))
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
        UpdateActiveWendigo();
        if(activeWendigo == null)
        {
            SelectNewActiveWendigo();
        }
    }

    public void CheckAndDespawnVisibleWendigos()
    {
        foreach (GameObject wendigo in activeWendigos)
        {
            if (playerLineOfSight.IsLookingAtWendigo(wendigo.transform.position))
            {
                foreach (GameObject w in activeWendigos)
                {
                    SkinnedMeshRenderer mesh = w.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (mesh != null) mesh.enabled = false;
                }
                activeWendigos.Clear(); // reset
                break;
            }
        }
    }

    public void SpawnWendigo()
    {
        // if (player == null) return;

        // possibleSpawns = new List<GameObject>();
        // activeWendigos = new List<GameObject>(); // Reset previous

        // foreach (GameObject spawnpoint in spawnPositions)
        // {
        //     float distance = Vector3.Distance(spawnpoint.transform.position, player.position);

        //     if (distance >= spawnDistance && !playerLineOfSight.IsLookingAtWendigo(spawnpoint.transform.position))
        //     {
        //         possibleSpawns.Add(spawnpoint);
        //     }
        // }

        // int numberToSpawn = Mathf.Min(3, possibleSpawns.Count); // spawn up to 3
        // for (int i = 0; i < numberToSpawn; i++)
        // {
        //     int index = Random.Range(0, possibleSpawns.Count);
        //     GameObject chosenSpawn = possibleSpawns[index];
        //     possibleSpawns.RemoveAt(index);

        //     SkinnedMeshRenderer mesh = chosenSpawn.GetComponentInChildren<SkinnedMeshRenderer>();
        //     if (mesh != null)
        //     {
        //         mesh.enabled = true;
        //         chosenSpawn.transform.forward = -player.transform.forward;
        //         activeWendigos.Add(chosenSpawn);
        //     }
        // }
    }

    // public void SpawnWendigo()
    // {   
        
    //     if (player == null) return;

    //     // if (capsuleCollider != null)
    //     // {
    //     //     // capsuleCollider.enabled = true;
    //     // }

    //     // Vector3 spawnpoint = FindValidSpawnPosition();

    //     // if (spawnpoint != Vector3.zero)
    //     // {
    //     //     wendigoPrefab.transform.position = spawnpoint;
    //     // }
        
    //     foreach (GameObject spawnpoint in spawnPositions)
    //     {   
    //         Debug.Log("Iterating through points");
    //         float distance = Vector3.Distance(spawnpoint.transform.position, player.position);
    //         if (distance >= spawnDistance)
    //         {   
    //             if(!playerLineOfSight.IsLookingAtWendigo(spawnpoint.transform.position)){
    //                 possibleSpawns.Add(spawnpoint);
    //             }

    //         }
    //     }
    //     foreach (GameObject possibleSpawn in possibleSpawns)
    //     {   
    //         Debug.Log("Iterating  near points");
    //         SkinnedMeshRenderer mesh = possibleSpawn.GetComponentInChildren<SkinnedMeshRenderer>();
    //         mesh.enabled = true;
    //         possibleSpawn.transform.forward = -player.transform.forward;
    //     }

    // }

    public void DespawnWendigo()
    {
        
        // if (wendigoPrefab == null) return;


            // if (capsuleCollider != null)
            // {   
                // wendigoPrefab.transform.position = despawnPoint.position;
                // capsuleCollider.enabled = false;
                // Debug.Log("Despawning Wendigo.");
            // }        
    }

    // private Vector3 FindValidSpawnPosition()
    // {   
    //     if (player == null) return Vector3.zero;
        
    //     Vector3 spawnPosition;
    //     int attempts = 0;
    //     // Debug.Log("Finding valid spawn position");
    //     while (attempts < 15)
    //     {   
            
    //         spawnPosition = player.position  + Random.insideUnitSphere * 80f;
    //         spawnPosition.y -= heightOffset;
    //         float distance = Vector3.Distance(player.position, spawnPosition);
    //         // Debug.Log("Checking point: " + spawnPosition + " Distance: " + distance);	
    //         wendigoPrefab.transform.forward = -player.transform.forward;
    //         if (distance < minSpawnDistance || distance > maxSpawnDistance)
    //         {   
    //             // Debug.Log("Distance is too close or too far");
    //             attempts++;
    //             continue;
    //         }

    //         if(HasLineOfSight(wendigoPrefab.transform.position, spawnPosition, maxSpawnDistance))
    //         {   
    //             // Debug.Log("No line of sight to player");
    //             attempts++;
    //             continue;
    //         }
    //         RaycastHit hit;
    //         if (Physics.Raycast(spawnPosition + Vector3.up * 200, Vector3.down, out hit, 400, groundLayer))
    //         {
    //             spawnPosition = hit.point;
    //             spawnPosition.y = hit.point.y + heightOffset;
    //             // Debug.Log("Spawn position: " + spawnPosition);
    //             return spawnPosition;
    //         }
    //         attempts++;
    //     }
    //     return Vector3.zero;
    // }

    private void FindValidSpawnPosition()
    {
        if (player == null) return ;
        
        
        
    }

    // private bool HasLineOfSight(Vector3 fromPos, Vector3 toPos, float maxDist)
    // {
    //     Vector3 dir = (toPos - fromPos).normalized;
    //     if (Physics.Raycast(fromPos, dir, out RaycastHit hit, maxDist))
    //     {
    //         return hit.transform.CompareTag("Player");
    //         Debug.Log("has line of sight");
    //     }
    //     return false;
    // }


}