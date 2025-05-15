using UnityEngine;

public class DangerZone : MonoBehaviour
{   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.dangerZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.dangerZone = false;
        }
    }
}