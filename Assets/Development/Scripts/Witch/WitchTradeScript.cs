using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitchTradeScript : MonoBehaviour
{
    [Header("Trade Window Variables")]
    public float distancetoturnoffwitchoverlay = 10;
    public ItemScript[] craftableItems = new ItemScript[7];
    public GameObject tradeslotprefab;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.gray;

    [Header("Dialogue System variables")]
    public WitchDialogueHandler dialogueHandler;
    
    // Private stuff, mostly references to other objects.
    private GameObject[] tradeslotgridchildren;
    private int currentindex = 0;
    private GameObject witchrecipegridspawner; 
    private GameObject playerinventory;
    private GameObject witchoverlay;
    private PlayerInventory playerinventoryscript;
    private GameObject player;
    private TextMeshProUGUI itemnametextmesh;
    private TextMeshProUGUI ingredientstextmesh;
    private bool currentlytrading = false;

    void Start()
    {
        tradeslotgridchildren = new GameObject[craftableItems.Length];
    }

    void Update()
    {
        if (currentlytrading)
        {
            // Turns off the trade window under set condition
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) > distancetoturnoffwitchoverlay)
            {
                deinitializeTradeWindow();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deinitializeTradeWindow();
            }

            // Increase trading slot, go right
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) 
            {
                selectCraftingSlot(currentindex + 1);
            }
            // Decrease trading slot, go left
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) 
            {
                selectCraftingSlot(currentindex - 1);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                craftItem();
            }
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

        for (int i = 0; i < craftableItems[currentindex].craftingrecipe.Length; i++)
        {
            // TODO: Needs to check the storage container whether that has the item aswell (if it is unlocked)
            if (!playerinventoryscript.hasItem(craftableItems[currentindex].craftingrecipe[i], count[craftableItems[currentindex].craftingrecipe[i].name]))
            {
                return;
            }
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

        if (!craftableItems[currentindex].structureitem)
        {
            bool inventoryFullCheck = playerinventoryscript.addItemToHotbar(craftableItems[currentindex]);
            if (!inventoryFullCheck)
            {
                // TODO: Needs to add to the storage container.
            }
        }
        else
        {
            // TODO: Needs to spawn potential structureitem, Campfire or Chest or more etc. 
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
            childObject.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = craftableItems[i].icon;
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

    public void initializeTradeWindow(GameObject witchtradecanvas, GameObject witchrecipegridspawnerobject, GameObject playerinventorycanvas, GameObject playerinventoryscriptobject, GameObject playerobject, TextMeshProUGUI nameofitemincanvastextmesh, TextMeshProUGUI ingredientslisttextmesh, GameObject subtitletextmesh)   
    {
        if (!dialogueHandler.isQueueEmpty())
        {
            dialogueHandler.intializeDialogue(subtitletextmesh, playerobject);
            return;
        }

        if (!currentlytrading)
        {
            playerinventory = playerinventorycanvas;
            witchoverlay = witchtradecanvas;
            witchrecipegridspawner = witchrecipegridspawnerobject;
            playerinventoryscript = playerinventoryscriptobject.GetComponent<PlayerInventory>();
            player = playerobject;
            itemnametextmesh = nameofitemincanvastextmesh;
            ingredientstextmesh = ingredientslisttextmesh;

            playerinventory.SetActive(false);
            witchoverlay.SetActive(true);
            playerinventoryscript.deleteHeldObjects();
            currentlytrading = true;
            currentindex = 0;
            spawnCraftingRecipeBoxes();
            selectCraftingSlot(currentindex);
        }
    }

    void deinitializeTradeWindow()
    {
        wipeCraftingRecipeBoxes();
        playerinventory.SetActive(true);
        witchoverlay.SetActive(false);
        playerinventoryscript.resetHotbarItems();
        currentlytrading = false;
    }
}
