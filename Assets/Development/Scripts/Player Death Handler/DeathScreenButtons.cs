using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenButtons : MonoBehaviour
{
    public PlayerDeathHandler playerdeathhandler;


    public void quitToMainMenu()
    {
        // Need to reset the timescale, since the death screen will set it to 0 on activation.
        playerdeathhandler.resetForNewGameScene();

        // TODO: Connect SceneManager.load() to main menu.
    }

    public void loadLastSave()
    {
        // Need to reset the timescale, since the death screen will set it to 0 on activation.
        playerdeathhandler.resetForNewGameScene();
        
        // TODO: Needs to actually load last save, for now ive made it reload the given scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quitGame()
    {
        // Need to reset the timescale, since the death screen will set it to 0 on activation.
        playerdeathhandler.resetForNewGameScene();
        
        Application.Quit();
    }
}
