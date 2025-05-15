using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreePlacement))]
public class TreePlacerButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreePlacement placer = (TreePlacement)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Place Trees"))
        {
            placer.PlaceTrees();
        }

        if (GUILayout.Button("Clear Trees"))
        {
            placer.ClearTrees();
        }

        if (GUILayout.Button("Randomize"))
        {
            placer.ClearTrees();
            placer.PlaceTrees();
        }

        // this button tbh shouldnt be here cus prefabs might be HUGE
        if (GUILayout.Button("Save as Prefab (careful)"))
        {
            SaveAsPrefab(placer.gameObject);
        }
    }

    private void SaveAsPrefab(GameObject parent)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Tree Group as Prefab",
            parent.name + "_Trees",
            "prefab",
            "Choose a location to save the prefab"
        );

        if (string.IsNullOrEmpty(path)) return;

        // Create prefab
        PrefabUtility.SaveAsPrefabAssetAndConnect(parent, path, InteractionMode.UserAction);
        Debug.Log($"Saved tree group as prefab: {path}");
    }
}
