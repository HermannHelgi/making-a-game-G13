using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Canvas variables")]
    public string mainMenuSceneName;
    public bool playerhasdied = false;

    public string winTitleMessage;
    public string winMessage;


    [Header("References")]
    public GameObject deathscreen;
    public GameObject uiplayercanvas;
    public GameObject witchtradecanvas;
    public TextMeshProUGUI deathmessagetextmesh;
    public TextMeshProUGUI deathTitleTextMesh;
    public GameObject playercontroller;
    public GameObject loadLastSaveButton;

    private FileDataHandler fileDataHandler;

    public void Start()
    {
        if (DataPersistenceManager.instance != null)
        {
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, DataPersistenceManager.instance.fileName);
        }
    }

    public void die(string deathmessage)
    {
        // Makes the player "dead", aka freezes time and turns on the canvas
        if (!playerhasdied)
        {
            GameManager.instance.inMenu = true;
            playercontroller.GetComponent<FirstPersonController>().freezecamera = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            deathmessagetextmesh.text = deathmessage;
            uiplayercanvas.SetActive(false);
            witchtradecanvas.SetActive(false);

            deathscreen.SetActive(true);
            playerhasdied = true;

            if (fileDataHandler != null)
            {
                if (!fileDataHandler.fileExists())
                {
                    loadLastSaveButton.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void playerDrankPotion()
    {
        GameManager.instance.inMenu = true;
        playercontroller.GetComponent<FirstPersonController>().freezecamera = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deathTitleTextMesh.text = winTitleMessage;
        deathmessagetextmesh.text = winMessage;

        uiplayercanvas.SetActive(false);
        witchtradecanvas.SetActive(false);
        deathscreen.SetActive(true);
    }

    public void resetForNewGameScene()
    // Resets timescale to load new scene.
    {
        playercontroller.GetComponent<FirstPersonController>().freezecamera = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void resetForMainMenu()
    // Resets timescale to load new scene.
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void quitToMainMenu()
    {
        resetForMainMenu();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void loadLastSave()
    {
        resetForNewGameScene();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quitGame()
    {
        resetForNewGameScene();
        
        Application.Quit();
    }
}
