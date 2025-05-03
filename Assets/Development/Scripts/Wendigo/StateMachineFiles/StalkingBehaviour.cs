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
    public float aggressionRange = 5.0f;
    public float yOffsetForRaycast = 2.0f;
    public bool inAggressionRange = false;

    private GameObject activeWendigo = null;
    private float spawnTimer ;
    [SerializeField] private float sightTimer = 0.0f;
    private bool seen = false;
    public SoundManager soundManager;

    void Awake()
    {
        spawnTimer = spawnCooldown;
    }
    public override void EnterState()
    {
        base.EnterState();
        playerSightings = 0;
        inAggressionRange = false;
        spawnTimer = spawnCooldown;
    }

    public override void ExitState()
    {
        base.ExitState();
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
        // soundManager.PlayGroup("Wendigo_Spawn");
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
                List<Vector3> pointArray = new List<Vector3>();
                pointArray.Add(activeWendigo.transform.position + new Vector3(0, yOffsetForRaycast, 0));
                pointArray.Add(activeWendigo.transform.position - (spawnPointTracker.playerCamera.transform.right * wendigoRadius) + new Vector3(0, yOffsetForRaycast, 0));
                pointArray.Add(activeWendigo.transform.position + (spawnPointTracker.playerCamera.transform.right * wendigoRadius) + new Vector3(0, yOffsetForRaycast, 0));
                int counter = 0;
                foreach(Vector3 point in pointArray)
                {   
                    Vector3 direction = point - spawnPointTracker.playerCamera.transform.position;
                    if(Physics.Raycast(spawnPointTracker.playerCamera.transform.position, direction.normalized, direction.magnitude, obstacleLayer))
                    {
                        Debug.DrawRay(spawnPointTracker.playerCamera.transform.position, direction, Color.red);
                        counter++;
                    }
                }
                if (counter == 3)
                {   
                    sightTimer = 0;
                    DeActivateWendigo();
                }
                else
                {
                    if (Vector3.Distance(spawnPointTracker.playerCamera.transform.position, activeWendigo.transform.position) <= aggressionRange)
                    {
                        playerSightings += 10;
                        inAggressionRange = true;
                    }
                    else
                    {
                        sightTimer += Time.deltaTime;
                        if (sightTimer > sightThreshold)
                        {   
                            soundManager.ChangeSoundsnapshot("SPOOKY", 0f);
                            soundManager.PlayGroup("WENDINGO_STARING");
                            
                            playerSightings++;
                            sightTimer = -sightThreshold * playerSightings/2;
                        }
                    }
                }   
            }
            else if (!spawnPointTracker.GameObjectWithinFrustum(activeWendigo) && seen)
            {   
                soundManager.ExitSoundsnapshot(0f);
                seen = false;
                DeActivateWendigo();
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
            if (inAggressionRange)
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
