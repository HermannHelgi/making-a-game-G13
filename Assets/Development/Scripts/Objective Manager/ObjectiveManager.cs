using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour, IDataPersistence
{

    [Header("Objective manager variables")]
    public static ObjectiveManager instance;
    public GameObject objectivesTitle;
    public GameObject objectiveGrid;
    public GameObject objectiveChildPrefab;
    public TextMeshProUGUI objectivesUpdated;
    public float objectiveUpdateMaxTimer;
    public float updateSpeed = 2;
    private float objectiveUpdateTimer = 0;

    [Header("Objective strings and references")]
    public string makeCampfire;
    public ItemScript campfire;
    public DialogueScriptableObject makeCampfireDialogue;

    public string collectFlower;
    public ItemScript flower;
    public string collectBottle;
    public ItemScript bottle;
    public string collectBodyPart;
    public ItemScript bodypart;
    public DialogueScriptableObject startingExposition;
    public DialogueScriptableObject startingExpositionSkipTutorial;

    public string makeLure;
    public string placeLure;
    public ItemScript lure;
    public DialogueScriptableObject makeLureDialogue;


    public string collectSkull;
    public ItemScript skull;
    public string makePotion;
    public ItemScript potion;
    public DialogueScriptableObject makePotionDialogue;
    public string drinkPotion;

    private bool makeCampfireState;
    private bool potionIngredientsState;
    private bool collectFlowerState;
    private bool collectBottleState;
    private bool collectBodyPartState;
    private bool createLureState;
    private bool placeLureState;
    private bool collectSkullState;
    private bool createPotionState;
    private bool drinkPotionState;

 /*

    For future reference, this script is essentially a large state machine with incredibly simple boolean variables.
    Depending on what the objective is and how it is supposed to be actived, the booleans respond to update the grid within the pause canvas of the player.
    These booleans are activated and deactivated from several different sources, such as InteractableItem, WitchDialogueHandler, WitchTradeScript, and EffigyScript.
    Only with the context of those scripts can this one be understood.

    TL;DR, Go read the other scripts or else this will not make any sense.

 */



    public void Awake()
    {
        instance = this;

        makeCampfireState = false;
        potionIngredientsState = false;
        collectFlowerState = false;
        collectBottleState = false;
        collectBodyPartState = false;
        createLureState = false;
        placeLureState = false;
        collectSkullState = false;
        createPotionState = false;
        drinkPotionState = false;

        objectivesTitle.SetActive(false);
    }

    public void loadData(GameData data)
    {
        makeCampfireState = data.makeCampfireState;
        potionIngredientsState = data.potionIngredientsState;
        collectFlowerState = data.collectFlowerState;
        collectBottleState = data.collectBottleState;
        collectBodyPartState = data.collectBodyPartState;
        createLureState = data.createLureState;
        placeLureState = data.placeLureState;
        collectSkullState = data.collectSkullState;
        createPotionState = data.createPotionState;
        drinkPotionState = data.drinkPotionState;

        if (makeCampfireState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(makeCampfire);
        }
        if (collectFlowerState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(collectFlower);
        }
        if (collectBottleState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(collectBottle);
        }
        if (collectBodyPartState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(collectBodyPart);
        }
        if (createLureState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(makeLure);
        }
        if (placeLureState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(placeLure);
        }
        if (collectSkullState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(collectSkull);
        }
        if (createPotionState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(makePotion);
        }
        if (drinkPotionState)
        {
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(drinkPotion);
        }
    }

    public void saveData(ref GameData data)
    {
        data.makeCampfireState = makeCampfireState;
        data.potionIngredientsState = potionIngredientsState;
        data.collectFlowerState = collectFlowerState;
        data.collectBottleState = collectBottleState;
        data.collectBodyPartState = collectBodyPartState;
        data.createLureState = createLureState;
        data.placeLureState = placeLureState;
        data.collectSkullState = collectSkullState;
        data.createPotionState = createPotionState;
        data.drinkPotionState = drinkPotionState;
    }

    public void pickedUpItem(ItemScript item)
    // Called by Interactable Item script
    {
        if (item == flower && collectFlowerState)
        {
            collectFlowerState = false;
            removeObjectiveByString(collectFlower);

            if (!collectFlowerState && !collectBodyPartState && !collectBottleState)
            {
                potionIngredientsState = false;
                objectivesTitle.SetActive(false);
            }
        }
        if (item == bottle && collectBottleState)
        {
            collectBottleState = false;
            removeObjectiveByString(collectBottle);
            
            if (!collectFlowerState && !collectBodyPartState && !collectBottleState)
            {
                potionIngredientsState = false;
                objectivesTitle.SetActive(false);
            }
        }
        if (item == bodypart && collectBodyPartState)
        {
            collectBodyPartState = false;
            removeObjectiveByString(collectBodyPart);

            if (!collectFlowerState && !collectBodyPartState && !collectBottleState)
            {
                potionIngredientsState = false;
                objectivesTitle.SetActive(false);
            }
        }

        if (item == skull && collectSkullState)
        {
            collectSkullState = false;
            removeObjectiveByString(collectSkull);
            objectivesTitle.SetActive(false);
        }
    }

    public void craftedItem(ItemScript item)
    // Called by Witch Trade Script
    {
        if (makeCampfireState && item == campfire)
        {
            makeCampfireState = false;
            removeObjectiveByString(makeCampfire);
            objectivesTitle.SetActive(false);
        }

        if (createLureState && item == lure)
        {
            createLureState = false;
            removeObjectiveByString(makeLure);

            placeLureState = true;
            spawnObjectiveChild(placeLure);
        }

        if (createPotionState && item == potion)
        {
            createPotionState = false;
            removeObjectiveByString(makePotion);

            drinkPotionState = true;
            spawnObjectiveChild(drinkPotion);
        }
    }

    public void finishedSpeakingWithWitch(DialogueScriptableObject dialogue)
    // Called by Witch Dialogue Handler
    {
        if (dialogue == makeCampfireDialogue)
        {
            makeCampfireState = true;
            spawnObjectiveChild(makeCampfire);
            objectivesTitle.SetActive(true);
        }
        else if (dialogue == startingExposition || dialogue == startingExpositionSkipTutorial)
        {
            potionIngredientsState = true;
            collectBottleState = true;
            collectFlowerState = true;
            collectBodyPartState = true;
            objectivesTitle.SetActive(true);

            spawnObjectiveChild(collectFlower);
            spawnObjectiveChild(collectBottle);
            spawnObjectiveChild(collectBodyPart);
        }
        else if (dialogue == makeLureDialogue)
        {
            createLureState = true;
            objectivesTitle.SetActive(true);
            spawnObjectiveChild(makeLure);
        }
        else if (dialogue == makePotionDialogue)
        {
            createPotionState = true;
            spawnObjectiveChild(makePotion);
            objectivesTitle.SetActive(true);
        }
    }

    public void placedLure()
    // Called by Effigy script
    {
        if (placeLureState)
        {
            placeLureState = false;
            removeObjectiveByString(placeLure);

            collectSkullState = true;
            spawnObjectiveChild(collectSkull);
        }
    }

    private void spawnObjectiveChild(string line)
    // Adds a child to the grid of the pause menu 
    {
        objectiveUpdateTimer = objectiveUpdateMaxTimer;
        GameObject childObject = Instantiate(objectiveChildPrefab);
        childObject.transform.SetParent(objectiveGrid.transform, false);
        childObject.GetComponent<TextMeshProUGUI>().text = line;
    }

    private void removeObjectiveByString(string line)
    // Removes a child from the grid of the pause menu based on the inputted string
    {
        for (int i = 0; i < objectiveGrid.transform.childCount; i++)
        {
            if (objectiveGrid.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text == line)
            {
                Destroy(objectiveGrid.transform.GetChild(i).gameObject);
                break;
            }
        }
    }

    private void Update()
    {
        if (objectiveUpdateTimer > 0)
        {
            objectiveUpdateTimer -= Time.deltaTime / updateSpeed;
            objectivesUpdated.alpha = Mathf.Lerp(0, 1, objectiveUpdateTimer);
        }
    }
}
