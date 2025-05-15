using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour, IDataPersistence
{
    [Header("Tutorial Manager Variables")]
    public static TutorialManager instance;
    public float maxlooktimer;
    public float tutorialboxfadeintime;
    public float backgroundalpha;
    public float craftingBackgroundAlpha;
    public bool tutorialinprogress;
    public bool cannotcraft;
    public bool cannottalk;
    public bool inactiveNecessityBars;
    public GameObject tutorialcavewall;
    public GameObject caveWallCollider;
    public GameObject walkCollider;

    public SoundManager soundManager;

    // References
    [Header("References to other objects")]
    [Tooltip("The capsule of the player, should start at a Z rotation of 90.")]
    public GameObject playercapsule;
    [Tooltip("The location of the 'player look' script.")]
    public PlayerLookScript playerlookscript;
    [Tooltip("The location of the 'player inventory' script.")]
    public PlayerInventory playerinventory;
    [Tooltip("The UI element which displays the tutorial tooltips.")]
    public TutorialBoxWrapper tutorialscreen;
    [Tooltip("The location of the Witch 'lerp' position, basically where the player should look when the witch speaks.")]
    public GameObject witchlerpposition;
    [Tooltip("The UI element for the witch's subtitles.")]
    public TextMeshProUGUI witchsubtitletext;
    [Tooltip("The necessity bars script.")]
    public NecessityBars necessityBars;
    [Tooltip("The UI cold bar element.")]
    public GameObject coldBar;
    [Tooltip("The UI hunger bar element.")]
    public GameObject hungerBar;
    [Tooltip("The UI element which displays the tutorial tooltips inside the witch's bargaining menu.")]
    public TutorialBoxWrapper craftingTutorialScreen;
    [Tooltip("The campfire gameobject.")]
    public GameObject campfire;


    // Tutorial Tooltips
    [Header("Strings and items used for the tutorial.")]
    public string explaincontrols;
    public string explaininteractbutton;
    public string explainhotbaranditems;
    public ItemScript berry;
    public string explainnecessitybars;
    public float hungerDecrease;
    public string explainItems;
    public string explainBargaining;
    public string explainCampfire;


    // Look text
    [Header("Witch subtitles and dialogue")]
    public string witchwalktext;
    public DialogueScriptableObject witchStartingDialogue;
    public string witchleavecavetext;
    public string witchdiscussfurther;
    public DialogueScriptableObject makecampfire;
    public DialogueScriptableObject howtobargain;
    public string witchHighlightCampfire;
    public string witchInitiateExposition;
    public DialogueScriptableObject witchExposition;
    public DialogueScriptableObject witchSkippedTutorialExposition;
    public WitchTradeScript witchTradeScript;
    public ItemScript[] unlockedItemsOnTutorialEnd;



    // State bools
    private bool wakingup = false;
    private bool canSkipTutorial = false;
    private bool interactwithwitch = false;
    private bool teachingitems = false;
    private bool findsticks = false;
    private bool teachingPickup = false;
    private bool loadNextDialogue = false;
    private bool teachingBargaining = false;
    private bool teachingCampfire = false;
    private bool doingExposition = false;

    // Timers for the tutorial boxes
    private float looktimer = 0;
    private bool showingtutorialbox = false;
    private float tutorialboxtimer = 0;

    private bool showingCraftingTutorialBox = false;
    private float craftingTutorialBoxTimer = 0;

/*

    For future reference, this script is essentially a large state machine with incredibly simple boolean variables.
    Depending on what the tutorial stage you are in and how it is supposed to be actived, the booleans respond the tutorial and activate / deactivate certain systems.
    These booleans are activated and deactivated from several different sources, such as InteractableItem, WitchDialogueHandler, WitchTradeScript, Tutorial Box Wrapper, and more.
    Only with the context of those scripts can this one be understood.

    TL;DR, Go read the other scripts or else this will not make any sense.

*/



    void Awake()
    {
        instance = this;
        coldBar.SetActive(false);
        hungerBar.SetActive(false);
    }

    public void loadData(GameData data)
    {
        tutorialinprogress = data.tutorialinprogress;
        if (tutorialinprogress)
        {
            cannotcraft = data.cannotcraft;
            cannottalk = data.cannottalk;
            inactiveNecessityBars = data.inactiveNecessityBars;
            if (!inactiveNecessityBars)
            {
                coldBar.SetActive(true);
                hungerBar.SetActive(true);
            }

            wakingup = data.wakingup;
            canSkipTutorial = data.canSkipTutorial;
            interactwithwitch = data.interactwithwitch;
            teachingitems = data.teachingitems;
            findsticks = data.findsticks;
            teachingPickup = data.teachingPickup;
            loadNextDialogue = data.loadNextDialogue;
            teachingBargaining = data.teachingBargaining;
            teachingCampfire = data.teachingCampfire;
            doingExposition = data.doingExposition;
            tutorialcavewall.SetActive(data.caveWallActive);
            caveWallCollider.SetActive(data.caveWallColliderActive);
            walkCollider.SetActive(data.walkColliderActive);
        }
        else
        {
            finishTutorial();
        }
    }

    public void saveData(ref GameData data)
    {
        data.tutorialinprogress = tutorialinprogress;
        data.cannotcraft = cannotcraft;
        data.cannottalk = cannottalk;
        data.inactiveNecessityBars = inactiveNecessityBars;

        data.caveWallActive = tutorialcavewall.activeSelf;
        data.caveWallColliderActive = caveWallCollider.activeSelf;
        data.walkColliderActive = walkCollider.activeSelf;
        
        data.wakingup = wakingup;
        data.canSkipTutorial = canSkipTutorial;
        data.interactwithwitch = interactwithwitch;
        data.teachingitems = teachingitems;
        data.findsticks = findsticks;
        data.teachingPickup = teachingPickup;
        data.loadNextDialogue = loadNextDialogue;
        data.teachingBargaining = teachingBargaining;
        data.teachingCampfire = teachingCampfire;
        data.doingExposition = doingExposition;
    }

    void Update()
    {
        if (!tutorialinprogress)
        {
            gameObject.SetActive(false);
        }
        handleLookTimer();
        handleTutorialFade();
    }

    void finishTutorial()
    {
        Vector3 currentRotation = playercapsule.transform.eulerAngles;
        currentRotation.z = 0f;
        playercapsule.transform.eulerAngles = currentRotation;

        tutorialinprogress = false;
        cannotcraft = false;
        cannottalk = false;
        inactiveNecessityBars = false;
        coldBar.SetActive(true);
        hungerBar.SetActive(true);

        tutorialscreen.gameObject.SetActive(false);
        craftingTutorialScreen.gameObject.SetActive(false);

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

        hideTutorialBox();

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < unlockedItemsOnTutorialEnd.Length; i++)
        {
            witchTradeScript.unlockItem(unlockedItemsOnTutorialEnd[i]);
        }
    }

    void showTutorialBox(string message)
    {
        tutorialboxtimer = 0;
        tutorialscreen.message.text = message;
        showingtutorialbox = true;
    }

    void hideTutorialBox()
    {
        showingtutorialbox = false;
    }

    void handleTutorialFade()
    {
        if (showingtutorialbox)
        {
            if (tutorialboxtimer <= 1)
            {
                tutorialboxtimer += Time.deltaTime / tutorialboxfadeintime;
                tutorialscreen.background.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, backgroundalpha), tutorialboxtimer);
                tutorialscreen.message.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), tutorialboxtimer);
                tutorialscreen.xtoclose.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), tutorialboxtimer);
            }

            if (Input.GetKeyDown(KeyCode.X) && tutorialboxtimer > 1)
            {
                hideTutorialBox();
            }
        }
        else if (!showingtutorialbox)
        {
            if (tutorialboxtimer >= 0)
            {
                tutorialboxtimer -= Time.deltaTime / tutorialboxfadeintime;
                tutorialscreen.background.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, backgroundalpha), tutorialboxtimer);
                tutorialscreen.message.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), tutorialboxtimer);
                tutorialscreen.xtoclose.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), tutorialboxtimer);

                if (loadNextDialogue && teachingPickup && tutorialboxtimer <= 0)
                {
                    showTutorialBox(explainItems);
                    teachingPickup = false;
                    teachingBargaining = true;
                    DialogueManager.instance.SetDialogueFlags(howtobargain);
                }
            }
        }

        if (showingCraftingTutorialBox)
        {
            if (craftingTutorialBoxTimer <= 1)
            {
                craftingTutorialBoxTimer += Time.deltaTime / tutorialboxfadeintime;
                craftingTutorialScreen.background.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, craftingBackgroundAlpha), craftingTutorialBoxTimer);
                craftingTutorialScreen.message.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), craftingTutorialBoxTimer);
                craftingTutorialScreen.xtoclose.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), craftingTutorialBoxTimer);
            }

            if (Input.GetKeyDown(KeyCode.X) && craftingTutorialBoxTimer > 1)
            {
                showingCraftingTutorialBox = false;
            }
        }
        else if (!showingCraftingTutorialBox)
        {
            if (craftingTutorialBoxTimer >= 0)
            {
                craftingTutorialBoxTimer -= Time.deltaTime / tutorialboxfadeintime;
                craftingTutorialScreen.background.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, craftingBackgroundAlpha), craftingTutorialBoxTimer);
                craftingTutorialScreen.message.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), craftingTutorialBoxTimer);
                craftingTutorialScreen.xtoclose.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), craftingTutorialBoxTimer);
            }
        }
    }

    void handleLookTimer()
    {
        if (wakingup)
        {
            GameManager.instance.inMenu = true;
            if (looktimer <= 1)
            {
                looktimer += Time.deltaTime / maxlooktimer;
                playercapsule.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0), looktimer);
                if (looktimer > 1)
                {
                    wakingup = false;
                    canSkipTutorial = true;
                    looktimer = 0;
                    GameManager.instance.activateMenuCooldown();
                    showTutorialBox(explaincontrols);
                }
            }
        }
        else if (canSkipTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                DialogueManager.instance.SetDialogueFlags(witchSkippedTutorialExposition);
                finishTutorial();
                DataPersistenceManager.instance.saveGame();
            }
        }
        else if (interactwithwitch)
        {
            if (looktimer <= 1)
            {
                looktimer += Time.deltaTime / maxlooktimer;
                if (looktimer > 1)
                {
                    playerlookscript.finishLook();
                    witchsubtitletext.gameObject.SetActive(false);
                    GameManager.instance.activateMenuCooldown();
                    showTutorialBox(explaininteractbutton);
                    cannottalk = false;
                }
            }
        }
        else if (findsticks)
        {
            if (looktimer <= 1)
            {
                looktimer += Time.deltaTime / maxlooktimer;
                if (looktimer > 1)
                {
                    playerlookscript.finishLook();
                    witchsubtitletext.gameObject.SetActive(false);
                    GameManager.instance.activateMenuCooldown();
                    cannottalk = false;
                }
            }
        }
        else if (teachingCampfire)
        {
            if (looktimer <= 1)
            {
                looktimer += Time.deltaTime / maxlooktimer;
                if (looktimer > 1)
                {
                    playerlookscript.finishLook();
                    witchsubtitletext.gameObject.SetActive(false);
                    GameManager.instance.activateMenuCooldown();
                    cannotcraft = true;

                    showTutorialBox(explainCampfire);
                }
            }
        }
        else if (doingExposition)
        {
            if (looktimer <= 1)
            {
                looktimer += Time.deltaTime / maxlooktimer;
                if (looktimer > 1)
                {
                    playerlookscript.finishLook();
                    witchsubtitletext.gameObject.SetActive(false);
                    GameManager.instance.activateMenuCooldown();
                }
            }
        }
    }

    public void playerWalked()
    {
        hideTutorialBox();
        canSkipTutorial = false;
        interactwithwitch = true;
        looktimer = 0;
        GameManager.instance.inMenu = true;
        witchsubtitletext.gameObject.SetActive(true);
        witchsubtitletext.text = witchwalktext;
        playerlookscript.playerLookAt(witchlerpposition);
        DialogueManager.instance.SetDialogueFlags(witchStartingDialogue);
        soundManager.PlayGroup("GRYLA_PSST");
    }

    public void playerTriedToLeaveCave()
    {
        if (interactwithwitch)
        {
            hideTutorialBox();
            looktimer = 0;
            GameManager.instance.inMenu = true;
            witchsubtitletext.gameObject.SetActive(true);
            witchsubtitletext.text = witchleavecavetext;
            playerlookscript.playerLookAt(witchlerpposition);
            soundManager.PlayGroup("GRYLA_OOH");
        }
    }

    public void playerTalkedWithWitch()
    {
        if (interactwithwitch)
        {
            hideTutorialBox();
        }
        else if (teachingPickup)
        {
            hideTutorialBox();
        }
        else if (teachingBargaining)
        {
            hideTutorialBox();
        }
    }

    public void playerFinishedTalkingWithWitch()
    {
        if (interactwithwitch)
        {
            interactwithwitch = false;
            teachingitems = true;
            cannottalk = true;

            coldBar.SetActive(true);
            hungerBar.SetActive(true);
            inactiveNecessityBars = false;
            necessityBars.increaseHunger(-hungerDecrease);

            playerinventory.addItemToHotbar(berry);
            showTutorialBox(explainhotbaranditems);
        }
        else if (findsticks)
        {
            findsticks = false;
            teachingPickup = true;
            tutorialcavewall.SetActive(false);
            showTutorialBox(explainnecessitybars);
        }
        else if (teachingBargaining)
        {
            cannotcraft = false;
        }
        else if (doingExposition)
        {
            finishTutorial();
            DataPersistenceManager.instance.saveGame();
        }
    }

    public void playerConsumedFood()
    {
        if (teachingitems)
        {
            hideTutorialBox();
            teachingitems = false;
            findsticks = true;
            DialogueManager.instance.SetDialogueFlags(makecampfire);

            looktimer = 0;
            GameManager.instance.inMenu = true;
            witchsubtitletext.gameObject.SetActive(true);
            witchsubtitletext.text = witchdiscussfurther;
            playerlookscript.playerLookAt(witchlerpposition);
            soundManager.PlayGroup("GRYLA_GENERIC_DIALOGUE");
        }
    }

    public void playerPickedUpItem()
    {
        if (teachingPickup)
        {
            if (showingtutorialbox)
            {
                hideTutorialBox();
                loadNextDialogue = true;
            }
            else if (!showingtutorialbox && !loadNextDialogue)
            {
                showTutorialBox(explainItems);
                teachingPickup = false;
                teachingBargaining = true;
                DialogueManager.instance.SetDialogueFlags(howtobargain);

            }
        }
    }

    public void playerHasOpenedTradeWindow()
    {
        if (teachingBargaining)
        {
            showingCraftingTutorialBox = true;
            craftingTutorialScreen.message.text = explainBargaining;
        }
    }

    public void playerHasCraftedCampfire()
    {
        if (teachingBargaining)
        {
            teachingBargaining = false;
            showingCraftingTutorialBox = false;
            teachingCampfire = true;
        }
    }

    public void playerHasClosedTradeWindow()
    {
        if (teachingBargaining)
        {
            GameManager.instance.activateMenuCooldown();
        }
        if (teachingCampfire)
        {
            looktimer = 0;
            GameManager.instance.inMenu = true;
            witchsubtitletext.gameObject.SetActive(true);
            witchsubtitletext.text = witchHighlightCampfire;
            playerlookscript.playerLookAt(campfire);
            soundManager.PlayGroup("GRYLA_GENERIC_DIALOGUE");
        }
    }

    public void playerHasWalkedAwayFromCampfire()
    {
        if (teachingCampfire)
        {
            hideTutorialBox();
            teachingCampfire = false;

            looktimer = 0;
            GameManager.instance.inMenu = true;
            witchsubtitletext.gameObject.SetActive(true);
            witchsubtitletext.text = witchInitiateExposition;
            playerlookscript.playerLookAt(witchlerpposition);
            cannotcraft = false;
            DialogueManager.instance.SetDialogueFlags(witchExposition);
            doingExposition = true;
            soundManager.PlayGroup("GRYLA_GENERIC_DIALOGUE");
        }
    }
}