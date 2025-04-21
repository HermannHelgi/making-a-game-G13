using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TreePlacement : MonoBehaviour
{
    [Header("Terrain & Trees")]
    public Terrain terrain;
    public GameObject[] treePrefabs;

    [Header("Placement Settings")]
    public int treeCount = 1000;
    public float maxSlope = 30f;
    public float minSpacing = 5f;

    [Header("Tree Scale Range")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Perlin Noise Settings")]
    public float noiseScale = 0.05f; 
    public float noiseThreshold = 0.5f; // 0-1

    public void PlaceTrees()
    {
        if (terrain == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("Missing terrain or tree prefabs.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;

        int placed = 0;
        int attempts = 0;
        int maxAttempts = treeCount * 10;

        while (placed < treeCount && attempts < maxAttempts)
        {
            attempts++;

            // Randomize position on the terrain
            float x = Random.Range(0f, terrainSize.x);
            float z = Random.Range(0f, terrainSize.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z));
            Vector3 worldPos = new Vector3(x, y, z) + terrainPosition;

            // space between trees
            Vector3 normal = terrainData.GetInterpolatedNormal(x / terrainSize.x, z / terrainSize.z);
            float slope = Vector3.Angle(normal, Vector3.up);
            if (slope > maxSlope) continue;

            // Perlin noise value
            float noiseValue = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
            if (noiseValue < noiseThreshold) continue;

            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            GameObject tree = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            
            // randomize rotation and scale
            tree.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
            float scale = Random.Range(minScale, maxScale);
            tree.transform.localScale = Vector3.one * scale;

            placed++;
        }

        Debug.Log($"Placed {placed} trees with Perlin noise, after {attempts} attempts.");
    }

    public void ClearTrees()
    {
        #if UNITY_EDITOR
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
        #endif
    }
}
