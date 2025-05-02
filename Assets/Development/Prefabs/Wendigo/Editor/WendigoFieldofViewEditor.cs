using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WendigoRaycast))]
public class WendigoRaycastEdtiro : Editor
{
    private void OnSceneGUI()
    {
        WendigoRaycast wendigofov = (WendigoRaycast)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(wendigofov.transform.position, Vector3.up, Vector3.forward, 360, wendigofov.followRange);

        Vector3 viewAngle01 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, -wendigofov.fovAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, wendigofov.fovAngle / 2);

        Handles.color = Color.green;
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle01 * wendigofov.followRange);
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle02 * wendigofov.followRange);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(wendigofov.transform.position, Vector3.up, Vector3.forward, 360, wendigofov.listenRadius);

        Vector3 viewAngle03 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, -360 / 2);
        Vector3 viewAngle04 = DirectionFromAngle(wendigofov.transform.eulerAngles.y, 360 / 2);

        Handles.color = Color.green;
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle03 * wendigofov.listenRadius);
        Handles.DrawLine(wendigofov.transform.position, wendigofov.transform.position + viewAngle04 * wendigofov.listenRadius);


        if (wendigofov.detected)
        {
            Handles.color = Color.red;
            Handles.DrawLine(wendigofov.transform.position, wendigofov.target.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}