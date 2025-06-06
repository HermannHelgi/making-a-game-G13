using UnityEngine;

public class CampfireScript : MonoBehaviour, IDataPersistence
{
    [Header("Campfire heating variables")]
    [Tooltip("The range which the campfire heats up the player.")]
    public float rangeofheat;
    [Tooltip("The speed at which the campfire heats up the player. This MUST be negative!")]
    // This has to be negative since it just flips the drain rate of the players temperature. Essentially, CurrentTemp - (-heatspeed) = CurrentTemp + heatspeed
    public float heatspeed;
    [Tooltip("How long (in seconds) the campfire stays lit per stick added.")]
    public float lengthofburn;
    [Tooltip("The gameobject of the lit campfire.")]
    public GameObject litcampfire;
    [Tooltip("The gameobject of the unlit campfire.")]
    public GameObject coldcampfire;

    [Header("Player references")]
    [Tooltip("The capsule of the player, used for distance measurement.")]
    public GameObject playercapsule;
    [Tooltip("The necessity bars script of the player. Used to up temperature.")]
    public NecessityBars necessitybars;


    [Header("Coal variables")]
    [Tooltip("The amount of coal currently in the campfire.")]
    public int coalstored;
    [Tooltip("The ItemScript of coal.")]
    public ItemScript coalitemscript;

    // Private vars, used for state management
    public int sticksemplaced;
    public float burntimer;
    public bool playerinrange;
    public float oldtemperaturedrainrate;


    public void loadData(GameData data)
    {
        sticksemplaced = data.sticksEmplaced;
        burntimer = data.burnTimer;
        coalstored = data.coalStored;

        if (burntimer > 0)
        {
            litcampfire.SetActive(true);
            coldcampfire.SetActive(false);
        }
        else
        {
            litcampfire.SetActive(false);
            coldcampfire.SetActive(true);
        }
    }

    public void saveData(ref GameData data)
    {
        data.sticksEmplaced = sticksemplaced;
        data.burnTimer = burntimer;
        data.coalStored = coalstored;
    }

    void Update()
    {
        if (burntimer > 0)
        {
            burntimer -= Time.deltaTime;

            if (Vector3.Distance(playercapsule.transform.position, this.transform.position) <= rangeofheat && !playerinrange)
            {
                warmPlayer();
            }

            if (burntimer <= 0)
            {
                turnOffHeat();
            }
        }
        else if (sticksemplaced > 0)
        {
            burnStick();
        }

        if (playerinrange)
        {
            if (Vector3.Distance(playercapsule.transform.position, this.transform.position) > rangeofheat)
            {
                turnOffHeat();
                if (TutorialManager.instance != null)
                {
                    if (TutorialManager.instance.tutorialinprogress)
                    {
                        TutorialManager.instance.playerHasWalkedAwayFromCampfire();
                    }   
                }
            }
        }
    }

    void warmPlayer()
    // Sets the players necessity bars to heat them up
    {
        playerinrange = true;
        oldtemperaturedrainrate = necessitybars.temperaturedrainrate;
        necessitybars.temperaturedrainrate = heatspeed;
    }

    void turnOffHeat()
    // Used to set the players necessity bar back to normal, if either the fire dies or they leave the range
    {
        playerinrange = false;
        necessitybars.temperaturedrainrate = oldtemperaturedrainrate;
        
        if (burntimer <= 0)
        {
            
            coalstored++;

            if (sticksemplaced > 0)
            {
                burnStick();
            }
            else
            {
                burntimer = 0;
                litcampfire.SetActive(false);
                coldcampfire.SetActive(true);
            }
        }
    }

    void burnStick()
    // Burns a stick to keep the fire going 
    {
        burntimer = lengthofburn;
        sticksemplaced--;
        litcampfire.SetActive(true);
        coldcampfire.SetActive(false);
    }

    public void addFuel()
    // Adds a stick to the fire. 
    {
        sticksemplaced++;        
    }

    public ItemScript gatherCoal()
    // Remvoes coal from campfire
    {
        coalstored--;
        return coalitemscript;        
    }

    public void failedToGatherCoal()
    // Used to restore coal that was gathered but couldnt be added to inventory, See PlayerInteractHandler
    {
        coalstored++;
    }
}
