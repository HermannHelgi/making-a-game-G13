using UnityEngine;
using UnityEngine.UI;

public class StorageSystem : MonoBehaviour
{
    [Header("Storage system variables")]
    [Tooltip("The maximum amount of slots in chest.")]
    public int maxstoragesize = 24;
    [Tooltip("The empty sprite image.")]
    public Sprite emptystoragesprite;    
    [Tooltip("Prefab for the UI element storage slot to display items in chest.")]
    public GameObject storageslotprefab;
    [Tooltip("The grid for which the storage slots are displayed within.")]
    public GameObject storageslotgrid;
    [Tooltip("The storage system canvas.")]
    public GameObject storagecanvas;
    [Tooltip("The grid for which the storage slots of the inventory are displayed within.")]
    public GameObject inventoryslotgrid;

    [Header("Specific item variables")]
    [Tooltip("The Itemscript for the Torch item. Needed for specific references.")]
    public ItemScript torch;
    [Tooltip("The Itemscript for the Emberstone item. Needed for specific references.")]
    public ItemScript emberstone;

    [Header("Sprite variables")]
    public Color selectedstoragecolor = Color.white;
    public Color deselectedstoragecolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.white;


    // Private stuffs
    private int currentindex = 0;
    private int inventorysize = 0;
    // Itemscripts for inventory
    private ItemScript[] inventoryitemcontents;
    // UI slots for inventory
    private GameObject[] inventoryslots;
    // Itemscripts for chest
    private ItemScript[] itemcontents;
    // UI slots for chest
    private GameObject[] storageslots;
    private GameObject playerinteracthandler;
    private bool active;
    private GameObject playerinventoryobject;
    private GameObject torchdurabilitybar = null;
    private GameObject emberstonedurabilitybar = null;

    void Start()
    {
        //  Instantiate the arrays
        itemcontents = new ItemScript[maxstoragesize];
        storageslots = new GameObject[maxstoragesize];
        inventoryslots = null;
        active = false;
        // Create the gameobject version of the storage slots within the content grid
        for (int i = 0; i < maxstoragesize; i++)
        {
            itemcontents[i] = null;
            GameObject childObject = Instantiate(storageslotprefab);
            childObject.transform.SetParent(storageslotgrid.transform, false);
            childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedstoragecolor;
            storageslots[i] = childObject;
        }
    }

    public void initializeStorageWindow(GameObject playerib, GameObject playerih)
    // Initializes the storage window and loads all relevant data for player.
    {
        // Collecting references and data
        playerinventoryobject = playerib;
        int invsize = playerinventoryobject.GetComponent<PlayerInventory>().getInventorySize();
        ItemScript[] inventoryitems = playerinventoryobject.GetComponent<PlayerInventory>().getInventoryItems();

        if (inventoryslots == null)
        {
            // If this is the first time the player is opening the chest, create the inventory
            // I went off the assumption that the chest doesn't know how large the players inventory is at start, and therefore needs to receive that data now and load it in.
            playerinteracthandler = playerih;
            inventorysize = invsize;
            inventoryitemcontents = new ItemScript[inventorysize];
            inventoryslots = new GameObject[inventorysize];
        
            for (int i = 0; i < inventorysize; i++)
            {
                inventoryitemcontents[i] = null;
                GameObject childObject = Instantiate(storageslotprefab);
                childObject.transform.SetParent(inventoryslotgrid.transform, false);
                childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedstoragecolor;
                inventoryslots[i] = childObject;
            }
        }

        // Wipe the storage systems contents and reload them, relevant for saving later on as well as if witch can grab items from chest for trading
        for (int i = 0; i < maxstoragesize; i++)
        {
            storageslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
            storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(false);
            storageslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(false);

            if (itemcontents[i] != null)
            {
                storageslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = itemcontents[i].icon;
                if (itemcontents[i] == torch)
                {
                    // IK these blocks of code seem copy-pasted, but honestly its not worth adding to a function since it could be in two different places and shiz, its like ten lines max added 
                    storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    storageslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    torchdurabilitybar = storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    torchdurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getTorchDurability();
                }
                else if (itemcontents[i] == emberstone)
                {
                    storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    storageslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    emberstonedurabilitybar = storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    emberstonedurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getEmberstoneDurability();
                }
            }
        }

        // Load all the necessary data in for the players hotbar.
        for (int i = 0; i < inventorysize; i++)
        {
            inventoryitemcontents[i] = inventoryitems[i];
            if (inventoryitemcontents[i] == null)
            {
                inventoryslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
            }
            else
            {
                inventoryslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = inventoryitems[i].icon;
                if (inventoryitemcontents[i] == torch)
                {
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    torchdurabilitybar = inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    torchdurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getTorchDurability();
                }
                else if (inventoryitemcontents[i] == emberstone)
                {
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                    emberstonedurabilitybar = inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                    emberstonedurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getEmberstoneDurability();
                }
            }
        }

        storagecanvas.SetActive(true);
        active = true;
        selectStorageSlot(0);
    }

    void selectStorageSlot(int index)
    // Used to move the currently selected storage slot.
    {
        // Wipe all storage slots, make em all deselected.
        for (int i = 0; i < maxstoragesize; i++)
        {
            storageslots[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedstoragecolor;
            storageslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

        for (int i = 0; i < inventorysize; i++)
        {
            inventoryslots[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedstoragecolor;
            inventoryslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

        // Edge checks
        if (index < 0)
        {
            index = maxstoragesize + inventorysize - 1;
        }
        if (index > maxstoragesize + inventorysize - 1)
        {
            index = 0;
        }
        currentindex = index;

        // Now figure out what to highlight.
        if (currentindex < maxstoragesize)
        {
            storageslots[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedstoragecolor;
            storageslots[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;
        }
        else
        {
            inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedstoragecolor;
            inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = selectedspritecolor;
        }
    }

    void moveItem()
    // Moves item from the storage system to the player hotbar and vice versa.
    {
        // If the item is null, ignore it
        if (currentindex < maxstoragesize)
        {
            if (itemcontents[currentindex] == null)
            {
                return;
            }
        }
        else
        {
            if (inventoryitemcontents[currentindex - maxstoragesize] == null)
            {
                return;
            }
        }

        // Item is in storage chest, needs to be moved to hotbar
        if (currentindex < maxstoragesize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                if (inventoryitemcontents[i] == null)
                {
                    // First, wipe the location in the chest.
                    storageslots[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
                    if (itemcontents[currentindex] == torch || itemcontents[currentindex] == emberstone)
                    {
                        storageslots[currentindex].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(false);
                        storageslots[currentindex].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(false);
                    }

                    // Copy the contents over and wipe the original
                    inventoryitemcontents[i] = itemcontents[currentindex];
                    itemcontents[currentindex] = null;

                    // Now reload the necessary sprites and potentional durability bars.
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = inventoryitemcontents[i].icon;
                    if (inventoryitemcontents[i] == torch)
                    {
                        inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                        inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                        torchdurabilitybar = inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                        torchdurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getTorchDurability();
                    }
                    else if (inventoryitemcontents[i] == emberstone)
                    {
                        inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                        inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                        emberstonedurabilitybar = inventoryslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                        emberstonedurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getEmberstoneDurability();
                    }
                    
                    break;
                }
            }
        }
        // Item is in hotbar, needs to be moved to chest
        else
        {
            for (int i = 0; i < maxstoragesize; i++)
            {
                if (itemcontents[i] == null)
                {
                    // First, wipe the location in the hotbar.
                    inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
                    if (inventoryitemcontents[currentindex - maxstoragesize] == torch || inventoryitemcontents[currentindex - maxstoragesize] == emberstone)
                    {
                        inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(false);
                        inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(false);
                    }

                    // Copy the contents over and wipe the original
                    itemcontents[i] = inventoryitemcontents[currentindex - maxstoragesize];
                    inventoryitemcontents[currentindex - maxstoragesize] = null;

                    // Now reload the necessary sprites and potentional durability bars.
                    storageslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = itemcontents[i].icon;
                    if (itemcontents[i] == torch)
                    {
                        storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                        storageslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                        torchdurabilitybar = storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                        torchdurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getTorchDurability();
                    }
                    else if (itemcontents[i] == emberstone)
                    {
                        storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar.SetActive(true);
                        storageslots[i].GetComponent<HotbarSlotWrapper>().durabilityframe.SetActive(true);
                        emberstonedurabilitybar = storageslots[i].GetComponent<HotbarSlotWrapper>().durabilitybar;
                        emberstonedurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getEmberstoneDurability();
                    }

                    break;
                }
            }
        }
    }

    void deInitializeStorageWindow()
    // De-initlaizes the storage window, is called when the player leaves.
    {
        active = false;
        storagecanvas.SetActive(false);
        playerinteracthandler.GetComponent<PlayerInteractHandler>().leavingStorageSystem(inventoryitemcontents);
    }

    void Update()
    {
        if (active)
        {
            // Increase storage slot, go right
            if (Input.GetAxis("Mouse ScrollWheel") < 0f ) 
            {
                selectStorageSlot(currentindex + 1);
            }
            // Decrease storage slot, go left
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f ) 
            {
                selectStorageSlot(currentindex - 1);
            }
            
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                moveItem();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deInitializeStorageWindow();
            }

            // Updates the players durability bars in chest / hotbar if needed.
            if (torchdurabilitybar != null)
            {
                torchdurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getTorchDurability();
            }
            if (emberstonedurabilitybar != null)
            {
                emberstonedurabilitybar.GetComponent<Image>().fillAmount = playerinventoryobject.GetComponent<PlayerInventory>().getEmberstoneDurability();
            }
        }
    }
}
