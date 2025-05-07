using UnityEngine;

public class InteractableItem : MonoBehaviour, IDataPersistence
{
    [Header("ItemScript variables")]
    [Tooltip("GUID for individual interactable item. DO NOT CHANGE. If empty, right click the script and press 'generate guid for id'")]
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void generateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public ItemScript pickupitem;
    public int totalpickuptimes = 1;
    [Tooltip("If a DialogueScriptableObject is emplaced, it will send the given dialogue on pickup to the Dialogue Manager. Leave empty if no message should be sent.")]
    public DialogueScriptableObject dialoguetriggerforwitch;

    [Header("Replacement model variables")]
    [Tooltip("This section is for when the InteractableItem script needs to be replaced. For example, a berry bush going from a model with berries, to one without. This change happens when totalpickuptimes reach zero.")]
    public bool replaceondepletion;
    public GameObject replacemodel;

    public PlayerInteractHandler playerInteractHandler;
    private Outline outlineHandler;

    void Start()
    {
        outlineHandler = GetComponent<Outline>();
    }

    void Update()
    {
        if(!playerInteractHandler.highlighting)
        {
            outlineHandler.enabled = false;
        }
    }
    public void loadData(GameData data)
    {
        if (data.interactableItemCounts.ContainsKey(id))
        {
            data.interactableItemCounts.TryGetValue(id, out int count);
            totalpickuptimes = count;
            if (totalpickuptimes == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void saveData(ref GameData data)
    {
        if (data.interactableItemCounts.ContainsKey(id))
        {
            data.interactableItemCounts.Remove(id);
        }
        data.interactableItemCounts.Add(id, totalpickuptimes);
    }

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
                // If a dialogue is emplaced, it will send the dialogue to the manager (and therefore the witch) on pickup
                if (dialoguetriggerforwitch != null)
                {
                    DialogueManager.instance.SetDialogueFlags(dialoguetriggerforwitch);
                }
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
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
                GameManager.instance.discovereditems[pickupitem.index] = true;

                if (TutorialManager.instance != null)
                {
                    if (TutorialManager.instance.tutorialinprogress)
                    {
                        TutorialManager.instance.playerPickedUpItem();
                    }
                }

                return true;
            }
        }
    }
}
