using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitchTradeScript : MonoBehaviour
{
    [Header("Playtest Temporary Variables")]
    public DialogueScriptableObject startdialogue;
    private bool hasadded = false;

    [Header("Trade Window Variables")]
    public float distancetoturnoffwitchoverlay = 10;
    public ItemScript[] craftableItems = new ItemScript[7];
    public GameObject tradeslotprefab;
    [Tooltip("The GameObject of the campfire within the scene.")]
    public GameObject campfire;
    [Tooltip("The ItemScript of the campfire.")]
    public ItemScript campfirereference;
    [Tooltip("The GameObject of the chest within the scene.")]
    public GameObject chest;
    [Tooltip("The ItemScript of the chest.")]
    public ItemScript chestreference;

    [Tooltip("The string which should be displayed on whether an item can be crafted or not.")]
    public string buttontocraftstring;
    [Tooltip("The string which should be displayed when an item can no longer be crafted.")]
    public string itemcannotbecraftedstring;
    public GameObject lerppos;
    public int maxtimetolerp;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.gray;
    [Tooltip("Used to display if an item has been crafted.")]
    public Sprite checkmark;

    [Header("Dialogue System variables")]
    public GameObject dialogueHandler;
    
    // Private stuff, mostly references to other objects.
    private GameObject[] tradeslotgridchildren;
    private bool[] hasbeencrafted;
    private int currentindex = 0;
    private GameObject witchrecipegridspawner; 
    private GameObject playerinventory;
    private GameObject witchoverlay;
    private PlayerInventory playerinventoryscript;
    private TextMeshProUGUI itemnametextmesh;
    private TextMeshProUGUI ingredientstextmesh;
    private bool currentlytrading = false;
    private TextMeshProUGUI crafttextmesh;
    private PlayerLookScript playerlook;

    void Start()
    {
        tradeslotgridchildren = new GameObject[craftableItems.Length];
        hasbeencrafted = new bool[craftableItems.Length];

        if (chest == null)
        {
            Debug.LogWarning("Chest gameobject is missing for Witch Trade Script.");
        }
        else
        {
            chest.SetActive(false);
        }

        if (campfire == null)
        {
            Debug.LogWarning("Campfire gameobject is missing for Witch Trade Script.");
        }
        else
        {
            campfire.SetActive(false);
        }
    }

    void Update()
    {
        if (currentlytrading)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deinitializeTradeWindow();
            }

            // Increase trading slot, go right
            if (Input.GetAxis("Mouse ScrollWheel") < 0f ) 
            {
                selectCraftingSlot(currentindex + 1);
            }
            // Decrease trading slot, go left
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f ) 
            {
                selectCraftingSlot(currentindex - 1);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                craftItem();
            }
        }

        if (!hasadded)
        {
            DialogueManager.instance.SetDialogueFlags(startdialogue);
            hasadded = true;
        }
    }

    void craftItem()
    {
        // I need to make a dictionary to get the total counts of each item, as i need to check how many unique amount of items there are
        Dictionary<string, int> count = new Dictionary<string, int>();

        for (int i = 0; i < craftableItems[currentindex].craftingrecipe.Length; i++)
        {
            if (count.ContainsKey(craftableItems[currentindex].craftingrecipe[i].name))
            {
                count[craftableItems[currentindex].craftingrecipe[i].name] += 1;
            }
            else
            {                
                count[craftableItems[currentindex].craftingrecipe[i].name] = 1;
            }
        }

        // Check, can I craft this item?
        for (int i = 0; i < craftableItems[currentindex].craftingrecipe.Length; i++)
        {
            // TODO: Needs to check the storage container whether that has the item aswell (if it is unlocked) and whether an item is shared in between the hotbar and storage
            if (!playerinventoryscript.hasItem(craftableItems[currentindex].craftingrecipe[i], count[craftableItems[currentindex].craftingrecipe[i].name]))
            {
                return;
            }
        }

        // Is this item a one-time craft item, and has it already been crafted?
        if (craftableItems[currentindex].onetimecraft && hasbeencrafted[currentindex])
        {
            return;
        }
        
        for (int i = 0; i < craftableItems[currentindex].craftingrecipe.Length; i++)
        {
            // TODO: Needs to check the storage container as well if the item exists there
            bool check =  playerinventoryscript.removeItemFromHotbar(craftableItems[currentindex].craftingrecipe[i]);
            if (!check)
            {
                Debug.LogError("ERROR! Item was attempted to be removed when it doesn't exist.");
            }
        }

        hasbeencrafted[currentindex] = true; 
        if (craftableItems[currentindex].onetimecraft)
        {
            tradeslotgridchildren[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = checkmark;
            crafttextmesh.text = itemcannotbecraftedstring;
        }

        if (craftableItems[currentindex] == campfirereference)
        {
            campfire.SetActive(true);
            return;
        }
        if (craftableItems[currentindex] == chestreference)
        {
            chest.SetActive(true);
            return;
        }

        bool inventoryFullCheck = playerinventoryscript.addItemToHotbar(craftableItems[currentindex]);
        if (!inventoryFullCheck)
        {
            // TODO: Needs to add to the storage container.
        }
    }

    void selectCraftingSlot(int index)
    {
        for (int i = 0; i < craftableItems.Length; i++)
        {
            tradeslotgridchildren[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            tradeslotgridchildren[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

        // Edge checks
        if (index >= craftableItems.Length)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = craftableItems.Length - 1;
        }

        // Select Hotbar
        currentindex = index;
        tradeslotgridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
        tradeslotgridchildren[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;

        // Modifying update text
        itemnametextmesh.text = craftableItems[index].name;
        ingredientstextmesh.text = "";

        if (craftableItems[currentindex].onetimecraft && hasbeencrafted[currentindex])
        {
            crafttextmesh.text = itemcannotbecraftedstring;
        }
        else
        {
            crafttextmesh.text = buttontocraftstring;
        }

        for (int i = 0; i < craftableItems[currentindex].craftingrecipe.Length; i++)
        {
            // This is accessing the game manager at the index of the current ingredient of the recipe
            if (GameManager.instance.discovereditems[craftableItems[currentindex].craftingrecipe[i].index])
            {
                ingredientstextmesh.text += craftableItems[currentindex].craftingrecipe[i].name;
            }
            else
            {
                ingredientstextmesh.text += "???";
            }


            if ((i + 1) < craftableItems[currentindex].craftingrecipe.Length)
            {
                ingredientstextmesh.text += ",\n";
            }
        }
    }

    void spawnCraftingRecipeBoxes()
    {
        for (int i = 0; i < craftableItems.Length; i++)
        {
            GameObject childObject = Instantiate(tradeslotprefab);
            childObject.transform.SetParent(witchrecipegridspawner.transform, false);
            childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            childObject.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
            if (hasbeencrafted[i] && craftableItems[i].onetimecraft)
            {
                childObject.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = checkmark;
            }
            else
            {
                childObject.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = craftableItems[i].icon;
            }
            tradeslotgridchildren[i] = childObject;
        }
    }

    void wipeCraftingRecipeBoxes()
    {
        for (int i = tradeslotgridchildren.Length - 1; i >= 0; i--)
        {
            Destroy(tradeslotgridchildren[i]);
        }
    }

    public bool canTalk()
    // Used by the interact handler for the player to check if the witch can speak.
    {
        return !dialogueHandler.GetComponent<WitchDialogueHandler>().isQueueEmpty();
    }

    public void initializeTradeWindow(GameObject witchtradecanvas, GameObject witchrecipegridspawnerobject, GameObject playerinventorycanvas, GameObject playerinventoryscriptobject, TextMeshProUGUI nameofitemincanvastextmesh, TextMeshProUGUI ingredientslisttextmesh, GameObject subtitletextmesh, TextMeshProUGUI crafttext, GameObject escapemessage, GameObject playerlookscript)   
    // Starts the trade window, if the witch has dialogue, does that first.
    {
        if (!dialogueHandler.GetComponent<WitchDialogueHandler>().isQueueEmpty())
        {
            dialogueHandler.GetComponent<WitchDialogueHandler>().intializeDialogue(subtitletextmesh, escapemessage, playerlookscript);
            return;
        }

        if (!currentlytrading)
        {
            // HOLY VARIABLES, BATMAN!
            // This is a lot, won't lie
            // I made this with the assumption that the witch doesn't have access to these variables on scene load to reduce inter-prefab references and reduce work on the inspector side.
            // Its just a bunch of miscellaneous references, so don't worry bout it
            playerinventory = playerinventorycanvas;
            witchoverlay = witchtradecanvas;
            witchrecipegridspawner = witchrecipegridspawnerobject;
            playerinventoryscript = playerinventoryscriptobject.GetComponent<PlayerInventory>();
            itemnametextmesh = nameofitemincanvastextmesh;
            ingredientstextmesh = ingredientslisttextmesh;
            playerlook = playerlookscript.GetComponent<PlayerLookScript>();
            playerlook.playerLookAt(lerppos);

            playerinventory.SetActive(false);
            witchoverlay.SetActive(true);
            playerinventoryscript.deleteHeldObjects();
            currentlytrading = true;
            currentindex = 0;
            crafttextmesh = crafttext;
            spawnCraftingRecipeBoxes();
            selectCraftingSlot(currentindex);
        }
    }

    void deinitializeTradeWindow()
    // Closes trade window
    {
        playerlook.finishLook();
        wipeCraftingRecipeBoxes();
        playerinventory.SetActive(true);
        witchoverlay.SetActive(false);
        playerinventoryscript.resetHotbarItems();
        currentlytrading = false;
        GameManager.instance.activateMenuCooldown();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
