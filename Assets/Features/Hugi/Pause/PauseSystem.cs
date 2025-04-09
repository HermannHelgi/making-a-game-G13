using UnityEngine;
using System.Collections;
using StarterAssets;

public class PauseSystem : MonoBehaviour
{
    public static bool isPaused = false; // Static so other scripts can check
    public GameObject pauseMenu; // Assign this in the Inspector
    public GameObject player;
    public PlayerDeathHandler deathhandler;
    private FirstPersonController playerController;

    void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathhandler.playerhasdied && !GameManager.instance.inMenu)
        {
            TogglePause();
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

          // Freeze/unfreeze the First Person Controller camera
        if (playerController != null)
        {
            playerController.freezecamera = isPaused;
        }
    }

    public void ApplicationQuit()
    {
        Application.Quit();
        // Change when Main_Menu is ready
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
    }
}