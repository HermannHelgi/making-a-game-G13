using UnityEngine;

public class PlayerLookScript : MonoBehaviour
{
    public GameObject playercapsule;
    public GameObject playercamera;
    public float looktime;
    
    private GameObject lookpoint;
    private bool lookingat = false;
    private float looktimer;

    void Update()
    {
        if (lookingat)
        {
            looktimer += Time.deltaTime / looktime;

            Vector3 directionToLook = (lookpoint.transform.position - playercamera.transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            playercamera.transform.rotation = Quaternion.Lerp(playercamera.transform.rotation, targetRotation, looktimer);

            Vector3 flatDirection = new Vector3(directionToLook.x, 0, directionToLook.z);
            Quaternion playerTargetRotation = Quaternion.LookRotation(flatDirection);
            playercapsule.transform.rotation = Quaternion.Lerp(playercapsule.transform.rotation, playerTargetRotation, looktimer);
        }
    }

    public void playerLookAt(GameObject newlookpoint)
    {
        lookpoint = newlookpoint;
        lookingat = true;
        looktimer = 0;
    }

    public void finishLook()
    {
        lookingat = false;
    }
}
