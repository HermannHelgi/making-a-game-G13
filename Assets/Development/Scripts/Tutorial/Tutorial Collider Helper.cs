using UnityEngine;

public class TutorialColliderHelper : MonoBehaviour
{
    public bool blocker;

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !blocker)
        {
            TutorialManager.instance.playerWalked();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && blocker)
        {
            TutorialManager.instance.playerTriedToLeaveCave();
            gameObject.SetActive(false);
        }
    }
}