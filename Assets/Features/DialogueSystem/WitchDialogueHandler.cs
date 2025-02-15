using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WitchDialogueHandler : MonoBehaviour
{

    public int distanceToDisableText;


    private bool displayingmessage = false;
    private GameObject subtitletextmesh;
    private GameObject player;

    private Queue<DialogueScriptableObject> dialoguequeue;
    private DialogueScriptableObject currentdialoguechain;
    private int currentindex;

    void Update()
    {
        if (displayingmessage)
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) > distanceToDisableText)
            {
                deinitializeDialogue();
            }
        }
    }

    public void runNextDialogue()
    {
        if (currentdialoguechain == null)
        {
            DialogueScriptableObject nextMessage = dialoguequeue.Peek();
            if (nextMessage != null)
            {
                currentdialoguechain = dialoguequeue.Dequeue();
                currentindex = 0;
                subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
            }
        }
        else
        {
            if (currentdialoguechain.dialogue.Length == currentindex + 1)
            {
                DialogueScriptableObject nextMessage = dialoguequeue.Peek();
                if (nextMessage != null)
                {
                    currentdialoguechain = dialoguequeue.Dequeue();
                    currentindex = 0;
                    subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
                }
                else
                {
                    currentdialoguechain = null;
                    deinitializeDialogue();
                }
            }
            else
            {
                currentindex++;
                subtitletextmesh.GetComponent<TextMeshProUGUI>().text = currentdialoguechain.dialogue[currentindex];
            }
        }
    }

    public void intializeDialogue(GameObject subtitleobject, GameObject playerobject)
    {
        if (displayingmessage)
        {
            runNextDialogue();
        }
        else
        {
            player = playerobject;
            subtitletextmesh = subtitleobject;
            
            subtitletextmesh.SetActive(true);
            displayingmessage = true;
            runNextDialogue();
        }
    }

    void deinitializeDialogue()
    {
        subtitletextmesh.SetActive(false);

        displayingmessage = false;
    }

    public bool isQueueEmpty()
    {
        return (dialoguequeue.Count == 0 || currentdialoguechain == null);
    }

    public void addDialogueToQueue(DialogueScriptableObject newdialogue)
    {
        dialoguequeue.Enqueue(newdialogue);
    }
}
