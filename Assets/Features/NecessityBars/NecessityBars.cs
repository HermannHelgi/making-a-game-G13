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
        currenttemperature -= temperaturedrainrate * Time.deltaTime;
        
        // I do this calculation constantly to avoid more complex logic. 
        starvationoverhead.color = new Color(1, 1, 1, (1 - currenthunger / starvationoverheaddisplaycutoff));
        frostbiteoverhead.color = new Color(1, 1, 1, (1 - currenttemperature / frostbiteoverheaddisplaycutoff));

        hungermeter.fillAmount = hungerpercent;
        temperaturemeter.fillAmount = temperaturepercent;

        if (currenthunger <= 0 || currenttemperature <= 0)
        {
            // TODO: Kill player
        }
    }

    public void increaseHunger(float hungerincrease)
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
