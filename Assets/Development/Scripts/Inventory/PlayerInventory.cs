using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
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
    

    private int currentindex = 0;
    private ItemScript[] hotbarinventory;
    private GameObject[] hotbargridchildren;

    void Start()
    {
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
            hotbargridchildren[i] = childObject;
        }

        // Selecting the first hotbarslot
        hotbargridchildren[currentindex].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = selectedhotbarcolor;
    }

    void selectHotbar(int index)
    {
        // Start with setting the colors of all hotbars to deselected
        // It is inefficient, but its like max 10 elements, so who cares
        for (int i = 0; i < maxhotbarsize; i++)
        {
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedhotbarcolor;
            hotbargridchildren[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().color = deselectedspritecolor;
        }

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

        deleteHeldObjects();

        // This instantiates the 3D model of the scriptable object
        if (hotbarinventory[currentindex] != null && hotbarinventory[currentindex].model != null)
        {
            GameObject newmodel = Instantiate(hotbarinventory[currentindex].model);
            newmodel.transform.SetParent(spawnlocation.transform, false);
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

    // This exists purely for the witch trading script. Its called when trades are over to update any sprites which exist within the hotbar if any trades occur.
    public void resetHotbarItems()
    {
        // I first remove all pictures that may be in the hotbar
        int index = 0;
        while (index < maxhotbarsize)
        {
            hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptyhotbar;
            index++;
        }

        // Then add back in those which exist
        index = 0;
        while (index < maxhotbarsize)
        {
            if (hotbarinventory[index] != null)
            {
                hotbargridchildren[index].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = hotbarinventory[index].icon;
            }
            index++;
        }

        // And select hotbar slot 0, to reset anything that is being held
        selectHotbar(0);
    }

    void Update()
    {
        // Increase Hotbarslot, go right
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) 
        {
            selectHotbar(currentindex + 1);
        }
        // Decrease Hotbarslot, go left
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) 
        {
            selectHotbar(currentindex - 1);
        }

        // ------------------ NEEDS TO BE MOVED TO PLAYER CONTROLS ---------------------
        if (Input.GetKeyDown(KeyCode.Q))
        {
            removeItemFromHotbar(currentindex);
        }
        // -----------------------------------------------------------------------------
    }
}
