using UnityEngine.UI;
using UnityEngine;
using StarterAssets;

public class NecessityBars : MonoBehaviour, IDataPersistence
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
    public float torchincreaserate = 0.333f;
    public float emberstoneincreaserate = 1.0f;
    public Image temperaturemeter;
    public Color coldcolor;
    public GameObject snowflakeicon;
    public Color warmcolor;
    public GameObject fireicon;

    public float frostbiteoverheaddisplaycutoff;
    public Image frostbiteoverhead;

    [Header("Player Death Variables")]
    public GameObject playerdeathhandler;
    public string starvationmessage;
    public string frostbitemessage;

    [Header("Player Death Variables")]
    public StarterAssetsInputs firstpersoncontroller;

    // Private
    [Tooltip("DO NOT TOUCH. This variable is serialized for viewability, not editing.")]
    [SerializeField] private float currenthunger;
    private float hungerpercent => currenthunger / maxhunger;
    [Tooltip("DO NOT TOUCH. This variable is serialized for viewability, not editing.")]
    [SerializeField] private float currenttemperature;
    private float temperaturepercent => currenttemperature / maxtemperature;
    private bool displayingincreaseinhunger;
    private float increaseinhunger;

    void Awake()
    {
        hungerdisplayconsumablemeter.fillAmount = 0;
    }

    public void loadData(GameData data)
    {
        currenthunger = data.currentHunger;
        currenttemperature = data.currentTemperature;
    }

    public void saveData(ref GameData data)
    {
        data.currentHunger = currenthunger;
        data.currentTemperature = currenttemperature;
    }

    void Update()
    {
        if (TutorialManager.instance != null)
        {
            if (TutorialManager.instance.inactiveNecessityBars)
            {
                return;
            }
        }

        // Updating necessity bars
        if (firstpersoncontroller.sprint && !GameManager.instance.inMenu)
        {
            hungermeter.color = sprintcolor;
            currenthunger -= runningdrainrate * Time.deltaTime;
        }
        else
        {
            hungermeter.color = walkcolor;
            currenthunger -= hungerdrainrate * Time.deltaTime;
        }

        float changetocurrenttemp = 0;
        changetocurrenttemp -= temperaturedrainrate * Time.deltaTime;
        if (GameManager.instance.holdingtorch && GameManager.instance.torchactive)
        {
            changetocurrenttemp += torchincreaserate * Time.deltaTime;
        }
        if (GameManager.instance.emberstoneactive)
        {
            changetocurrenttemp += emberstoneincreaserate * Time.deltaTime;
        }

        if (changetocurrenttemp <= 0)
        {
            temperaturemeter.color = coldcolor;
            snowflakeicon.SetActive(true);
            fireicon.SetActive(false);
        }
        else if (changetocurrenttemp > 0)
        {
            temperaturemeter.color = warmcolor;
            snowflakeicon.SetActive(false);
            fireicon.SetActive(true);
        }

        currenttemperature += changetocurrenttemp;
        // I do a check here since the player could be near the campfire, or could be gaining heat from torch + emberstone.
        if (currenttemperature > maxtemperature)
        {
            currenttemperature = maxtemperature;
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