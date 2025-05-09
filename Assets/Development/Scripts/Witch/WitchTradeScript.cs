using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitchTradeScript : MonoBehaviour, IDataPersistence
{
    [Header("Trade Window Variables")]
    public ItemScript[] craftableItems = new ItemScript[7];
    [Tooltip("Which items the player has unlocked, MUST match the size of craftableItems!")]
    public bool[] unlockedItems = new bool[7];
    public GameObject tradeslotprefab;
    [Tooltip("The GameObject of the campfire within the scene.")]
    public GameObject campfire;
    [Tooltip("The ItemScript of the campfire.")]
    public ItemScript campfirereference;
    [Tooltip("The GameObject of the chest within the scene.")]
    public GameObject chest;
    [Tooltip("The ItemScript of the chest.")]
    public ItemScript chestreference;
     [Tooltip("The ItemScript of the lure.")]
    public ItemScript lureReference;

    [Tooltip("The string which should be displayed on whether an item can be crafted or not.")]
    public string buttontocraftstring;
    [Tooltip("The string which should be displayed when an item can no longer be crafted.")]
    public string itemcannotbecraftedstring;
    public GameObject lerppos;
    public int maxtimetolerp;

    public GameObject witchrecipegridspawner; 
    public TextMeshProUGUI itemnametextmesh;
    public TextMeshProUGUI crafttextmesh;
    public GameObject witchoverlay;
    public TextMeshProUGUI itemDescription;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.gray;
    public Color itemInInventory = Color.white;
    [Tooltip("Used to display if an item has been crafted.")]
    public Sprite checkmark;
    public Sprite questionMark;

    [Header("Dialogue System variables")]
    public GameObject dialogueHandler;
    
    // Private stuff, mostly references to other objects.
    private GameObject[] tradeslotgridchildren;
    private bool[] hasbeencrafted;
    private int currentindex = 0;
    private GameObject playerinventory;
    private PlayerInventory playerinventoryscript;
    private bool currentlytrading = false;
    private PlayerLookScript playerlook;

    [Header("Scrollview Variables")]
    public RectTransform verticalLayoutGroup;
    public float elementSize;
    public float paddingSize;
    public GameObject witchBargainingSlotPrefab;

    void Awake()
    {
        tradeslotgridchildren = new GameObject[craftableItems.Length];
        hasbeencrafted = new bool[craftableItems.Length];

        if (chest == null)
        {
            Debug.LogWarning("Chest gameobject is missing for Witch Trade Script.");
        }

        if (campfire == null)
        {
            Debug.LogWarning("Campfire gameobject is missing for Witch Trade Script.");
        }
    }
    
    public void loadData(GameData data)
    {
        if (data.chestUnlocked)
        {
            chest.SetActive(true);
        }
        else
        {
            chest.SetActive(false);
        }

        if (data.campfireUnlocked)
        {
            campfire.SetActive(true);
        }
        else
        {
            campfire.SetActive(false);
        }

        unlockedItems = data.unlockedItems.ToArray();
        hasbeencrafted = data.hasBeenCrafted.ToArray();
    }

    public void saveData(ref GameData data)
    {
        data.chestUnlocked = chest.activeSelf;
        data.campfireUnlocked = campfire.activeSelf;
        data.unlockedItems = new List<bool>(unlockedItems);
        data.hasBeenCrafted = new List<bool>(hasbeencrafted);
    }

    void Update()
    {
        if (currentlytrading)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
            {
                deinitializeTradeWindow();
            }

            // Increase trading slot, go right
            if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) 
            {
                selectCraftingSlot(currentindex + 1);
            }
            // Decrease trading slot, go left
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) 
            {
                selectCraftingSlot(currentindex - 1);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
            {
                craftItem();
            }
        }
    }

    void SnapToElement(int index)
    {
        int count = 0;
        for (int i = 0; i < index; i++)
        {
            if (unlockedItems[i])
            {
                count++;
            }
        }
        index = count;

        verticalLayoutGroup.anchoredPosition = new Vector2(verticalLayoutGroup.anchoredPosition.x, index * (elementSize + paddingSize) + (elementSize / 2));
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
            tradeslotgridchildren[currentindex].GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = checkmark;
            crafttextmesh.text = itemcannotbecraftedstring;
        }

        ItemScript[] playerHeldItems = playerinventoryscript.getInventoryItems();
        List<GameObject> hotbarslots = new List<GameObject>();

        foreach (GameObject bargainingSlots in tradeslotgridchildren)
        {
            for (int i = 0; i < bargainingSlots.GetComponent<WitchBargainingSlotWrapper>().content.transform.childCount; i++)
            {
                hotbarslots.Add(bargainingSlots.GetComponent<WitchBargainingSlotWrapper>().content.transform.GetChild(i).gameObject);
            }

            foreach (ItemScript playerItem in playerHeldItems)
            {
                if (playerItem == null)
                {
                    continue;
                }

                foreach (GameObject resourceGO in hotbarslots)
                {
                    if (resourceGO.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite == playerItem.icon)
                    {
                        resourceGO.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = itemInInventory;
                        resourceGO.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;
                        hotbarslots.Remove(resourceGO);
                        break;
                    }
                }
            }

            foreach(GameObject resourceGO in hotbarslots)
            {
                resourceGO.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
                resourceGO.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
            }
            
            hotbarslots.Clear();
        }


        if (craftableItems[currentindex] == campfirereference)
        {
            campfire.SetActive(true);
            if (TutorialManager.instance != null)
            {
                if (TutorialManager.instance.tutorialinprogress)
                {
                    TutorialManager.instance.playerHasCraftedCampfire();
                }
            }
            return;
        }
        if (craftableItems[currentindex] == chestreference)
        {
            chest.SetActive(true);
            return;
        }
        if (craftableItems[currentindex] == lureReference)
        {
            GameManager.instance.lureCrafted = true;
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
            if (tradeslotgridchildren[i] == null)
            {
                continue;
            }
            tradeslotgridchildren[i].GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            tradeslotgridchildren[i].GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

        // Edge checks
        if (index >= craftableItems.Length)
        {
            index = craftableItems.Length - 1;
        }
        if (index < 0)
        {
            index = 0;
        }

        while (!unlockedItems[index])
        {
            if (index > currentindex)
            {
                index++;

                if (index >= craftableItems.Length)
                {
                    index = currentindex;
                }
            }
            else if (index < currentindex)
            {
                index--;

                if (index < 0)
                {
                    index = currentindex;
                }
            }
        }

        // Select Hotbar
        currentindex = index;
        SnapToElement(currentindex);
        tradeslotgridchildren[currentindex].GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
        tradeslotgridchildren[currentindex].GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;

        // Modifying update text
        itemnametextmesh.text = craftableItems[index].name;
        itemDescription.text = craftableItems[index].craftingDescription;

        if (craftableItems[currentindex].onetimecraft && hasbeencrafted[currentindex])
        {
            crafttextmesh.text = itemcannotbecraftedstring;
        }
        else
        {
            crafttextmesh.text = buttontocraftstring;
        }
    }

    void spawnCraftingRecipeBoxes()
    {
        ItemScript[] playerHeldItems = playerinventoryscript.getInventoryItems();

        for (int i = 0; i < craftableItems.Length; i++)
        {
            if (!unlockedItems[i])
            {
                continue;
            }

            GameObject childObject = Instantiate(witchBargainingSlotPrefab);
            childObject.transform.SetParent(witchrecipegridspawner.transform, false);
            childObject.GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            childObject.GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
            
            if (hasbeencrafted[i] && craftableItems[i].onetimecraft)
            {
                childObject.GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = checkmark;
            }
            else
            {
                childObject.GetComponent<WitchBargainingSlotWrapper>().craftableItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = craftableItems[i].icon;
            }

            List<GameObject> hotbarslots = new List<GameObject>();
            
            foreach (ItemScript resource in craftableItems[i].craftingrecipe)
            {
                GameObject resourceItem = Instantiate(tradeslotprefab);
                resourceItem.transform.SetParent(childObject.GetComponent<WitchBargainingSlotWrapper>().content.transform, false);
                if (GameManager.instance.discovereditems[resource.index])
                {
                    resourceItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = resource.icon;
                }
                else
                {
                    resourceItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = questionMark;
                }
                resourceItem.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
                resourceItem.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;

                hotbarslots.Add(resourceItem);  
            }

            foreach (ItemScript playerItem in playerHeldItems)
            {
                if (playerItem == null)
                {
                    continue;
                }

                foreach (GameObject resourceGO in hotbarslots)
                {
                    if (resourceGO.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite == playerItem.icon)
                    {
                        resourceGO.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = itemInInventory;
                        resourceGO.GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;
                        hotbarslots.Remove(resourceGO);
                        break;
                    }
                }
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

    public void unlockItem(ItemScript item)
    {
        for (int i = 0; i < craftableItems.Length; i++)
        {
            if (craftableItems[i] == item)
            {
                unlockedItems[i] = true;
                break;
            }
        }
    }

    public void initializeTradeWindow(GameObject playerinventorycanvas, GameObject playerinventoryscriptobject, GameObject subtitletextmesh, GameObject escapemessage, GameObject playerlookscript)   
    // Starts the trade window, if the witch has dialogue, does that first.
    {
        if (!dialogueHandler.GetComponent<WitchDialogueHandler>().isQueueEmpty())
        {
            dialogueHandler.GetComponent<WitchDialogueHandler>().intializeDialogue(subtitletextmesh, escapemessage, playerlookscript);
            return;
        }

        if (TutorialManager.instance != null)
        {
            if (TutorialManager.instance.cannotcraft)
            {
                GameManager.instance.inMenu = false;
                return;
            }
        }

        if (!currentlytrading)
        {
            // HOLY VARIABLES, BATMAN!
            // This is a lot, won't lie
            // I made this with the assumption that the witch doesn't have access to these variables on scene load to reduce inter-prefab references and reduce work on the inspector side.
            // Its just a bunch of miscellaneous references, so don't worry bout it
            playerinventory = playerinventorycanvas;
            playerinventoryscript = playerinventoryscriptobject.GetComponent<PlayerInventory>();
            playerlook = playerlookscript.GetComponent<PlayerLookScript>();
            playerlook.playerLookAt(lerppos);

            playerinventory.SetActive(false);
            witchoverlay.SetActive(true);
            playerinventoryscript.deleteHeldObjects();
            currentlytrading = true;
            currentindex = 0;
            spawnCraftingRecipeBoxes();
            selectCraftingSlot(currentindex);

            if (TutorialManager.instance != null)
            {
                if (TutorialManager.instance.tutorialinprogress)
                {
                    TutorialManager.instance.playerHasOpenedTradeWindow();
                }
            }
        }
    }

    void deinitializeTradeWindow()
    // Closes trade window
    {
        if (TutorialManager.instance != null)
        {
            if (!TutorialManager.instance.tutorialinprogress)
            {
                GameManager.instance.activateMenuCooldown();
            }
        }
        else
        {
            GameManager.instance.activateMenuCooldown();
        }

        playerlook.finishLook();
        wipeCraftingRecipeBoxes();
        playerinventory.SetActive(true);
        witchoverlay.SetActive(false);
        playerinventoryscript.resetHotbarItems();
        currentlytrading = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (TutorialManager.instance != null)
        {
            if (TutorialManager.instance.tutorialinprogress)
            {
                TutorialManager.instance.playerHasClosedTradeWindow();
            }
        }
    }
}