using System.Collections.Generic;
using System.Threading;

using UnityEngine;


public class StalkingBehaviour : WendigoBehaviour
{
    public WendigoSpawnPointTracker spawnPointTracker;
    
    public LayerMask obstacleLayer;
    public float spawnCooldown = 5.0f;
    public float wendigoRadius = 0.55f; 
    public int playerSightings = 0;
    public float sightThreshold = 2.0f;

    private GameObject activeWendigo = null;
    private float spawnTimer ;
    [SerializeField] private float sightTimer = 0.0f;
    private bool seen = false;

    void Awake()
    {
        spawnTimer = spawnCooldown;
    }
    public override void EnterState()
    {
        base.EnterState();
        playerSightings = 0;
        spawnTimer = spawnCooldown;
    }

    public override void ExitState()
    {
        base.ExitState();
        playerSightings = 0;
    }
    public Vector3 ReturnCurrentPosition()
    {   
        if(activeWendigo != null)
        {
            return activeWendigo.transform.position;
        }
        return Vector3.zero;
    }

    public void DespawnWendigo()
    {
        DeActivateWendigo();
    }

    void DeActivateWendigo()
    {
        // SkinnedMeshRenderer mesh = activeWendigo.GetComponent<SkinnedMeshRenderer>();
        // mesh.enabled = false;
        SkinnedMeshRenderer mesh = FindSkinnedMeshRenderer(activeWendigo);
        if (mesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on activeWendigo or its children!");
            return;
        }   
        mesh.enabled = false;
        activeWendigo = null;
        spawnTimer = spawnCooldown;
        sightTimer = 0.0f;
    }

    void ActivateWendigo()
    {   
        Debug.Log("activating wendigo at: " + activeWendigo.transform.position + "named " + activeWendigo.name);
        // SkinnedMeshRenderer mesh = activeWendigo.GetComponent<SkinnedMeshRenderer>();
        // mesh.enabled = true;
        SkinnedMeshRenderer mesh = FindSkinnedMeshRenderer(activeWendigo);
        if (mesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on activeWendigo or its children!");
            return;
        }
        mesh.enabled = true;
        sightTimer = 0.0f;
    }

    SkinnedMeshRenderer FindSkinnedMeshRenderer(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            SkinnedMeshRenderer mesh = child.GetComponent<SkinnedMeshRenderer>();
            if (mesh != null)
            {
                return mesh;
            }
            // Recursively search deeper in the hierarchy
            SkinnedMeshRenderer nestedMesh = FindSkinnedMeshRenderer(child.gameObject);
            if (nestedMesh != null)
            {
                return nestedMesh;
            }
        }
        return null;
    }

    void UpdateActiveWendigo()
    {
        if (activeWendigo != null)
        {   
            if (spawnPointTracker.GameObjectWithinFrustum(activeWendigo))
            {
                seen = true;
            }
            else if (!spawnPointTracker.GameObjectWithinFrustum(activeWendigo) && seen)
            {
                Debug.Log("FRUSTRUM");
                seen = false;
                DeActivateWendigo();
            }
            else
            {   
                
                List<Vector3> pointArray = new List<Vector3>();
                pointArray.Add(activeWendigo.transform.position);
                pointArray.Add(activeWendigo.transform.position - (spawnPointTracker.playerCamera.transform.right * wendigoRadius));
                pointArray.Add(activeWendigo.transform.position + (spawnPointTracker.playerCamera.transform.right * wendigoRadius));
                int counter = 0;
                foreach(Vector3 point in pointArray)
                {   
                    Vector3 direction = point - spawnPointTracker.playerCamera.transform.position;
                    Debug.DrawRay(spawnPointTracker.playerCamera.transform.position, direction, Color.red);
                    if(Physics.Raycast(spawnPointTracker.playerCamera.transform.position, direction.normalized, direction.magnitude, obstacleLayer))
                    {
                       counter++;
                       break;
                    }
                }
                if (counter == 3)
                {   
                    sightTimer = 0;
                    Debug.Log("ALL HIT");
                    DeActivateWendigo();
                }
                else
                {
                    sightTimer += Time.deltaTime;
                    if (sightTimer > sightThreshold)
                    {
                        playerSightings++;
                        sightTimer = -sightThreshold * playerSightings;
                    }
                }                
            }
        }
    }

    public override void Run()
    {
        if(activeWendigo != null)
        {   
            Transform parentTransform = activeWendigo.GetComponentInParent<Transform>();
            parentTransform.LookAt(spawnPointTracker.playerCamera.transform);
            activeWendigo.transform.forward = (spawnPointTracker.playerCamera.transform.position - activeWendigo.transform.position).normalized;
            spawnPointTracker.SelectRandomSpawn();
        }

        if (isActive)
        {
            spawnTimer -= Time.deltaTime;
            UpdateActiveWendigo();
            if(activeWendigo == null && spawnTimer < 0.0f)
            {
                activeWendigo = spawnPointTracker.SelectRandomSpawn();
                if (activeWendigo)
                {   
                    Debug.Log("Activating wendigo");
                    ActivateWendigo();
                }
            }
        }
        else if (isEnding)
        {
            UpdateActiveWendigo();
            if(activeWendigo == null)
            {
                isEnding = false;
            }
        }
    }

//    private  void Update()
//     {
//         Run();   
//     }
}
