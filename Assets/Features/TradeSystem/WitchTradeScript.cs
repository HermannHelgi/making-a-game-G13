using UnityEngine;
using UnityEngine.UI;

public class WitchTradeScript : MonoBehaviour
{
   
    // GAMEMANAGER STUFF
    public bool[] discovereditems = new bool[17];


    // Normal things
    public float distancetoturnoffwitchoverlay = 10;

    public ItemScript[] craftableItems = new ItemScript[7];

    public GameObject tradeslotprefab;

    [Header("Sprite variables")]
    public Color selectedhotbarcolor = Color.white;
    public Color deselectedhotbarcolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.gray;

    

    // Private stuff, yeah ik its a lot
    private GameObject[] tradeslotgridchildren;
    private int currentindex = 0;
    private GameObject witchrecipegridspawner; 
    private GameObject playerinventory;
    private GameObject witchoverlay;
    private PlayerInventory playerinventoryscript;
    private GameObject player;
    private bool currentlytrading = false;


    void Start()
    {
        tradeslotgridchildren = new GameObject[craftableItems.Length];
    }

    void Update()
    {
        if (currentlytrading)
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) > distancetoturnoffwitchoverlay)
            {
                deinitializeTradeWindow();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deinitializeTradeWindow();
            }
        }

        // need to check if the player is using the mousewheel to scroll through recipes
        // need to check through the players hotbar as well as their storage, if applicable
    }

    // Need function for de intializing windows
    // Need a function for updating the hotbar as well as updating storagesystem
    // need a function for removing items and adding crafted item

    public void initializeTradeWindow(GameObject witchtradecanvas, GameObject witchrecipegridspawnerobject, GameObject playerinventorycanvas, GameObject playerinventoryscriptobject, GameObject playerobject)
    {
        // Might need to initialize / turn on the players cursor
        playerinventory = playerinventorycanvas;
        witchoverlay = witchtradecanvas;
        witchrecipegridspawner = witchrecipegridspawnerobject;
        playerinventoryscript = playerinventoryscriptobject.GetComponent<PlayerInventory>();
        player = playerobject;

        playerinventory.SetActive(false);
        witchoverlay.SetActive(true);
        playerinventoryscript.deleteHeldObjects();
        currentlytrading = true;
        currentindex = 0;
        spawnCraftingRecipeBoxes();
        selectCraftingSlot(currentindex);
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

    void deinitializeTradeWindow()
    {
        wipeCraftingRecipeBoxes();
        playerinventory.SetActive(true);
        witchoverlay.SetActive(false);
        playerinventoryscript.resetHotbarItems();
        currentlytrading = false;
    }
}
