using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;


public class StalkingBehaviour : WendigoBehaviour
{
    public WendigoSpawnPointTracker spawnPointTracker;

    public LayerMask obstacleLayer;
    public float spawnCooldown = 5.0f;
    public float wendigoRadius = 0.55f;
    public int wendigoSpawns = 0;
    public float sightThreshold = 2.0f;
    public float aggressionRange = 3.0f;
    public float yOffsetForRaycast = 2.0f;
    public bool inAggressionRange = false;
    private GameObject activeWendigo = null;
    private float spawnTimer;
    [SerializeField] private float sightTimer = 0.0f;
    private bool seen = false;
    public SoundManager soundManager;
    public AudioClip spawnAudio;

    void Awake()
    {
        spawnTimer = spawnCooldown;
    }
    public override void EnterState()
    {
        base.EnterState();
        inAggressionRange = false;
        spawnTimer = spawnCooldown;
        wendigoSpawns = 0;

    }

    public override void ExitState()
    {
        base.ExitState();
    }
    public Vector3 ReturnCurrentPosition()
    {
        if (activeWendigo != null)
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
        if (activeWendigo == null)
        {
            return;
        }
        SkinnedMeshRenderer mesh = FindSkinnedMeshRenderer(activeWendigo);
        Animator myAnimator = FindAnimator(activeWendigo);
        if (mesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on activeWendigo or its children!");
            return;
        }
        mesh.enabled = false;
        GameObject redEyes = FindRedEyes(activeWendigo);
        if (redEyes == null)
        {
            Debug.Log("No eyes on this guy");
            return;
        }
        redEyes.SetActive(false);
        mesh.enabled = true;
        activeWendigo = null;
        spawnTimer = spawnCooldown;
        sightTimer = 0.0f;
        myAnimator.SetBool("despawn", true);
    }

    Animator FindAnimator(GameObject activeWendigo)
    {
        if (activeWendigo == null)
        {
            return null;
        }
        foreach (Transform child in activeWendigo.transform)
        {
            Animator myAnimator = child.GetComponent<Animator>();
            if (myAnimator != null)
            {
                return myAnimator;
            }
            Animator nesterAnimator = FindAnimator(child.gameObject);
            if (nesterAnimator != null)
            {
                return nesterAnimator;
            }
        }
        return null;
    }

    void ActivateWendigo()
    {

        if (activeWendigo == null)
        {
            return;
        }
        Debug.Log("activating wendigo at: " + activeWendigo.transform.position + "named " + activeWendigo.name);
        // soundManager.PlayGroup("WENDIGO_STALKING");
        SkinnedMeshRenderer mesh = FindSkinnedMeshRenderer(activeWendigo);
        // Debug.Log("the mesh: " + mesh);
        if (mesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on activeWendigo or its children!");
            return;
        }
        GameObject redEyes = FindRedEyes(activeWendigo);
        if (redEyes == null)
        {
            Debug.Log("No eyes on this guy");
            return;
        }
        redEyes.SetActive(true);
        mesh.enabled = true;
        sightTimer = 0.0f;
        AudioSource spawnSound = mesh.GetComponentInParent<AudioSource>();
        spawnSound.PlayOneShot(spawnAudio);
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

    GameObject FindRedEyes(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            Transform redEyes = child.Find("RedEyes");
            if (redEyes != null)
            {
                return redEyes.gameObject;
            }
            // Recursively search deeper in the hierarchy
            GameObject nestedRedEyes = FindRedEyes(child.gameObject);
            if (nestedRedEyes != null)
            {
                return nestedRedEyes;
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
                foreach (Vector3 point in pointArray)
                {
                    Vector3 direction = point - spawnPointTracker.playerCamera.transform.position;
                    if (Physics.Raycast(spawnPointTracker.playerCamera.transform.position, direction.normalized, direction.magnitude, obstacleLayer))
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
                        inAggressionRange = true;
                    }
                    else
                    {
                        sightTimer += Time.deltaTime;
                        if (sightTimer > sightThreshold)
                        {
                            soundManager.ChangeSoundsnapshot("SPOOKY", 0f);
                            soundManager.PlayGroup("WENDIGO_STARING");
                            inAggressionRange = true;


                        }
                    }
                }
            }
            else if (!spawnPointTracker.GameObjectWithinFrustum(activeWendigo) && seen)
            {
                soundManager.StopPlayGroup("WENDIGO_STARING");
                soundManager.ExitSoundsnapshot(0f);
                seen = false;
                sightTimer = 0f;
                DeActivateWendigo();
            }
        }
    }

    public override void Run()
    {
        Animator myAnimator = FindAnimator(activeWendigo);
        if (activeWendigo != null)
        {
            Transform parentTransform = activeWendigo.GetComponentInParent<Transform>();
            parentTransform.LookAt(spawnPointTracker.playerCamera.transform);
            activeWendigo.transform.forward = (spawnPointTracker.playerCamera.transform.position - activeWendigo.transform.position).normalized;
            spawnPointTracker.SelectRandomSpawn(false);
        }

        if (isActive)
        {
            spawnTimer -= Time.deltaTime;
            UpdateActiveWendigo();
            if (activeWendigo == null && spawnTimer < 0.0f)
            {
                activeWendigo = spawnPointTracker.SelectRandomSpawn(false);
                if (activeWendigo)
                {
                    Debug.Log("Activating wendigo");
                    ActivateWendigo();
                    wendigoSpawns++;
                }
            }
            if (myAnimator != null)
            {
                myAnimator.SetBool("isIdle", true);
                // myAnimator.Play("Idle");
            }
            if (GameManager.instance.dangerZone)
            {
                myAnimator.SetBool("isIdle", false);
                DespawnWendigo();
            }

        }

        else if (isEnding)
        {
            UpdateActiveWendigo();
            if (activeWendigo == null)
            {
                isEnding = false;
            }
            if (inAggressionRange)
            {
                isEnding = false;
            }
        }
    }

    void Update()
    {
        if (activeWendigo != null)
        {
        }
    }
    //    private  void Update()
    //     {
    //         Run();   
    //     }
}

