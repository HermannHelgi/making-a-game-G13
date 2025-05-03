using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseSystem : MonoBehaviour
{
    [Header("Pause Menu Variables")]
    public static bool isPaused = false; // Static so other scripts can check
    public GameObject pauseMenu; // Assign this in the Inspector
    public float maxMessageTimer;
    
    [Header("References")]
    public string mainMenuSceneName;
    public GameObject loadLastSaveButton;
    public GameObject saveButton;
    public GameObject messageToPlayer;
    

    private FileDataHandler fileDataHandler;
    private float messageTimer;

    void Start()
    {
        if (DataPersistenceManager.instance != null)
        {
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, DataPersistenceManager.instance.fileName);
        }
        else
        {
            saveButton.GetComponent<Button>().interactable = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (!GameManager.instance.inMenu || isPaused))
        {
            // Need to check if new save has been made to make load button "interactable"
            if (fileDataHandler.fileExists())
            {
                loadLastSaveButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                loadLastSaveButton.GetComponent<Button>().interactable = false;
            }

            TogglePause();
        }

        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                messageToPlayer.SetActive(false);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        // Lock/unlock cursor
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        // Pause/unpause audio
        AudioListener.pause = isPaused;

        // Triggers the InMenu variable, freezing the player
        if (isPaused)
        {
            GameManager.instance.inMenu = isPaused;
        }
        else
        {
            GameManager.instance.activateMenuCooldown();
        }
    }

    public void leaveToMainMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = false;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void loadLastSave()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        AudioListener.pause = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void saveGame()
    {
        DataPersistenceManager.instance.saveGame();
        messageToPlayer.SetActive(true);
        loadLastSaveButton.GetComponent<Button>().interactable = true;
        messageTimer = maxMessageTimer;
    }
}