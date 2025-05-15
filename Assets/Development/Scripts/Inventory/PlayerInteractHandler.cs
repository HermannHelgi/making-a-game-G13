using System.Linq;
using StarterAssets;
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
    [Tooltip("The text that should be displayed on being able to interact with an object.")]
    public string interactwithobjectstring;
    [Tooltip("TextMeshPro element for error messages which should be displayed to the player.")]
    public TextMeshProUGUI errormessagetext;
    [Tooltip("String of text which is displayed when the player tries to pick up an item with full inventory.")]
    public string inventoryfullstring;
    [Tooltip("The amount of time the error messages which should be displayed to the player is displayed.")]
    public float errormessagemaxtimer = 3;

    // The clock variable is the actual timer which is manipulated, maxtimer is the value it resets to
    private float errormessageclock = 0;

    [Header("Witch in the Wall variables")]
    [Tooltip("The players normal overlay, meant to be turned off when another menu rises starts.")]
    public GameObject inventoryoverlay;
    [Tooltip("The player, needed to measure distance from the witch and the player.")]
    public GameObject playerobject;
    [Tooltip("The text mesh component within the InventoryCanvas which should be updated on new dialogue.")]
    public GameObject subtitletextmesh;
    [Tooltip("The text mesh component within the Witch Trade Canvas which should be updated when switching between craftable items.")]
    public string bargainpopupstring;
    [Tooltip("The text that should be displayed on being able to talk with the witch in the wall.")]
    public string dialoguepopupstring;
    [Tooltip("A reference to the player look script.")]
    public GameObject playerlookscript;
    [Tooltip("The text that should be displayed to indicate the player can leave the meanu by pressing escape.")]
    public GameObject escapemessage;

    [Header("Campfire variables")]
    [Tooltip("The ItemScript which should be referenced as fuel for the campfire.")]
    public ItemScript stick;
    [Tooltip("The text that should be displayed on wanting to interact with the campfire.")]
    public string campfirenosticktext;
    [Tooltip("The text that should be displayed on being able to add fuel to the campfire.")]
    public string campfireaddfuelstring;
    [Tooltip("The text that should be displayed on being able to pickup coal from campfire.")]
    public string campfirepickupcoal;

    [Header("Storage system variables")]
    [Tooltip("The interact text which is shown on being able to open storage system.")]
    public string storageinteracttext;
    public bool highlighting = false;

    [Header("Effigy variables")]
    [Tooltip("The displayed text on being able to interact with the effigy in game.")]
    public string effigyInteractText;
    [Tooltip("Reference to the lure ItemScript.")]
    public ItemScript lure;

    void Start()
    {
        popuptext.gameObject.SetActive(false);
        errormessagetext.gameObject.SetActive(false);
        subtitletextmesh.SetActive(false);

        if (playerinventoryobject == null)
        {
            Debug.LogWarning("PlayerInteractHandler in the scene is missing the object with the PlayerInventory Script! Fix this!");
        }
    }

    void Update()
    {
        handlePopupText();
        handleInteraction();
        handleInventoryFullTimer();
    }

    void handlePopupText()
    {
        // This handles the "pop-up" text for interactions
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastlength))
        {
            var interactableitemscript = hit.transform.GetComponent<InteractableItem>();
            // The witch trade script kinda works as a wrapper for both the trade system and dialogue system.
            var witchscript = hit.transform.GetComponent<WitchTradeScript>();
            var campfirescript = hit.transform.GetComponent<CampfireScript>();
            var storagescript = hit.transform.GetComponent<StorageSystem>();
            var effigyScript = hit.transform.GetComponent<EffigyScript>();
            popuptext.gameObject.SetActive(false);

            if (interactableitemscript != null)
            {
                popuptext.text = interactwithobjectstring;
                popuptext.gameObject.SetActive(true);
                hit.transform.GetComponent<Outline>().enabled = true;
                highlighting = true;
            }
            else if (witchscript != null)
            // Witch scripts, dialogue and bargaining
            {
                if (witchscript.canTalk())
                {
                    if (TutorialManager.instance != null)
                    {
                        if (!TutorialManager.instance.cannottalk)
                        {
                            popuptext.text = dialoguepopupstring;
                            popuptext.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        popuptext.text = dialoguepopupstring;
                        popuptext.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (TutorialManager.instance != null)
                    {
                        if (!TutorialManager.instance.cannotcraft)
                        {
                            popuptext.text = bargainpopupstring;
                            popuptext.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        popuptext.text = bargainpopupstring;
                        popuptext.gameObject.SetActive(true);
                    }
                }
            }
            else if (campfirescript != null)
            // Campfire
            {
                if (playerinventoryobject.GetComponent<PlayerInventory>().isHoldingItem(stick))
                {
                    popuptext.text = campfireaddfuelstring;
                    popuptext.gameObject.SetActive(true);
                }
                else if (campfirescript.coalstored > 0)
                {
                    popuptext.text = campfirepickupcoal;
                    popuptext.gameObject.SetActive(true);
                }
                else
                {
                    popuptext.text = campfirenosticktext;
                    popuptext.gameObject.SetActive(true);
                }
            }
            else if (storagescript != null)
            // Storage system
            {
                popuptext.text = storageinteracttext;
                popuptext.gameObject.SetActive(true);
            }
            else if (effigyScript != null && playerinventoryobject.GetComponent<PlayerInventory>().isHoldingItem(lure))
            // Effigy
            {
                popuptext.text = effigyInteractText;
                popuptext.gameObject.SetActive(true);
            }
            else
            {
                popuptext.gameObject.SetActive(false);
                highlighting = false;
            }
        }
        else
        {
            popuptext.gameObject.SetActive(false);
            highlighting = false;
        }
    }

    void handleInteraction()
    // Handles all interactions, such as the player interacting with the witch to start a dialogue.
    {
        if (GameManager.instance.inMenu)
        {
            playerobject.GetComponent<FirstPersonController>().freezecamera = true;
            playerobject.GetComponent<FirstPersonController>().freezeinmenu = true;
        }
        else
        {
            playerobject.GetComponent<FirstPersonController>().freezecamera = false;
            playerobject.GetComponent<FirstPersonController>().freezeinmenu = false;
        }

        // If the player wants to interact with something
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast to get the object
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastlength))
            {
                // I check for a script since the player can do this raycast at any time, so need to make sure whatever object is there doesn't crash the game when we call an unknown method lol
                var interactableitemscript = hit.transform.GetComponent<InteractableItem>();
                if (interactableitemscript != null)
                {
                    // The item somehow has to gain access to the PlayerInventory script, first idea I came up with.
                    bool test = interactableitemscript.pickUp(playerinventoryobject);
                    // if it fails, then the inventory is full.
                    if (!test)
                    {
                        // Starts the timer, or resets it if it's already going
                        displayErrorMessage(inventoryfullstring);
                    }
                }

                // Additional check for if it has the witchTradeScript
                var witchscript = hit.transform.GetComponent<WitchTradeScript>();
                if (witchscript != null && !GameManager.instance.inMenu)
                {
                    // this initialize Trade Window will also handle the dialogue for the witch
                    GameManager.instance.inMenu = true;
                    witchscript.initializeTradeWindow(
                        inventoryoverlay,
                        playerinventoryobject,
                        subtitletextmesh,
                        escapemessage,
                        playerlookscript);
                }

                // Additional check for the campfire
                var campfirescript = hit.transform.GetComponent<CampfireScript>();
                if (campfirescript != null)
                {
                    // Player is trying to feed the flame
                    if (playerinventoryobject.GetComponent<PlayerInventory>().isHoldingItem(stick))
                    {
                        playerinventoryobject.GetComponent<PlayerInventory>().removeItemFromHotbar(playerinventoryobject.GetComponent<PlayerInventory>().getCurrentIndex());
                        campfirescript.addFuel();
                    }
                    // Player is trying to collect coal
                    else if (campfirescript.coalstored > 0)
                    {
                        ItemScript gatheredcoal = campfirescript.gatherCoal();
                        bool test = playerinventoryobject.GetComponent<PlayerInventory>().addItemToHotbar(gatheredcoal);
                        if (!test)
                        {
                            displayErrorMessage(inventoryfullstring);
                            campfirescript.failedToGatherCoal();
                        }
                        else
                        {
                            GameManager.instance.discovereditems[gatheredcoal.index] = true;
                        }
                    }
                }

                // Additional check for the storage system
                var storagescript = hit.transform.GetComponent<StorageSystem>();
                if (storagescript != null && !GameManager.instance.inMenu)
                {
                    inventoryoverlay.SetActive(false);
                    storagescript.initializeStorageWindow(playerinventoryobject, this.gameObject);
                    GameManager.instance.inMenu = true;
                }

                // Final check for effigy
                var effigyScript = hit.transform.GetComponent<EffigyScript>();
                if (effigyScript != null && playerinventoryobject.GetComponent<PlayerInventory>().isHoldingItem(lure))
                {
                    playerinventoryobject.GetComponent<PlayerInventory>().removeItemFromHotbar(lure);
                    playerinventoryobject.GetComponent<PlayerInventory>().resetHotbarItems();
                    effigyScript.activateLure();
                }
            }
        }
    }

    void handleInventoryFullTimer()
    {
        // I just made a timer for the error messages, its simplest
        if (errormessageclock > 0)
        {
            errormessageclock -= Time.deltaTime;
            if (errormessageclock <= 0)
            {
                errormessagetext.gameObject.SetActive(false);
            }
        }
    }

    public void displayErrorMessage(string text)
    {
        errormessagetext.gameObject.SetActive(true);
        errormessagetext.text = text;
        errormessageclock = errormessagemaxtimer;
    }

    public void leavingStorageSystem(ItemScript[] newinv)
    {
        playerinventoryobject.GetComponent<PlayerInventory>().updateInventoryContents(newinv);
        inventoryoverlay.SetActive(true);
        GameManager.instance.activateMenuCooldown();
    }
}