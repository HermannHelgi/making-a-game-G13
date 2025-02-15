using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("ItemScript variables")]
    public ItemScript pickupitem;
    public int totalpickuptimes = 1;

    [Header("Replacement model variables")]
    [Tooltip("This section is for when the InteractableItem script needs to be replaced. For example, a berry bush going from a model with berries, to one without. This change happens when totalpickuptimes reach zero.")]
    public bool replaceondepletion;
    public GameObject replacemodel;

    public bool pickUp(GameObject playercamera)
    {
        var script = playercamera.transform.GetComponent<PlayerInventory>();

        if (script == null)
        {
            return false;
        }
        else
        {
            bool result = script.addItemToHotbar(pickupitem);
            if (!result)
            {
                return false;
            }
            else
            {
                totalpickuptimes--;
                
                if (totalpickuptimes <= 0)
                {
                    if (replaceondepletion)
                    {
                        // Since its replaceondepletion, then itll replace the model completely, copying all stats.
                        GameObject newmodel = Instantiate(replacemodel);
                        newmodel.transform.SetParent(gameObject.transform.parent, true);
                        newmodel.transform.position = gameObject.transform.position;
                        newmodel.transform.rotation = gameObject.transform.rotation;
                        newmodel.transform.localScale = gameObject.transform.localScale;
                        // See below comment
                        Destroy(gameObject);
                    }
                    else
                    {
                        // This works, surprisingly. The object is marked as "deleted" but it doesn't get removed until next frame. Allowing it to send the bool.
                        Destroy(gameObject);
                    }
                }
                GameManager.instance.discovereditems[pickupitem.index] = true;
                return true;
            }
        }
    }
}
