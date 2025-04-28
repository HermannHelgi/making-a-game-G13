using UnityEngine;
using System.Collections.Generic;


public class PlayerLineofSight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isLooking;

    public float fovThreshold = 0.75f; // 45 degrees

    public Camera playerCamera;
    public List<GameObject> spawnBoxes;

    public wendigoRandomizedSpawner spawner;

    public bool wasLooking;
    private wendigoRandomizedSpawner spawnerLogic;
    
    void Start()
    {
        
    }

    void Awake()
    {
        spawnerLogic = FindFirstObjectByType<wendigoRandomizedSpawner>();

        if (spawnerLogic == null)
        {
            Debug.LogError("WendigoRandomizedSpawner not found in the scene!");
        }
    }

    
    void Update()

    {


        

    }

    public bool isLookingAtWendigo()
    {
        return spawnerLogic.isWendigoWithinFrustum();   
    }

    

    }






