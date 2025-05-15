using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BakePhysicsObject : MonoBehaviour
{
    [Header("Drop Settings")]
    public float simulateDuration = 2f;
    public float simulateStep = 0.02f;

    private double startTime;
    private float elapsed = 0f;
    private bool isSimulating = false;

    private List<Rigidbody> rbList = new List<Rigidbody>();

    public void DropAndBake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Debug.LogWarning("This script is meant to run in Edit Mode only.");
            return;
        }

        PrepareObjects();

        // Simulate physics in Edit mode
        Physics.simulationMode = SimulationMode.Script;
        startTime = EditorApplication.timeSinceStartup;
        elapsed = 0f;
        isSimulating = true;

        EditorApplication.update += SimulateInEditor;
        Debug.Log("Dropping objects...");
#endif
    }

    private void PrepareObjects()
    {

        rbList.Clear();

        foreach (Transform child in transform)
        {
            // simulate physics for objects with rigidbodies)
            GameObject go = child.gameObject;
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (!rb) rb = go.AddComponent<Rigidbody>();

            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (!go.GetComponent<Collider>())
                go.AddComponent<BoxCollider>();

            rbList.Add(rb);
        }
    }

    private void SimulateInEditor()
    {
#if UNITY_EDITOR
        if (!isSimulating) return;

        elapsed += simulateStep;
        Physics.Simulate(simulateStep);
        SceneView.RepaintAll();

        if (elapsed >= simulateDuration)
        {
            EditorApplication.update -= SimulateInEditor;
            BakeResults();
            Debug.Log("New object positions baked.");
        }
#endif
    }

    private void BakeResults()
    {
#if UNITY_EDITOR
        foreach (Rigidbody rb in rbList)
        {
            if (rb == null) continue;

            GameObject go = rb.gameObject;

            // uncomment if you want to remove colliders after baking
            // Collider col = go.GetComponent<Collider>();
            // if (col) DestroyImmediate(col);

            // remove Rigidbody and set object to static for baking
            DestroyImmediate(rb);

            go.isStatic = true;
        }

        rbList.Clear();
        isSimulating = false;

        Physics.simulationMode = SimulationMode.FixedUpdate;

        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
}
