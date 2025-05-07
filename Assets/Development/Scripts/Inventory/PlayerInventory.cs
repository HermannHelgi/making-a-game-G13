using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour, IDataPersistence
{
    [Header("Hotbar variables")]
    public int maxhotbarsize = 6;
    public Sprite emptyhotbar;    
    public GameObject hotbarslotprefab;
    public GameObject hotbarslotgrid;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.white;
    
    [Header("3D model variables")]
    public GameObject spawnlocation;

    [Header("Drop item variables")]
    public GameObject droplocation;
    public GameObject droppeditemprefab;

    [Header("Consumable variables")]

    [Tooltip("TextMeshPro element for the 'Press F to consume' text.")]
    public GameObject consumableindicator;
    public GameObject necessitybargameobject;
    public string consumabletext;
    public PlayerInteractHandler playerinteracthandler;
    public ItemScript thePotion;
    public PlayerDeathHandler playerDeathHandler;
    public float maxConsumableTimer;
    private float consumableTimer;

    [Header("Torch variables")]
    [Tooltip("ItemScript of the Torch, required for Wendigo AI and durability stuff.")]
    public ItemScript torch;
    [Tooltip("The total time the torch should be able to last, measured in seconds.")]
    public float maxtorchdurability = 300;
    [Tooltip("DO NOT TOUCH. This variable is serialized for viewability, not editing.")]
    [SerializeField] private float currenttorchdurability = 0;
    public string refueltorchtext;
    public string failedtorefuel;
    public ItemScript coal;
    private GameObject torchdurabilitybar;

    [Header("Ember stone variables")]
    [Tooltip("ItemScript of the Torch, required for Wendigo AI and durability stuff.")]
    public ItemScript emberstone;
    [Tooltip("The total time the torch should be able to last, measured in seconds.")]
    public float maxemberstonedurability = 300;
    [Tooltip("DO NOT TOUCH. This variable is serialized for viewability, not editing.")]
    [SerializeField] private float currentemberstonedurability = 0;
    private bool emberstoneininventory = false;
    public string refuelemberstonetext;
    private GameObject emberstonedurabilitybar;

    // Private stuffs
    private int currentindex = 0;
    private ItemScript[] hotbarinventory;
    private GameObject[] hotbargridchildren;

    void Awake()
    {
        consumableindicator.SetActive(false);

        //  Instantiate the arrays
        hotbarinventory = new ItemScript[maxhotbarsize];
        hotbargridchildren = new GameObject[maxhotbarsize];

        // Create the gameobject version of the hotbar slots within the content grid
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbarinventory[i] = null;
            GameObject childObject = Instantiate(hotbarslotprefab);
            childObject.transform.SetParent(hotbarslotgrid.transform, false);
            childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            childObject.GetComponent<HotbarSlotWrapper>().hotbarSlotNumber.GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
            childObject.GetComponent<HotbarSlotWrapper>().hotbarSlotNumber.GetComponent<TextMeshProUGUI>().color = deselectedspritecolor;
            hotbargridchildren[i] = childObject;
        }

        // Selecting the first hotbarslot
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
    }

    void Start()
    {
        necessitybargameobject.GetComponent<NecessityBars>().turnOffDisplayHungerIncrease();
    }

    public void loadData(GameData data)
    {
        currenttorchdurability = data.torchDurability;
        currentemberstonedurability = data.emberstoneDurability;

        for (int i = 0; i < data.playerInventory.Count; i++)
        {
            if (data.playerInventory[i] == -1)
            {
                hotbarinventory[i] = null;
            }
            else
            {
                hotbarinventory[i] = GameManager.instance.items[data.playerInventory[i]];
            }
        }

        resetHotbarItems();
    }

    public void saveData(ref GameData data)
    {
        data.torchDurability = currenttorchdurability;
        data.emberstoneDurability = currentemberstonedurability;

        for (int i = 0; i < hotbarinventory.Length; i++)
        {
            if (hotbarinventory[i] == null)
            {
                data.playerInventory[i] = -1;
            }
            else
            {
                data.playerInventory[i] = hotbarinventory[i].index;
            }
        }
    }

    void selectHotbar(int index)
    {
        // Start with setting the colors of all hotbars to deselected
        // It is inefficient, but its like max 10 elements, so who cares
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().hotbarSlotNumber.GetComponent<TextMeshProUGUI>().color = deselectedspritecolor;
        }

        consumableindicator.SetActive(false);
        necessitybargameobject.GetComponent<NecessityBars>().turnOffDisplayHungerIncrease();

        // Edge checks
        if (index >= maxhotbarsize)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = maxhotbarsize - 1;
        }

        // Select Hotbar
        currentindex = index;
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().hotbarSlotNumber.GetComponent<TextMeshProUGUI>().color = selectedspritecolor;

        if (hotbarinventory[currentindex] != null)
        {
            if (hotbarinventory[currentindex].consumable)
            {
                consumableindicator.SetActive(true);
                consumableindicator.GetComponent<TextMeshProUGUI>().text = consumabletext;
                consumableTimer = maxConsumableTimer;
                necessitybargameobject.GetComponent<NecessityBars>().displayHungerIncrease(hotbarinventory[currentindex].hungergain);
            }
            else if (hotbarinventory[currentindex] == torch)
            {
                if (currenttorchdurability <= 0)
                {
                    consumableindicator.SetActive(true);
                    consumableindicator.GetComponent<TextMeshProUGUI>().text = refueltorchtext;
                }
            }
            else if (hotbarinventory[currentindex] == emberstone)
            {
                if (currentemberstonedurability <= 0)
                {
                    consumableindicator.SetActive(true);
                    consumableindicator.GetComponent<TextMeshProUGUI>().text = refuelemberstonetext;
                }
            }
        }

        deleteHeldObjects();

        // This instantiates the 3D model of the scriptable object
        if (hotbarinventory[currentindex] != null && hotbarinventory[currentindex].model != null)
        {
            GameObject newmodel = Instantiate(hotbarinventory[currentindex].model);
            newmodel.transform.SetParent(spawnlocation.transform, false);
        }

        if (hotbarinventory[currentindex] == torch)
        {
            GameManager.instance.holdingtorch = true;
        }
        else
        {
            GameManager.instance.holdingtorch = false;
        }
    }

    public bool addItemToHotbar(ItemScript newItem)
    // Adds item to hotbar, returns false if hotbar is full.
    {
        int index = 0;
        while (index < maxhotbarsize)
        {
            if (hotbarinventory[index] == null)
            {
                hotbarinventory[index] = newItem;
                hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = newItem.icon;
                if (newItem == torch)
                {
                    currenttorchdurability = maxtorchdurability;
                    GameManager.instance.torchactive = true;
                    torchdurabilitybar = hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                }
                if (newItem == emberstone)
                {
                    emberstoneininventory = true;
                    currentemberstonedurability = maxemberstonedurability;
                    GameManager.instance.emberstoneactive = true;
                    emberstonedurabilitybar = hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                }                    
                selectHotbar(index); 
                return true;
            }
            index++;
        }

        return false;
    }

    public bool removeItemFromHotbar(int index)
    // Removes item from hotbar, returns false if hotbarslot is empty.
    {
        if (hotbarinventory[index] == null)
        {
            return false;
        }

        hotbarinventory[index] = null; 
        hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptyhotbar;
        consumableindicator.SetActive(false);

        if (currentindex == index)
        {
            deleteHeldObjects();
        }

        return true;
    }

    // This is meant to be used by the witch trading script. Due to this, it doesn't update the hotbar whatsoever, as resetHotbarItems will do it for it.
    public bool removeItemFromHotbar(ItemScript item)
    // Removes a specific item from the hotbar, return false if it does not exist
    {
        for (int i = 0; i < maxhotbarsize; i++)
        {
            if (hotbarinventory[i] == item)
            {
                hotbarinventory[i] = null; 
                return true;
            }
        }
        return false;
    }

    // Used by the Witch Trade Script
    public bool hasItem(ItemScript itemCheck, int count)
    {
        for (int i = 0; i < maxhotbarsize; i++)
        {
            if (hotbarinventory[i] == itemCheck)
            {
                count--;
                if (count == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void deleteHeldObjects()
    // Deletes held objects which are loaded in the scene
    {
        // DO NOT CHANGE! Childcount is not updated until next frame, if this were to be turned into a while loop it will ***crash the game.***
        for (int i = spawnlocation.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnlocation.transform.GetChild(i).gameObject);
        }
    }

    // This function is used to completely reset the players hotbar. Its used by the witch trading script, storage system, and itself. 
    public void resetHotbarItems()
    // Visually resets the players hotbar.
    {
        // I first remove all pictures that may be in the hotbar
        int index = 0;
        while (index < maxhotbarsize)
        {
            emberstoneininventory = false;
            GameManager.instance.emberstoneactive = false;
            hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptyhotbar;
            hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(false);
            hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(false);
            index++;
        }

        // Then add back in those which exist
        index = 0;
        while (index < maxhotbarsize)
        {
            if (hotbarinventory[index] != null)
            {
                hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = hotbarinventory[index].icon;

                if (hotbarinventory[index] == torch)
                {
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    torchdurabilitybar = hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar;

                    torchdurabilitybar.GetComponent<Image>().fillAmount = currenttorchdurability / maxtorchdurability;
                }
                else if (hotbarinventory[index] == emberstone)
                {
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    emberstonedurabilitybar = hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    emberstoneininventory = true;
                    GameManager.instance.emberstoneactive = true;
                    
                    emberstonedurabilitybar.GetComponent<Image>().fillAmount = currentemberstonedurability / maxemberstonedurability;
                }
            }
            index++;
        }

        // And select hotbar slot to the current index, to reset anything that is being held
        selectHotbar(currentindex);
    }

    public bool isHoldingItem(ItemScript item)
    {
        if (hotbarinventory[currentindex] == item)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int getCurrentIndex()
    {
        return currentindex;
    }

    public int getInventorySize()
    {
        return maxhotbarsize;
    }

    public ItemScript[] getInventoryItems()
    {
        return hotbarinventory;
    }

    public float getEmberstoneDurability()
    {
        return currentemberstonedurability / maxemberstonedurability;
    }
    
    public float getTorchDurability()
    {
        return currenttorchdurability / maxtorchdurability;
    }

    public void updateInventoryContents(ItemScript[] newinv)
    {
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbarinventory[i] = newinv[i];
        }

        resetHotbarItems();
    }

    void dropItem()
    {
        if (hotbarinventory[currentindex] == null)
        {
            return;
        }

        if (hotbarinventory[currentindex].unique)
        {
            return;
        }
        else
        {
            GameObject droppedItem = Instantiate(droppeditemprefab);
            droppedItem.transform.position = droplocation.transform.position;
            droppedItem.GetComponent<InteractableItem>().pickupitem = hotbarinventory[currentindex];
            removeItemFromHotbar(currentindex);
            selectHotbar(currentindex);
        }
    }

    void updateControls()
    {
        // Increase Hotbarslot, go right
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) 
        {
            selectHotbar(currentindex + 1);
        }
        // Decrease Hotbarslot, go left
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {
            selectHotbar(currentindex - 1);
        }
        
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                int selectedIndex = i;
                selectHotbar(selectedIndex);
                break; 
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            dropItem();
        }

        if (consumableTimer > 0)
        {
            consumableTimer -= Time.deltaTime;
            consumableindicator.GetComponent<TextMeshProUGUI>().alpha = consumableTimer / 2;
        }

        if (consumableindicator.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            if (hotbarinventory[currentindex].consumable)
            {
                if (TutorialManager.instance != null)
                {   
                    if (TutorialManager.instance.tutorialinprogress)
                    {
                        TutorialManager.instance.playerConsumedFood();
                    }
                }


                necessitybargameobject.GetComponent<NecessityBars>().increaseHunger(hotbarinventory[currentindex].hungergain);
                if (hotbarinventory[currentindex] == thePotion)
                {
                    playerDeathHandler.playerDrankPotion();
                }
                removeItemFromHotbar(currentindex);
                necessitybargameobject.GetComponent<NecessityBars>().turnOffDisplayHungerIncrease();
            }
            else if (hotbarinventory[currentindex] == torch)
            {
                bool check = removeItemFromHotbar(coal);
                if (check)
                {
                    GameManager.instance.torchactive = true;
                    currenttorchdurability = maxtorchdurability;
                    consumableindicator.SetActive(false);
                    resetHotbarItems();
                }
                else
                {
                    playerinteracthandler.displayErrorMessage(failedtorefuel);
                }
            }
            else if (hotbarinventory[currentindex] == emberstone)
            {
                bool check = removeItemFromHotbar(coal);
                if (check)
                {
                    GameManager.instance.emberstoneactive = true;
                    currentemberstonedurability = maxemberstonedurability;
                    consumableindicator.SetActive(false);
                    resetHotbarItems();
                }
                else
                {
                    playerinteracthandler.displayErrorMessage(failedtorefuel);
                }
            }
        }
    }

    void updateDurabilityBars()
    {
        if (hotbarinventory[currentindex] == torch)
        {
            if (currenttorchdurability > 0)
            {
                currenttorchdurability -= 1 * Time.deltaTime;
                torchdurabilitybar.GetComponent<Image>().fillAmount = currenttorchdurability / maxtorchdurability;
                if (currenttorchdurability <= 0)
                {
                    GameManager.instance.torchactive = false;
                    if (hotbarinventory[currentindex] == torch)
                    {
                        consumableindicator.SetActive(true);
                        consumableindicator.GetComponent<TextMeshProUGUI>().text = refueltorchtext;
                    }
                }
            }
        }
        if (emberstoneininventory)
        {
            if (currentemberstonedurability > 0)
            {
                currentemberstonedurability -= 1 * Time.deltaTime;
                emberstonedurabilitybar.GetComponent<Image>().fillAmount = currentemberstonedurability / maxemberstonedurability;
                if (currentemberstonedurability <= 0)
                {
                    GameManager.instance.emberstoneactive = false;
                    if (hotbarinventory[currentindex] == emberstone)
                    {
                        consumableindicator.SetActive(true);
                        consumableindicator.GetComponent<TextMeshProUGUI>().text = refuelemberstonetext;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (!GameManager.instance.inMenu)
        {
            updateControls();
        }
        updateDurabilityBars();
    }
}
