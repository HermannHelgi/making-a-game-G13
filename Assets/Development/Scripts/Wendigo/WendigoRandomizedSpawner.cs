
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;


// public class wendigoRandomizedSpawner : MonoBehaviour
// {
//     public Transform player;
//     public GameObject wendigo;
//     public float spawnTimer;
//     public int playerSightings;
//     public int maxPlayerSightings = 3;
//     public Transform despawnPoint;
//     private HashSet<GameObject> spawnPositions;
//     public LayerMask wendigoLayer;
//     public LayerMask obstacleLayer;
//     [Header("SpawnPoints distance")]
//     public float minSpawnDistance;
//     // public float midSpawnDistance;
//     public float maxSpawnDistance;
//     private GameObject activeWendigo = null;
//     public float wendigoRadius = 0.55f; 

//     void Awake()
//     {   

        
//     }
//     void Start()
//     {   
//         spawnPositions = new HashSet<GameObject>();
//     }

//     void  FindWendigosWithinRange()
//     {   
//         HashSet<GameObject> temporaryWendigos = new HashSet<GameObject>();
//         foreach(GameObject spawn in spawnPositions)
//         {
//             float distance = Vector3.Distance(spawn.transform.position , player.transform.position);

//             if( distance >= maxSpawnDistance)
//             {   
//                 temporaryWendigos.Add(spawn);
//             }
//         }

//         foreach(GameObject potentialRemove in temporaryWendigos)
//         {
//             spawnPositions.Remove(potentialRemove);
//         }

//         foreach (Collider collision in Physics.OverlapSphere(transform.position, maxSpawnDistance,wendigoLayer))
//         {   
//             if(!GameObjectWithinFrustum(collision.gameObject, playerLineOfSight.playerCamera))
//             {   
//                 spawnPositions.Add(collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);
//             }
//         }
//     }

//     public Transform ReturnCurrentPosition()
//     {
//         if(activeWendigo != null)
//         {
//             return activeWendigo.transform;
//         }
//         return null;
//     }

//     bool GameObjectWithinFrustum(GameObject go, Camera camera, float offset = 15)
//     {
//         Vector3 dir = go.transform.position - camera.transform.position;
//         dir.y = 0;
//         Vector3 cameraForward = camera.transform.forward;
//         cameraForward.y = 0;
//         float adjustedFOV = camera.fieldOfView + offset;
//         return Vector3.Angle(dir.normalized , cameraForward) <= camera.fieldOfView + offset;
//         // return Vector3.Angle(dir.normalized , cameraForward) <= adjustedFOV / 2;
//     }

//     public bool isWendigoWithinFrustum()
//     {   if(activeWendigo != null)
//         {   
//         return GameObjectWithinFrustum(activeWendigo, playerLineOfSight.playerCamera);
//         }
//         return false;
//     }

//     GameObject SelectRandom(HashSet<GameObject> set)
//     {
//         if (set.Count > 0)
//         {
//             int index = Random.Range(0, set.Count);
//             foreach (GameObject obj in set)
//             {
//                 if (index == 0)
//                 {
//                     return obj;
//                 }
//                 index--;
//             }
//         }
//         return null;
//     }

//     void DeActivateWendigo()
//     {
//         SkinnedMeshRenderer mesh = activeWendigo.gameObject.GetComponent<SkinnedMeshRenderer>();
//         mesh.enabled = false;
//         activeWendigo = null;
//     }

//     void ActivateWendigo()
//     {
//         SkinnedMeshRenderer mesh = activeWendigo.gameObject.GetComponent<SkinnedMeshRenderer>();
//         mesh.enabled = true;
//     }

//     void UpdateActiveWendigo()
//     {
//         if (activeWendigo != null)
//         {   

//             if (!GameObjectWithinFrustum(activeWendigo, playerLineOfSight.playerCamera))
//             {
//                 DeActivateWendigo();
//             }
//             List<Vector3> pointArray = new List<Vector3>();
//             pointArray.Add(activeWendigo.transform.position);
//             pointArray.Add(activeWendigo.transform.position - (playerLineOfSight.playerCamera.transform.right * wendigoRadius));
//             pointArray.Add(activeWendigo.transform.position + (playerLineOfSight.playerCamera.transform.right * wendigoRadius));
//             bool allHit = true;
//             foreach(Vector3 point in pointArray)
//             {   
                
//                 Debug.DrawRay(playerLineOfSight.playerCamera.transform.position, point - playerLineOfSight.transform.position, Color.red);

//                 if(!Physics.Raycast(playerLineOfSight.playerCamera.transform.position, point - playerLineOfSight.transform.position,Mathf.Infinity , wendigoLayer))
//                 {
//                    allHit = false;
//                    break;
//                 } 
//             }
//             if(allHit)
//             {   
//                 playerSightings++;
//                 DeActivateWendigo();
//             }

             
//         }
//     }

//     void SelectNewActiveWendigo()
//     {   
//         Debug.Log("selecting new wendigo"); 
//         HashSet<GameObject> potentialSpawns = new HashSet<GameObject>();
//         foreach (GameObject wendigo in spawnPositions)
//         {
//             Vector3 wendigoVector = wendigo.transform.position - playerLineOfSight.playerCamera.transform.position;
//             wendigoVector.y = 0;
//             if (wendigoVector.magnitude < minSpawnDistance)
//             {
//                 continue;
//             }
//             if (GameObjectWithinFrustum(wendigo, playerLineOfSight.playerCamera, 50))
//             {
//                 potentialSpawns.Add(wendigo);
//             }
//         }
//         if (potentialSpawns.Count > 0)
//         {   
//             activeWendigo = SelectRandom(potentialSpawns);
//             ActivateWendigo();
//         }
//     }

//     void Update()
//     {   
//         FindWendigosWithinRange();
//         if(activeWendigo != null)
//         {   
//             Transform parentTransform = activeWendigo.GetComponentInParent<Transform>();
//             parentTransform.LookAt(player.transform);
//             activeWendigo.transform.forward = (playerLineOfSight.playerCamera.transform.position - activeWendigo.transform.position).normalized;
//         }

//         UpdateActiveWendigo();
//         if(activeWendigo == null)
//         {
//             SelectNewActiveWendigo();
//         }
//     }

//     public GameObject FindStartPoint()
//     {
//         return SelectRandom(spawnPositions);
//     }
    
//     public void SpawnWendigo()
//     {
        
        
//         if(activeWendigo == null)
//         {
//             SelectNewActiveWendigo();
//         }

//         if (activeWendigo != null)
//         {
//             Debug.Log("Wendigo spawned at: " + activeWendigo.transform.position);
//         }
//         else
//         {
//             Debug.LogWarning("No valid spawn points for Wendigo!");
//         }

//     }


//     public void DespawnWendigo()
//     {   
//         if(activeWendigo != null)
//         {
//             DeActivateWendigo();
//         }
       
//     }



// }