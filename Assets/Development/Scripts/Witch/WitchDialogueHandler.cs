using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WitchDialogueHandler : MonoBehaviour
{
    [Header("Dialogue Variables")]
    public int distanceToDisableText;


    // Private stuff, mostly references to other objects...
    private GameObject subtitletextmesh;
    private GameObject player;

    // ... and stuff to keep track of the current dialogue.
    private bool displayingmessage = false;
    private Queue<DialogueScriptableObject> dialoguequeue;
    private DialogueScriptableObject currentdialoguechain;
    private int currentindex;

    void Start()
    {
        // Need to initialize the queue
        dialoguequeue = new Queue<DialogueScriptableObject>();
        currentdialoguechain = null;
    }

    void Update()
    {
        // just checks the distance of the player and whether to turn off the text
        if (displayingmessage)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                runNextDialogue();       
            }
        }
    }

    bool runNextDialogue()
    // Returns a bool, if true, that means it is continuing to show dialogue. Else, if false, it means it has finished displaying.
    {
        // Here below are some comments to show the general flow of the dialogue management and what its doing in each step.
        // This function is essentially doing all the leg work for the actual text management and switching, so best make it clear


        // Is there some dialogue chain which is already running?
        if (currentdialoguechain == null)
        {
            /// ...no, is there some dialogue chain waiting to be checked?
            if (dialoguequeue.Count > 0)
            {
                //// ...yes, so run that dialogue chain

                runAudioForDialogue();
                currentdialoguechain = dialoguequeue.Dequeue();
                currentindex = 0;
                subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
                return true;
            }
            /// ...no dialogue exists, so leave.
            return false;
        }
        else
        {
            /// ...some dialogue chain is already running, so check if you can continue.
            if (currentdialoguechain.dialogue.Length == currentindex + 1)
            {
                //// ... the dialogue chain has finished, can we start another one?
                if (dialoguequeue.Count > 0)
                {
                    ///// ...yes, so start the next one
                    
                    runAudioForDialogue();
                    currentdialoguechain = dialoguequeue.Dequeue();
                    currentindex = 0;
                    subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
                    return true;
                }
                else
                {
                    ///// ...no, so stop.
                    currentdialoguechain = null;
                    deinitializeDialogue();
                    return false;
                }
            }
            else
            {
                //// ...the dialogue chain is still ongoing, so index by one and show the next string.
                currentindex++;
                subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
                return true;
            }
        }
    }

    void runAudioForDialogue()
    {
        // TODO: Add functionality to run dialogue for each individual dialogue chain.
    }

    public bool intializeDialogue(GameObject subtitleobject, GameObject playerobject)
    // Initializes the dialogue text. 
    {
        player = playerobject;
        subtitletextmesh = subtitleobject;
        
        subtitletextmesh.SetActive(true);
        displayingmessage = true;
        return runNextDialogue();
    }

    void deinitializeDialogue()
    // Turns off the dialogue text
    {
        subtitletextmesh.SetActive(false);
        displayingmessage = false;
        GameManager.instance.activateMenuCooldown();
    }

    public bool isQueueEmpty()
    // Checks if there is any ongoing dialogue or whether there are any dialogues which can be run.
    {
        return dialoguequeue.Count == 0 && currentdialoguechain == null;
    }

    public void addDialogueToQueue(DialogueScriptableObject newdialogue)
    // Adds dialogue to queue.
    {
        dialoguequeue.Enqueue(newdialogue);
    }
}
