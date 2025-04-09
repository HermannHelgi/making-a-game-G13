using System.ComponentModel.Design;
using Polybrush;
using UnityEngine;
using UnityEngine.UI;

public class StorageSystem : MonoBehaviour
{
    public int maxstoragesize = 20;
    public Sprite emptystoragesprite;    
    public GameObject storageslotprefab;
    public GameObject storageslotgrid;
    public GameObject storagecanvas;


    private int inventorysize = 0;
    public GameObject inventoryslotgrid;
    private ItemScript[] inventoryitemcontents;
    private GameObject[] inventoryslots;



    [Header("Sprite variables")]
    public Color selectedstoragecolor = Color.white;
    public Color deselectedstoragecolor = Color.gray;
    public Color selectedspritecolor = Color.white;
    public Color deselectedspritecolor = Color.white;


    private int currentindex = 0;
    private ItemScript[] itemcontents;
    private GameObject[] storageslots;
    private GameObject playerinteracthandler;
    private bool active;


    void Start()
    {
        //  Instantiate the arrays
        itemcontents = new ItemScript[maxstoragesize];
        storageslots = new GameObject[maxstoragesize];
        inventoryslots = null;
        active = false;
        // Create the gameobject version of the hotbar slots within the content grid
        for (int i = 0; i < maxstoragesize; i++)
        {
            itemcontents[i] = null;
            GameObject childObject = Instantiate(storageslotprefab);
            childObject.transform.SetParent(storageslotgrid.transform, false);
            childObject.GetComponent<HotbarSlotWrapper>().frame.GetComponent<Image>().color = deselectedstoragecolor;
            storageslots[i] = childObject;
        }
    }

    public void initializeStorageWindow(int invsize, ItemScript[] inventoryitems, GameObject playerih)
    {
        active = true;

        if (inventoryslots == null)
        {
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
            }
        }

        storagecanvas.SetActive(true);
        selectStorageSlot(0);
    }

    void selectStorageSlot(int index)
    {
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

        if (index < 0)
        {
            index = maxstoragesize + inventorysize - 1;
        }
        if (index > maxstoragesize + inventorysize - 1)
        {
            index = 0;
        }
        currentindex = index;

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
    {
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

        if (currentindex < maxstoragesize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                if (inventoryitemcontents[i] == null)
                {
                    storageslots[currentindex].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
                    inventoryitemcontents[i] = itemcontents[currentindex];
                    itemcontents[currentindex] = null;
                    inventoryslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = inventoryitemcontents[i].icon;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < maxstoragesize; i++)
            {
                if (itemcontents[i] == null)
                {
                    inventoryslots[currentindex - maxstoragesize].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = emptystoragesprite;
                    itemcontents[i] = inventoryitemcontents[currentindex - maxstoragesize];
                    inventoryitemcontents[currentindex - maxstoragesize] = null;
                    storageslots[i].GetComponent<HotbarSlotWrapper>().sprite.GetComponent<Image>().sprite = itemcontents[i].icon;
                    break;
                }
            }
        }

        // TODO:
        // Lock player when interacting

    }

    void deInitializeStorageWindow()
    {
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
        }
    }
}
