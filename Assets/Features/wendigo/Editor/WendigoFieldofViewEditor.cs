using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WendigoFieldofViewEditor))]
public class WendigoFieldofViewEditor : Editor
{
    private void OnSceneGUI()
    {
        WendigoRaycast wendigofov = (WendigoRaycast)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(wendigofov.transform.position, Vector3.up, Vector3.forward, 360, wendigofov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, -wendigofov.fovAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, wendigofov.fovAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle01 * wendigofov.radius);
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle02 * wendigofov.radius);

        if (wendigofov.detected)
        {
            Handles.color = Color.green;
            Handles.DrawLine(wendigofov.transform.position, wendigofov.player.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}