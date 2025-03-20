using UnityEngine.UI;
using UnityEngine;
using StarterAssets;

public class NecessityBars : MonoBehaviour
{
    [Header("Hunger Bar Variables")]
    public float maxhunger;
    public float hungerdrainrate = 1f;
    public float runningdrainrate = 2f;
    public Image hungermeter;
    public Image hungerdisplayconsumablemeter;
    public Color walkcolor;
    public Color sprintcolor;

    public float starvationoverheaddisplaycutoff;
    public Image starvationoverhead;

    [Header("Cold Bar Variables")]
    public float maxtemperature;
    public float temperaturedrainrate = 1f;
    public Image temperaturemeter;
    public Color coldcolor;
    public Color warmcolor;

    public float frostbiteoverheaddisplaycutoff;
    public Image frostbiteoverhead;

    [Header("Player Death Variables")]

    public GameObject playerdeathhandler;
    public string starvationmessage;
    public string frostbitemessage;

    [Header("Player Death Variables")]

    public StarterAssetsInputs firstpersoncontroller;

    // Private
    private float currenthunger;
    private float hungerpercent => currenthunger / maxhunger;
    public float currenttemperature;
    private float temperaturepercent => currenttemperature / maxtemperature;
    private bool displayingincreaseinhunger;
    private float increaseinhunger;

    void Start()
    {
        currenthunger = maxhunger;
        currenttemperature = maxtemperature;
    }

    void Update()
    {
        // Updating necessity bars
        if (firstpersoncontroller.sprint)
        {
            hungermeter.color = sprintcolor;
            currenthunger -= runningdrainrate * Time.deltaTime;
        }
        else
        {
            hungermeter.color = walkcolor;
            currenthunger -= hungerdrainrate * Time.deltaTime;
        }

        // I do a check here as the campfire simply makes the drain rate negative, so to make sure the player doesn't "go over" the max, this edge check is needed.
        if (currenttemperature - temperaturedrainrate * Time.deltaTime > maxtemperature)
        {
            currenttemperature = maxtemperature;
        }
        else
        {
            currenttemperature -= temperaturedrainrate * Time.deltaTime;
        }
        
        if (temperaturedrainrate < 0)
        {
            temperaturemeter.color = warmcolor;
        }
        else
        {
            temperaturemeter.color = coldcolor;
        }

        // I do this calculation constantly to avoid more complex logic. 
        starvationoverhead.color = new Color(1, 1, 1, (1 - currenthunger / starvationoverheaddisplaycutoff));
        frostbiteoverhead.color = new Color(1, 1, 1, (1 - currenttemperature / frostbiteoverheaddisplaycutoff));

        // Updating bars on screen
        hungermeter.fillAmount = hungerpercent;
        temperaturemeter.fillAmount = temperaturepercent;

        if (displayingincreaseinhunger)
        {
            hungerdisplayconsumablemeter.fillAmount = (currenthunger + increaseinhunger) / maxhunger; 
        }
        else
        {
            hungerdisplayconsumablemeter.fillAmount = 0;
        }

        if (currenthunger <= 0)
        {
            playerdeathhandler.GetComponent<PlayerDeathHandler>().die(starvationmessage);
        }

        if (currenttemperature <= 0)
        {
            playerdeathhandler.GetComponent<PlayerDeathHandler>().die(frostbitemessage);
        }
    }

    public void increaseHunger(float hungerincrease)
    // Can be called to increase hunger, used by foods.
    {
        float newhunger = hungerincrease + currenthunger;

        if (newhunger > maxhunger)
        {
            currenthunger = maxhunger;
        }
        else
        {
            currenthunger = newhunger;
        }
    }

    public void displayHungerIncrease(float hungerincrease)
    // Can be called to display the amount that hunger will increase upon consuming item
    {
        displayingincreaseinhunger = true;
        increaseinhunger = hungerincrease;
    }

    public void turnOffDisplayHungerIncrease()
    // Can be called to turn off displaying hunger given by item
    {
        displayingincreaseinhunger = false;
        increaseinhunger = 0;
    }

    public void increaseTemperature(float temperatureincrease)
    // Can be called to increase temperature, used by heat sources.
    {
        float newtemperature = temperatureincrease + currenttemperature;

        if (newtemperature > maxtemperature)
        {
            currenttemperature = maxtemperature;
        }
        else
        {
            currenttemperature = newtemperature;
        }
    }
}