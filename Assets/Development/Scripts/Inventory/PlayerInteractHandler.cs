using TMPro;
using UnityEngine;

public class PlayerInteractHandler : MonoBehaviour
{
    [Header("Raycast variables")]
    [Tooltip("Length of interact raycast, increase for the player to have longer reach.")]
    public float raycastlength = 3;
    [Tooltip("The gameobject in the scene which has the PlayerInventory script. Required for the script to function.")]
    public GameObject playerinventoryobject;


    [Header("Text pop-up variables")]
    [Tooltip("TextMeshPro element for the 'Press E to interact' text.")]
    public TextMeshProUGUI popuptext;
    [Tooltip("TextMeshPro element for the 'Inventory full' text.")]
    public TextMeshProUGUI inventoryfulltext;
    [Tooltip("The amount of time the 'Inventory full' text is displayed.")]
    public float inventoryfullmaxtimer = 3;

    // The clock variable is the actual timer which is manipulated, maxtimer is the value it resets to
    private float inventoryfullclock = 0;

    [Header("Witch in the Wall variables")]
    [Tooltip("The canvas for which the trade overlay appears.")]
    public GameObject witchtradeoverlay;
    [Tooltip("The grid which will be used to spawn the crafting recipes inside.")]
    public GameObject witchrecipegridspawn;
    [Tooltip("The players normal overlay, meant to be turned off when trading starts.")]
    public GameObject inventoryoverlay;
    [Tooltip("The player, needed to measure distance from the witch and the player.")]
    public GameObject playerobject;    
    [Tooltip("The text mesh component within the Witch Trade Canvas which should be updated on new Item craft.")]
    public TextMeshProUGUI nameofitemincanvastextmesh;
    [Tooltip("The text mesh component within the Witch Trade Canvas which should be updated on new Item craft.")]
    public TextMeshProUGUI ingredientslisttextmesh;


    void Start()
    {
        popuptext.gameObject.SetActive(false);
        inventoryfulltext.gameObject.SetActive(false);

        if (playerinventoryobject == null)
        {
            Debug.LogWarning("PlayerInteractHandler in the scene is missing the object with the PlayerInventory Script! Fix this!");
        }
    }


    void Update()
    {
        // This handles the "pop-up" text for the interact, if we want to customise it at all then it needs to be changed.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastlength))
        { 
            var script = hit.transform.GetComponent<InteractableItem>();
            // -------------- WARNING/TODO! This probably needs to change for when dialogue is implemented. --------------
            var witchscript = hit.transform.GetComponent<WitchTradeScript>();
            if (script != null || witchscript != null)
            // -------------- WARNING/TODO! This probably needs to change for when dialogue is implemented. --------------
            {
                popuptext.gameObject.SetActive(true);
            }
            else
            {
                popuptext.gameObject.SetActive(false);
            }
        }
        else
        { 
            popuptext.gameObject.SetActive(false);
        }

        // If the player wants to interact with something
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast to get the object
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastlength))
            { 
                // I check for a script since the player can do this raycast at any time, so need to make sure whatever object is there doesn't crash the game when we call an unknown method lol
                var script = hit.transform.GetComponent<InteractableItem>();
                if (script != null)
                {
                    // The item somehow has to gain access to the PlayerInventory script, first idea I came up with.
                    bool test = script.pickUp(playerinventoryobject);
                    // if it fails, then the inventory is full.
                    if (!test)
                    {
                        inventoryfulltext.gameObject.SetActive(true);
                        // Starts the timer, or resets it if it's already going
                        inventoryfullclock = inventoryfullmaxtimer;
                    }
                }

                // Additional check for if it has the witchTradeScript
                // -------------- WARNING/TODO! This probably needs to change for when dialogue is implemented. --------------
                var witchscript = hit.transform.GetComponent<WitchTradeScript>();
                if (witchscript != null)
                {
                    witchscript.initializeTradeWindow(witchtradeoverlay, witchrecipegridspawn, inventoryoverlay, playerinventoryobject, playerobject, nameofitemincanvastextmesh, ingredientslisttextmesh);
                }
                // -------------- WARNING/TODO! This probably needs to change for when dialogue is implemented. --------------
            }
        }

        // I just made a timer for the "Inventory Full" message, its simplest
        if (inventoryfullclock > 0)
        {
            inventoryfullclock -= Time.deltaTime;
            if (inventoryfullclock <= 0)
            {
                inventoryfulltext.gameObject.SetActive(false);
            }
        }

    }
}
