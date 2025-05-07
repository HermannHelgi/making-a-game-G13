using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [Header("GameManager variables")]
    [Tooltip("Base singleton instance of the Game Manager, do not touch!")]
    public static GameManager instance;

    [Header("Player variables.")]
    [Tooltip("Bool variable used by the Wendigo AI script to see whether the player is holding a torch.")]
    public bool holdingtorch;
    public bool torchactive;
    public bool emberstoneactive;
    public bool inMenu = false; 
    private float menucooldown = 0;
    private float maxmenucooldown = 0.10f;
    [Tooltip("An array for all Item scriptable objects in the game.")]
    public ItemScript[] items = new ItemScript[18];

    [Header("Witch in the wall variables.")]
    [Tooltip("An item array for all the possible items in the game, once set to true the item is discovered.")]
    public bool[] discovereditems = new bool[18];

    [Header("Day and night variables.")]
    [Tooltip("A bool which says whether the game is day or night.")]
    public bool isNight = false;
    

    [Header("Player variables.")]
    [Tooltip("A bool which says the player is in a deemed safe area")]
    public bool safeArea = false;

    [Header("Effigy variables.")]
    public bool lureCrafted = false;
    public bool lurePlaced = false;
    public bool skullPickedUp = false;


    void Awake()
    {
        instance = this;
    }

    public void loadData(GameData data)
    {
        discovereditems = data.discoveredItems.ToArray();
        torchactive = data.torchActive;
        emberstoneactive = data.emberstoneActive;
        isNight = data.isNight;

        lureCrafted = data.lureCrafted;
        lurePlaced = data.lurePlaced;
        skullPickedUp = data.skullPickedUp;
    }

    public void saveData(ref GameData data)
    {
        data.discoveredItems = new List<bool>(discovereditems);
        data.emberstoneActive = emberstoneactive;
        data.torchActive = torchactive;
        data.isNight = isNight;

        data.lureCrafted = lureCrafted;
        data.lurePlaced = lurePlaced;
        data.skullPickedUp = skullPickedUp;
    }

    void Update()
    {
        if (menucooldown > 0)
        {
            menucooldown -= Time.deltaTime;
            if (menucooldown <= 0)
            {
                inMenu = false;
            }
        }
    }

    public void activateMenuCooldown()
    // This function is used by menus like the storage system, witch menus, etc to give the player their controls back.
    // Due to the decentralised nature of our scripts, we need to use the Game manager as a mutex to prevent overlapping, such as pressing escape to leave the menu, but that opening the pause menu immedietly after.
    {
        menucooldown = maxmenucooldown;
    }
}
