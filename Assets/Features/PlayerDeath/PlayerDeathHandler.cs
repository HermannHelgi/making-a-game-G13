using TMPro;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{

    [Header("Canvas variables")]
    public GameObject deathscreen;
    public GameObject uiplayercanvas;
    public GameObject witchtradecanvas;
    public TextMeshProUGUI deathmessagetextmesh;


    private bool playerhasdied = false;
    public void die(string deathmessage)
    {
        if (!playerhasdied)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            deathmessagetextmesh.text = deathmessage;
            uiplayercanvas.SetActive(false);
            witchtradecanvas.SetActive(false);

            deathscreen.SetActive(true);
            playerhasdied = true;
        }
    }

    public void resetForNewGameScene()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
