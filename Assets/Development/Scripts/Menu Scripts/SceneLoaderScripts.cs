using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderScripts : MonoBehaviour
{

    [Header("Scene variables.")]
    public string sceneName;
    public string introScene;
    public GameObject continueGame;
    [SerializeField] private string saveFileName;
    private FileDataHandler fileDataHandler;


    private void Awake()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
    }

    private void Start()
    {
        if (fileDataHandler.fileExists())
        {
            continueGame.GetComponent<Button>().interactable = true;
        }
        else
        {
            continueGame.GetComponent<Button>().interactable = false;
        }
    }

    public void loadGivenScene()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(sceneName);
    }

    public void loadNewScene()
    {
        // datahandler.delete() always returns true. just making sure the file is removed before the scene is loaded.
        if (fileDataHandler.delete())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene(introScene);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
