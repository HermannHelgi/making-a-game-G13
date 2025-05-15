using UnityEngine;

public class QuestItemParticleHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InteractableItem childItem;
    
    void Start()
    {
        childItem = GetComponentInChildren<InteractableItem>();
    }

    void Update()
    {
        if(childItem.totalpickuptimes <=0)
        {
            gameObject.SetActive(false);
        }
    }
}
