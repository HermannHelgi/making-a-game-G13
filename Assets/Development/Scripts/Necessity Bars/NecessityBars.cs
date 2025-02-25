using UnityEngine.UI;
using UnityEngine;

public class NecessityBars : MonoBehaviour
{
    [Header("Hunger Bar Variables")]
    public float maxhunger;
    public float hungerdrainrate = 1f;
    public Image hungermeter;

    public float starvationoverheaddisplaycutoff;
    public Image starvationoverhead;


    [Header("Cold Bar Variables")]
    public float maxtemperature;
    public float temperaturedrainrate = 1f;
    public Image temperaturemeter;

    public float frostbiteoverheaddisplaycutoff;
    public Image frostbiteoverhead;

    [Header("Player Death Variables")]

    public GameObject playerdeathhandler;
    public string starvationmessage;
    public string frostbitemessage;


    // Private
    private float currenthunger;
    private float hungerpercent => currenthunger / maxhunger;
    private float currenttemperature;
    private float temperaturepercent => currenttemperature / maxtemperature;

    void Start()
    {
        currenthunger = maxhunger;
        currenttemperature = maxtemperature;
    }

    void Update()
    {
        // Updating necessity bars
        currenthunger -= hungerdrainrate * Time.deltaTime;

        // I do a check here as the campfire simply makes the drain rate negative, so to make sure the player doesn't "go over" the max, this edge check is needed.
        if (currenttemperature - temperaturedrainrate * Time.deltaTime > maxtemperature)
        {
            currenttemperature = maxtemperature;
        }
        else
        {
            currenttemperature -= temperaturedrainrate * Time.deltaTime;
        }
        
        // I do this calculation constantly to avoid more complex logic. 
        starvationoverhead.color = new Color(1, 1, 1, (1 - currenthunger / starvationoverheaddisplaycutoff));
        frostbiteoverhead.color = new Color(1, 1, 1, (1 - currenttemperature / frostbiteoverheaddisplaycutoff));

        // Updating bars on screen
        hungermeter.fillAmount = hungerpercent;
        temperaturemeter.fillAmount = temperaturepercent;

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
