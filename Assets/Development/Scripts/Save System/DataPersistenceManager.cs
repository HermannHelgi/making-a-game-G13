using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    public string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set;}

    [Header("Base variables")]
    // These variables are essentially the base variables for a multitude of scripts at the start of a new game.
    // Due to how this manager is made, it will essentially overwrite the base variables set in-editor.
    public Vector3 playerPos;
    public float currentHunger;
    public float currentTemperature;
    public float torchDurability;
    public float emberstoneDurability;
    public bool torchActive;
    public bool emberstoneActive;
    public bool campfireUnlocked;
    public int coalStored;
    public int sticksEmplaced;
    public float burnTimer;
    public bool chestUnlocked;
    public List<bool> discoveredItems;
    public List<bool> unlockedItems;
    public List<bool> hasBeenCrafted;
    public List<bool> dialogueFlags;
    public List<int> witchDialogueQueue;
    public List<int> playerInventory;
    public List<int> chestInventory;
    public bool isNight;
    public float timeOfDay;
    public bool tutorialinprogress;
    public bool cannotcraft;
    public bool cannottalk;
    public bool inactiveNecessityBars;
    public bool caveWallActive;
    public bool caveWallColliderActive;
    public bool walkColliderActive;
    public bool wakingup;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple Data Managers present in scene, please fix.");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = findAllDataPersistenceObjects();
        loadGame();
    }

    public void newGame()
    {
        // Set base variables
        this.gameData = new GameData();
        gameData.playerPos = playerPos;
        gameData.emberstoneDurability = emberstoneDurability;
        gameData.torchDurability = torchDurability;
        gameData.torchActive = torchActive;
        gameData.emberstoneActive = emberstoneActive;
        gameData.currentTemperature = currentTemperature;
        gameData.currentHunger = currentHunger;
        gameData.burnTimer = burnTimer;
        gameData.coalStored = coalStored;
        gameData.sticksEmplaced = sticksEmplaced;
        gameData.campfireUnlocked = campfireUnlocked;
        gameData.chestUnlocked = chestUnlocked;
        gameData.discoveredItems = discoveredItems;
        gameData.unlockedItems = unlockedItems;
        gameData.hasBeenCrafted = hasBeenCrafted;
        gameData.dialogueFlags = dialogueFlags;
        gameData.witchDialogueQueue = witchDialogueQueue;
        gameData.playerInventory = playerInventory;
        gameData.chestInventory = chestInventory;
        gameData.timeOfDay = timeOfDay;
        gameData.isNight = isNight;
        
        gameData.tutorialinprogress = tutorialinprogress;
        gameData.cannotcraft = cannotcraft;
        gameData.cannottalk = cannottalk;
        gameData.inactiveNecessityBars = inactiveNecessityBars;
        gameData.caveWallActive = caveWallActive;
        gameData.caveWallColliderActive = caveWallColliderActive;
        gameData.walkColliderActive = walkColliderActive;
        gameData.wakingup = wakingup;
    }

    public void loadGame()
    {
        this.gameData = dataHandler.load();

        if (this.gameData == null)
        {
            Debug.Log("Initializing new save file.");
            newGame();
        }

        foreach (IDataPersistence dataObj in dataPersistenceObjects)
        {
            dataObj.loadData(gameData);
        }
    }

    public void saveGame()
    {
        gameData.witchDialogueQueue.Clear();

        foreach (IDataPersistence dataObj in dataPersistenceObjects)
        {
            dataObj.saveData(ref gameData);
        }
        
        dataHandler.save(this.gameData);
    }

    private List<IDataPersistence> findAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
