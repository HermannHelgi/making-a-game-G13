using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour, IDataPersistence
{
    [Header("GameManager variables")]
    [Tooltip("Base singleton instance of the Game Manager, do not touch!")]
    public static DialogueManager instance;
    [Tooltip("All dialogue objects in the game. If any elements are missing then it will go badly.")]
    public DialogueScriptableObject[] allDialogue;
    public int[] indexForPotionDialogue;
    public DialogueScriptableObject winConditionText;

    // Private stuffs
    // This dialogueflags will always be generated the same way. Due to this, we can just save this array for which flags have been activated to redo everything :D
    private bool[] dialogueflags;
    private WitchDialogueHandler witchdialogue;

    void Awake()
    {
        instance = this;
        dialogueflags = new bool[allDialogue.Length];
    }

    void Start()
    {
        WitchDialogueHandler[] check = GameObject.FindObjectsByType<WitchDialogueHandler>(FindObjectsSortMode.None);
        if (check.Length == 0)
        {
            Debug.LogError("Could not find Witch Dialogue Handler in scene, please add the object and try again.");
        }
        else if (check.Length > 1)
        {
            Debug.LogError("Too many Witch Dialogue Handlers in scene, please remove them down to one and try again.");
        }
        else
        {
            witchdialogue = check[0];
        }
    }

    public void loadData(GameData data)
    {
        dialogueflags = data.dialogueFlags.ToArray();
    }

    public void saveData(ref GameData data)
    {
        data.dialogueFlags = new List<bool>(dialogueflags);
    }

    public void SetDialogueFlags(DialogueScriptableObject newdialogue)
    {
        for (int i = 0; i < allDialogue.Length; i++)
        {
            if (allDialogue[i] == newdialogue)
            {
                // This dialogue feature has already been added, ignore duplicates.
                if (dialogueflags[i])
                {
                    break;
                }
                dialogueflags[i] = true;
                witchdialogue.addDialogueToQueue(newdialogue);

                // This is hardcoded as there are ATM always three items required for the potion. If the recipe changes, then this needs to be changed as well.
                if (dialogueflags[indexForPotionDialogue[0]] && dialogueflags[indexForPotionDialogue[1]] && dialogueflags[indexForPotionDialogue[2]])
                {
                    SetDialogueFlags(winConditionText);
                }

                break;
            }
        }
    }
}
