using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Tutorial Manager exclusive variables
    public static TutorialManager instance;
    public float maxlooktimer;     
    public float tutorialboxfadeintime;     
    public float backgroundalpha;
    public bool tutorialinprogress;
    public bool cannotcraft;
    public GameObject tutorialcavewall;

    // References
    public GameObject playercapsule;
    public PlayerLookScript playerlookscript; 
    public PlayerInventory playerinventory; 
    public TutorialBoxWrapper tutorialscreen;
    public GameObject witchlerpposition;
    public TextMeshProUGUI witchsubtitletext;


    // Tutorial Tooltips
    public string explaincontrols;
    public string explaininteractbutton;
    public string explainhotbaranditems;
    public ItemScript berry;
    public string explainnecessitybars;


    // Look text
    public string witchwalktext;
    public string witchleavecavetext;
    public string witchdiscussfurther;
    public DialogueScriptableObject makecampfire;



    // State bools
    private bool wakingup = true;
    private bool interactwithwitch = false;
    private bool teachingitems = false;
    private bool findsticks = false;
    private bool teachingcrafting = false;


    private float looktimer = 0;
    private bool showingtutorialbox = false;
    private float tutorialboxtimer = 0;

    void Start()
    {
        instance = this;
        tutorialinprogress = true;
        cannotcraft = true;
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
                    looktimer = 0;
                    GameManager.instance.activateMenuCooldown();
                    showTutorialBox(explaincontrols);
                }
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
                }
            }
        }
    }

    public void playerWalked()
    {
        hideTutorialBox();
        interactwithwitch = true;
        looktimer = 0;
        GameManager.instance.inMenu = true;
        witchsubtitletext.gameObject.SetActive(true);
        witchsubtitletext.text = witchwalktext;
        playerlookscript.playerLookAt(witchlerpposition);
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
        }
    }

    public void playerTalkedWithWitch()
    {
        if (interactwithwitch)
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
            playerinventory.addItemToHotbar(berry);
            showTutorialBox(explainhotbaranditems);
        }
        else if (findsticks)
        {
            findsticks = false;
            teachingcrafting = true;
            tutorialcavewall.SetActive(false);
            showTutorialBox(explainnecessitybars);
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
        }
    }
}