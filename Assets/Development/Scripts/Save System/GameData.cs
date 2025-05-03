using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData 
{
    public Vector3 playerPos;
    public float currentHunger;
    public float currentTemperature;
    public float torchDurability;
    public float emberstoneDurability;
    public bool torchActive;
    public bool emberstoneActive;

    public bool campfireUnlocked;
    public int coalStored;
    public int sticksEmplaced;
    public float burnTimer;

    public bool chestUnlocked;

    public List<bool> discoveredItems;
    public List<bool> hasBeenCrafted;

    public List<bool> dialogueFlags;
    public List<int> witchDialogueQueue;

    public List<int> playerInventory;
    public List<int> chestInventory;

    public SerializableDictionary<string, int> interactableItemCounts;

    public bool isNight;
    public float timeOfDay;

    // This is all for the tutorial, don't ask...

    public bool tutorialinprogress;
    public bool cannotcraft;
    public bool cannottalk;
    public bool inactiveNecessityBars;

    public bool caveWallActive;
    public bool caveWallColliderActive;
    public bool walkColliderActive;

    public bool wakingup;
    public bool canSkipTutorial;
    public bool interactwithwitch;
    public bool teachingitems;
    public bool findsticks;
    public bool teachingPickup;
    public bool loadNextDialogue;
    public bool teachingBargaining;
    public bool teachingCampfire;
    public bool doingExposition;

    public GameData()
    {
        playerPos = Vector3.zero;
        currentHunger = 0;
        currentTemperature = 0;
        torchDurability = 0;
        emberstoneDurability = 0;
        torchActive = false;
        emberstoneActive = false;
        coalStored = 0;
        sticksEmplaced = 0; 
        burnTimer = 0;
        campfireUnlocked = false;
        chestUnlocked = false;
        discoveredItems = new List<bool>();
        hasBeenCrafted = new List<bool>();
        dialogueFlags = new List<bool>();
        witchDialogueQueue = new List<int>();
        playerInventory = new List<int>();
        chestInventory = new List<int>();
        interactableItemCounts = new SerializableDictionary<string, int>();
        isNight = false;
        timeOfDay = 0;

        tutorialinprogress = false;
        cannotcraft = false;
        cannottalk = false;
        inactiveNecessityBars = false;
        wakingup = false;
        canSkipTutorial = false;
        interactwithwitch = false;
        teachingitems = false;
        findsticks = false;
        teachingPickup = false;
        loadNextDialogue = false;
        teachingBargaining = false;
        teachingCampfire = false;
        doingExposition = false;
        caveWallActive = false;
        caveWallColliderActive = false;
        walkColliderActive = false;
    }
}
