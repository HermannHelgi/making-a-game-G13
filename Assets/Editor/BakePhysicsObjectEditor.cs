using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BakePhysicsObject))]
public class BakePhysicsObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BakePhysicsObject baker = (BakePhysicsObject)target;

        if (GUILayout.Button("Drop & Bake"))
        {
            baker.DropAndBake();
        }
    }
}
